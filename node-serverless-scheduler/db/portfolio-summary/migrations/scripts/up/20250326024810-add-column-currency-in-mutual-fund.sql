ALTER TABLE `portfolio_mutual_fund_daily_snapshot` ADD `currency` VARCHAR(255);
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;\nINSERT INTO `SequelizeMeta` (`name`) VALUES ('20250326024810-add-column-currency-in-mutual-fund.js');
