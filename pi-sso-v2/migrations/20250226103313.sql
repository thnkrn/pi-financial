-- Modify "accounts" table
ALTER TABLE `accounts` RENAME COLUMN `failed_login_attempts` TO `failed_password_attempts`, ADD COLUMN `failed_pin_attempts` bigint NULL DEFAULT 0;
