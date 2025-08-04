package adapter

import (
	"context"
	"go.uber.org/zap"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
	"time"
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

// ---------------------
// Context-aware logging
// ---------------------

func (z *ZapImplement) Info(ctx context.Context, msg string, fields ...zap.Field) {
	z.logger.Info(msg, z.withTraceContext(ctx, fields...)...)
}

func (z *ZapImplement) Debug(ctx context.Context, msg string, fields ...zap.Field) {
	z.logger.Debug(msg, z.withTraceContext(ctx, fields...)...)
}

func (z *ZapImplement) Error(ctx context.Context, msg string, fields ...zap.Field) {
	z.logger.Error(msg, z.withTraceContext(ctx, fields...)...)
}

func (z *ZapImplement) Fatal(ctx context.Context, msg string, fields ...zap.Field) {
	z.logger.Fatal(msg, z.withTraceContext(ctx, fields...)...)
}

// ---------------------
// Fallback: no context
// ---------------------

func (z *ZapImplement) InfoNoCtx(msg string, fields ...zap.Field) {
	z.logger.Info(msg, fields...)
}

func (z *ZapImplement) ErrorNoCtx(msg string, fields ...zap.Field) {
	z.logger.Error(msg, fields...)
}

// ---------------------
// Internal: trace injection
// ---------------------

func (z *ZapImplement) withTraceContext(ctx context.Context, fields ...zap.Field) []zap.Field {
	span, ok := tracer.SpanFromContext(ctx)
	if ok {
		fields = append(fields,
			zap.Time("timestamp", time.Now()),
			zap.Uint64("trace_id", span.Context().TraceID()),
			zap.Uint64("span_id", span.Context().SpanID()),
		)
	}
	return fields
}
