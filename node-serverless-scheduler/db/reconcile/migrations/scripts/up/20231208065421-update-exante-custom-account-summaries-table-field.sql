ALTER TABLE `exante_custom_account_summaries` CHANGE `value` `value` DECIMAL(65,8);
ALTER TABLE `exante_custom_account_summaries` CHANGE `value_in_thb` `value_in_thb` DECIMAL(65,8);
ALTER TABLE `exante_custom_account_summaries` CHANGE `ccy` `ccy` VARCHAR(255);
ALTER TABLE `exante_custom_account_summaries` CHANGE `qty` `qty` DECIMAL(65,8);
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'reconcile_db';
SHOW INDEX FROM `SequelizeMeta`;
INSERT INTO `SequelizeMeta` (`name`) VALUES ('20231208065421-update-exante-custom-account-summaries-table-field.js');
