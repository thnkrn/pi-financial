CREATE TABLE IF NOT EXISTS `pending_deposit_snapshot` (`id` CHAR(36) BINARY NOT NULL , `payment_received_datetime` DATETIME NOT NULL, `sender_account` VARCHAR(255) NOT NULL, `bank_code` VARCHAR(255) NOT NULL, `sender_channel` VARCHAR(255) NOT NULL, `transaction_number` VARCHAR(255) NOT NULL, `customer_bank_name` VARCHAR(255) NOT NULL, `created_at` DATETIME, `deleted_at` DATETIME, PRIMARY KEY (`id`)) ENGINE=InnoDB;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'report_db';
SHOW INDEX FROM `SequelizeMeta`;
INSERT INTO `SequelizeMeta` (`name`) VALUES ('20231009153538-create-pending-deposit-snapshot-table.js');
