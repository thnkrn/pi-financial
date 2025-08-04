-- Create "mt4" table
CREATE TABLE `mt4` (
  `id` char(36) NOT NULL,
  `trading_account` varchar(15) NOT NULL,
  `effective_date` char(8) NOT NULL,
  `service_type` char(1) NOT NULL,
  `package_type` char(3) NOT NULL,
  `is_exported` bool NULL DEFAULT 0,
  `created_at` datetime(3) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `uni_mt4_trading_account` (`trading_account`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
-- Create "mt5" table
CREATE TABLE `mt5` (
  `id` char(36) NOT NULL,
  `trading_account` varchar(15) NOT NULL,
  `effective_date` char(8) NOT NULL,
  `is_exported` bool NULL DEFAULT 0,
  `created_at` datetime(3) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `uni_mt5_trading_account` (`trading_account`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
