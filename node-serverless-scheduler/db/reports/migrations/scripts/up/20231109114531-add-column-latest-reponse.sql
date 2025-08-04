ALTER TABLE `pending_deposit_snapshot` ADD `latest_response` VARCHAR(255);
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'report_db';
SHOW INDEX FROM `SequelizeMeta`;
INSERT INTO `SequelizeMeta` (`name`) VALUES ('20231102114531-add-column-settlement-date-to-and-update.js');

