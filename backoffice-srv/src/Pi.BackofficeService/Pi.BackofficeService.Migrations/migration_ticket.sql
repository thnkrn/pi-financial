CREATE TABLE IF NOT EXISTS `__TicketDbContext` (
    `migration_id` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `product_version` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk___ticket_db_context` PRIMARY KEY (`migration_id`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230526040015_AddTicketTable') THEN

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230526040015_AddTicketTable') THEN

    CREATE TABLE `ticket_state` (
        `correlation_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `transaction_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `current_state` longtext CHARACTER SET utf8mb4 NULL,
        `status` varchar(255) CHARACTER SET utf8mb4 NULL,
        `method` varchar(255) CHARACTER SET utf8mb4 NULL,
        `maker_id` char(36) COLLATE ascii_general_ci NULL,
        `maker_remark` longtext CHARACTER SET utf8mb4 NULL,
        `checker_id` char(36) COLLATE ascii_general_ci NULL,
        `checker_remark` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `pk_ticket_state` PRIMARY KEY (`correlation_id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230526040015_AddTicketTable') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20230526040015_AddTicketTable', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230607090025_UpdateTicketColumns') THEN

    ALTER TABLE `ticket_state` MODIFY COLUMN `status` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230607090025_UpdateTicketColumns') THEN

    ALTER TABLE `ticket_state` MODIFY COLUMN `method` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230607090025_UpdateTicketColumns') THEN

    ALTER TABLE `ticket_state` ADD `customer_name` varchar(255) CHARACTER SET utf8mb4 NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230607090025_UpdateTicketColumns') THEN

    ALTER TABLE `ticket_state` ADD `error_mapping_id` int NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230607090025_UpdateTicketColumns') THEN

    ALTER TABLE `ticket_state` ADD `request_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230607090025_UpdateTicketColumns') THEN

    ALTER TABLE `ticket_state` ADD `response_address` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230607090025_UpdateTicketColumns') THEN

    ALTER TABLE `ticket_state` ADD `transaction_no` varchar(255) CHARACTER SET utf8mb4 NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230607090025_UpdateTicketColumns') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20230607090025_UpdateTicketColumns', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230607102711_ChangeErrorMappingType') THEN

    ALTER TABLE `ticket_state` MODIFY COLUMN `transaction_no` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230607102711_ChangeErrorMappingType') THEN

    ALTER TABLE `ticket_state` MODIFY COLUMN `error_mapping_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230607102711_ChangeErrorMappingType') THEN

    ALTER TABLE `ticket_state` MODIFY COLUMN `customer_name` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230607102711_ChangeErrorMappingType') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20230607102711_ChangeErrorMappingType', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230608040906_AddTransactionTypeToTicket') THEN

    ALTER TABLE `ticket_state` ADD `transaction_type` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230608040906_AddTransactionTypeToTicket') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20230608040906_AddTransactionTypeToTicket', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230609075931_FixTransactionTypeColumn') THEN

    ALTER TABLE `ticket_state` MODIFY COLUMN `transaction_type` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230609075931_FixTransactionTypeColumn') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20230609075931_FixTransactionTypeColumn', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230613033233_AddCustCodeAndDateTimeColumns') THEN

    ALTER TABLE `ticket_state` ADD `created_at` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230613033233_AddCustCodeAndDateTimeColumns') THEN

    ALTER TABLE `ticket_state` ADD `customer_code` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230613033233_AddCustCodeAndDateTimeColumns') THEN

    ALTER TABLE `ticket_state` ADD `updated_at` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230613033233_AddCustCodeAndDateTimeColumns') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20230613033233_AddCustCodeAndDateTimeColumns', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230613091927_RemoveUpdateAtColumn') THEN

    ALTER TABLE `ticket_state` DROP COLUMN `updated_at`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230613091927_RemoveUpdateAtColumn') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20230613091927_RemoveUpdateAtColumn', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230614044556_UpdateActionColumns') THEN

    ALTER TABLE `ticket_state` RENAME COLUMN `method` TO `request_action`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230614044556_UpdateActionColumns') THEN

    ALTER TABLE `ticket_state` ADD `checker_action` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230614044556_UpdateActionColumns') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20230614044556_UpdateActionColumns', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230615030228_AddTimestampColumns') THEN

    ALTER TABLE `ticket_state` ADD `checked_at` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230615030228_AddTimestampColumns') THEN

    ALTER TABLE `ticket_state` ADD `requested_at` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230615030228_AddTimestampColumns') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20230615030228_AddTimestampColumns', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230615045038_AddTicketNoColumn') THEN

    ALTER TABLE `ticket_state` ADD `ticket_no` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230615045038_AddTicketNoColumn') THEN

    CREATE UNIQUE INDEX `ix_ticket_state_ticket_no` ON `ticket_state` (`ticket_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230615045038_AddTicketNoColumn') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20230615045038_AddTicketNoColumn', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230623061520_UpdateColumns') THEN

    ALTER TABLE `ticket_state` MODIFY COLUMN `transaction_no` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230623061520_UpdateColumns') THEN

    ALTER TABLE `ticket_state` MODIFY COLUMN `transaction_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230623061520_UpdateColumns') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20230623061520_UpdateColumns', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230807100935_UpdateErrrorColumns') THEN

    ALTER TABLE `ticket_state` RENAME COLUMN `error_mapping_id` TO `response_code_id`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20230807100935_UpdateErrrorColumns') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20230807100935_UpdateErrrorColumns', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20231102073150_AddTransactionStateColumn') THEN

    ALTER TABLE `ticket_state` ADD `transaction_state` varchar(255) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20231102073150_AddTransactionStateColumn') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20231102073150_AddTransactionStateColumn', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20231102075944_UpdateTransactionStateToNullable') THEN

    ALTER TABLE `ticket_state` MODIFY COLUMN `transaction_state` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20231102075944_UpdateTransactionStateToNullable') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20231102075944_UpdateTransactionStateToNullable', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20240326044214_AddPayloadFieldIntoSagaDb') THEN

    ALTER TABLE `ticket_state` ADD `payload` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__TicketDbContext` WHERE `migration_id` = '20240326044214_AddPayloadFieldIntoSagaDb') THEN

    INSERT INTO `__TicketDbContext` (`migration_id`, `product_version`)
    VALUES ('20240326044214_AddPayloadFieldIntoSagaDb', '7.0.5');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

