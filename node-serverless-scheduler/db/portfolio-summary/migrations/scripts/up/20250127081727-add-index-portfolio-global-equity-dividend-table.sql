ALTER TABLE `portfolio_global_equity_dividend_daily_snapshot` ADD INDEX `idx_ge_dividend_custcode` (`custcode`);
ALTER TABLE `portfolio_global_equity_dividend_daily_snapshot` ADD INDEX `idx_ge_dividend_trading_account_no` (`trading_account_no`);
ALTER TABLE `portfolio_global_equity_dividend_daily_snapshot` ADD INDEX `idx_ge_dividend_sharecode` (`sharecode`);
ALTER TABLE `portfolio_global_equity_dividend_daily_snapshot` ADD INDEX `idx_ge_dividend_daily_snapshot` (`date_key`, `trading_account_no`, `sharecode`, `custcode`);
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;\nINSERT INTO `SequelizeMeta` (`name`) VALUES ('20250127081727-add-index-portfolio-global-equity-dividend-table.js');
