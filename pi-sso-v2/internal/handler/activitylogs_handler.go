package handler

import (
	"net/http"
	"time"

	"github.com/labstack/echo/v4"
	"github.com/pi-financial/pi-sso-v2/config"
	"github.com/pi-financial/pi-sso-v2/internal/service/ssodb_service"
)

type ActivityLogHandler struct {
	ActivityLogService ssodb_service.ActivityLogService
	Cfg                config.Config
}

// NewActivityLogHandler - ฟังก์ชันสำหรับการสร้าง Handler และกำหนด Route
func NewActivityLogHandler(e *echo.Echo, cfg config.Config, activityLogService ssodb_service.ActivityLogService) {
	handler := &ActivityLogHandler{
		ActivityLogService: activityLogService,
		Cfg:                cfg,
	}

	e.GET("/activitylogs", handler.GetActivityLogsByDate)
}

// GetActivityLogsByDate - ฟังก์ชันดึงข้อมูล Activity Logs ตามวันที่ (yyyyMMdd)
func (h *ActivityLogHandler) GetActivityLogsByDate(c echo.Context) error {
	// รับค่าจาก query parameter 'date' ที่ถูกส่งเข้ามาในรูปแบบ yyyyMMdd
	dateStr := c.QueryParam("date")
	if dateStr == "" {
		return c.JSON(http.StatusBadRequest, map[string]string{"error": "date is required"})
	}

	// แปลงจาก string เป็นรูปแบบวันที่
	date, err := time.Parse("20060102", dateStr)
	if err != nil {
		return c.JSON(http.StatusBadRequest, map[string]string{"error": "Invalid date format. Use yyyyMMdd"})
	}

	// เรียกใช้ service เพื่อดึงข้อมูลจากฐานข้อมูล
	activityLogs, err := h.ActivityLogService.GetActivityLogsByDate(date)
	if err != nil {
		return c.JSON(http.StatusInternalServerError, map[string]string{"error": err.Error()})
	}

	return c.JSON(http.StatusOK, activityLogs)
}
