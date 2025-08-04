-- Create "login_with2_fa_sections" table
CREATE TABLE `login_with2_fa_sections` (
  `id` varchar(36) NOT NULL,
  `user_id` longtext NULL,
  `phone_number` longtext NULL,
  `ref_code` longtext NULL,
  `ref_id` longtext NULL,
  `is_verify` bool NULL,
  `created_at` datetime(3) NULL,
  `expired_at` datetime(3) NULL,
  `updated_at` datetime(3) NULL,
  `deleted_at` datetime(3) NULL,
  PRIMARY KEY (`id`),
  INDEX `idx_login_with2_fa_sections_deleted_at` (`deleted_at`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
