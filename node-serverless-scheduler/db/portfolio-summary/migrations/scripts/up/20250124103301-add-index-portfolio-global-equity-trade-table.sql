ALTER TABLE `portfolio_global_equity_trade_daily_snapshot` ADD INDEX `idx_ge_trade_custcode` (`custcode`);
ALTER TABLE `portfolio_global_equity_trade_daily_snapshot` ADD INDEX `idx_ge_trade_trading_account_no` (`trading_account_no`);
ALTER TABLE `portfolio_global_equity_trade_daily_snapshot` ADD INDEX `idx_ge_trade_sharecode_no` (`sharecode`);
ALTER TABLE `portfolio_global_equity_trade_daily_snapshot` ADD INDEX `idx_ge_trade_daily_snapshot` (`date_key`, `trading_account_no`, `sharecode`, `custcode`);
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;\nINSERT INTO `SequelizeMeta` (`name`) VALUES ('20250124103301-add-index-portfolio-global-equity-trade-table.js');
