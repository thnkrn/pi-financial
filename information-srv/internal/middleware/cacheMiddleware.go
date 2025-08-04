package middleware

import (
	"net/http"
	"strings"
	"time"

	"github.com/labstack/echo/v4"
	"github.com/labstack/gommon/log"
	"github.com/pi-financial/information-srv/internal/adapters/cache"
)

var cacheVar = struct {
	header       string
	cacheControl string
	noCache      string
	noStore      string
	hit          string
	miss         string
}{
	header:       "X-Cache",
	cacheControl: "Cache-Control",
	noCache:      "no-cache",
	noStore:      "no-store",
	hit:          "HIT",
	miss:         "MISS",
}

// CacheMiddleware caches responses for GET requests
func CacheMiddleware(cache *cache.RedisCacheRepository) echo.MiddlewareFunc {
	return func(next echo.HandlerFunc) echo.HandlerFunc {
		return func(ctx echo.Context) error {
			req := ctx.Request()
			res := ctx.Response()
			cacheKey := req.RequestURI + "?" + req.URL.RawQuery
			cacheControlReq := req.Header.Get(cacheVar.cacheControl)

			if shouldIgnoreCache(req) {
				setNoCacheHeader(ctx)
				return next(ctx)
			}

			if shouldServeFromCache(cacheControlReq) {
				if cachedResponse, _ := cache.Get(cacheKey); cachedResponse != "" {
					serveCache(cachedResponse, res)
					return nil
				}
			}

			rec := newResponseRecorder(res)
			ctx.Response().Writer = rec

			if err := next(ctx); err != nil {
				ctx.Error(err)
				return err
			}

			if shouldCacheResponse(res, cacheControlReq) {
				setCache(cacheKey, req, rec, cache)
			} else {
				appendNoStoreHeader(rec)
			}

			return nil
		}
	}
}

func setNoCacheHeader(ctx echo.Context) {
	rec := newResponseRecorder(ctx.Response())
	rec.Header().Set(cacheVar.cacheControl, cacheVar.noCache)
	ctx.Response().Writer = rec
}

func shouldServeFromCache(cacheControlReq string) bool {
	return !strings.Contains(cacheControlReq, cacheVar.noCache)
}

func shouldCacheResponse(res *echo.Response, cacheControlReq string) bool {
	return res.Status == http.StatusOK && !strings.Contains(cacheControlReq, cacheVar.noStore)
}

func appendNoStoreHeader(rec *responseRecorder) {
	cacheControl := rec.Header().Get(cacheVar.cacheControl)
	rec.Header().Set(cacheVar.cacheControl, strings.Join([]string{cacheControl, cacheVar.noStore}, ","))
}

type responseRecorder struct {
	echo.Response
	Body *strings.Builder
}

func newResponseRecorder(res *echo.Response) *responseRecorder {
	res.Header().Set(cacheVar.header, cacheVar.miss)
	return &responseRecorder{
		Response: *res,
		Body:     new(strings.Builder),
	}
}

func (r *responseRecorder) Write(b []byte) (int, error) {
	r.Body.Write(b)
	return r.Response.Writer.Write(b)
}

var ignoreExactPaths = []string{"/", "/favicon.ico"}
var ignorePrefixPaths = []string{"/swagger"}

func shouldIgnoreCache(req *http.Request) bool {
	// Only cache GET requests
	if req.Method != http.MethodGet {
		return true
	}
	for _, path := range ignoreExactPaths {
		if req.URL.Path == path {
			return true
		}
	}
	for _, path := range ignorePrefixPaths {
		if strings.HasPrefix(req.URL.Path, path) {
			return true
		}
	}
	return false
}

func serveCache(cachedResponse string, res *echo.Response) {
	res.Header().Set(cacheVar.header, cacheVar.hit)
	res.Header().Set(echo.HeaderContentType, echo.MIMEApplicationJSON)
	res.WriteHeader(http.StatusOK)
	_, err := res.Write([]byte(cachedResponse))
	if err != nil {
		log.Errorf("Failed to serveCache: %v", err)
	}
}

func setCache(cacheKey string, req *http.Request, rec *responseRecorder, cache *cache.RedisCacheRepository) {
	duration := time.Hour * 12
	if strings.Contains(req.URL.Path, "/address") {
		duration = time.Hour * 24 * 7
	} else if strings.Contains(req.URL.Path, "/exchange-rate") {
		duration = time.Hour * 4
	}
	if err := cache.Set(cacheKey, rec.Body.String(), duration); err != nil {
		log.Errorf("Failed to set cache: %v", err)
	}
}
