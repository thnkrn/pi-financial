DROP INDEX `idx_ge_dividend_daily_snapshot_datekey_custcode` ON `portfolio_global_equity_dividend_daily_snapshot`;
DROP TABLE IF EXISTS `portfolio_mutual_fund_dividend_daily_transaction`;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20250721090134-create-mutual-fund-dividend-table.js';
