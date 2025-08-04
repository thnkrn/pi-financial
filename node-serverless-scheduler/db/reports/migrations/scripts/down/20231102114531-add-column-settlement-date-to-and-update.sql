ALTER TABLE `report_history` CHANGE `settlement_date_from` `settlement_date` DATE NOT NULL;
SELECT CONSTRAINT_NAME as constraint_name,CONSTRAINT_NAME as constraintName,CONSTRAINT_SCHEMA as constraintSchema,CONSTRAINT_SCHEMA as constraintCatalog,TABLE_NAME as tableName,TABLE_SCHEMA as tableSchema,TABLE_SCHEMA as tableCatalog,COLUMN_NAME as columnName,REFERENCED_TABLE_SCHEMA as referencedTableSchema,REFERENCED_TABLE_SCHEMA as referencedTableCatalog,REFERENCED_TABLE_NAME as referencedTableName,REFERENCED_COLUMN_NAME as referencedColumnName FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE (REFERENCED_TABLE_NAME = 'report_history' AND REFERENCED_TABLE_SCHEMA = 'report_db' AND REFERENCED_COLUMN_NAME = 'settlement_date_to') OR (TABLE_NAME = 'report_history' AND TABLE_SCHEMA = 'report_db' AND COLUMN_NAME = 'settlement_date_to' AND REFERENCED_TABLE_NAME IS NOT NULL)
ALTER TABLE `report_history` DROP `settlement_date_to`;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'report_db'
SHOW INDEX FROM `SequelizeMeta`
DELETE FROM `SequelizeMeta` WHERE `name` = '20231102114531-add-column-settlement-date-to-and-update.js'

