ALTER TABLE `portfolio_job_history` ADD `identification_hash` VARCHAR(255);
ALTER TABLE `portfolio_job_history` ADD INDEX `portfolio_job_history_identification_hash_idx` (`identification_hash`);
ALTER TABLE `portfolio_job_history` ADD INDEX `portfolio_job_history_identification_hash_marketing_id_idx` (`identification_hash`, `marketing_id`);
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;\nINSERT INTO `SequelizeMeta` (`name`) VALUES ('20240605115311-add-column-id-no-portfolio-job-history.js');
