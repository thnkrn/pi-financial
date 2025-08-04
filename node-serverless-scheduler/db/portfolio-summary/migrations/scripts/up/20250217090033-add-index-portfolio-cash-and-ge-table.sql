ALTER TABLE `portfolio_cash_daily_snapshot` ADD INDEX `idx_cash_trading_account_no` (`trading_account_no`);
ALTER TABLE `portfolio_global_equity_daily_snapshot` ADD INDEX `idx_ge_trading_account_no` (`trading_account_no`);
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;\nINSERT INTO `SequelizeMeta` (`name`) VALUES ('20250217090033-add-index-portfolio-cash-and-ge-table.js');
