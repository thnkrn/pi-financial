CREATE TABLE IF NOT EXISTS `__WalletDbContext` (
    `migration_id` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `product_version` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk___wallet_db_context` PRIMARY KEY (`migration_id`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE TABLE `cash_deposit_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `transaction_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `user_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `account_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `purpose` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `requested_amount` decimal(65,30) NOT NULL,
        `payment_received_date_time` datetime(6) NULL,
        `bank_name` varchar(50) COLLATE utf8mb4_0900_ai_ci NULL,
        `failed_reason` varchar(200) COLLATE utf8mb4_0900_ai_ci NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL DEFAULT '2023-10-16 19:21:12.036835',
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_cash_deposit_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE TABLE `cash_withdraw_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `transaction_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `user_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `account_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_fee` decimal(65,30) NULL,
        `requested_amount` decimal(65,30) NOT NULL,
        `bank_name` varchar(50) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_code` varchar(10) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_account_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `otp_request_ref` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `otp_request_id` char(36) COLLATE ascii_general_ci NULL,
        `otp_confirmed_date_time` datetime(6) NULL,
        `device_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `failed_reason` varchar(200) COLLATE utf8mb4_0900_ai_ci NULL,
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL DEFAULT '2023-10-16 19:21:12.039134',
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_cash_withdraw_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE TABLE `deposit_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `transaction_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `user_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `account_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `purpose` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `requested_amount` decimal(65,30) NOT NULL,
        `bank_fee` decimal(65,30) NULL,
        `payment_received_date_time` datetime(6) NULL,
        `payment_received_amount` decimal(65,30) NULL,
        `amount` decimal(65,30) NULL,
        `customer_name` varchar(200) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_account_name` varchar(200) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_name` varchar(50) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_code` varchar(10) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_account_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `deposit_qr_generate_date_time` datetime(6) NULL,
        `qr_code_expired_time_in_minute` int NOT NULL,
        `qr_transaction_no` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `qr_value` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `qr_transaction_ref` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `failed_reason` varchar(200) COLLATE utf8mb4_0900_ai_ci NULL,
        `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL DEFAULT '2023-10-16 19:21:12.03439',
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `requester_device_id` char(36) COLLATE ascii_general_ci NULL,
        CONSTRAINT `pk_deposit_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE TABLE `freewill_request_logs` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `refer_id` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `trans_id` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `request` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `response` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `callback` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `type` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `created_at` datetime(6) NOT NULL DEFAULT '2023-10-16 19:21:12.030354',
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_freewill_request_logs` PRIMARY KEY (`id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE TABLE `global_manual_allocation_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transaction_no` varchar(255) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `global_account` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `currency` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `amount` decimal(65,30) NOT NULL,
        `initiate_transfer_at` datetime(6) NULL,
        `completed_transfer_at` datetime(6) NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL,
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `request_type` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        CONSTRAINT `pk_global_manual_allocation_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE TABLE `global_wallet_transaction_histories` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `transaction_no` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transaction_type` int NOT NULL,
        `user_id` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_id` bigint NOT NULL,
        `customer_code` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `global_account` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `current_state` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `requested_amount` decimal(65,30) NOT NULL,
        `requested_currency` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `requested_amount_with_currency` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `requested_fx_amount` decimal(65,30) NOT NULL,
        `requested_fx_currency` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `requested_fx_amount_with_currency` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `payment_received_amount` decimal(65,30) NULL,
        `payment_received_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transaction_fee` decimal(65,30) NULL,
        `fx_transaction_id` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `fx_initiate_request_date_time` datetime(6) NULL,
        `fx_confirmed_date_time` datetime(6) NULL,
        `fx_confirmed_exchange_rate` decimal(65,30) NULL,
        `fx_confirmed_amount` decimal(65,30) NULL,
        `fx_confirmed_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `fx_confirmed_amount_with_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_from_account` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_amount` decimal(65,30) NULL,
        `transfer_to_account` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_amount_with_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_request_time` datetime(6) NULL,
        `transfer_complete_time` datetime(6) NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `refund_amount` decimal(65,30) NULL,
        `net_amount` decimal(65,30) NULL,
        `requester_device_id` char(36) COLLATE ascii_general_ci NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL,
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_global_wallet_transaction_histories` PRIMARY KEY (`id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE TABLE `global_wallet_transfer_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `transaction_no` varchar(255) COLLATE utf8mb4_0900_ai_ci NULL,
        `transaction_type` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `user_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_id` bigint NOT NULL,
        `customer_code` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `global_account` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `current_state` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `requested_amount` decimal(65,30) NOT NULL,
        `requested_currency` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `requested_fx_amount` decimal(65,30) NOT NULL,
        `requested_fx_currency` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `payment_received_amount` decimal(65,30) NULL,
        `payment_received_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `fx_initiate_request_date_time` datetime(6) NULL,
        `fx_transaction_id` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `fx_confirmed_amount` decimal(65,30) NULL,
        `fx_confirmed_exchange_rate` decimal(65,30) NULL,
        `fx_confirmed_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `fx_confirmed_date_time` datetime(6) NULL,
        `transfer_from_account` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_amount` decimal(65,30) NULL,
        `transfer_fee` decimal(65,30) NULL,
        `transfer_to_account` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_request_time` datetime(6) NULL,
        `transfer_complete_time` datetime(6) NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `refund_amount` decimal(65,30) NULL,
        `net_amount` decimal(65,30) NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL,
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `requester_device_id` char(36) COLLATE ascii_general_ci NULL,
        CONSTRAINT `pk_global_wallet_transfer_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE TABLE `online_direct_debit_registrations` (
        `id` varchar(20) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `user_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `bank` varchar(6) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `created_at` datetime(6) NOT NULL,
        `is_success` tinyint(1) NOT NULL,
        `external_status_code` varchar(10) COLLATE utf8mb4_0900_ai_ci NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_online_direct_debit_registrations` PRIMARY KEY (`id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE TABLE `refund_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `deposit_transaction_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `transaction_no` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `user_id` varchar(100) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `account_code` varchar(100) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_code` varchar(100) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `amount` decimal(65,30) NOT NULL,
        `bank_account_no` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_name` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_code` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_fee` decimal(65,30) NULL,
        `refunded_at` datetime(6) NULL,
        `created_at` datetime(6) NOT NULL,
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        CONSTRAINT `pk_refund_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE TABLE `transaction_histories` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `transaction_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `transaction_type` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `user_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `account_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `global_account` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `purpose` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `state` varchar(100) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `requested_amount` decimal(65,30) NOT NULL,
        `bank_fee` decimal(65,30) NULL,
        `transaction_date_time` datetime(6) NULL,
        `transaction_amount` decimal(65,30) NULL,
        `customer_name` varchar(200) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_account_name` varchar(200) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_name` varchar(50) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_code` varchar(10) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_account_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `failed_reason` varchar(200) COLLATE utf8mb4_0900_ai_ci NULL,
        `created_at` datetime(6) NOT NULL DEFAULT '2023-10-16 19:21:12.03043',
        `requester_device_id` char(36) COLLATE ascii_general_ci NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_transaction_histories` PRIMARY KEY (`id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE TABLE `withdraw_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `transaction_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `user_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `account_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_fee` decimal(65,30) NULL,
        `created_at` datetime(6) NOT NULL DEFAULT '2023-10-16 19:21:12.038977',
        `payment_disbursed_date_time` datetime(6) NULL,
        `payment_disbursed_amount` decimal(65,30) NULL,
        `payment_confirmed_amount` decimal(65,30) NULL,
        `bank_name` varchar(50) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_code` varchar(10) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_account_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `otp_request_ref` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `otp_request_id` char(36) COLLATE ascii_general_ci NULL,
        `otp_confirmed_date_time` datetime(6) NULL,
        `failed_reason` varchar(200) COLLATE utf8mb4_0900_ai_ci NULL,
        `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `requester_device_id` char(36) COLLATE ascii_general_ci NULL,
        CONSTRAINT `pk_withdraw_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE UNIQUE INDEX `ix_cash_deposit_state_transaction_no` ON `cash_deposit_state` (`transaction_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE UNIQUE INDEX `ix_cash_withdraw_state_transaction_no` ON `cash_withdraw_state` (`transaction_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE UNIQUE INDEX `ix_deposit_state_transaction_no` ON `deposit_state` (`transaction_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE INDEX `ix_global_manual_allocation_state_transaction_no` ON `global_manual_allocation_state` (`transaction_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE UNIQUE INDEX `ix_global_wallet_transfer_state_transaction_no` ON `global_wallet_transfer_state` (`transaction_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    CREATE UNIQUE INDEX `ix_withdraw_state_transaction_no` ON `withdraw_state` (`transaction_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231016122112_InitWalletDb') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20231016122112_InitWalletDb', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231020044707_RemoveLimitOnEncrpytedField') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231020044707_RemoveLimitOnEncrpytedField') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2023-10-20 11:47:07.148132';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231020044707_RemoveLimitOnEncrpytedField') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `customer_name` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231020044707_RemoveLimitOnEncrpytedField') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2023-10-20 11:47:07.150527';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231020044707_RemoveLimitOnEncrpytedField') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `bank_account_name` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231020044707_RemoveLimitOnEncrpytedField') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2023-10-20 11:47:07.150443';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231020044707_RemoveLimitOnEncrpytedField') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231020044707_RemoveLimitOnEncrpytedField') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `customer_name` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231020044707_RemoveLimitOnEncrpytedField') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2023-10-20 11:47:07.143953';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231020044707_RemoveLimitOnEncrpytedField') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `bank_account_name` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231020044707_RemoveLimitOnEncrpytedField') THEN

    ALTER TABLE `cash_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2023-10-20 11:47:07.148288';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231020044707_RemoveLimitOnEncrpytedField') THEN

    ALTER TABLE `cash_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2023-10-20 11:47:07.146028';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231020044707_RemoveLimitOnEncrpytedField') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20231020044707_RemoveLimitOnEncrpytedField', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2023-11-01 16:28:24.200579';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2023-11-01 16:28:24.20316';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2023-11-01 16:28:24.203078';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2023-11-01 16:28:24.195716';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    ALTER TABLE `cash_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2023-11-01 16:28:24.200761';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    ALTER TABLE `cash_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2023-11-01 16:28:24.198182';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    CREATE TABLE `inbox_state` (
        `id` bigint NOT NULL AUTO_INCREMENT,
        `message_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `consumer_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `lock_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `received` datetime(6) NOT NULL,
        `receive_count` int NOT NULL,
        `expiration_time` datetime(6) NULL,
        `consumed` datetime(6) NULL,
        `delivered` datetime(6) NULL,
        `last_sequence_number` bigint NULL,
        CONSTRAINT `pk_inbox_state` PRIMARY KEY (`id`),
        CONSTRAINT `ak_inbox_state_message_id_consumer_id` UNIQUE (`message_id`, `consumer_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    CREATE TABLE `outbox_message` (
        `sequence_number` bigint NOT NULL AUTO_INCREMENT,
        `enqueue_time` datetime(6) NULL,
        `sent_time` datetime(6) NOT NULL,
        `headers` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `properties` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `inbox_message_id` char(36) COLLATE ascii_general_ci NULL,
        `inbox_consumer_id` char(36) COLLATE ascii_general_ci NULL,
        `outbox_id` char(36) COLLATE ascii_general_ci NULL,
        `message_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `content_type` varchar(256) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `body` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `conversation_id` char(36) COLLATE ascii_general_ci NULL,
        `correlation_id` char(36) COLLATE ascii_general_ci NULL,
        `initiator_id` char(36) COLLATE ascii_general_ci NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `source_address` varchar(256) COLLATE utf8mb4_0900_ai_ci NULL,
        `destination_address` varchar(256) COLLATE utf8mb4_0900_ai_ci NULL,
        `response_address` varchar(256) COLLATE utf8mb4_0900_ai_ci NULL,
        `fault_address` varchar(256) COLLATE utf8mb4_0900_ai_ci NULL,
        `expiration_time` datetime(6) NULL,
        CONSTRAINT `pk_outbox_message` PRIMARY KEY (`sequence_number`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    CREATE TABLE `outbox_state` (
        `outbox_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `lock_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created` datetime(6) NOT NULL,
        `delivered` datetime(6) NULL,
        `last_sequence_number` bigint NULL,
        CONSTRAINT `pk_outbox_state` PRIMARY KEY (`outbox_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    CREATE INDEX `ix_inbox_state_delivered` ON `inbox_state` (`delivered`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    CREATE INDEX `ix_outbox_message_enqueue_time` ON `outbox_message` (`enqueue_time`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    CREATE INDEX `ix_outbox_message_expiration_time` ON `outbox_message` (`expiration_time`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    CREATE UNIQUE INDEX `ix_outbox_message_inbox_message_id_inbox_consumer_id_sequence_n` ON `outbox_message` (`inbox_message_id`, `inbox_consumer_id`, `sequence_number`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    CREATE UNIQUE INDEX `ix_outbox_message_outbox_id_sequence_number` ON `outbox_message` (`outbox_id`, `sequence_number`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    CREATE INDEX `ix_outbox_state_created` ON `outbox_state` (`created`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20231101092824_AddTransactionalOutBox') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20231101092824_AddTransactionalOutBox', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    ALTER TABLE `global_wallet_transfer_state` DROP INDEX `ix_global_wallet_transfer_state_transaction_no`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.082615';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.099834';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    ALTER TABLE `global_wallet_transfer_state` MODIFY COLUMN `transaction_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    ALTER TABLE `global_wallet_transfer_state` MODIFY COLUMN `customer_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.099731';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.078397';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    ALTER TABLE `cash_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.076556';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    ALTER TABLE `cash_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.074978';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    CREATE TABLE `activity_logs` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `transaction_no` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transaction_type` int NOT NULL,
        `user_id` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `account_code` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_code` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `channel` int NOT NULL,
        `product` int NOT NULL,
        `purpose` int NOT NULL,
        `state_machine` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `state` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `requested_amount` decimal(65,30) NOT NULL,
        `payment_received_date_time` datetime(6) NULL,
        `payment_received_amount` decimal(65,30) NULL,
        `payment_disbursed_date_time` datetime(6) NULL,
        `payment_disbursed_amount` decimal(65,30) NULL,
        `payment_confirmed_amount` decimal(65,30) NULL,
        `otp_request_ref` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `otp_request_id` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `otp_confirmed_date_time` datetime(6) NULL,
        `fee` decimal(65,30) NULL,
        `bank_account_no` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_account_name` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_name` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_code` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `deposit_generated_date_time` datetime(6) NULL,
        `qr_transaction_no` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `qr_transaction_ref` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `qr_value` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `requested_currency` int NULL,
        `requested_amount_with_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `requested_fx_amount` decimal(65,30) NULL,
        `requested_fx_currency` int NULL,
        `requested_fx_amount_with_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `payment_received_currency` int NULL,
        `transfer_fee` decimal(65,30) NULL,
        `fx_transaction_id` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `fx_initiate_request_date_time` datetime(6) NULL,
        `fx_confirmed_date_time` datetime(6) NULL,
        `fx_confirmed_exchange_rate` decimal(65,30) NULL,
        `fx_confirmed_amount` decimal(65,30) NULL,
        `fx_confirmed_currency` int NULL,
        `fx_confirmed_amount_with_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_from_account` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_amount` decimal(65,30) NULL,
        `transfer_to_account` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_currency` int NULL,
        `transfer_amount_with_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_request_time` datetime(6) NULL,
        `transfer_complete_time` datetime(6) NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `request_id` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `requester_device_id` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `created_at` datetime(6) NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_activity_logs` PRIMARY KEY (`id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    CREATE TABLE `deposit_entrypoint_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `transaction_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `user_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `account_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `purpose` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `requested_amount` decimal(65,30) NOT NULL,
        `net_amount` decimal(65,30) NULL,
        `customer_name` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_account_name` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_account_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_name` varchar(50) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_code` varchar(10) COLLATE utf8mb4_0900_ai_ci NULL,
        `refund_id` char(36) COLLATE ascii_general_ci NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `requester_device_id` char(36) COLLATE ascii_general_ci NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.085606',
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_deposit_entrypoint_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    CREATE TABLE `global_transfer_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transaction_type` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_id` bigint NOT NULL,
        `global_account` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `requested_currency` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `requested_fx_amount` decimal(65,30) NOT NULL,
        `requested_fx_currency` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `payment_received_amount` decimal(65,30) NULL,
        `payment_received_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `payment_received_date_time` datetime(6) NULL,
        `fx_initiate_request_date_time` datetime(6) NULL,
        `fx_transaction_id` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `fx_confirmed_amount` decimal(65,30) NULL,
        `fx_confirmed_exchange_rate` decimal(65,30) NULL,
        `fx_confirmed_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `fx_confirmed_date_time` datetime(6) NULL,
        `transfer_from_account` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_amount` decimal(65,30) NULL,
        `transfer_fee` decimal(65,30) NULL,
        `transfer_to_account` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_request_time` datetime(6) NULL,
        `transfer_complete_time` datetime(6) NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.096052',
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_global_transfer_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    CREATE TABLE `odd_deposit_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `payment_received_date_time` datetime(6) NULL,
        `payment_received_amount` decimal(65,30) NULL,
        `fee` decimal(65,30) NULL,
        `otp_request_ref` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `otp_request_id` char(36) COLLATE ascii_general_ci NULL,
        `otp_confirmed_date_time` datetime(6) NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.087929',
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_odd_deposit_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    CREATE TABLE `odd_withdraw_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `payment_disbursed_date_time` datetime(6) NULL,
        `payment_disbursed_amount` decimal(65,30) NULL,
        `fee` decimal(65,30) NULL,
        `otp_request_ref` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `otp_request_id` char(36) COLLATE ascii_general_ci NULL,
        `otp_confirmed_date_time` datetime(6) NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.08913',
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_odd_withdraw_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    CREATE TABLE `qr_deposit_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `transaction_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `payment_received_amount` decimal(65,30) NULL,
        `payment_received_date_time` datetime(6) NULL,
        `fee` decimal(65,30) NULL,
        `deposit_qr_generate_date_time` datetime(6) NULL,
        `qr_code_expired_time_in_minute` int NOT NULL,
        `qr_transaction_no` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `qr_value` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `qr_transaction_ref` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.086712',
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_qr_deposit_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    CREATE TABLE `refund_info` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `amount` decimal(65,30) NOT NULL,
        `transfer_to_account_no` varchar(20) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `transfer_to_account_name` varchar(100) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `fee` decimal(65,30) NOT NULL,
        `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.099772',
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_refund_info` PRIMARY KEY (`id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    CREATE TABLE `up_back_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transaction_type` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.089751',
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_up_back_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    CREATE TABLE `withdraw_entrypoint_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `transaction_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `user_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `account_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `purpose` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `requested_amount` decimal(65,30) NOT NULL,
        `net_amount` decimal(65,30) NULL,
        `customer_name` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_account_name` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_account_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_name` varchar(50) COLLATE utf8mb4_0900_ai_ci NULL,
        `bank_code` varchar(10) COLLATE utf8mb4_0900_ai_ci NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `requester_device_id` char(36) COLLATE ascii_general_ci NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL DEFAULT '2024-01-19 17:17:00.097487',
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_withdraw_entrypoint_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    CREATE UNIQUE INDEX `ix_deposit_entrypoint_state_transaction_no` ON `deposit_entrypoint_state` (`transaction_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    CREATE UNIQUE INDEX `ix_withdraw_entrypoint_state_transaction_no` ON `withdraw_entrypoint_state` (`transaction_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240119101700_AddWalletV2') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240119101700_AddWalletV2', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.819764';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `withdraw_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.827955';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.825115';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.830632';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.830567';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.822989';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.824599';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.823791';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.826738';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.83053';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.816072';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.822014';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `cash_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.81451';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `cash_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-01 13:45:02.813214';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    ALTER TABLE `activity_logs` ADD `customer_name` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240201064502_AddCustomerNameToActivityLog') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240201064502_AddCustomerNameToActivityLog', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-14 15:22:05.589324';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `withdraw_entrypoint_state` MODIFY COLUMN `customer_name` varchar(500) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `withdraw_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `withdraw_entrypoint_state` MODIFY COLUMN `bank_account_name` varchar(500) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `withdraw_entrypoint_state` ADD `bank_account_tax_id` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `withdraw_entrypoint_state` ADD `bank_branch_code` varchar(10) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-14 15:22:05.600012';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-14 15:22:05.610668';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-14 15:22:05.610585';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-14 15:22:05.596915';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-14 15:22:05.599278';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-14 15:22:05.598107';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-14 15:22:05.602373';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-14 15:22:05.610527';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-14 15:22:05.583109';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `customer_name` varchar(500) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-02-14 15:22:05.595416';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `bank_account_no` varchar(250) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `bank_account_name` varchar(500) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `deposit_entrypoint_state` ADD `bank_account_tax_id` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `deposit_entrypoint_state` ADD `bank_branch_code` varchar(10) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `cash_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `cash_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `user_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `transfer_to_account` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `transfer_from_account` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `transfer_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `transfer_amount_with_currency` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `transaction_type` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `transaction_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `state_machine` varchar(100) COLLATE utf8mb4_0900_ai_ci NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `state` varchar(100) COLLATE utf8mb4_0900_ai_ci NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `requester_device_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `requested_fx_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `requested_fx_amount_with_currency` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `requested_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `requested_amount_with_currency` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `request_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `qr_value` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `qr_transaction_ref` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `qr_transaction_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `purpose` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `payment_received_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `otp_request_ref` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `otp_request_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `fx_transaction_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `fx_confirmed_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `fx_confirmed_amount_with_currency` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `failed_reason` varchar(200) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `customer_name` varchar(500) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `customer_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `channel` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `bank_name` varchar(50) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `bank_code` varchar(10) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `bank_account_no` varchar(10) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `bank_account_name` varchar(500) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` MODIFY COLUMN `account_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` ADD `bank_account_tax_id` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    ALTER TABLE `activity_logs` ADD `bank_branch_code` varchar(10) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    CREATE TABLE `ats_deposit_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `payment_received_date_time` datetime(6) NULL,
        `payment_received_amount` decimal(65,30) NULL,
        `fee` decimal(65,30) NULL,
        `otp_request_ref` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `otp_request_id` char(36) COLLATE ascii_general_ci NULL,
        `otp_confirmed_date_time` datetime(6) NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL,
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_ats_deposit_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240214082205_AddAtsDepositSaga') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240214082205_AddAtsDepositSaga', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-08 14:10:05.290518';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-08 14:10:05.295901';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-08 14:10:05.307161';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-08 14:10:05.307103';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-08 14:10:05.293725';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-08 14:10:05.295395';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-08 14:10:05.294561';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-08 14:10:05.297588';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-08 14:10:05.307066';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-08 14:10:05.286824';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-08 14:10:05.292781';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    CREATE TABLE `recovery_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transaction_type` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_id` bigint NOT NULL,
        `global_account` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `payment_received_amount` decimal(65,30) NULL,
        `payment_received_date_time` datetime(6) NULL,
        `transaction_no` varchar(255) COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_from_account` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_amount` decimal(65,30) NULL,
        `transfer_to_account` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `transfer_request_time` datetime(6) NULL,
        `transfer_complete_time` datetime(6) NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL,
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_recovery_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    CREATE UNIQUE INDEX `ix_recovery_state_transaction_no` ON `recovery_state` (`transaction_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240308071005_AddRecoveryStateEngine') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240308071005_AddRecoveryStateEngine', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-12 12:43:56.401061';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `withdraw_entrypoint_state` ADD `confirmed_amount` decimal(65,30) NOT NULL DEFAULT 0.0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-12 12:43:56.407812';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-12 12:43:56.417816';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-12 12:43:56.417745';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-12 12:43:56.405165';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-12 12:43:56.407183';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-12 12:43:56.406217';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-12 12:43:56.409865';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-12 12:43:56.417698';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-12 12:43:56.392924';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-12 12:43:56.403908';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240312054356_CleanupAndAddColumnWithdraw') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240312054356_CleanupAndAddColumnWithdraw', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `global_transfer_state` RENAME COLUMN `requested_fx_amount` TO `requested_fx_rate`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-14 16:41:14.896112';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-14 16:41:14.902648';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-14 16:41:14.912232';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-14 16:41:14.912165';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-14 16:41:14.900254';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-14 16:41:14.902069';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-14 16:41:14.901188';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-14 16:41:14.904486';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-14 16:41:14.912125';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-14 16:41:14.887752';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-14 16:41:14.899121';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    ALTER TABLE `deposit_entrypoint_state` ADD `confirmed_amount` decimal(65,30) NOT NULL DEFAULT 0.0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240314094115_RefactorV2Columns') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240314094115_RefactorV2Columns', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-27 17:32:46.140991';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `withdraw_entrypoint_state` ADD `global_manual_allocate_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-27 17:32:46.147118';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-27 17:32:46.156295';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-27 17:32:46.15623';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-27 17:32:46.144832';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-27 17:32:46.146571';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-27 17:32:46.145708';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-27 17:32:46.148913';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `global_manual_allocation_state` ADD `transaction_id` char(36) COLLATE ascii_general_ci NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-27 17:32:46.156187';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-27 17:32:46.133801';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-27 17:32:46.143827';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    ALTER TABLE `deposit_entrypoint_state` ADD `global_manual_allocate_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240327103246_GlobalManualAllocateIdToEntryPoint') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240327103246_GlobalManualAllocateIdToEntryPoint', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `recovery_state` DROP COLUMN `channel`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `recovery_state` DROP COLUMN `customer_id`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `recovery_state` DROP COLUMN `payment_received_amount`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `recovery_state` DROP COLUMN `payment_received_date_time`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `recovery_state` DROP COLUMN `transaction_no`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:38:48.241962';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:38:48.248345';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:38:48.257829';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:38:48.257764';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:38:48.245996';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `qr_deposit_state` ADD `qr_expire_token_id` char(36) COLLATE ascii_general_ci NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:38:48.247778';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:38:48.246905';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:38:48.250206';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:38:48.257719';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:38:48.234256';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:38:48.244929';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328083848_QrExpiredColumn') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240328083848_QrExpiredColumn', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:52:48.481377';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:52:48.489361';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:52:48.500782';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:52:48.5007';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `qr_expire_token_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:52:48.486256';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:52:48.488685';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:52:48.487556';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:52:48.491563';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:52:48.500647';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:52:48.473026';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-28 15:52:48.484915';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240328085248_ModifyQrExpireIdToNullable') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240328085248_ModifyQrExpireIdToNullable', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-29 11:00:45.606208';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-29 11:00:45.613986';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-29 11:00:45.625271';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-29 11:00:45.625196';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-29 11:00:45.611076';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-29 11:00:45.613331';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `odd_withdraw_state` ADD `otp_validation_expire_token_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-29 11:00:45.612235';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `odd_deposit_state` ADD `otp_validation_expire_token_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-29 11:00:45.616107';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-29 11:00:45.625145';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-29 11:00:45.597687';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-03-29 11:00:45.609622';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    ALTER TABLE `ats_deposit_state` ADD `otp_validation_expire_token_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240329040045_AddOtpExpiredColumn') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240329040045_AddOtpExpiredColumn', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 13:46:36.597283';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 13:46:36.605954';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 13:46:36.61934';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `transfer_to_account_name` varchar(512) COLLATE utf8mb4_0900_ai_ci NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 13:46:36.619252';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `refund_info` ADD `ticket_id` char(36) COLLATE ascii_general_ci NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 13:46:36.602473';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 13:46:36.605179';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 13:46:36.603882';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 13:46:36.60827';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 13:46:36.619197';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 13:46:36.591639';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 13:46:36.601059';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    CREATE INDEX `ix_refund_info_id` ON `refund_info` (`id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403064636_EncryptRefundInfo') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240403064636_EncryptRefundInfo', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 16:16:00.516199';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 16:16:00.529263';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 16:16:00.53947';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 16:16:00.539412';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 16:16:00.519785';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 16:16:00.528723';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 16:16:00.520651';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 16:16:00.530914';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 16:16:00.539375';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 16:16:00.51206';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 16:16:00.5188';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    CREATE TABLE `ats_withdraw_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
        `product` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `channel` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `payment_disbursed_date_time` datetime(6) NULL,
        `payment_disbursed_amount` decimal(65,30) NULL,
        `fee` decimal(65,30) NULL,
        `otp_request_ref` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `otp_request_id` char(36) COLLATE ascii_general_ci NULL,
        `otp_confirmed_date_time` datetime(6) NULL,
        `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `response_address` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `otp_validation_expire_token_id` char(36) COLLATE ascii_general_ci NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL DEFAULT '2024-04-03 16:16:00.534223',
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_ats_withdraw_state` PRIMARY KEY (`correlation_id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    CREATE UNIQUE INDEX `ix_ats_withdraw_state_correlation_id` ON `ats_withdraw_state` (`correlation_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240403091600_AddAtsWithdraw') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240403091600_AddAtsWithdraw', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240411092602_AddCustomerCodeToOddRegistrationTable') THEN

    ALTER TABLE `online_direct_debit_registrations` ADD `customer_code` varchar(10) COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240411092602_AddCustomerCodeToOddRegistrationTable') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240411092602_AddCustomerCodeToOddRegistrationTable', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240417104303_AlterCustomerCodeColumnNameToIdCardNo') THEN

    ALTER TABLE `online_direct_debit_registrations` DROP COLUMN `customer_code`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240417104303_AlterCustomerCodeColumnNameToIdCardNo') THEN

    ALTER TABLE `online_direct_debit_registrations` ADD `identification_card_no` varchar(20) COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240417104303_AlterCustomerCodeColumnNameToIdCardNo') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240417104303_AlterCustomerCodeColumnNameToIdCardNo', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240424023234_RemoveIdNoColumnFromODDTable') THEN

    ALTER TABLE `online_direct_debit_registrations` DROP COLUMN `identification_card_no`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240424023234_RemoveIdNoColumnFromODDTable') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240424023234_RemoveIdNoColumnFromODDTable', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-26 13:31:25.477319';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-26 13:31:25.490596';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-26 13:31:25.511458';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-26 13:31:25.51137';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `refund_info` ADD `current_state` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-26 13:31:25.482067';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-26 13:31:25.489304';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-26 13:31:25.48517';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-26 13:31:25.493206';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-26 13:31:25.511306';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-26 13:31:25.472178';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-26 13:31:25.480749';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    ALTER TABLE `ats_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-04-26 13:31:25.499498';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240426063125_AddCurrentStateToRefundInfo') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240426063125_AddCurrentStateToRefundInfo', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-08 10:52:05.031435';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-08 10:52:05.04484';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-08 10:52:05.055546';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-08 10:52:05.055486';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-08 10:52:05.035139';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-08 10:52:05.044294';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-08 10:52:05.036057';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-08 10:52:05.046555';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `global_transfer_state` ADD `fx_mark_up_rate` decimal(65,30) NOT NULL DEFAULT 0.0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-08 10:52:05.055448';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-08 10:52:05.027542';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-08 10:52:05.03413';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    ALTER TABLE `ats_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-08 10:52:05.050157';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240508035205_AddFxMarkUpColumn') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240508035205_AddFxMarkUpColumn', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 11:04:51.8068';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 11:04:51.822321';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 11:04:51.834444';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 11:04:51.834368';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 11:04:51.811038';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 11:04:51.821622';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 11:04:51.812015';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 11:04:51.824516';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `global_transfer_state` ADD `pre_mark_up_confirmed_exchange_rate` decimal(65,30) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `global_transfer_state` ADD `pre_mark_up_requested_fx_rate` decimal(65,30) NOT NULL DEFAULT 0.0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 11:04:51.834326';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 11:04:51.802169';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 11:04:51.809897';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    ALTER TABLE `ats_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 11:04:51.82842';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513040451_AddOriginalFxColumns') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240513040451_AddOriginalFxColumns', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 15:37:18.26223';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 15:37:18.270252';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 15:37:18.283064';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 15:37:18.282892';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 15:37:18.267093';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 15:37:18.269545';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 15:37:18.268369';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 15:37:18.27248';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 15:37:18.281853';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 15:37:18.256719';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 15:37:18.265804';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    ALTER TABLE `ats_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-13 15:37:18.27717';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    CREATE TABLE `holidays` (
        `holiday_date` date NOT NULL,
        `holiday_name` varchar(150) COLLATE utf8mb4_0900_ai_ci NULL,
        `valid` tinyint(1) NOT NULL,
        CONSTRAINT `pk_holidays` PRIMARY KEY (`holiday_date`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    INSERT INTO `holidays` (`holiday_date`, `holiday_name`, `valid`)
    VALUES (DATE '2024-01-01', 'New Year''s Day', TRUE),
    (DATE '2024-01-02', 'Substitution for New Year’s Eve (Sunday 31st December 2023) (cancelled)', TRUE),
    (DATE '2024-02-26', 'Substitution for Makha Bucha Day (Saturday 24th February 2024)', TRUE),
    (DATE '2024-04-08', 'Substitution for Chakri Memorial Day (Saturday 6th April 2024)', TRUE),
    (DATE '2024-04-12', 'Additional special holiday (added)', TRUE),
    (DATE '2024-04-15', 'Songkran Festival', TRUE),
    (DATE '2024-04-16', 'Substitution for Songkran Festival (Saturday 13th April 2024 and Sunday 14th April 2024)', TRUE),
    (DATE '2024-05-01', 'National Labour Day', TRUE),
    (DATE '2024-05-06', 'Substitution for Coronation Day (Saturday 4th May 2024)', TRUE),
    (DATE '2024-05-22', 'Visakha Bucha Day', TRUE),
    (DATE '2024-06-03', 'H.M. Queen Suthida Bajrasudhabimalalakshana’s Birthday', TRUE),
    (DATE '2024-07-22', 'Substitution for Asarnha Bucha Day (Saturday 20th July 2024)', TRUE),
    (DATE '2024-07-29', 'Substitution for H.M. King Maha Vajiralongkorn Phra Vajiraklaochaoyuhua’s Birthday (Sunday 28th July 2024)', TRUE),
    (DATE '2024-08-12', 'H.M. Queen Sirikit The Queen Mother’s Birthday / Mother’s Day', TRUE),
    (DATE '2024-10-14', 'Substitution for H.M. King Bhumibol Adulyadej The Great Memorial Day (Sunday 13th October 2024)', TRUE),
    (DATE '2024-10-23', 'H.M. King Chulalongkorn The Great Memorial Day', TRUE),
    (DATE '2024-12-05', 'H.M. King Bhumibol Adulyadej The Great’s Birthday / National Day / Father’s Day', TRUE),
    (DATE '2024-12-10', 'Constitution Day', TRUE),
    (DATE '2024-12-31', 'New Year’s Eve', TRUE);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240513083718_AddHolidayTable') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240513083718_AddHolidayTable', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `global_transfer_state` DROP COLUMN `pre_mark_up_confirmed_exchange_rate`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `global_transfer_state` DROP COLUMN `pre_mark_up_requested_fx_rate`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `global_transfer_state` RENAME COLUMN `payment_received_currency` TO `exchange_currency`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `global_transfer_state` RENAME COLUMN `payment_received_amount` TO `exchange_amount`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-24 14:35:19.709869';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-24 14:35:19.717949';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-24 14:35:19.730937';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-24 14:35:19.730763';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-24 14:35:19.714875';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-24 14:35:19.717256';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-24 14:35:19.716058';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-24 14:35:19.72046';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `global_transfer_state` ADD `fx_confirmed_exchange_amount` decimal(65,30) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `global_transfer_state` ADD `fx_confirmed_exchange_currency` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-24 14:35:19.729837';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-24 14:35:19.704772';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-24 14:35:19.713496';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `ats_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-24 14:35:19.727226';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `activity_logs` ADD `exchange_amount` decimal(65,30) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    ALTER TABLE `activity_logs` ADD `exchange_currency` int NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240524073519_ReviseGlobalTransferColumns') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240524073519_ReviseGlobalTransferColumns', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `global_transfer_state` DROP COLUMN `payment_received_date_time`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-27 17:02:51.885392';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-27 17:02:51.893231';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-27 17:02:51.905999';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-27 17:02:51.905833';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-27 17:02:51.890291';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-27 17:02:51.892558';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-27 17:02:51.891447';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-27 17:02:51.89575';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `global_transfer_state` ADD `actual_fx_rate` decimal(65,30) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-27 17:02:51.904914';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-27 17:02:51.880536';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-27 17:02:51.888873';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    ALTER TABLE `ats_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-27 17:02:51.902223';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240527100252_AddActualFxRate') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240527100252_AddActualFxRate', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN


                    CREATE OR REPLACE VIEW deposit_transactions AS
                    SELECT
    	                des.transaction_no,
    	                des.current_state,
    	                des.channel,
    	                des.product,
    	                des.requested_amount as 'amount',
    	                gts.transfer_amount as 'global_transfer_amount',
                        CASE
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 002%' THEN 'Freewill Failed (002)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 003%' THEN 'Freewill Failed (003)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 005%' THEN 'Freewill Failed (005)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 008%' THEN 'Freewill Failed (008)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 023%' THEN 'Freewill Failed (023)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 900%' THEN 'Freewill Failed (900)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 906%' THEN 'Freewill Failed (906)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 999%' THEN 'Freewill Failed (999)'
                            WHEN ubs.failed_reason LIKE 'SetTrade%' THEN 'SetTrade Failed'
                            ELSE ubs.failed_reason
                        END AS 'upback_failed_reason',
    	                des.created_at
                    FROM deposit_entrypoint_state des
                    LEFT JOIN up_back_state ubs ON des.correlation_id = ubs.correlation_id
                    LEFT JOIN global_transfer_state gts ON des.correlation_id = gts.correlation_id
                    WHERE des.transaction_no IS NOT NULL;

                    CREATE OR REPLACE VIEW withdraw_transactions AS
                    SELECT
    	                wes.transaction_no,
    	                wes.current_state,
    	                wes.channel,
    	                wes.product,
    	                CASE
    		                WHEN wes.product = 'GlobalEquities' THEN wes.requested_amount * gts.requested_fx_rate
    		                ELSE wes.requested_amount
    	                END AS 'amount',
                        gts.transfer_amount AS 'global_transfer_amount',
                        CASE
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 002%' THEN 'Freewill Failed (002)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 003%' THEN 'Freewill Failed (003)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 005%' THEN 'Freewill Failed (005)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 008%' THEN 'Freewill Failed (008)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 023%' THEN 'Freewill Failed (023)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 900%' THEN 'Freewill Failed (900)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 906%' THEN 'Freewill Failed (906)'
                            WHEN ubs.failed_reason LIKE 'Freewill update failed. ResultCode: 999%' THEN 'Freewill Failed (999)'
                            WHEN ubs.failed_reason LIKE 'SetTrade%' THEN 'SetTrade Failed'
                            ELSE ubs.failed_reason
                        END AS 'upback_failed_reason',
    	                wes.created_at
                    FROM withdraw_entrypoint_state wes
                    LEFT JOIN up_back_state ubs ON wes.correlation_id = ubs.correlation_id
                    LEFT JOIN global_transfer_state gts ON wes.correlation_id = gts.correlation_id
                    WHERE wes.transaction_no IS NOT NULL;

                    CREATE OR REPLACE VIEW activity_view AS
                    SELECT
                        al.correlation_id,
                        al.transaction_no,
                        al.transaction_type,
                        al.product,
                        al.channel,
                        al.state_machine,
                        al.state,
                        al.requested_amount,
                        al.payment_received_date_time,
                        al.payment_received_amount,
                        al.payment_disbursed_date_time,
                        al.payment_disbursed_amount,
                        al.failed_reason,
                        al.created_at,
                        al.updated_at
                    FROM activity_logs al;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-28 18:31:18.820344';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-28 18:31:18.835664';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-28 18:31:18.844849';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-28 18:31:18.844684';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-28 18:31:18.824562';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-28 18:31:18.835043';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-28 18:31:18.833651';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-28 18:31:18.837695';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-28 18:31:18.84383';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-28 18:31:18.815719';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-28 18:31:18.823431';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    ALTER TABLE `ats_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-28 18:31:18.841484';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240528113118_AddTransactionView') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240528113118_AddTransactionView', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-30 13:42:18.016988';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `withdraw_entrypoint_state` ADD `effective_date` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-30 13:42:18.030912';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-30 13:42:18.038791';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-30 13:42:18.038629';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-30 13:42:18.02093';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-30 13:42:18.030407';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-30 13:42:18.029514';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-30 13:42:18.032704';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-30 13:42:18.037945';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-30 13:42:18.012986';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-30 13:42:18.019932';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `deposit_entrypoint_state` ADD `effective_date` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    ALTER TABLE `ats_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-05-30 13:42:18.036118';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240530064218_AddEffectiveDateColumn') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240530064218_AddEffectiveDateColumn', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-06-12 16:01:34.742841';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `up_back_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-06-12 16:01:34.756801';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `transaction_histories` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-06-12 16:01:34.765827';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `refund_info` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-06-12 16:01:34.76569';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `qr_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-06-12 16:01:34.754422';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `outbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `odd_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-06-12 16:01:34.756243';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `odd_deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-06-12 16:01:34.755363';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `inbox_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `global_transfer_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-06-12 16:01:34.75878';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `freewill_request_logs` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-06-12 16:01:34.764941';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `deposit_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-06-12 16:01:34.738636';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `deposit_entrypoint_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-06-12 16:01:34.745674';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    ALTER TABLE `ats_withdraw_state` MODIFY COLUMN `created_at` datetime(6) NOT NULL DEFAULT '2024-06-12 16:01:34.762472';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    CREATE TABLE `email_history` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `ticket_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `transaction_no` varchar(255) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `email_type` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `sent_at` datetime(6) NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_email_history` PRIMARY KEY (`id`)
    ) COLLATE=utf8mb4_0900_ai_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    CREATE INDEX `ix_email_history_transaction_no` ON `email_history` (`transaction_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__WalletDbContext` WHERE `migration_id` = '20240612090134_AddEmailHistoryTable') THEN

    INSERT INTO `__WalletDbContext` (`migration_id`, `product_version`)
    VALUES ('20240612090134_AddEmailHistoryTable', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

