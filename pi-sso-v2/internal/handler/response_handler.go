package handler

import (
	"encoding/json"
	"os"

	"github.com/labstack/echo/v4"
)

var errorMessages map[string]string

type ErrorResponseMessage struct {
	Message string `json:"message"`
	Code    string `json:"error"`
}

func LoadErrorMessages(filename string) error {
	file, err := os.Open(filename)
	if err != nil {
		return err
	}
	defer file.Close()
	return json.NewDecoder(file).Decode(&errorMessages)
}

func ErrorResponseDetail(c echo.Context, code int, errorCode string) error {
	message, exists := errorMessages[errorCode]
	if !exists {
		message = errorCode // ใช้ errorCode เป็นข้อความ error หากไม่พบใน JSON
		code = 999          // ตั้งค่า code เป็น 999 สำหรับ error ที่ไม่รู้จัก
	}
	return c.JSON(code, ErrorResponseMessage{Message: message, Code: errorCode})
}

func SuccessResponseDetail(c echo.Context) error {
	message := errorMessages["E000"]

	return c.JSON(200, ErrorResponseMessage{Message: message, Code: "E000"})
}
