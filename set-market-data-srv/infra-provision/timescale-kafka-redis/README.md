# Database setup

## Insert data to MongoDB
```bash
mongosh --port 27018 -u root -p root --authenticationDatabase admin insert_instruments_set_tfex.js
```

## Create RealtimeMarketData table for TimescaleDB
```sql
CREATE TABLE IF NOT EXISTS realtime_market_data (
    date_time TIMESTAMPTZ NOT NULL,
    symbol TEXT NOT NULL,
    venue TEXT NOT NULL,
    price DOUBLE PRECISION NULL,
    volume INT NULL,
    amount DOUBLE PRECISION NULL
);
ALTER TABLE realtime_market_data
ADD PRIMARY KEY (date_time, symbol, venue);
CREATE INDEX ix_symbol_date_time ON realtime_market_data (symbol, venue, date_time DESC);
ALTER TABLE realtime_market_data
ADD CONSTRAINT ux_datetime_symbol_venue UNIQUE (date_time, symbol, venue);
```

## Redis PriceInfo
```bash
redis-cli
SELECT 1
HSET "marketdata::price-info-65545" "data" "{ \"PriceInfoId\": 0, \"InstrumentId\": 0, \"Price\": \"5975000\", \"Currency\": \"THB\", \"AuctionPrice\": \"-2147483648\", \"AuctionVolume\": \"170000\", \"Open\": \"5975000\", \"High24H\": \"6900000\", \"Low24H\": \"5975000\", \"High52W\": null, \"Low52W\": null, \"PriceChanged\": \"6900000\", \"PriceChangedRate\": null, \"Volume\": \"50000\", \"Amount\": \"298750000000\", \"ChangeAmount\": null, \"ChangeVolume\": null, \"TurnoverRate\": null, \"Open2\": \"-2147483648\", \"Ceiling\": \"8950000\", \"Floor\": \"4850000\", \"Average\": \"6114706\", \"AverageBuy\": \"5975000\", \"AverageSell\": null, \"Aggressor\": \"B\", \"PreClose\": \"6900000\", \"Status\": null, \"Yield\": null, \"Pe\": null, \"Pb\": null, \"TotalAmount\": \"1039500000000\", \"TotalAmountK\": \"1039500000\", \"TotalVolume\": \"170000\", \"TotalVolumeK\": \"170000\", \"TradableEquity\": null, \"TradableAmount\": null, \"Eps\": null, \"LastTrade\": \"0001-01-01T00:00:00\", \"ToLastTrade\": 0, \"Moneyness\": null, \"MaturityDate\": null, \"ExercisePrice\": null, \"IntrinsicValue\": null, \"PSettle\": null, \"Poi\": null, \"Underlying\": null, \"Open0\": null, \"Open1\": \"-2147483648\", \"Basis\": null, \"Settle\": null, \"Symbol\": \"CPALL\", \"SecurityType\": \"CS\", \"LastPriceTime\": -62135596800000, \"Instrument\": null }"
```