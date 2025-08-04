package middleware

import (
	"fmt"
	"github.com/pi-financial/pi-sso-v2/internal/log"
	"go.uber.org/zap"
	"net/http"
	"strings"

	"github.com/dgrijalva/jwt-go"
	"github.com/google/uuid"
	"github.com/labstack/echo/v4"
	"github.com/pi-financial/pi-sso-v2/config"
	"github.com/pi-financial/pi-sso-v2/internal/repository/ssodb_repository"
	echotrace "gopkg.in/DataDog/dd-trace-go.v1/contrib/labstack/echo.v4"
)

type ErrorHandler struct {
}

func NewErrorHandler() *ErrorHandler {
	return &ErrorHandler{}
}

func (e *ErrorHandler) EchoErrorHandler(err error, ctx echo.Context) {
	var code int
	var message interface{}

	switch e := err.(type) {
	case *echo.HTTPError:
		code = e.Code
		message = e.Message
	default:
		code = http.StatusInternalServerError
		message = http.StatusText(http.StatusInternalServerError)
	}

	if !ctx.Response().Committed {
		_ = ctx.JSON(code, map[string]interface{}{
			"error": message,
			"code":  code,
		})
	}
}

type PiMiddlewares struct {
	Echo            *echo.Echo
	Cfg             config.Config
	ActivityLogRepo ssodb_repository.ActivityLogRepository
}

func NewPiMiddleware(e *echo.Echo, cfg config.Config, repository ssodb_repository.ActivityLogRepository) *PiMiddlewares {
	return &PiMiddlewares{
		Cfg:             cfg,
		Echo:            e,
		ActivityLogRepo: repository,
	}
}

func jwtErrorResponse(c echo.Context, detail string) error {
	return c.JSON(http.StatusBadRequest, map[string]interface{}{
		"status": 400,
		"title":  "SSOJWT",
		"detail": detail,
	})
}

// JWTAccessTokenMiddleware สำหรับตรวจสอบ Access Token
func (e *PiMiddlewares) JWTAccessTokenMiddleware(next echo.HandlerFunc) echo.HandlerFunc {
	return func(c echo.Context) error {
		tokenString := extractToken(c)

		if tokenString == "" {
			return jwtErrorResponse(c, "Missing or invalid token")
		}

		claims := jwt.MapClaims{}
		token, err := jwt.ParseWithClaims(tokenString, claims, func(token *jwt.Token) (interface{}, error) {
			return []byte(e.Cfg.JwtSecret), nil
		})

		if err != nil || !token.Valid {
			return jwtErrorResponse(c, "Invalid access token")
		}

		// ตรวจสอบว่าไม่ใช่ refresh token
		if claims["is_refresh_token"] == true {
			return jwtErrorResponse(c, "Refresh token cannot be used here")
		}

		// เก็บ claims ลงใน context
		c.Set("user", claims)

		return next(c)
	}
}

// JWTRefreshTokenMiddleware สำหรับตรวจสอบ Refresh Token
func (e *PiMiddlewares) JWTRefreshTokenMiddleware(next echo.HandlerFunc) echo.HandlerFunc {
	return func(c echo.Context) error {
		tokenString := extractToken(c)

		if tokenString == "" {
			return jwtErrorResponse(c, "Missing or invalid token")
		}

		claims := jwt.MapClaims{}
		token, err := jwt.ParseWithClaims(tokenString, claims, func(token *jwt.Token) (interface{}, error) {
			return []byte(e.Cfg.JwtSecret), nil
		})

		if err != nil || !token.Valid {
			return jwtErrorResponse(c, "Invalid refresh token")
		}

		// ตรวจสอบว่า token เป็น refresh token จริง ๆ
		if claims["is_refresh_token"] != true {
			return jwtErrorResponse(c, "Invalid token for refresh")
		}

		// เก็บ claims ลงใน context
		c.Set("user", claims)

		return next(c)
	}
}

var ignoreTracerPaths = []string{"/favicon.ico", "/swagger", "/health"}

func (e *PiMiddlewares) TracerMiddleware() echo.MiddlewareFunc {
	return echotrace.Middleware(
		echotrace.WithIgnoreRequest(func(ctx echo.Context) bool {
			if ctx.Request().URL.Path == "/" {
				return true
			}
			for _, path := range ignoreTracerPaths {
				if strings.HasPrefix(ctx.Request().URL.Path, path) {
					return true
				}
			}
			return false
		}),
	)
}

// extractToken ฟังก์ชันช่วยสำหรับดึง JWT จาก header
func extractToken(c echo.Context) string {
	authHeader := c.Request().Header.Get("Authorization")
	if authHeader == "" {
		return ""
	}

	parts := strings.Split(authHeader, " ")
	if len(parts) == 2 && strings.ToLower(parts[0]) == "bearer" {
		return parts[1]
	}

	return ""
}

// ActivityLogMiddleware เก็บ log กิจกรรมของผู้ใช้
func (e *PiMiddlewares) ActivityLogMiddleware(logger log.Logger) echo.MiddlewareFunc {
	return func(next echo.HandlerFunc) echo.HandlerFunc {
		return func(c echo.Context) error {
			// เรียกใช้ handler จริง
			err := next(c)

			// ถ้า request path เป็นหน้าแรก ไม่ต้องเก็บ Log
			if c.Path() == "/" {
				return err
			}

			// ดึงข้อมูลที่จำเป็นจากคำขอ
			ipAddress := c.RealIP()
			userAgent := c.Request().UserAgent()
			requestPath := c.Path()
			method := c.Request().Method

			// ดึง accountID จาก claims
			accountID, err := extractAccountID(c, logger)
			if err != nil {
				logger.Error(c.Request().Context(), "ActivityMiddleware Error extracting account ID", zap.Error(err))
			}

			// สร้าง activity log
			logError := createActivityLog(e, accountID, method, requestPath, ipAddress, userAgent, c.Response().Status)
			if logError != nil {
				logger.Error(c.Request().Context(), "ActivityMiddleware Error Save Activity", zap.Error(err))
			}

			logger.Info(c.Request().Context(), "request received",
				zap.String("accountID", accountID.String()),
				zap.String("method", method),
				zap.String("path", requestPath),
				zap.String("ip", ipAddress),
				zap.String("userAgent", userAgent),
				zap.Int("status", c.Response().Status),
			)

			// ส่งกลับไปยังผู้ใช้ (ถ้ามี error จาก handler จริง)
			return err
		}
	}
}

// extractAccountID ดึง accountID จาก claims
func extractAccountID(c echo.Context, logger log.Logger) (uuid.UUID, error) {
	var accountID uuid.UUID
	claims := c.Get("user")

	if claims != nil {
		userClaims, ok := claims.(jwt.MapClaims)
		if ok {
			accountIDStr, ok := userClaims["account_id"].(string)
			if ok {
				return uuid.Parse(accountIDStr)
			}
			logger.Error(c.Request().Context(), "ActivityMiddleware user_id missing in claims")
		} else {
			logger.Error(c.Request().Context(), "ActivityMiddleware Invalid claims format")
		}
	}
	return accountID, nil
}

// createActivityLog สร้าง activity log
func createActivityLog(e *PiMiddlewares, accountID uuid.UUID, method, requestPath, ipAddress, userAgent string, status int) error {
	return e.ActivityLogRepo.Create(
		accountID,
		method+" "+requestPath,
		"User made a request to "+requestPath,
		ipAddress,
		userAgent,
		fmt.Sprintf("status:%v", status),
	)
}
