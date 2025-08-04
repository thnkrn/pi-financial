CREATE TABLE IF NOT EXISTS `exante_transactions` (`id` CHAR(36) BINARY NOT NULL , `transaction_id` VARCHAR(255) NOT NULL, `account_id` VARCHAR(255) NOT NULL, `symbol_id` VARCHAR(255) NOT NULL, `isin` VARCHAR(255), `operation_type` VARCHAR(255) NOT NULL, `when` DATETIME NOT NULL, `sum` DECIMAL(65,8) NOT NULL, `asset` VARCHAR(255) NOT NULL, `eur_equivalent` DECIMAL(65,8) NOT NULL, `comment` VARCHAR(255), `uuid` CHAR(36) BINARY NOT NULL, `parent_uuid` CHAR(36) BINARY, PRIMARY KEY (`id`)) ENGINE=InnoDB;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'reconcile_db';
SHOW INDEX FROM `SequelizeMeta`;
INSERT INTO `SequelizeMeta` (`name`) VALUES ('20230926072533-create-exante-transactions-table.js');

