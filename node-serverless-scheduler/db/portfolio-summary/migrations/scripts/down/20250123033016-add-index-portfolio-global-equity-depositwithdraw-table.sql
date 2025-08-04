DROP INDEX `idx_ge_depositwithdraw_custcode` ON `portfolio_global_equity_depositwithdraw_daily_snapshot`;
DROP INDEX `idx_ge_depositwithdraw_type` ON `portfolio_global_equity_depositwithdraw_daily_snapshot`;
DROP INDEX `idx_ge_depositwithdraw_trading_account_no` ON `portfolio_global_equity_depositwithdraw_daily_snapshot`;
DROP INDEX `idx_ge_depositwithdraw_daily_snapshot` ON `portfolio_global_equity_depositwithdraw_daily_snapshot`;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20250123033016-add-index-portfolio-global-equity-depositwithdraw-table.js';
