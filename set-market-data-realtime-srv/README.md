# Stock Message Subscription Service (Pi.SetMarketDataRealTime.DataHandler)

This service subscribes to ITCH market data messages, parses binary messages into JSON, and publishes them to Kafka. For local development, the `Pi.SetMarketDataRealTime.DataServer` project simulates the ITCH message server to emulate the SETTRADE platform.

---

## 📋 Prerequisites
- **.NET 8.0 SDK** (or newer)
- Git
- Kafka/Redis/Cloud services (for production mode)
- ITCH BinLog file (e.g., `set-itch-binlog-live.20240301`)

---

## 🛠️ Setup & Configuration

### 1. Clone the Repository
```bash
git clone https://github.com/your-organization/set-market-data-realtime-srv.git
cd set-market-data-realtime-srv/src
```
### 2. Configure NuGet Credentials
Update nuget.config in the project root:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageRestore>
        <add key="enabled" value="True" />
    </packageRestore>
    <packageSources>
        <add key="Github" value="https://nuget.pkg.github.com/pi-financial/index.json" />
    </packageSources>
    <packageSourceCredentials>
        <Github>
            <add key="Username" value="<<USER>>" />
            <add key="ClearTextPassword" value="<<PASSWORD>>" />
        </Github>
    </packageSourceCredentials>
</configuration>
```
### 3. Prepare BinLogs
Copy your ITCH BinLog file (e.g., `set-itch-binlog-live.20240301`) to:
```
set-market-data-realtime-srv/src/Pi.SetMarketDataRealTime.DataServer/BinLogs
```
---
## 🚀 Running the System
### 1. Start the ITCH Server Simulator (DataServer)
```bash
cd Pi.SetMarketDataRealTime.DataServer
dotnet run
# Listens on port 5501 (config: appsettings.json)
```
The server will listen on port `5501` (configured in `appsettings.json`).
### 2. Configure the DataHandler for Local Mode
Ensure `SETTRADE_RUN_LOCAL_MODE is set to true` in:
```
set-market-data-realtime-srv/src/Pi.SetMarketDataRealTime.DataHandler/appsettings.json
```
#### Ensure appsettings.json has:
```json
{
  "SETTRADE_RUN_LOCAL_MODE": true,
  "SETTRADE_CLIENT_CONFIG": {
    "IP_ADDRESS": "127.0.0.1",
    "PORT": 5501
  }
}
```
### 3. Start the DataHandler
```bash
cd ../Pi.SetMarketDataRealTime.DataHandler
dotnet run
```
## 🔄 Workflow
1. DataServer simulates ITCH messages using the provided BinLog file.
2. DataHandler connects to the local server (or production servers if `SETTRADE_RUN_LOCAL_MODE=false`).
3. Messages are parsed to JSON and published to Kafka (topic: `set_stock_market_data`).

## 🌐 Production Configuration
Set `SETTRADE_RUN_LOCAL_MODE to false` in appsettings.json.

Configure cloud services (Kafka, Redis, AWS S3) in appsettings.json:

```json
"KAFKA": {
  "BOOTSTRAP_SERVERS": "pkc-312o0.ap-southeast-1.aws.confluent.cloud:9092",
  "TOPIC": "set_stock_market_data",
  "SASL_USERNAME": "IE54Z5EXJ5X5XO6O",
  "SASL_PASSWORD": "<<PASSWORD>>"
},
"AWS_S3": {
  "BUCKET_NAME": "market-data-set-binlog",
  "REGION": "ap-southeast-1"
}
```
## 🧪 Verification
### Data Flow
1. DataServer → Simulates ITCH messages from BinLog.
2. DataHandler → Parses to JSON → Publishes to Kafka topic set_stock_market_data.

3. Check Server Logs:
- DataServer: Confirm it is listening on port 5501.
- DataHandler: Look for Received real-time message logs.

4. Kafka Consumer:
```bash
kafka-console-consumer --bootstrap-server pkc-312o0.ap-southeast-1.aws.confluent.cloud:9092 \
--topic set_stock_market_data --from-beginning
```
## ⚠️ Troubleshooting
| Issue  | Solution  |
|---|---|
Authentication failed |	Verify nuget.config credentials
NuGet authentication failed |	Verify credentials in nuget.config
Missing BinLog file |	Add a valid file to DataServer/BinLogs
Port 5501 in use |	Free the port or update appsettings.json
Kafka connection errors |	Check SASL_USERNAME and SASL_PASSWORD
## 📂 Project Structure
```
set-market-data-realtime-srv/
├── src/
│   ├── Pi.SetMarketDataRealTime.DataHandler/    # Processes & publishes messages
│   │   ├── appsettings.json                     # Local/production configurations
│   │   └── ...
│   ├── Pi.SetMarketDataRealTime.DataServer/     # ITCH server simulator
│   │   ├── BinLogs/                             # ITCH log storage
│   │   └── ...
└── ...
```
## Note
Replace placeholder credentials in appsettings.json and nuget.config before deployment.
Use secret managers for production credentials (e.g., AWS Secrets Manager).