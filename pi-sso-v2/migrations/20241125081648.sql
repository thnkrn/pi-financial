-- Create "request_records" table
CREATE TABLE `request_records` (
  `id` varchar(36) NOT NULL,
  `user_id` varchar(36) NULL,
  `device_id` varchar(36) NULL,
  `mobile_ref` longtext NULL,
  `email_ref` longtext NULL,
  `mobile_completed_at` datetime(3) NULL,
  `email_completed_at` datetime(3) NULL,
  `is_used` bool NULL,
  `expired_at` datetime(3) NULL,
  `created_at` datetime(3) NULL,
  `updated_at` datetime(3) NULL,
  PRIMARY KEY (`id`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
