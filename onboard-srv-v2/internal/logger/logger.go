package logger

import (
	"fmt"
	"time"

	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
	"go.uber.org/zap"
)

type loggerImplement struct {
	logger *zap.Logger
}

func NewLogger(logger *zap.Logger) port.Logger {
	return &loggerImplement{logger: logger}
}

func (z *loggerImplement) Debug(msg string, args ...interface{}) {
	z.logger.Debug(fmt.Sprintf(msg, args...), zap.Time("time", time.Now()))
}

func (z *loggerImplement) Info(msg string, args ...interface{}) {
	z.logger.Info(fmt.Sprintf(msg, args...), zap.Time("time", time.Now()))
}

func (z *loggerImplement) Warn(msg string, args ...interface{}) {
	z.logger.Warn(fmt.Sprintf(msg, args...), zap.Time("time", time.Now()))
}

func (z *loggerImplement) Error(msg string, args ...interface{}) {
	z.logger.Error(fmt.Sprintf(msg, args...), zap.Time("time", time.Now()))
}

func (z *loggerImplement) Fatal(msg string, args ...interface{}) {
	z.logger.Fatal(fmt.Sprintf(msg, args...), zap.Time("time", time.Now()))
}

func (z *loggerImplement) Sync() error {
	return z.logger.Sync()
}
