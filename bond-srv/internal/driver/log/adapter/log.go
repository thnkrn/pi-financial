package adapter

import (
	"context"
	"time"

	"github.com/pi-financial/bond-srv/internal/constants"
	"go.uber.org/zap"
)

type ZapImplement struct {
	logger *zap.Logger
}

func NewZapLogger(logger *zap.Logger) *ZapImplement {
	return &ZapImplement{logger}
}

func ProvideLogger(logger *zap.Logger) *ZapImplement {
	return NewZapLogger(logger)
}

func (z *ZapImplement) Debug(ctx context.Context, msg string) {
	z.logger.Debug(msg, z.getLogFields(ctx, nil)...)
}

func (z *ZapImplement) Info(ctx context.Context, msg string) {
	z.logger.Info(msg, z.getLogFields(ctx, nil)...)
}

func (z *ZapImplement) Error(ctx context.Context, msg string, err error) {
	z.logger.Error(msg, z.getLogFields(ctx, err)...)
}

func (z *ZapImplement) ErrorWithFields(ctx context.Context, msg string, fields ...zap.Field) {
	z.logger.Error(msg, fields...)
}

func (z *ZapImplement) Fatal(ctx context.Context, msg string, err error) {
	z.logger.Fatal(msg, z.getLogFields(ctx, err)...)
}

func (z *ZapImplement) getLogFields(ctx context.Context, err error) []zap.Field {
	fields := []zap.Field{zap.Time("time", time.Now())}
	if err != nil {
		fields = append(fields, zap.Error(err))
	}
	if logFields, ok := ctx.Value(constants.LoggerKey).([]zap.Field); ok {
		fields = append(fields, logFields...)
	}

	return fields
}
