#!/bin/bash

# Function to run a project in a new terminal window
run_project() {
    local project_dir=$1
    local project_name=$2
    local wait_time=$3

    echo "Running $project_name in a new terminal window..."
    osascript -e 'tell application "Terminal"' \
              -e 'activate' \
              -e "do script \"cd $project_dir && dotnet run\"" \
              -e "set custom title of front window to \"$project_name\"" \
              -e 'end tell'

    echo "Waiting for $wait_time seconds before running the next project..."
    sleep $wait_time
}

# Run each project in a separate terminal window with a wait time
run_project "/Users/aransutthirak/Documents/Bitbucket/pi-financial/set-market-data/src/Pi.SetMarketData/Pi.SetMarketData.RealTimeDataServer" "RealTimeDataServer" 1
run_project "/Users/aransutthirak/Documents/Bitbucket/pi-financial/set-market-data/src/Pi.SetMarketData/Pi.SetMarketData.RealTimeDataHandler" "RealTimeDataHandler" 5
run_project "/Users/aransutthirak/Documents/Bitbucket/pi-financial/set-market-data/src/Pi.SetMarketData/Pi.SetMarketData.DataSubscriber" "DataSubscriber" 10
run_project "/Users/aransutthirak/Documents/Bitbucket/pi-financial/set-market-data/src/Pi.SetMarketData/Pi.SetMarketData.SignalRHub" "SignalRHub" 10

echo "All projects are running in separate terminal windows."