DROP INDEX `portfolio_job_history_identification_hash_marketing_id_idx` ON `portfolio_job_history`;
DROP INDEX `portfolio_job_history_identification_hash_idx` ON `portfolio_job_history`;
SELECT CONSTRAINT_NAME as constraint_name,CONSTRAINT_NAME as constraintName,CONSTRAINT_SCHEMA as constraintSchema,CONSTRAINT_SCHEMA as constraintCatalog,TABLE_NAME as tableName,TABLE_SCHEMA as tableSchema,TABLE_SCHEMA as tableCatalog,COLUMN_NAME as columnName,REFERENCED_TABLE_SCHEMA as referencedTableSchema,REFERENCED_TABLE_SCHEMA as referencedTableCatalog,REFERENCED_TABLE_NAME as referencedTableName,REFERENCED_COLUMN_NAME as referencedColumnName FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE (REFERENCED_TABLE_NAME = 'portfolio_job_history' AND REFERENCED_TABLE_SCHEMA = 'portfolio_summary_db' AND REFERENCED_COLUMN_NAME = 'identification_hash') OR (TABLE_NAME = 'portfolio_job_history' AND TABLE_SCHEMA = 'portfolio_summary_db' AND COLUMN_NAME = 'identification_hash' AND REFERENCED_TABLE_NAME IS NOT NULL);
ALTER TABLE `portfolio_job_history` DROP `identification_hash`;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20240605115311-add-column-id-no-portfolio-job-history.js';
