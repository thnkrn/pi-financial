DROP INDEX `pending_deposit_snapshot_updated_at` ON `pending_deposit_snapshot`;
DROP INDEX `pending_deposit_snapshot_deleted_at` ON `pending_deposit_snapshot`;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'report_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20231116105053-add-index-updated-at-deleted-at-in-snapshot.js';

