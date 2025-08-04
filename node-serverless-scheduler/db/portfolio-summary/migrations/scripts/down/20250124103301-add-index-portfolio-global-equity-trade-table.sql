DROP INDEX `idx_ge_trade_custcode` ON `portfolio_global_equity_trade_daily_snapshot`;
DROP INDEX `idx_ge_trade_trading_account_no` ON `portfolio_global_equity_trade_daily_snapshot`;
DROP INDEX `idx_ge_trade_sharecode_no` ON `portfolio_global_equity_trade_daily_snapshot`;
DROP INDEX `idx_ge_trade_daily_snapshot` ON `portfolio_global_equity_trade_daily_snapshot`;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20250124103301-add-index-portfolio-global-equity-trade-table.js';
