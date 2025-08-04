# Pi.MarketData.SearchAPI

## Prepare Environment

At the root directory, run the following command to start the environment

```bash
docker compose up -d
```

## Prepare Search Data

Search indexer will index data from MongoDB and save to OpenSearch.

1. Check `src/Pi.MarketData.SearchIndexer/appsettings.json` for MongoDB and OpenSearch connection string.
2. Prepare MongoDB data

### Global Equity

Insert the following documents into `ge_stock_market_data.ge_instrument` collection.

```json
{
    "_id" : ObjectId("67349bd9f23514accd3d580d"),
    "ge_instrument_id" : NumberInt(0),
    "symbol" : "AAPL",
    "exchange" : "NASDAQ",
    "name" : "Apple",
    "symbol_type" : "STOCK",
    "currency" : "USD",
    "country" : "US",
    "figi" : "BBG000B9XRY4",
    "isin" : "US0378331005",
    "mic" : "XNAS",
    "standard_ticker" : "AAPL",
    "investment_type" : "EQ",
    "morning_star_stock_status" : "Active",
    "morning_star_suspend_flag" : "0",
    "instrument_type" : "GlobalEquity",
    "instrument_category" : "Global Stocks",
    "venue" : "NASDAQ",
    "updated_at" : ISODate("2024-11-13T12:16:17.554+0000"),
    "exchangeid_ms" : "NAS"
}
```

Add more symbols e.g. `AAPL, META` to the collection if needed.

### SET

Insert the following documents into `set_stock_market_data.instrument` collection.

```json
{
    "_id" : ObjectId("672c1cf1510f2096155a652f"),
    "instrument_id" : NumberInt(0),
    "order_book_id" : NumberInt(65804),
    "instrument_type" : "Equity",
    "instrument_category" : "Thai Stocks",
    "venue" : "SET",
    "exchange" : null,
    "symbol" : "CPALL",
    "friendly_name" : "CPALL_CP ALL",
    "long_name" : "CPALL_CP ALL",
    "logo" : null,
    "security_type" : "CS",
    "exchange_timezone" : null,
    "minimum_order_size" : "100",
    "currency" : "THB",
    "country" : null,
    "market" : null,
    "offset_seconds" : NumberInt(0),
    "is_projected" : false,
    "trading_unit" : "10000",
    "min_bid_unit" : null,
    "multiplier" : "1",
    "initial_margin" : null,
    "exercise_ratio" : "1 : 1",
    "exercise_Date" : null,
    "exercise_price" : "",
    "conversion_ratio" : "1 : 1",
    "days_to_exercise" : null,
    "days_to_last_trade" : null,
    "price_infos" : null,
    "order_books" : null,
    "public_trades" : null,
    "instrument_details" : null,
    "corporate_actions" : null,
    "trading_signs" : null,
    "financials" : null,
    "fund_performances" : null,
    "fund_details" : null,
    "nav_lists" : null,
    "fund_trade_dates" : null,
    "indicators" : null,
    "intermissions" : null,
    "last_trading_date" : "01/01/0001 00:00:00"
}
```

Add more symbols e.g. `ADVANC, BBL` to the collection if needed.

### Sync data to OpenSearch

```bash
cd src/Pi.MarketData.SearchIndexer
dotnet run --
```

## Prepare Environment

```bash
docker compose up -d
dotnet run --
```

## Search for instruments

http://localhost:5095/secure/instrument/search?keyword=AAPL&instrumentType=all

## Swagger UI

http://localhost:5095/swagger/index.html

## Curl

```bash
curl --location 'http://localhost:5095/secure/instrument/search?keyword=AAPL&instrumentType=all' \
--header 'sid: session-id'
```
