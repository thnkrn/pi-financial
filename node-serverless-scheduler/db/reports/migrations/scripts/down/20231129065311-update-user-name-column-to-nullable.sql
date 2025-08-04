ALTER TABLE `report_history` CHANGE `user_name` `user_name` VARCHAR(255) NOT NULL;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'report_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20231129065311-update-user-name-column-to-nullable.js';
