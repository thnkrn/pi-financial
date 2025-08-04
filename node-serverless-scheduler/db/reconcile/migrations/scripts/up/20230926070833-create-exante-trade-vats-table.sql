CREATE TABLE IF NOT EXISTS `exante_trade_vats` (`id` CHAR(36) BINARY NOT NULL , `transaction_id` VARCHAR(255), `account_id` VARCHAR(255) NOT NULL, `symbol_id` VARCHAR(255) NOT NULL, `order_id` VARCHAR(255) NOT NULL, `isin` VARCHAR(255), `operation_type` VARCHAR(255) NOT NULL, `when` DATETIME NOT NULL, `sum` DECIMAL(65,8) NOT NULL, `commission_before_vat` DECIMAL(65,8), `other_fees` DECIMAL(65,8), `vat_amount` DECIMAL(65,8), `total_commission` DECIMAL(65,8), `exante_commission_with_other_fees` DECIMAL(65,8), `partner_rebate` DECIMAL(65,8), `asset` VARCHAR(255) NOT NULL, PRIMARY KEY (`id`)) ENGINE=InnoDB;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'reconcile_db'
SHOW INDEX FROM `SequelizeMeta`;
INSERT INTO `SequelizeMeta` (`name`) VALUES ('20230926070833-create-exante-trade-vats-table.js');

