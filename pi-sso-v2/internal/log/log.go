package log

import (
	"context"
	"go.uber.org/zap"
)

type Logger interface {
	Debug(ctx context.Context, msg string, fields ...zap.Field)
	Info(ctx context.Context, msg string, fields ...zap.Field)
	Error(ctx context.Context, msg string, fields ...zap.Field)
	Fatal(ctx context.Context, msg string, fields ...zap.Field)

	InfoNoCtx(msg string, fields ...zap.Field)
	ErrorNoCtx(msg string, fields ...zap.Field)
}
