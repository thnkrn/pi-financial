ALTER TABLE `exante_custom_account_summaries` CHANGE `value` `value` DECIMAL(65,8) NOT NULL;
ALTER TABLE `exante_custom_account_summaries` CHANGE `value_in_thb` `value_in_thb` DECIMAL(65,8) NOT NULL;
ALTER TABLE `exante_custom_account_summaries` CHANGE `ccy` `ccy` DECIMAL(65,8);
ALTER TABLE `exante_custom_account_summaries` CHANGE `qty` `qty` INTEGER;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'reconcile_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20231208065421-update-exante-custom-account-summaries-table-field.js';
