ALTER TABLE `portfolio_global_equity_depositwithdraw_daily_snapshot` ADD INDEX `idx_ge_depositwithdraw_custcode` (`custcode`);
ALTER TABLE `portfolio_global_equity_depositwithdraw_daily_snapshot` ADD INDEX `idx_ge_depositwithdraw_type` (`type`);
ALTER TABLE `portfolio_global_equity_depositwithdraw_daily_snapshot` ADD INDEX `idx_ge_depositwithdraw_trading_account_no` (`trading_account_no`);
ALTER TABLE `portfolio_global_equity_depositwithdraw_daily_snapshot` ADD INDEX `idx_ge_depositwithdraw_daily_snapshot` (`date_key`, `trading_account_no`, `type`, `custcode`);
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;\nINSERT INTO `SequelizeMeta` (`name`) VALUES ('20250123033016-add-index-portfolio-global-equity-depositwithdraw-table.js');
