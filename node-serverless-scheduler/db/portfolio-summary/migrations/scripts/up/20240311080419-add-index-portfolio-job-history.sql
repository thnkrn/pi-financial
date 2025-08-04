ALTER TABLE `portfolio_job_history` ADD INDEX `portfolio_job_history_custcode` (`custcode`);
ALTER TABLE `portfolio_job_history` ADD INDEX `portfolio_job_history_custcode_marketing_id` (`custcode`, `marketing_id`);
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;\nINSERT INTO `SequelizeMeta` (`name`) VALUES ('20240311080419-add-index-portfolio-job-history.js');
