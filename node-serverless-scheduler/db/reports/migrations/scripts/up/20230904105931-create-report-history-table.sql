CREATE TABLE IF NOT EXISTS `report_history` (`id` CHAR(36) BINARY NOT NULL , `report_name` VARCHAR(255) NOT NULL, `user_name` VARCHAR(255) NOT NULL, `status` VARCHAR(255) NOT NULL, `settlement_date` DATE NOT NULL, `file_path` VARCHAR(2048), `created_at` DATETIME, `updated_at` DATETIME, PRIMARY KEY (`id`)) ENGINE=InnoDB;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'report_db';
SHOW INDEX FROM `SequelizeMeta`;
INSERT INTO `SequelizeMeta` (`name`) VALUES ('20230904105931-create-report-history-table.js');

