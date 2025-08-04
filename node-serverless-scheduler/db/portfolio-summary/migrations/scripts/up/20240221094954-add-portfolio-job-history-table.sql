CREATE TABLE IF NOT EXISTS `portfolio_job_history` (`id` CHAR(36) BINARY NOT NULL , `custcode` VARCHAR(255) NOT NULL, `marketing_id` VARCHAR(255) NOT NULL, `metadata` VARCHAR(255) NOT NULL, `send_date` DATE NOT NULL, `status` VARCHAR(255) NOT NULL, `created_at` DATETIME, PRIMARY KEY (`id`)) ENGINE=InnoDB;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;\nINSERT INTO `SequelizeMeta` (`name`) VALUES ('20240221094954-add-portfolio-job-history-table.js');
