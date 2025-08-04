## Apply Migration to TimescaleDB
```bash
dotnet ef migrations add InitialCreate # To undo this action, use 'ef migrations remove'
dotnet ef database update 
```

## Issue
- Cannot create view with Dotnet EF (Need to modify Migrations file which is auto generate code by dotnet ef & part of gitignore)
- Still need to be directly execute RAW SQL to the Database

### Example
```sql
DROP MATERIALIZED VIEW candle_1_min;

CREATE MATERIALIZED VIEW candle_1_min WITH (timescaledb.continuous)
AS
SELECT
    time_bucket('1 minute', date_time) AS bucket,
    symbol,
    venue,
    FIRST(price, date_time) AS "open",
    MAX(price) AS "high",
    MIN(price) AS "low",
    LAST(price, date_time) AS "close",
    SUM(volume) AS "volume",
    SUM(price * volume) AS "amount"
FROM realtime_market_data
GROUP BY bucket, symbol, venue;

CREATE INDEX idx_candle_1_min_symbol_venue_date_time 
ON candle_1_min (symbol, venue, bucket DESC);
```

### Aggregate policy
```sql
SELECT remove_continuous_aggregate_policy('candle_1_min');

SELECT add_continuous_aggregate_policy(
    'candle_1_min', 
    start_offset => INTERVAL '10 mins', 
    end_offset => INTERVAL '1 min', 
    schedule_interval => INTERVAL '1 minute'
);
```

## Reference
- [Completed SQL Initial Script](https://appsynth.atlassian.net/wiki/spaces/PI/pages/2705162298/Completed+SQL+Initial+Script)
- [Timescale DB Notes](https://appsynth.atlassian.net/wiki/spaces/PI/pages/2643329043/Timescale+DB+Notes)