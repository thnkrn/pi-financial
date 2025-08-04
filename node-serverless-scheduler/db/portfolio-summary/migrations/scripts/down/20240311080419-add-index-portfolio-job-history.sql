DROP INDEX `portfolio_job_history_custcode` ON `portfolio_job_history`;
DROP INDEX `portfolio_job_history_custcode_marketing_id` ON `portfolio_job_history`;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20240311080419-add-index-portfolio-job-history.js';
