CREATE TABLE `SequelizeMeta` (
  `name` VARCHAR(255) NOT NULL,
  PRIMARY KEY (`name`)
);

CREATE TABLE `kbank_dd_register` (
  `id` INT AUTO_INCREMENT NOT NULL,
  `citizen_id` VARCHAR(255) NULL,
  `customer_code` VARCHAR(255) NULL,
  `redirect_url` VARCHAR(255) NULL,
  `registration_ref_code` VARCHAR(255) NULL,
  `remarks` VARCHAR(255) NULL,
  `reg_id` VARCHAR(255) NULL,
  `return_code` VARCHAR(255) NULL,
  `return_message` VARCHAR(255) NULL,
  `return_status` VARCHAR(255) NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
);

CREATE TABLE `kbank_dd_register_result` (
  `id` INT AUTO_INCREMENT NOT NULL,
  `account_no` VARCHAR(255) NULL,
  `espa_id` VARCHAR(255) NULL,
  `external_reference` VARCHAR(255) NULL,
  `id_matching` VARCHAR(255) NULL,
  `payer_short_name` VARCHAR(255) NULL,
  `return_code` VARCHAR(255) NULL,
  `return_message` VARCHAR(255) NULL,
  `return_status` VARCHAR(255) NULL,
  `timestamp` VARCHAR(255) NULL,
  `user_email_matching` VARCHAR(255) NULL,
  `user_mobile_matching` VARCHAR(255) NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
);

CREATE TABLE `kkp_payment_inquiry` (
  `id` INT AUTO_INCREMENT NOT NULL,
  `amount` DOUBLE NULL,
  `external_ref_code` VARCHAR(255) NULL,
  `external_ref_time` VARCHAR(255) NULL,
  `transaction_no` VARCHAR(255) NULL,
  `channel_code` VARCHAR(255) NULL,
  `service_name` VARCHAR(255) NULL,
  `system_code` VARCHAR(255) NULL,
  `transaction_date_time` VARCHAR(255) NULL,
  `transaction_id` VARCHAR(255) NULL,
  `response_code` VARCHAR(255) NULL,
  `response_message` VARCHAR(255) NULL,
  `code` VARCHAR(255) NULL,
  `description` VARCHAR(255) NULL,
  `message` VARCHAR(255) NULL,
  `txn_reference_no` VARCHAR(255) NULL,
  `fee_amount` DOUBLE NULL,
  `status_code` VARCHAR(255) NULL,
  `status_message` VARCHAR(255) NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
);

CREATE TABLE `kkp_payment_result` (
  `id` INT AUTO_INCREMENT NOT NULL,
  `account_no` VARCHAR(255) NULL,
  `amount` DOUBLE NULL,
  `destination_bank_code` VARCHAR(255) NULL,
  `transaction_no` VARCHAR(255) NULL,
  `transaction_ref_code` VARCHAR(255) NULL,
  `customer_code` VARCHAR(255) NULL,
  `product` VARCHAR(255) NULL,
  `channel_code` VARCHAR(255) NULL,
  `service_name` VARCHAR(255) NULL,
  `system_code` VARCHAR(255) NULL,
  `transaction_date_time` VARCHAR(255) NULL,
  `transaction_id` VARCHAR(255) NULL,
  `response_code` VARCHAR(255) NULL,
  `response_message` VARCHAR(255) NULL,
  `effective_date` VARCHAR(255) NULL,
  `rtp_reference_no` VARCHAR(255) NULL,
  `transfer_amount` DOUBLE NULL,
  `receiving_account_no` VARCHAR(255) NULL,
  `receiving_bank_code` VARCHAR(255) NULL,
  `txn_reference_no` VARCHAR(255) NULL,
  `fee_amount` DOUBLE NULL,
  `code` VARCHAR(255) NULL,
  `description` VARCHAR(255) NULL,
  `message` VARCHAR(255) NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
);

