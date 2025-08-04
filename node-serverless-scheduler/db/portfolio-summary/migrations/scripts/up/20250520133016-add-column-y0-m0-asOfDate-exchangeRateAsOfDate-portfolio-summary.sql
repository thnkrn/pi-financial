ALTER TABLE `portfolio_summary_daily_snapshot` ADD `y_0` DECIMAL(65,8);
ALTER TABLE `portfolio_summary_daily_snapshot` ADD `m_0` DECIMAL(65,8);
ALTER TABLE `portfolio_summary_daily_snapshot` ADD `as_of_date` DATE;
ALTER TABLE `portfolio_summary_daily_snapshot` ADD `exchange_rate_as_of` DATE;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;\nINSERT INTO `SequelizeMeta` (`name`) VALUES ('20250520133016-add-column-y0-m0-asOfDate-exchangeRateAsOfDate-portfolio-summary.js');
