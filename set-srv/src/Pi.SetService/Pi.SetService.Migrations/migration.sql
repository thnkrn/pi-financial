CREATE TABLE IF NOT EXISTS `__SetDbContext` (
    `migration_id` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `product_version` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk___set_db_context` PRIMARY KEY (`migration_id`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240702071637_InitSetDb') THEN

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240702071637_InitSetDb') THEN

    CREATE TABLE `equity_order_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` varchar(64) CHARACTER SET utf8mb4 NULL,
        `trading_account_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `trading_account_no` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `customer_code` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `enter_id` varchar(10) CHARACTER SET utf8mb4 NULL,
        `order_no` varchar(64) CHARACTER SET utf8mb4 NULL,
        `broker_order_id` varchar(64) CHARACTER SET utf8mb4 NULL,
        `condition_price` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `status` varchar(64) CHARACTER SET utf8mb4 NULL,
        `broker_status` varchar(64) CHARACTER SET utf8mb4 NULL,
        `price` decimal(65,30) NULL,
        `volume` int NOT NULL,
        `pub_volume` int NOT NULL,
        `matched_volume` int NULL,
        `side` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `type` varchar(255) CHARACTER SET utf8mb4 NULL,
        `sec_symbol` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `condition` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `trustee_id` varchar(64) CHARACTER SET utf8mb4 NULL,
        `service_type` varchar(64) CHARACTER SET utf8mb4 NULL,
        `life` varchar(64) CHARACTER SET utf8mb4 NULL,
        `ttf` varchar(255) CHARACTER SET utf8mb4 NULL,
        `today_sell` tinyint(1) NULL,
        `stock_type` varchar(64) CHARACTER SET utf8mb4 NULL,
        `ip_address` varchar(64) CHARACTER SET utf8mb4 NULL,
        `failed_reason` longtext CHARACTER SET utf8mb4 NULL,
        `response_address` longtext CHARACTER SET utf8mb4 NULL,
        `request_id` char(36) COLLATE ascii_general_ci NULL,
        `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
        `updated_at` datetime(6) NULL DEFAULT CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_equity_order_state` PRIMARY KEY (`correlation_id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240702071637_InitSetDb') THEN

    CREATE TABLE `number_generators` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `module` longtext CHARACTER SET utf8mb4 NOT NULL,
        `prefix` longtext CHARACTER SET utf8mb4 NOT NULL,
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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240702071637_InitSetDb') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20240702071637_InitSetDb', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240703084131_AddUniqueIndexForOrderNo') THEN

    DROP TABLE `number_generators`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240703084131_AddUniqueIndexForOrderNo') THEN

    ALTER TABLE `equity_order_state` ADD `created_at_date` date AS (DATE(created_at));

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240703084131_AddUniqueIndexForOrderNo') THEN

    CREATE UNIQUE INDEX `unique_order_date` ON `equity_order_state` (`order_no`, `created_at_date`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240703084131_AddUniqueIndexForOrderNo') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20240703084131_AddUniqueIndexForOrderNo', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240705041657_UpdateColumns') THEN

    ALTER TABLE `equity_order_state` DROP COLUMN `broker_status`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240705041657_UpdateColumns') THEN

    ALTER TABLE `equity_order_state` DROP COLUMN `life`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240705041657_UpdateColumns') THEN

    ALTER TABLE `equity_order_state` DROP COLUMN `trustee_id`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240705041657_UpdateColumns') THEN

    ALTER TABLE `equity_order_state` MODIFY COLUMN `service_type` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240705041657_UpdateColumns') THEN

    ALTER TABLE `equity_order_state` ADD `action` varchar(255) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240705041657_UpdateColumns') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20240705041657_UpdateColumns', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240712084207_UpdateColumnsForSell') THEN

    ALTER TABLE `equity_order_state` MODIFY COLUMN `type` varchar(64) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240712084207_UpdateColumnsForSell') THEN

    ALTER TABLE `equity_order_state` MODIFY COLUMN `ttf` varchar(64) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240712084207_UpdateColumnsForSell') THEN

    ALTER TABLE `equity_order_state` MODIFY COLUMN `side` varchar(64) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240712084207_UpdateColumnsForSell') THEN

    ALTER TABLE `equity_order_state` MODIFY COLUMN `service_type` varchar(64) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240712084207_UpdateColumnsForSell') THEN

    ALTER TABLE `equity_order_state` MODIFY COLUMN `condition_price` varchar(64) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240712084207_UpdateColumnsForSell') THEN

    ALTER TABLE `equity_order_state` MODIFY COLUMN `condition` varchar(64) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240712084207_UpdateColumnsForSell') THEN

    ALTER TABLE `equity_order_state` MODIFY COLUMN `action` varchar(64) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240712084207_UpdateColumnsForSell') THEN

    ALTER TABLE `equity_order_state` ADD `lending` tinyint(1) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240712084207_UpdateColumnsForSell') THEN

    ALTER TABLE `equity_order_state` ADD `trading_account_type` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240712084207_UpdateColumnsForSell') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20240712084207_UpdateColumnsForSell', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240712090046_UpdateTradingAccountTypeColumn') THEN

    ALTER TABLE `equity_order_state` MODIFY COLUMN `trading_account_type` varchar(64) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240712090046_UpdateTradingAccountTypeColumn') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20240712090046_UpdateTradingAccountTypeColumn', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240716103907_AddOrderPrefix') THEN

    ALTER TABLE `equity_order_state` RENAME COLUMN `type` TO `order_type`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240716103907_AddOrderPrefix') THEN

    ALTER TABLE `equity_order_state` RENAME COLUMN `status` TO `order_status`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240716103907_AddOrderPrefix') THEN

    ALTER TABLE `equity_order_state` RENAME COLUMN `side` TO `order_side`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240716103907_AddOrderPrefix') THEN

    ALTER TABLE `equity_order_state` RENAME COLUMN `action` TO `order_action`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20240716103907_AddOrderPrefix') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20240716103907_AddOrderPrefix', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241016035303_InitEquityInfoTable') THEN

    CREATE TABLE `equity_infos` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `symbol` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `initial_margin_rate` decimal(65,30) NOT NULL,
        `margin_code` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `is_turnover_list` tinyint(1) NOT NULL,
        `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
        `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
        `row_version` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_equity_infos` PRIMARY KEY (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241016035303_InitEquityInfoTable') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20241016035303_InitEquityInfoTable', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241018042329_AddInitialMarginTable') THEN

    ALTER TABLE `equity_infos` DROP COLUMN `initial_margin_rate`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241018042329_AddInitialMarginTable') THEN

    CREATE TABLE `equity_initial_margins` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `margin_code` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `rate` decimal(65,30) NOT NULL,
        `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
        `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
        `row_version` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_equity_initial_margins` PRIMARY KEY (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241018042329_AddInitialMarginTable') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20241018042329_AddInitialMarginTable', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241018051428_AddUniqueIndexForInitialMargin') THEN

    CREATE UNIQUE INDEX `unique_margin_code` ON `equity_initial_margins` (`margin_code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241018051428_AddUniqueIndexForInitialMargin') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20241018051428_AddUniqueIndexForInitialMargin', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241028071806_AddSblTables') THEN

    CREATE TABLE `sbl_instruments` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `symbol` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `interest_rate` decimal(65,30) NOT NULL,
        `retail_lender` decimal(65,30) NOT NULL,
        `borrow_outstanding` decimal(65,30) NOT NULL,
        `available_lending` decimal(65,30) NOT NULL,
        `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
        `updated_at` datetime(6) NULL DEFAULT CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_sbl_instruments` PRIMARY KEY (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241028071806_AddSblTables') THEN

    CREATE TABLE `sbl_orders` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `trading_account_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `trading_account_no` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `customer_code` varchar(36) CHARACTER SET utf8mb4 NOT NULL,
        `order_no` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `symbol` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `type` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `volume` int NOT NULL,
        `status` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `rejected_reason` longtext CHARACTER SET utf8mb4 NULL,
        `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
        `updated_at` datetime(6) NULL DEFAULT CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_sbl_orders` PRIMARY KEY (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241028071806_AddSblTables') THEN

    CREATE UNIQUE INDEX `sbl_order_no` ON `sbl_orders` (`order_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241028071806_AddSblTables') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20241028071806_AddSblTables', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241030023022_UpdateSblOrderUniqueIndex') THEN

    ALTER TABLE `sbl_orders` DROP INDEX `sbl_order_no`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241030023022_UpdateSblOrderUniqueIndex') THEN

    ALTER TABLE `sbl_orders` DROP COLUMN `order_no`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241030023022_UpdateSblOrderUniqueIndex') THEN

    ALTER TABLE `sbl_orders` ADD `order_id` bigint unsigned NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241030023022_UpdateSblOrderUniqueIndex') THEN

    ALTER TABLE `sbl_orders` ADD `created_at_date` date AS (DATE(created_at));

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241030023022_UpdateSblOrderUniqueIndex') THEN

    CREATE UNIQUE INDEX `unique_order_date` ON `sbl_orders` (`order_id`, `created_at_date`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241030023022_UpdateSblOrderUniqueIndex') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20241030023022_UpdateSblOrderUniqueIndex', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241104032737_AddIndexForSblOrderTable') THEN

    CREATE INDEX `ix_sbl_orders_symbol` ON `sbl_orders` (`symbol`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241104032737_AddIndexForSblOrderTable') THEN

    CREATE INDEX `ix_sbl_orders_trading_account_no` ON `sbl_orders` (`trading_account_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241104032737_AddIndexForSblOrderTable') THEN

    CREATE INDEX `ix_sbl_instruments_symbol` ON `sbl_instruments` (`symbol`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241104032737_AddIndexForSblOrderTable') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20241104032737_AddIndexForSblOrderTable', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241112061638_AddReviewerIdColumn') THEN

    ALTER TABLE `sbl_orders` ADD `reviewer_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241112061638_AddReviewerIdColumn') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20241112061638_AddReviewerIdColumn', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241120070801_AddStatusIndexToSblOrdersTable') THEN

    CREATE INDEX `ix_sbl_orders_status` ON `sbl_orders` (`status`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241120070801_AddStatusIndexToSblOrdersTable') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20241120070801_AddStatusIndexToSblOrdersTable', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241126100208_AddOrderDateTimeColumn') THEN

    ALTER TABLE `equity_order_state` ADD `order_date_time` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241126100208_AddOrderDateTimeColumn') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20241126100208_AddOrderDateTimeColumn', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241126101536_AddMacthedPriceAndCanlledVolume') THEN

    ALTER TABLE `equity_order_state` ADD `cancelled_volume` int NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241126101536_AddMacthedPriceAndCanlledVolume') THEN

    ALTER TABLE `equity_order_state` ADD `matched_price` decimal(65,30) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20241126101536_AddMacthedPriceAndCanlledVolume') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20241126101536_AddMacthedPriceAndCanlledVolume', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20250114041424_AddRowVersionToStateMachine') THEN

    ALTER TABLE `equity_order_state` ADD `row_version` timestamp(6) NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__SetDbContext` WHERE `migration_id` = '20250114041424_AddRowVersionToStateMachine') THEN

    INSERT INTO `__SetDbContext` (`migration_id`, `product_version`)
    VALUES ('20250114041424_AddRowVersionToStateMachine', '8.0.7');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