CREATE TABLE `kkp_qr_generated_result` (
  `id` INT AUTO_INCREMENT NOT NULL,
  `channel_code` VARCHAR(255) NULL,
  `service_name` VARCHAR(255) NULL,
  `system_code` VARCHAR(255) NULL,
  `transaction_date_time` VARCHAR(255) NULL,
  `transaction_id` VARCHAR(255) NULL,
  `amount` DOUBLE NULL,
  `transaction_no` VARCHAR(255) NULL,
  `transaction_ref_code` VARCHAR(255) NULL,
  `customer_code` VARCHAR(255) NULL,
  `product` VARCHAR(255) NULL,
  `bill_payment_biller_id` VARCHAR(255) NULL,
  `bill_payment_reference1` VARCHAR(255) NULL,
  `bill_payment_reference2` VARCHAR(255) NULL,
  `bill_payment_reference3` VARCHAR(255) NULL,
  `bill_payment_suffix` VARCHAR(255) NULL,
  `bill_payment_tax_id` VARCHAR(255) NULL,
  `credit_transfer_bank_account` VARCHAR(255) NULL,
  `credit_transfer_e_wallet_id` VARCHAR(255) NULL,
  `credit_transfer_mobile_number` VARCHAR(255) NULL,
  `credit_transfer_tax_id` VARCHAR(255) NULL,
  `transaction_amount` VARCHAR(255) NULL,
  `format` VARCHAR(255) NULL,
  `qr_value` VARCHAR(255) NULL,
  `response_code` VARCHAR(255) NULL,
  `response_message` VARCHAR(255) NULL,
  `code` VARCHAR(255) NULL,
  `description` VARCHAR(255) NULL,
  `message` VARCHAR(255) NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
);

CREATE TABLE `kkp_qr_payment_result` (
  `id` INT AUTO_INCREMENT NOT NULL,
  `biller_reference_no` VARCHAR(255) NULL,
  `customer_name` VARCHAR(255) NULL,
  `payment_amount` VARCHAR(255) NULL,
  `payment_date` VARCHAR(255) NULL,
  `payment_type` VARCHAR(255) NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `account_bank` VARCHAR(255) NULL,
  `biller_id` VARCHAR(255) NULL,
  `account_no` VARCHAR(255) NULL,
  `transaction_status` ENUM('PD', 'RV') NULL,
  PRIMARY KEY (`id`)
);

CREATE TABLE `scb_dd_register` (
  `id` INT AUTO_INCREMENT NOT NULL,
  `citizen_id` VARCHAR(255) NULL,
  `customer_code` VARCHAR(255) NULL,
  `redirect_url` VARCHAR(255) NULL,
  `registration_ref_code` VARCHAR(255) NULL,
  `remarks` VARCHAR(255) NULL,
  `merchant_id` VARCHAR(255) NULL,
  `sub_account_id` VARCHAR(255) NULL,
  `code` VARCHAR(255) NULL,
  `description` VARCHAR(255) NULL,
  `details` VARCHAR(255) NULL,
  `web_url` VARCHAR(255) NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
);

CREATE TABLE `scb_dd_register_result` (
  `id` INT AUTO_INCREMENT NOT NULL,
  `account_no` VARCHAR(255) NULL,
  `back_url` VARCHAR(255) NULL,
  `error_code` VARCHAR(255) NULL,
  `ref1` VARCHAR(255) NULL,
  `ref2` VARCHAR(255) NULL,
  `reg_ref` VARCHAR(255) NULL,
  `status_code` VARCHAR(255) NULL,
  `status_desc` VARCHAR(255) NULL,
  `registration_ref_code` VARCHAR(255) NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
);

CREATE TABLE `kkp_bill_payment_result` (
  `id` VARCHAR(36) NOT NULL,
  `biller_id` VARCHAR(100) NULL,
  `transaction_id` VARCHAR(50) NULL,
  `channel_id` VARCHAR(50) NULL,
  `customer_name` VARCHAR(255) NULL,
  `payment_amount` DECIMAL(65,8) NULL,
  `reference_no_1` VARCHAR(100) NULL,
  `reference_no_2` VARCHAR(100) NULL,
  `payment_date` VARCHAR(255) NULL,
  `payment_type` VARCHAR(255) NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `account_bank` VARCHAR(255) NULL,
  `account_no` VARCHAR(100) NULL,
  PRIMARY KEY (`id`)
);