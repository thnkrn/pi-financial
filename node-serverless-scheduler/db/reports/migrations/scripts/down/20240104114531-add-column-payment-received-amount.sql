ALTER TABLE `pending_deposit_snapshot` DROP `payment_received_amount`;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'report_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20240104114531-add-column-payment-received-amount.js';
