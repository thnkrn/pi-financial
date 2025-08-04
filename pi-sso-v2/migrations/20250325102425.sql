-- Modify "sync_tokens" table
ALTER TABLE `sync_tokens` ADD COLUMN `account_id` varchar(191) NOT NULL, ADD INDEX `idx_sync_tokens_account_id` (`account_id`);
