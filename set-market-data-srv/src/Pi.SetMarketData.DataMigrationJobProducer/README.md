# DataMigrationJobProducer

**Example Usage for OHLC:**

```bash
dotnet run -- "ohlc" "{from-date}" "{to-date}" "{venue}" "{symbol},{symbol},...,{symbol}"
```

e.g.

```bash
dotnet run -- "ohlc" "2015-01-01" "2024-07-01" "Equity" "TISCO,CPALL,BGRIM,KBANK,HMPRO,SET50"
```

**Example Usage for Indicators:**

```bash
dotnet run -- "indicator" "{from-date}" "{to-date}" "{venue}" "{symbol},{symbol},...,{symbol}" "{timeframe}"
```

**Example Usage for Indicators with timeframe:**

```bash
dotnet run -- "indicator" "2015-01-01" "2024-07-01" "Equity" "TISCO,CPALL,BGRIM,KBANK,HMPRO,SET50" "1min"
```
