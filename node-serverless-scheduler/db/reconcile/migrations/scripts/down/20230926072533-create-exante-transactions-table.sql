DROP TABLE IF EXISTS `exante_transactions`;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'reconcile_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20230926072533-create-exante-transactions-table.js';

