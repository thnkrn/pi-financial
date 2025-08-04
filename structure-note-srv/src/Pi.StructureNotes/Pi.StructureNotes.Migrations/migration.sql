CREATE TABLE IF NOT EXISTS `__SnDbContext`
(
    `migration_id`
    varchar
(
    150
) CHARACTER SET utf8mb4 NOT NULL,
    `product_version` varchar
(
    32
) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk___sn_db_context` PRIMARY KEY
(
    `migration_id`
)
    ) CHARACTER SET =utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE TABLE `cash`
(
    `id`                char(36) COLLATE ascii_general_ci NOT NULL,
    `symbol`            varchar(50) COLLATE utf8mb4_0900_ai_ci NULL,
    `currency`          varchar(50) COLLATE utf8mb4_0900_ai_ci NULL,
    `market_value`      decimal(65, 30) NULL,
    `cost_value`        decimal(65, 30) NULL,
    `gain_in_portfolio` decimal(65, 30) NULL,
    `account_id`        varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
    `account_no`        varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
    `created_at`        datetime(6) NOT NULL,
    `updated_at`        datetime(6) NOT NULL,
    `as_of`             datetime(6) NULL,
    CONSTRAINT `pk_cash` PRIMARY KEY (`id`)
) COLLATE=utf8mb4_0900_ai_ci;

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE TABLE `notes`
(
    `id`             char(36) COLLATE ascii_general_ci NOT NULL,
    `isin`           varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
    `symbol`         varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
    `currency`       varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
    `market_value`   decimal(65, 30) NULL,
    `cost_value`     decimal(65, 30) NULL,
    `type`           varchar(200) COLLATE utf8mb4_0900_ai_ci NULL,
    `yield`          decimal(65, 30) NULL,
    `rebate`         decimal(65, 30) NULL,
    `underlying`     varchar(400) COLLATE utf8mb4_0900_ai_ci NULL,
    `tenors`         int NULL,
    `trade_date`     datetime(6) NOT NULL,
    `issue_date`     datetime(6) NOT NULL,
    `valuation_date` datetime(6) NOT NULL,
    `issuer`         varchar(400) COLLATE utf8mb4_0900_ai_ci NULL,
    `account_id`     varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
    `account_no`     varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
    `created_at`     datetime(6) NOT NULL,
    `updated_at`     datetime(6) NOT NULL,
    `as_of`          datetime(6) NULL,
    CONSTRAINT `pk_notes` PRIMARY KEY (`id`)
) COLLATE=utf8mb4_0900_ai_ci;

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE TABLE `stocks`
(
    `id`         char(36) COLLATE ascii_general_ci NOT NULL,
    `symbol`     varchar(50) COLLATE utf8mb4_0900_ai_ci NULL,
    `currency`   varchar(50) COLLATE utf8mb4_0900_ai_ci NULL,
    `units`      int NULL,
    `available`  int NULL,
    `cost_price` decimal(65, 30) NULL,
    `account_id` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
    `account_no` varchar(100) COLLATE utf8mb4_0900_ai_ci NULL,
    `created_at` datetime(6) NOT NULL,
    `updated_at` datetime(6) NOT NULL,
    `as_of`      datetime(6) NULL,
    CONSTRAINT `pk_stocks` PRIMARY KEY (`id`)
) COLLATE=utf8mb4_0900_ai_ci;

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE INDEX `ix_cash_account_id` ON `cash` (`account_id`);

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE INDEX `ix_cash_account_no` ON `cash` (`account_no`);

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE INDEX `ix_cash_currency` ON `cash` (`currency`);

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE INDEX `ix_notes_account_id` ON `notes` (`account_id`);

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE INDEX `ix_notes_account_no` ON `notes` (`account_no`);

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE INDEX `ix_notes_currency` ON `notes` (`currency`);

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE INDEX `ix_notes_symbol` ON `notes` (`symbol`);

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE INDEX `ix_stocks_account_id` ON `stocks` (`account_id`);

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE INDEX `ix_stocks_account_no` ON `stocks` (`account_no`);

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE INDEX `ix_stocks_currency` ON `stocks` (`currency`);

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

CREATE INDEX `ix_stocks_symbol` ON `stocks` (`symbol`);

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER
//
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM `__SnDbContext` WHERE `migration_id` = '20240319092509_mg0') THEN

    INSERT INTO `__SnDbContext` (`migration_id`, `product_version`)
    VALUES ('20240319092509_mg0', '7.0.11');

END IF;
END
//
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

