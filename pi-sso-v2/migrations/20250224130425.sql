-- Modify "generate_otp_to_email_for_setups" table
ALTER TABLE `generate_otp_to_email_for_setups` ADD COLUMN `flow` longtext NULL;
-- Modify "generate_otp_to_phone_for_setups" table
ALTER TABLE `generate_otp_to_phone_for_setups` ADD COLUMN `flow` longtext NULL;
