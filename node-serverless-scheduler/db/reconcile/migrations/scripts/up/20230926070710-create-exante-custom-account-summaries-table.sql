CREATE TABLE IF NOT EXISTS `exante_custom_account_summaries` (`id` CHAR(36) BINARY NOT NULL , `date` DATETIME NOT NULL, `account` VARCHAR(255) NOT NULL, `instrument` VARCHAR(255) NOT NULL, `iso` VARCHAR(255) NOT NULL, `instrument_name` VARCHAR(255), `qty` INTEGER, `avg_price` DECIMAL(65,8), `price` DECIMAL(65,8), `ccy` DECIMAL(65,8), `p_and_l` DECIMAL(65,8), `p_and_l_in_thb` DECIMAL(65,8), `p_and_l_percent` DECIMAL(65,8), `value` DECIMAL(65,8) NOT NULL, `value_in_thb` DECIMAL(65,8) NOT NULL, `daily_p_and_l` DECIMAL(65,8), `daily_p_and_l_in_thb` DECIMAL(65,8), `daily_p_and_l_percent` DECIMAL(65,8), `isin` VARCHAR(255), PRIMARY KEY (`id`)) ENGINE=InnoDB;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'reconcile_db';
SHOW INDEX FROM `SequelizeMeta`;
INSERT INTO `SequelizeMeta` (`name`) VALUES ('20230926070710-create-exante-custom-account-summaries-table.js');

