#!/bin/bash

# Wait a moment before stunnel to initialize
sleep 5

# Start stunnel in the background
stunnel /etc/stunnel/stunnel.conf &

# Wait a moment for stunnel to initialize
sleep 5

# Start the .NET application
dotnet Pi.GlobalMarketDataRealTime.DataHandler.dll
