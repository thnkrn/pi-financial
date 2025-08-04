package middleware

import (
	"fmt"

	gb "github.com/growthbook/growthbook-golang"

	"encoding/json"
	"io"
	"net/http"

	"github.com/labstack/echo/v4"
	"github.com/pi-financial/bond-srv/config"
	"github.com/pi-financial/bond-srv/internal/driver/log"
	"go.uber.org/zap"
)

var (
	GrowthbookHeaderAttributes = []string{
		"User-Id",
		"Deviceid",
		"Loggedin",
		"Deviceos",
		"Deviceosversion",
		"Devicemodel",
		"Custcode",
	}
	GrowthbookHeaderAttributesMap = map[string]string{
		"User-Id":         "userId",
		"Deviceid":        "deviceId",
		"Loggedin":        "loggedIn",
		"Deviceos":        "deviceOS",
		"Deviceosversion": "deviceOSVersion",
		"Devicemodel":     "deviceModel",
		"Custcode":        "custCode",
	}
)

type GrowthBookClient struct {
	Client *gb.Client
}

type GrowthBookApiResp struct {
	Features json.RawMessage
	Status   int
}

// Parse the headers whose name matches the growthbook attribute.
func (d *GrowthBookClient) HandleGrowthbook(log log.Logger, cfg config.Config) echo.MiddlewareFunc {
	return func(next echo.HandlerFunc) echo.HandlerFunc {
		return func(c echo.Context) error {
			var (
				requestAttributes = gb.Attributes{}
				headers           = c.Request().Header
			)
			for _, attrHeader := range GrowthbookHeaderAttributes {
				// Get header values list whose header name have an associated growthbook attribute.
				// Use the first header value as the attribute value.
				if header, ok := headers[attrHeader]; ok && len(header) != 0 {
					attrKey := GrowthbookHeaderAttributesMap[attrHeader]
					requestAttributes[attrKey] = header[0]
				}
			}

			// Update the growthbook client's attributes with the values from the header of the current request.
			currentSessionClient, err := d.Client.WithAttributes(requestAttributes)
			if err != nil {
				log.ErrorWithFields(
					c.Request().Context(),
					fmt.Sprintf("[~] Failed to parse growthbook attributes from request header for request method: %s path: %s headers: %+v", c.Request().Method, c.Request().URL.Path, headers),
					zap.Any("method", c.Request().Method),
					zap.Any("path", c.Request().URL.Path),
					zap.Any("status", c.Response().Status),
					zap.Any("request", nil),
					zap.Any("response", nil),
					zap.Any("remote_ip", c.RealIP()),
					zap.Any("referer", c.Request().Referer()),
					zap.Any("host", c.Request().Host),
					zap.Any("query_params", c.QueryParams()),
					zap.Any("headers", c.Request().Header))
				panic(err)
			}

			// Set latest features and rules from growthbook.
			rawFeatureMap, err := GetRawFeatureMap(cfg.GrowthbookHost, cfg.GrowthBookApiKey, cfg.GrowthbookProjectId)
			if err != nil {
				log.ErrorWithFields(
					c.Request().Context(),
					"[~] Failed to get features from growthbook while parsing growthbook attributes from request header",
					zap.Any("method", c.Request().Method),
					zap.Any("path", c.Request().URL.Path),
					zap.Any("status", c.Response().Status),
					zap.Any("request", nil),
					zap.Any("response", nil),
					zap.Any("remote_ip", c.RealIP()),
					zap.Any("referer", c.Request().Referer()),
					zap.Any("host", c.Request().Host),
					zap.Any("query_params", c.QueryParams()),
					zap.Any("headers", c.Request().Header))
				panic(err)
			}

			err = currentSessionClient.SetJSONFeatures(string(rawFeatureMap))
			if err != nil {
				log.ErrorWithFields(
					c.Request().Context(),
					"[~] Failed to set growthbook features for client while parsing growthbook attributes from request header",
					zap.Any("method", c.Request().Method),
					zap.Any("path", c.Request().URL.Path),
					zap.Any("status", c.Response().Status),
					zap.Any("request", nil),
					zap.Any("response", nil),
					zap.Any("remote_ip", c.RealIP()),
					zap.Any("referer", c.Request().Referer()),
					zap.Any("host", c.Request().Host),
					zap.Any("query_params", c.QueryParams()),
					zap.Any("headers", c.Request().Header))
				panic(err)
			}

			// Update the global client's instance with the new child instance.
			d.Client = currentSessionClient

			return next(c)
		}
	}
}

// Get map containing all feature information from the growthbook project as json data.
func GetRawFeatureMap(host string, apiKey string, projectId string) (_ []byte, err error) {
	defer func() {
		if err != nil {
			err = fmt.Errorf("in GetRawFeatureMap: %w", err)
		}
	}()

	// Fetch features JSON from api.
	url := fmt.Sprintf("http://%s/api/features/%s?project=%s", host, apiKey, projectId)
	resp, err := http.Get(url)
	if err != nil {
		return nil, fmt.Errorf("connect to api: %w", err)
	}

	defer resp.Body.Close()

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, fmt.Errorf("read in features: %w", err)
	}

	// Just return the features map from the API response.
	apiResp := &GrowthBookApiResp{}
	err = json.Unmarshal(body, apiResp)
	if err != nil {
		return nil, fmt.Errorf("map features to valid struct: %w", err)
	}

	return apiResp.Features, nil
}
