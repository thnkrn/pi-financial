-- Create "user_hierarchies" table
CREATE TABLE `user_hierarchies` (
  `id` varchar(36) NOT NULL,
  `user_id` varchar(36) NULL,
  `sub_user_id` varchar(36) NULL,
  `created_at` datetime(3) NULL,
  `updated_at` datetime(3) NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `idx_user_sub_user` (`user_id`, `sub_user_id`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
