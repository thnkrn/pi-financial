CREATE TABLE IF NOT EXISTS `__TfexDbContext` (
    `migration_id` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `product_version` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk___tfex_db_context` PRIMARY KEY (`migration_id`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20240711063944_AddActivitiesLogTable') THEN

    CREATE TABLE `activities_logs` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `user_id` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `customer_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `account_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `request_type` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `request_body` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `order_no` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL,
        `response_body` longtext COLLATE utf8mb4_0900_ai_ci NULL,
        `requested_at` datetime(6) NULL,
        `completed_at` datetime(6) NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime NOT NULL DEFAULT NOW(),
        `updated_at` datetime NOT NULL DEFAULT NOW(),
        CONSTRAINT `pk_activities_logs` PRIMARY KEY (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20240711063944_AddActivitiesLogTable') THEN

    CREATE INDEX `ix_activities_logs_order_no` ON `activities_logs` (`order_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20240711063944_AddActivitiesLogTable') THEN

    INSERT INTO `__TfexDbContext` (`migration_id`, `product_version`)
    VALUES ('20240711063944_AddActivitiesLogTable', '8.0.6');

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
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20240816092208_AddInitialMarginTable') THEN

    CREATE TABLE `initial_margins` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `symbol` varchar(36) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `product_type` varchar(6) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `im` decimal(65,30) NOT NULL,
        `im_outright` decimal(65,30) NOT NULL,
        `im_spread` decimal(65,30) NOT NULL,
        `as_of_date` date NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime NOT NULL DEFAULT NOW(),
        `updated_at` datetime NOT NULL DEFAULT NOW(),
        CONSTRAINT `pk_initial_margins` PRIMARY KEY (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20240816092208_AddInitialMarginTable') THEN

    CREATE INDEX `ix_initial_margins_symbol` ON `initial_margins` (`symbol`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20240816092208_AddInitialMarginTable') THEN

    INSERT INTO `__TfexDbContext` (`migration_id`, `product_version`)
    VALUES ('20240816092208_AddInitialMarginTable', '8.0.6');

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
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241206063923_AddColumnForActivityLog') THEN

    ALTER TABLE `activities_logs` ADD `failed_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241206063923_AddColumnForActivityLog') THEN

    ALTER TABLE `activities_logs` ADD `is_success` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241206063923_AddColumnForActivityLog') THEN

    ALTER TABLE `activities_logs` ADD `price` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241206063923_AddColumnForActivityLog') THEN

    ALTER TABLE `activities_logs` ADD `price_type` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241206063923_AddColumnForActivityLog') THEN

    ALTER TABLE `activities_logs` ADD `qty` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241206063923_AddColumnForActivityLog') THEN

    ALTER TABLE `activities_logs` ADD `reject_code` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241206063923_AddColumnForActivityLog') THEN

    ALTER TABLE `activities_logs` ADD `reject_reason` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241206063923_AddColumnForActivityLog') THEN

    ALTER TABLE `activities_logs` ADD `side` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241206063923_AddColumnForActivityLog') THEN

    ALTER TABLE `activities_logs` ADD `symbol` longtext COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241206063923_AddColumnForActivityLog') THEN

    INSERT INTO `__TfexDbContext` (`migration_id`, `product_version`)
    VALUES ('20241206063923_AddColumnForActivityLog', '8.0.6');

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
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241206064014_AddActivityLogView') THEN


                    CREATE OR REPLACE VIEW activities_logs_view AS
                    SELECT
                        al.user_id,
                        al.customer_code,
                        al.account_code,
                        al.order_no,
                        al.request_type,
                        al.request_body,
                        al.response_body,
                        al.is_success,
                        al.failed_reason,
                        al.symbol,
                        al.side,
                        al.price_type,
                        al.price,
                        al.qty,
                        al.reject_code,
                        al.reject_reason,
                        al.requested_at,
                        al.completed_at,
                        al.created_at,
                        al.updated_at
                    FROM activities_logs al;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241206064014_AddActivityLogView') THEN

    INSERT INTO `__TfexDbContext` (`migration_id`, `product_version`)
    VALUES ('20241206064014_AddActivityLogView', '8.0.6');

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
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241209044035_EditActivityLogColumn') THEN

    ALTER TABLE `activities_logs` MODIFY COLUMN `symbol` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241209044035_EditActivityLogColumn') THEN

    ALTER TABLE `activities_logs` MODIFY COLUMN `side` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241209044035_EditActivityLogColumn') THEN

    ALTER TABLE `activities_logs` MODIFY COLUMN `reject_reason` varchar(200) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241209044035_EditActivityLogColumn') THEN

    ALTER TABLE `activities_logs` MODIFY COLUMN `reject_code` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241209044035_EditActivityLogColumn') THEN

    ALTER TABLE `activities_logs` MODIFY COLUMN `qty` int NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241209044035_EditActivityLogColumn') THEN

    ALTER TABLE `activities_logs` MODIFY COLUMN `price_type` varchar(36) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241209044035_EditActivityLogColumn') THEN

    ALTER TABLE `activities_logs` MODIFY COLUMN `price` decimal(65,30) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241209044035_EditActivityLogColumn') THEN

    ALTER TABLE `activities_logs` MODIFY COLUMN `failed_reason` varchar(200) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241209044035_EditActivityLogColumn') THEN

    INSERT INTO `__TfexDbContext` (`migration_id`, `product_version`)
    VALUES ('20241209044035_EditActivityLogColumn', '8.0.6');

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
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241209052833_EditActivityLogView') THEN


                    CREATE OR REPLACE VIEW activities_logs_view AS
                    SELECT
                        al.user_id,
                        al.customer_code,
                        al.account_code,
                        al.order_no,
                        al.request_type,
                        al.request_body,
                        al.response_body,
                        al.is_success,
                        al.failed_reason,
                        al.symbol,
                        al.side,
                        al.price_type,
                        al.price,
                        al.qty,
                        al.reject_code,
                        al.reject_reason,
                        al.requested_at,
                        al.completed_at,
                        al.created_at,
                        al.updated_at
                    FROM activities_logs al;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TfexDbContext` WHERE `migration_id` = '20241209052833_EditActivityLogView') THEN

    INSERT INTO `__TfexDbContext` (`migration_id`, `product_version`)
    VALUES ('20241209052833_EditActivityLogView', '8.0.6');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

