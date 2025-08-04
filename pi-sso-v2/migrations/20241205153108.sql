-- Create "send_link_accounts" table
CREATE TABLE `send_link_accounts` (
  `id` varchar(36) NOT NULL,
  `email` longtext NULL,
  `custcode` varchar(191) NOT NULL,
  `user_id` longtext NULL,
  `created_at` datetime(3) NULL,
  `used_at` datetime(3) NULL,
  `is_used` bool NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `uni_send_link_accounts_custcode` (`custcode`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
