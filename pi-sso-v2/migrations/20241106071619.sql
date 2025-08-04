-- Create "biometrics" table
CREATE TABLE `biometrics` (
  `id` varchar(36) NOT NULL,
  `token` longtext NOT NULL,
  `device_id` longtext NOT NULL,
  `user_id` longtext NOT NULL,
  `account_id` longtext NOT NULL,
  `is_active` bool NOT NULL,
  `created_at` datetime(3) NULL,
  `updated_at` datetime(3) NULL,
  `deleted_at` datetime(3) NULL,
  PRIMARY KEY (`id`),
  INDEX `idx_biometrics_deleted_at` (`deleted_at`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
