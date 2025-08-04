DROP INDEX `structure_note_cash_movement_custcode` ON `structure_note_cash_movement`;
DROP INDEX `structure_note_cash_movement_date_key` ON `structure_note_cash_movement`;
DROP INDEX `structure_note_cash_movement_date_key_custcode` ON `structure_note_cash_movement`;
DROP TABLE IF EXISTS `structure_note_cash_movement`;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20240906094954-add-structure-note-cash-movement-table.js';
