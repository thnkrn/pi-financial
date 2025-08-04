-- Modify "generate_otp_to_email_for_setups" table
ALTER TABLE `generate_otp_to_email_for_setups` ADD COLUMN `hashed_email` longtext NULL;
