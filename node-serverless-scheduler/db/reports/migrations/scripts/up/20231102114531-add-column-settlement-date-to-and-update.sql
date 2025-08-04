ALTER TABLE `report_history` ADD `settlement_date_to` DATE;
UPDATE report_history SET settlement_date_to = settlement_date;
SHOW FULL COLUMNS FROM `report_history`;
ALTER TABLE `report_history` CHANGE `settlement_date` `settlement_date_from` DATE NOT NULL;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'report_db';
SHOW INDEX FROM `SequelizeMeta`;
INSERT INTO `SequelizeMeta` (`name`) VALUES ('20231102114531-add-column-settlement-date-to-and-update.js');

