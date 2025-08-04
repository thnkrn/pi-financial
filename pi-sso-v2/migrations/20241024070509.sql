-- Create "accounts" table
CREATE TABLE `accounts` (
  `id` varchar(36) NOT NULL,
  `username` varchar(191) NOT NULL,
  `username_hash` varchar(191) NOT NULL,
  `password` longtext NULL,
  `salt_password` longtext NULL,
  `pin` longtext NULL,
  `salt_pin` longtext NULL,
  `failed_login_attempts` bigint NULL DEFAULT 0,
  `is_locked` bool NULL DEFAULT 0,
  `user_id` longtext NULL,
  `created_at` datetime(3) NULL,
  `updated_at` datetime(3) NULL,
  `deleted_at` datetime(3) NULL,
  PRIMARY KEY (`id`),
  INDEX `idx_accounts_deleted_at` (`deleted_at`),
  UNIQUE INDEX `uni_accounts_username` (`username`),
  UNIQUE INDEX `uni_accounts_username_hash` (`username_hash`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
-- Create "activity_logs" table
CREATE TABLE `activity_logs` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `created_at` datetime(3) NULL,
  `updated_at` datetime(3) NULL,
  `deleted_at` datetime(3) NULL,
  `account_id` char(36) NULL,
  `action_type` varchar(255) NULL,
  `description` text NULL,
  `ip_address` varchar(45) NULL,
  `user_agent` varchar(255) NULL,
  `extra_data` varchar(255) NULL,
  PRIMARY KEY (`id`),
  INDEX `idx_activity_logs_deleted_at` (`deleted_at`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
-- Create "password_reset_requests" table
CREATE TABLE `password_reset_requests` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `account_id` longtext NOT NULL,
  `reset_token` varchar(191) NOT NULL,
  `created_at` datetime(3) NULL,
  `expires_at` datetime(3) NOT NULL,
  `used_at` datetime(3) NULL,
  `is_used` bool NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `uni_password_reset_requests_reset_token` (`reset_token`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
