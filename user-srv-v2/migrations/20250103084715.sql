-- Create "watchlists" table
CREATE TABLE `watchlists` (
  `id` varchar(36) NOT NULL,
  `user_id` varchar(36) NULL,
  `venue` varchar(255) NULL,
  `symbol` varchar(255) NULL,
  `sequence` bigint NULL,
  `created_at` datetime(3) NULL,
  `updated_at` datetime(3) NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `idx_user_venue_symbol` (`user_id`, `venue`, `symbol`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
