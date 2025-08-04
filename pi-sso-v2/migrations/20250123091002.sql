-- Create "generate_otp_to_phone_for_setups" table
CREATE TABLE `generate_otp_to_phone_for_setups` (
  `id` varchar(36) NOT NULL,
  `phone` longtext NULL,
  `hashed_phone` longtext NULL,
  `created_at` datetime(3) NULL,
  `updated_at` datetime(3) NULL,
  `ref_code` longtext NULL,
  `expires_at` datetime(3) NULL,
  `used_at` datetime(3) NULL,
  `is_used` bool NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
