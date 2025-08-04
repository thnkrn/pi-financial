CREATE TABLE IF NOT EXISTS `__FundDbContext` (
    `migration_id` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `product_version` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk___fund_db_context` PRIMARY KEY (`migration_id`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20220927112858_InitialCreate') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20220927112858_InitialCreate') THEN

    CREATE TABLE `fund_account_opening_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `customer_code` varchar(10) CHARACTER SET utf8mb4 NULL,
        `ndid` tinyint(1) NOT NULL,
        `request_received_time` datetime(6) NOT NULL,
        `documents_generation_ticket_id` char(36) COLLATE ascii_general_ci NULL,
        `current_state` varchar(64) CHARACTER SET utf8mb4 NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_fund_account_opening_state` PRIMARY KEY (`correlation_id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20220927112858_InitialCreate') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20220927112858_InitialCreate', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20230103125107_AddNdidRequestIdAndRequestDateTime') THEN

    ALTER TABLE `fund_account_opening_state` ADD `ndid_date_time` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20230103125107_AddNdidRequestIdAndRequestDateTime') THEN

    ALTER TABLE `fund_account_opening_state` ADD `ndid_request_id` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20230103125107_AddNdidRequestIdAndRequestDateTime') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20230103125107_AddNdidRequestIdAndRequestDateTime', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20230213104406_AddFailedReasonAndIsOpenSegregateAccountFlag') THEN

    ALTER TABLE `fund_account_opening_state` ADD `failed_reason` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20230213104406_AddFailedReasonAndIsOpenSegregateAccountFlag') THEN

    ALTER TABLE `fund_account_opening_state` ADD `is_open_segregate_account` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20230213104406_AddFailedReasonAndIsOpenSegregateAccountFlag') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20230213104406_AddFailedReasonAndIsOpenSegregateAccountFlag', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20230330103629_AddColumnForCallback') THEN

    ALTER TABLE `fund_account_opening_state` ADD `customer_id` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20230330103629_AddColumnForCallback') THEN

    ALTER TABLE `fund_account_opening_state` ADD `open_account_register_uid` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20230330103629_AddColumnForCallback') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20230330103629_AddColumnForCallback', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20230405032851_RetypeCustomerIdToLong') THEN

    ALTER TABLE `fund_account_opening_state` MODIFY COLUMN `customer_id` bigint NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20230405032851_RetypeCustomerIdToLong') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20230405032851_RetypeCustomerIdToLong', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20231120063422_AddUserIdColumn') THEN

    ALTER TABLE `fund_account_opening_state` ADD `user_id` char(36) COLLATE ascii_general_ci NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20231120063422_AddUserIdColumn') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20231120063422_AddUserIdColumn', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240118042940_CreateFundOrderStateAndUnitHolderTables') THEN

    CREATE TABLE `fund_order_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `order_no` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `broker_order_id` varchar(36) CHARACTER SET utf8mb4 NULL,
        `exchange_order_id` varchar(36) CHARACTER SET utf8mb4 NULL,
        `trading_account_no` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `unit_holder_id` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `customer_code` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `venue_id` int NULL,
        `instrument_id` int NULL,
        `fund_code` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `order_side` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `redemption_type` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `order_type` varchar(255) CHARACTER SET utf8mb4 NULL,
        `unit` decimal(65,30) NOT NULL,
        `amount` decimal(65,30) NOT NULL,
        `allotted_volume` decimal(65,30) NULL,
        `allotted_amount` decimal(65,30) NULL,
        `allotted_nav` decimal(65,30) NULL,
        `allotted_date` date NULL,
        `order_status` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `payment_type` varchar(255) CHARACTER SET utf8mb4 NULL,
        `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
        `updated_at` datetime(6) NULL DEFAULT CURRENT_TIMESTAMP(6),
        `effective_date` date NULL,
        `extra_info` longtext CHARACTER SET utf8mb4 NULL,
        `sale_license` varchar(36) CHARACTER SET utf8mb4 NULL,
        `bank_account` varchar(36) CHARACTER SET utf8mb4 NULL,
        `bank_code` varchar(36) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `pk_fund_order_state` PRIMARY KEY (`correlation_id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240118042940_CreateFundOrderStateAndUnitHolderTables') THEN

    CREATE TABLE `unit_holders` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `amc_code` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `unit_holder_id` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `customer_code` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `trading_account_no` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `unit_holder_type` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `status` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
        `updated_at` datetime(6) NULL,
        CONSTRAINT `pk_unit_holders` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240118042940_CreateFundOrderStateAndUnitHolderTables') THEN

    CREATE UNIQUE INDEX `ix_fund_order_state_order_no` ON `fund_order_state` (`order_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240118042940_CreateFundOrderStateAndUnitHolderTables') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240118042940_CreateFundOrderStateAndUnitHolderTables', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240119025751_UpdateColumnsFromFundOrderTable') THEN

    ALTER TABLE `fund_order_state` DROP COLUMN `extra_info`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240119025751_UpdateColumnsFromFundOrderTable') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `redemption_type` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240119025751_UpdateColumnsFromFundOrderTable') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240119025751_UpdateColumnsFromFundOrderTable', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240119031558_UpdateNullableColumnsFromFundOrderTable') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `unit` decimal(65,30) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240119031558_UpdateNullableColumnsFromFundOrderTable') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `amount` decimal(65,30) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240119031558_UpdateNullableColumnsFromFundOrderTable') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240119031558_UpdateNullableColumnsFromFundOrderTable', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240125050428_UpdateFundOrderColumns') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `unit_holder_id` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240125050428_UpdateFundOrderColumns') THEN

    UPDATE `fund_order_state` SET `order_type` = ''
    WHERE `order_type` IS NULL;
    SELECT ROW_COUNT();


    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240125050428_UpdateFundOrderColumns') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `order_type` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240125050428_UpdateFundOrderColumns') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `order_no` varchar(36) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240125050428_UpdateFundOrderColumns') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `current_state` varchar(64) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240125050428_UpdateFundOrderColumns') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `bank_code` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240125050428_UpdateFundOrderColumns') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `bank_account` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240125050428_UpdateFundOrderColumns') THEN

    ALTER TABLE `fund_order_state` ADD `failed_reason` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240125050428_UpdateFundOrderColumns') THEN

    ALTER TABLE `fund_order_state` ADD `request_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240125050428_UpdateFundOrderColumns') THEN

    ALTER TABLE `fund_order_state` ADD `response_address` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240125050428_UpdateFundOrderColumns') THEN

    ALTER TABLE `fund_order_state` ADD `sell_all_unit` tinyint(1) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240125050428_UpdateFundOrderColumns') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240125050428_UpdateFundOrderColumns', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240201043420_AddTradingAccountIdAndCounterFundCodeColumns') THEN

    ALTER TABLE `fund_order_state` ADD `counter_fund_code` varchar(36) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240201043420_AddTradingAccountIdAndCounterFundCodeColumns') THEN

    ALTER TABLE `fund_order_state` ADD `trading_account_id` char(36) COLLATE ascii_general_ci NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240201043420_AddTradingAccountIdAndCounterFundCodeColumns') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240201043420_AddTradingAccountIdAndCounterFundCodeColumns', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240205082822_AddSettlementColumnsWithChanneAndAccountType') THEN

    ALTER TABLE `fund_order_state` ADD `account_type` varchar(255) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240205082822_AddSettlementColumnsWithChanneAndAccountType') THEN

    ALTER TABLE `fund_order_state` ADD `channel` varchar(255) NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240205082822_AddSettlementColumnsWithChanneAndAccountType') THEN

    ALTER TABLE `fund_order_state` ADD `nav_date` date NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240205082822_AddSettlementColumnsWithChanneAndAccountType') THEN

    ALTER TABLE `fund_order_state` ADD `reject_reason` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240205082822_AddSettlementColumnsWithChanneAndAccountType') THEN

    ALTER TABLE `fund_order_state` ADD `settlement_bank_account` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240205082822_AddSettlementColumnsWithChanneAndAccountType') THEN

    ALTER TABLE `fund_order_state` ADD `settlement_bank_code` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240205082822_AddSettlementColumnsWithChanneAndAccountType') THEN

    ALTER TABLE `fund_order_state` ADD `settlement_date` date NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240205082822_AddSettlementColumnsWithChanneAndAccountType') THEN

    ALTER TABLE `fund_order_state` RENAME COLUMN `allotted_volume` TO `allotted_unit`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240205082822_AddSettlementColumnsWithChanneAndAccountType') THEN

    ALTER TABLE `fund_order_state` RENAME COLUMN `exchange_order_id` TO `amc_order_id`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240205082822_AddSettlementColumnsWithChanneAndAccountType') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240205082822_AddSettlementColumnsWithChanneAndAccountType', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240208042319_AddUnitHolderIndexToFundOrderAndUnitHolderTables') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `unit_holder_id` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240208042319_AddUnitHolderIndexToFundOrderAndUnitHolderTables') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `settlement_bank_code` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240208042319_AddUnitHolderIndexToFundOrderAndUnitHolderTables') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `settlement_bank_account` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240208042319_AddUnitHolderIndexToFundOrderAndUnitHolderTables') THEN

    CREATE INDEX `ix_unit_holders_unit_holder_id` ON `unit_holders` (`unit_holder_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240208042319_AddUnitHolderIndexToFundOrderAndUnitHolderTables') THEN

    CREATE INDEX `ix_fund_order_state_unit_holder_id` ON `fund_order_state` (`unit_holder_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240208042319_AddUnitHolderIndexToFundOrderAndUnitHolderTables') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240208042319_AddUnitHolderIndexToFundOrderAndUnitHolderTables', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240219102216_ChangeRemoveUniqueIndexFromOrderNumberColumn') THEN

    ALTER TABLE `fund_order_state` DROP INDEX `ix_fund_order_state_order_no`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240219102216_ChangeRemoveUniqueIndexFromOrderNumberColumn') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240219102216_ChangeRemoveUniqueIndexFromOrderNumberColumn', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240220064548_MakeCustomerCodeNullableInFundOrderState') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `customer_code` varchar(36) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240220064548_MakeCustomerCodeNullableInFundOrderState') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240220064548_MakeCustomerCodeNullableInFundOrderState', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240222021242_MakeTradingAccountIdNullableInFundOrderState') THEN

    ALTER TABLE `fund_order_state` MODIFY COLUMN `trading_account_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240222021242_MakeTradingAccountIdNullableInFundOrderState') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240222021242_MakeTradingAccountIdNullableInFundOrderState', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240222094836_AddCompositKey') THEN

    CREATE UNIQUE INDEX `ix_fund_order_state_order_no_order_type` ON `fund_order_state` (`order_no`, `order_type`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240222094836_AddCompositKey') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240222094836_AddCompositKey', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240228050225_AddBrokerOrderIdAndOrderTypeIndex') THEN

    CREATE UNIQUE INDEX `ix_fund_order_state_broker_order_id_order_type` ON `fund_order_state` (`broker_order_id`, `order_type`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240228050225_AddBrokerOrderIdAndOrderTypeIndex') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240228050225_AddBrokerOrderIdAndOrderTypeIndex', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240228075621_RemoveFundInfoFieldsOnFundOderState') THEN

    ALTER TABLE `fund_order_state` DROP COLUMN `instrument_id`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240228075621_RemoveFundInfoFieldsOnFundOderState') THEN

    ALTER TABLE `fund_order_state` DROP COLUMN `venue_id`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240228075621_RemoveFundInfoFieldsOnFundOderState') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240228075621_RemoveFundInfoFieldsOnFundOderState', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240307101922_Init-Number-Generators-Table') THEN

    CREATE TABLE `number_generators` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `module` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `prefix` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `current_counter` int NOT NULL,
        `daily_reset` tinyint(1) NOT NULL,
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_number_generators` PRIMARY KEY (`id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240307101922_Init-Number-Generators-Table') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240307101922_Init-Number-Generators-Table', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240318161339_MakeCustCodeNullableInUnitHolderTable') THEN

    ALTER TABLE `unit_holders` MODIFY COLUMN `customer_code` varchar(36) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240318161339_MakeCustCodeNullableInUnitHolderTable') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240318161339_MakeCustCodeNullableInUnitHolderTable', '7.0.11');

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
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240626064807_CreateCustomerDataSyncHistoryTable') THEN

    ALTER TABLE `number_generators` MODIFY COLUMN `prefix` longtext CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240626064807_CreateCustomerDataSyncHistoryTable') THEN

    ALTER TABLE `number_generators` MODIFY COLUMN `module` longtext CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240626064807_CreateCustomerDataSyncHistoryTable') THEN

    CREATE TABLE `customer_data_sync_histories` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `customer_code` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
        `profile_update_success` tinyint(1) NULL,
        `account_update_success` tinyint(1) NULL,
        `failed_reason` longtext CHARACTER SET utf8mb4 NULL,
        `created_at` datetime(6) NOT NULL,
        `updated_at` datetime(6) NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_customer_data_sync_histories` PRIMARY KEY (`correlation_id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__FundDbContext` WHERE `migration_id` = '20240626064807_CreateCustomerDataSyncHistoryTable') THEN

    INSERT INTO `__FundDbContext` (`migration_id`, `product_version`)
    VALUES ('20240626064807_CreateCustomerDataSyncHistoryTable', '7.0.11');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

