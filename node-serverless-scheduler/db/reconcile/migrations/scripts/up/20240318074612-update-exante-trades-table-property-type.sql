ALTER TABLE `exante_trades` CHANGE `quantity` `quantity` DECIMAL(65,8);
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'reconcile_db';
SHOW INDEX FROM `SequelizeMeta`;
INSERT INTO `SequelizeMeta` (`name`) VALUES ('20240318074612-update-exante-trades-table-property-type.js');
