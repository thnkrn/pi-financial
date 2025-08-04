ALTER TABLE `pending_deposit_snapshot` ADD INDEX `pending_deposit_snapshot_updated_at` (`updated_at`);
ALTER TABLE `pending_deposit_snapshot` ADD INDEX `pending_deposit_snapshot_deleted_at` (`deleted_at`);
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'report_db';
SHOW INDEX FROM `SequelizeMeta`;
INSERT INTO `SequelizeMeta` (`name`) VALUES ('20231116105053-add-index-updated-at-deleted-at-in-snapshot.js');

