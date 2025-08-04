ALTER TABLE `bloomberg_equity_closeprice` CHANGE `px_last_eod` `px_last_eod` DECIMAL(10);
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'backoffice_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20250630070950-update-bloomberg-equity-close-price-table-property.js';
