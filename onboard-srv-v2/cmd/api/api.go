package api

import (
	"context"
	"errors"
	"net/http"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/pi-financial/onboard-srv-v2/config"
	"github.com/pi-financial/onboard-srv-v2/internal/core/port"
)

func StartServer(hs *http.Server, l port.Logger) {
	shutdownChan := make(chan bool, 1)

	go func() {
		l.Info("Start serving at %s", config.Get().Server.Port)
		if err := hs.ListenAndServe(); !errors.Is(err, http.ErrServerClosed) {
			l.Fatal("HTTP server error: %v", err)
		}
		l.Info("Stopped serving new connections.")
		shutdownChan <- true
	}()

	// Gracefully shutdown
	sigChan := make(chan os.Signal, 1)
	signal.Notify(sigChan, syscall.SIGINT, syscall.SIGTERM)
	<-sigChan

	shutdownCtx, shutdownRelease := context.WithTimeout(context.Background(), 10*time.Second)
	defer shutdownRelease()

	if err := hs.Shutdown(shutdownCtx); err != nil {
		l.Fatal("HTTP shutdown error: %v", err)
	}

	<-shutdownChan
	l.Info("Graceful shutdown complete.")
}
