CREATE TABLE IF NOT EXISTS `portfolio_exchange_rate_daily_snapshot` (`currency` VARCHAR(255) NOT NULL , `exchange_rate` DECIMAL(65,8) NOT NULL, `date_key` DATE NOT NULL , `created_at` DATETIME, PRIMARY KEY (`currency`, `date_key`)) ENGINE=InnoDB;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;\nINSERT INTO `SequelizeMeta` (`name`) VALUES ('20240311043943-create-portfolio-exchange-rate-daily-snapshot-table.js');
