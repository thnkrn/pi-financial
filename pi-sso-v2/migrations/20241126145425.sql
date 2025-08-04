-- Modify "login_with2_fa_sections" table
ALTER TABLE `login_with2_fa_sections` RENAME COLUMN `ref_id` TO `device_id`, ADD COLUMN `account_id` longtext NULL;
