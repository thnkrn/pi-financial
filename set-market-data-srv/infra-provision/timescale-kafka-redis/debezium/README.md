# Debeziaum TimescaleDB Connector

## Configure Timescale DB

```sql
--- CDC publisher ---
ALTER SYSTEM SET wal_level = logical;
DROP PUBLICATION dbz_publication;
CREATE PUBLICATION dbz_publication FOR ALL TABLES WITH (publish = 'insert, update');

```

## Configure Connector

### Create

```bash
curl -X POST -H "Content-Type: application/json" --data @timescaledb-market-connector.json http://localhost:8083/connectors
```

### Update

Adjust configuration in `timescaledb-market-connector.json` then

```bash
curl -X PUT -H "Content-Type: application/json" --data @timescaledb-market-connector.json http://localhost:8083/connectors/timescaledb-market-connector/config
```

### Delete

```bash
curl -X DELETE http://localhost:8083/connectors/timescaledb-market-connector
```