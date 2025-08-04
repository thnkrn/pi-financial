#!/bin/bash
set -e

# Function for logging
log() {
    echo "[$(date +'%Y-%m-%d %H:%M:%S %Z')] $1"
}

# Log startup information
log "Starting application with timezone: $(date)"
log "Timezone from /etc/timezone: $(cat /etc/timezone)"

# Clean up any existing stunnel files
log "Cleaning up existing stunnel files..."
rm -f /var/run/stunnel.pid
rm -f /var/log/stunnel/stunnel.log

# Check certificate file
log "Checking certificate file..."
if [ ! -f /etc/stunnel/certs/Bridges_CA.crt ]; then
    log "ERROR: Certificate file not found at /etc/stunnel/certs/Bridges_CA.crt"
    ls -l /etc/stunnel/certs/
    exit 1
fi

# Start stunnel
log "Starting stunnel..."
stunnel /etc/stunnel/stunnel.conf &
STUNNEL_PID=$!

# Wait for stunnel to initialize
sleep 5

# Check if stunnel is running
if ! kill -0 $STUNNEL_PID 2>/dev/null; then
    log "ERROR: stunnel failed to start"
    log "Stunnel logs:"
    if [ -f /var/log/stunnel/stunnel.log ]; then
        cat /var/log/stunnel/stunnel.log
    else
        log "No stunnel log file found"
    fi
    exit 1
fi

log "Stunnel started successfully with PID: $STUNNEL_PID"

# Start the .NET application
log "Starting .NET application..."
exec dotnet Pi.GlobalMarketDataRealTime.DataHandler.dll