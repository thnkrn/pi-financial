-- Create "sync_tokens" table
CREATE TABLE `sync_tokens` (
  `id` varchar(36) NOT NULL,
  `user_id` varchar(191) NOT NULL,
  `is_use` bool NULL DEFAULT 0,
  `created_at` datetime(3) NULL,
  `used_at` datetime(3) NULL,
  PRIMARY KEY (`id`),
  INDEX `idx_sync_tokens_user_id` (`user_id`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
