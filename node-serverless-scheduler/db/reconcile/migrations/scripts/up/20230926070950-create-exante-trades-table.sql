CREATE TABLE IF NOT EXISTS `exante_trades` (`id` CHAR(36) BINARY NOT NULL , `time` DATETIME NOT NULL, `account_id` VARCHAR(255) NOT NULL, `side` VARCHAR(255) NOT NULL, `symbol_id` VARCHAR(255) NOT NULL, `isin` VARCHAR(255) NOT NULL, `type` VARCHAR(255) NOT NULL, `price` DECIMAL(65,8) NOT NULL, `currency` VARCHAR(255) NOT NULL, `quantity` INTEGER NOT NULL, `commission` DECIMAL(65,8) NOT NULL, `commission_currency` VARCHAR(255) NOT NULL, `p_and_l` DECIMAL(65,8) NOT NULL, `traded_volume` DECIMAL(65,8) NOT NULL, `order_id` VARCHAR(255) NOT NULL, `order_pos` INTEGER NOT NULL, `value_date` DATE NOT NULL, `uti` VARCHAR(255), `trade_type` VARCHAR(255) NOT NULL, PRIMARY KEY (`id`)) ENGINE=InnoDB;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'reconcile_db';
SHOW INDEX FROM `SequelizeMeta`;
INSERT INTO `SequelizeMeta` (`name`) VALUES ('20230926070950-create-exante-trades-table.js');

