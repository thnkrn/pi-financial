package log

import (
	"context"

	"go.uber.org/zap"
)

type Logger interface {
	Debug(ctx context.Context, msg string)
	Error(ctx context.Context, msg string, err error)
	ErrorWithFields(ctx context.Context, msg string, fields ...zap.Field)
	Fatal(ctx context.Context, msg string, err error)
	Info(ctx context.Context, msg string)
}
