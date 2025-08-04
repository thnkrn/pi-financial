package middleware

import (
	"context"
	"fmt"
	"github.com/pi-financial/pi-sso-v2/internal/log"
	"go.uber.org/zap"
	"gorm.io/gorm/logger"
	"gorm.io/gorm/utils"
	"time"
)

type ZapGormLogger struct {
	logger        log.Logger
	logLevel      logger.LogLevel
	slowThreshold time.Duration
}

func NewZapGormLogger(logger log.Logger, level logger.LogLevel) logger.Interface {
	return &ZapGormLogger{
		logger:        logger,
		logLevel:      level,
		slowThreshold: 200 * time.Millisecond, // or config it
	}
}

func (l *ZapGormLogger) LogMode(level logger.LogLevel) logger.Interface {
	return &ZapGormLogger{
		logger:        l.logger,
		logLevel:      level,
		slowThreshold: l.slowThreshold,
	}
}

func (l *ZapGormLogger) Info(ctx context.Context, msg string, data ...interface{}) {
	if l.logLevel >= logger.Info {
		l.logger.Info(ctx, fmt.Sprintf(msg, data...))
	}
}

func (l *ZapGormLogger) Warn(ctx context.Context, msg string, data ...interface{}) {
	if l.logLevel >= logger.Warn {
		l.logger.Info(ctx, fmt.Sprintf(msg, data...)) // or .Warn if you add it
	}
}

func (l *ZapGormLogger) Error(ctx context.Context, msg string, data ...interface{}) {
	if l.logLevel >= logger.Error {
		l.logger.Error(ctx, fmt.Sprintf(msg, data...))
	}
}

func (l *ZapGormLogger) Trace(ctx context.Context, begin time.Time, fc func() (string, int64), err error) {
	if l.logLevel == logger.Silent {
		return
	}

	elapsed := time.Since(begin)
	sql, rows := fc()
	fields := []zap.Field{
		zap.String("source", utils.FileWithLineNum()),
		zap.Duration("elapsed", elapsed),
		zap.Int64("rows", rows),
		zap.String("sql", sql),
	}

	switch {
	case err != nil && l.logLevel >= logger.Error:
		fields = append(fields, zap.Error(err))
		l.logger.Error(ctx, "GORM error", fields...)

	case elapsed > l.slowThreshold && l.logLevel >= logger.Warn:
		l.logger.Info(ctx, "GORM slow query", fields...)

	case l.logLevel >= logger.Info:
		l.logger.Info(ctx, "GORM query", fields...)
	}
}
