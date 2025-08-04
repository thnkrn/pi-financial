DROP INDEX `idx_cash_trading_account_no` ON `portfolio_cash_daily_snapshot`;
DROP INDEX `idx_ge_trading_account_no` ON `portfolio_global_equity_daily_snapshot`;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20250217090033-add-index-portfolio-cash-and-ge-table.js';
