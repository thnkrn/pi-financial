-- Create "generate_otp_to_email_for_setups" table
CREATE TABLE `generate_otp_to_email_for_setups` (
  `id` varchar(36) NOT NULL,
  `email` longtext NULL,
  `custcode` varchar(191) NOT NULL,
  `created_at` datetime(3) NULL,
  `updated_at` datetime(3) NULL,
  `ref_code` longtext NULL,
  `otp_code` longtext NULL,
  `expires_at` datetime(3) NULL,
  `used_at` datetime(3) NULL,
  `is_used` bool NULL DEFAULT 0,
  `link_account_id` longtext NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `uni_generate_otp_to_email_for_setups_custcode` (`custcode`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
