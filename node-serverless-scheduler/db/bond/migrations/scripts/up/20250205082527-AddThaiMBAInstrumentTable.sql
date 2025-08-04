CREATE TABLE IF NOT EXISTS `thai_bma_instruments` (`id` CHAR(36) BINARY NOT NULL , `symbol` VARCHAR(255) NOT NULL, `issue_rating` VARCHAR(255), `company_rating` VARCHAR(255), `coupon_type` VARCHAR(255), `coupon_rate` DECIMAL(65,8), `issued_date` DATE, `maturity_date` DATE, `ttm` DECIMAL(65,8), `outstanding` DECIMAL(65,8), `created_at` DATETIME NOT NULL, PRIMARY KEY (`id`)) ENGINE=InnoDB;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'bond_db';
SHOW INDEX FROM `SequelizeMeta`;
INSERT INTO `SequelizeMeta` (`name`) VALUES ('20250205082527-AddThaiMBAInstrumentTable.js');
