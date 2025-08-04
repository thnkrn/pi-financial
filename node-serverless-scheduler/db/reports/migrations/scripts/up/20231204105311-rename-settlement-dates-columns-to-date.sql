ALTER TABLE `report_history` CHANGE `settlement_date_from` `date_from` DATE NOT NULL;
SHOW FULL COLUMNS FROM `report_history`;
ALTER TABLE `report_history` CHANGE `settlement_date_to` `date_to` DATE DEFAULT NULL;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'report_db';
SHOW INDEX FROM `SequelizeMeta`;
INSERT INTO `SequelizeMeta` (`name`) VALUES ('20231204105311-rename-settlement-dates-columns-to-date.js');
