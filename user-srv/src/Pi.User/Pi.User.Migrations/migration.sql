DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID TINYINT(1);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `Extra` = 'auto_increment'
			AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS `POMELO_AFTER_ADD_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID INT(11);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
			AND `COLUMN_TYPE` LIKE '%int%'
			AND `COLUMN_KEY` = 'PRI';
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

CREATE TABLE IF NOT EXISTS `__UserDbContext` (
    `migration_id` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `product_version` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk___user_db_context` PRIMARY KEY (`migration_id`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228033504_InitializedDatabase') THEN

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228033504_InitializedDatabase') THEN

    CREATE TABLE `user_infos` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `customer_id` longtext CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `pk_user_infos` PRIMARY KEY (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228033504_InitializedDatabase') THEN

    CREATE TABLE `cust_codes` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `customer_code` longtext CHARACTER SET utf8mb4 NOT NULL,
        `user_info_id` char(36) COLLATE ascii_general_ci NULL,
        CONSTRAINT `pk_cust_codes` PRIMARY KEY (`id`),
        CONSTRAINT `fk_cust_codes_user_infos_user_info_id` FOREIGN KEY (`user_info_id`) REFERENCES `user_infos` (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228033504_InitializedDatabase') THEN

    CREATE TABLE `devices` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `device_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `device_token` longtext CHARACTER SET utf8mb4 NOT NULL,
        `device_identifier` longtext CHARACTER SET utf8mb4 NOT NULL,
        `language` longtext CHARACTER SET utf8mb4 NOT NULL,
        `user_info_id` char(36) COLLATE ascii_general_ci NULL,
        CONSTRAINT `pk_devices` PRIMARY KEY (`id`),
        CONSTRAINT `fk_devices_user_infos_user_info_id` FOREIGN KEY (`user_info_id`) REFERENCES `user_infos` (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228033504_InitializedDatabase') THEN

    CREATE TABLE `trading_accounts` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `trading_account_id` longtext CHARACTER SET utf8mb4 NOT NULL,
        `user_info_id` char(36) COLLATE ascii_general_ci NULL,
        CONSTRAINT `pk_trading_accounts` PRIMARY KEY (`id`),
        CONSTRAINT `fk_trading_accounts_user_infos_user_info_id` FOREIGN KEY (`user_info_id`) REFERENCES `user_infos` (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228033504_InitializedDatabase') THEN

    CREATE TABLE `notification_preferences` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `important` tinyint(1) NOT NULL,
        `order` tinyint(1) NOT NULL,
        `portfolio` tinyint(1) NOT NULL,
        `wallet` tinyint(1) NOT NULL,
        `market` tinyint(1) NOT NULL,
        `device_foreign_key` char(36) COLLATE ascii_general_ci NOT NULL,
        `user_info_id` char(36) COLLATE ascii_general_ci NULL,
        CONSTRAINT `pk_notification_preferences` PRIMARY KEY (`id`),
        CONSTRAINT `fk_notification_preferences_devices_device_foreign_key` FOREIGN KEY (`device_foreign_key`) REFERENCES `devices` (`id`) ON DELETE CASCADE,
        CONSTRAINT `fk_notification_preferences_user_infos_user_info_id` FOREIGN KEY (`user_info_id`) REFERENCES `user_infos` (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228033504_InitializedDatabase') THEN

    CREATE INDEX `ix_cust_codes_user_info_id` ON `cust_codes` (`user_info_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228033504_InitializedDatabase') THEN

    CREATE INDEX `ix_devices_user_info_id` ON `devices` (`user_info_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228033504_InitializedDatabase') THEN

    CREATE UNIQUE INDEX `ix_notification_preferences_device_foreign_key` ON `notification_preferences` (`device_foreign_key`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228033504_InitializedDatabase') THEN

    CREATE INDEX `ix_notification_preferences_user_info_id` ON `notification_preferences` (`user_info_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228033504_InitializedDatabase') THEN

    CREATE INDEX `ix_trading_accounts_user_info_id` ON `trading_accounts` (`user_info_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228033504_InitializedDatabase') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20221228033504_InitializedDatabase', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228094546_AddCreatedAndUpdatedAt') THEN

    ALTER TABLE `user_infos` ADD `created_at` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228094546_AddCreatedAndUpdatedAt') THEN

    ALTER TABLE `user_infos` ADD `updated_at` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228094546_AddCreatedAndUpdatedAt') THEN

    ALTER TABLE `trading_accounts` ADD `created_at` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228094546_AddCreatedAndUpdatedAt') THEN

    ALTER TABLE `trading_accounts` ADD `updated_at` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228094546_AddCreatedAndUpdatedAt') THEN

    ALTER TABLE `notification_preferences` ADD `created_at` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228094546_AddCreatedAndUpdatedAt') THEN

    ALTER TABLE `notification_preferences` ADD `updated_at` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228094546_AddCreatedAndUpdatedAt') THEN

    ALTER TABLE `devices` ADD `created_at` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228094546_AddCreatedAndUpdatedAt') THEN

    ALTER TABLE `devices` ADD `is_active` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228094546_AddCreatedAndUpdatedAt') THEN

    ALTER TABLE `devices` ADD `updated_at` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228094546_AddCreatedAndUpdatedAt') THEN

    ALTER TABLE `cust_codes` ADD `created_at` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228094546_AddCreatedAndUpdatedAt') THEN

    ALTER TABLE `cust_codes` ADD `updated_at` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20221228094546_AddCreatedAndUpdatedAt') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20221228094546_AddCreatedAndUpdatedAt', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230112080404_platform') THEN

    ALTER TABLE `devices` ADD `platform` longtext CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230112080404_platform') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230112080404_platform', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230120075529_AddNavigationPropertyInDevice') THEN

    ALTER TABLE `cust_codes` DROP FOREIGN KEY `fk_cust_codes_user_infos_user_info_id`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230120075529_AddNavigationPropertyInDevice') THEN

    ALTER TABLE `devices` DROP FOREIGN KEY `fk_devices_user_infos_user_info_id`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230120075529_AddNavigationPropertyInDevice') THEN

    ALTER TABLE `notification_preferences` DROP FOREIGN KEY `fk_notification_preferences_user_infos_user_info_id`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230120075529_AddNavigationPropertyInDevice') THEN

    ALTER TABLE `trading_accounts` DROP FOREIGN KEY `fk_trading_accounts_user_infos_user_info_id`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230120075529_AddNavigationPropertyInDevice') THEN

    ALTER TABLE `cust_codes` ADD CONSTRAINT `fk_cust_codes_user_infos_user_info_id1` FOREIGN KEY (`user_info_id`) REFERENCES `user_infos` (`id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230120075529_AddNavigationPropertyInDevice') THEN

    ALTER TABLE `devices` ADD CONSTRAINT `fk_devices_user_infos_user_info_id1` FOREIGN KEY (`user_info_id`) REFERENCES `user_infos` (`id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230120075529_AddNavigationPropertyInDevice') THEN

    ALTER TABLE `notification_preferences` ADD CONSTRAINT `fk_notification_preferences_user_infos_user_info_id1` FOREIGN KEY (`user_info_id`) REFERENCES `user_infos` (`id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230120075529_AddNavigationPropertyInDevice') THEN

    ALTER TABLE `trading_accounts` ADD CONSTRAINT `fk_trading_accounts_user_infos_user_info_id1` FOREIGN KEY (`user_info_id`) REFERENCES `user_infos` (`id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230120075529_AddNavigationPropertyInDevice') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230120075529_AddNavigationPropertyInDevice', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230123082904_MakeUserInfoCustomerIdUnique') THEN

    ALTER TABLE `user_infos` MODIFY COLUMN `customer_id` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230123082904_MakeUserInfoCustomerIdUnique') THEN

    CREATE UNIQUE INDEX `ix_user_infos_customer_id` ON `user_infos` (`customer_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230123082904_MakeUserInfoCustomerIdUnique') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230123082904_MakeUserInfoCustomerIdUnique', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230209174056_IndexCustCode') THEN

    ALTER TABLE `cust_codes` MODIFY COLUMN `customer_code` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230209174056_IndexCustCode') THEN

    CREATE UNIQUE INDEX `ix_cust_codes_customer_code` ON `cust_codes` (`customer_code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230209174056_IndexCustCode') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230209174056_IndexCustCode', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230215120150_AddSubscriptionIdentifer') THEN

    ALTER TABLE `devices` ADD `subscription_identifier` longtext CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230215120150_AddSubscriptionIdentifer') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230215120150_AddSubscriptionIdentifer', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230228030640_AddCitizenIdColumnToUserInfo') THEN

    ALTER TABLE `user_infos` ADD `citizen_id` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230228030640_AddCitizenIdColumnToUserInfo') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230228030640_AddCitizenIdColumnToUserInfo', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230412110914_AddFirstnameAndLastnameColumn') THEN

    ALTER TABLE `user_infos` ADD `firstname_en` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230412110914_AddFirstnameAndLastnameColumn') THEN

    ALTER TABLE `user_infos` ADD `firstname_th` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230412110914_AddFirstnameAndLastnameColumn') THEN

    ALTER TABLE `user_infos` ADD `lastname_en` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230412110914_AddFirstnameAndLastnameColumn') THEN

    ALTER TABLE `user_infos` ADD `lastname_th` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230412110914_AddFirstnameAndLastnameColumn') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230412110914_AddFirstnameAndLastnameColumn', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230522031119_AddTransactionIdTable') THEN

    ALTER TABLE `user_infos` ADD `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230522031119_AddTransactionIdTable') THEN

    ALTER TABLE `trading_accounts` ADD `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230522031119_AddTransactionIdTable') THEN

    ALTER TABLE `notification_preferences` ADD `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230522031119_AddTransactionIdTable') THEN

    ALTER TABLE `devices` ADD `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230522031119_AddTransactionIdTable') THEN

    ALTER TABLE `cust_codes` ADD `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230522031119_AddTransactionIdTable') THEN

    CREATE TABLE `transaction_ids` (
        `prefix` varchar(4) CHARACTER SET utf8mb4 NOT NULL,
        `date` date NOT NULL,
        `sequence` int NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL,
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_transaction_ids` PRIMARY KEY (`prefix`, `date`)
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230522031119_AddTransactionIdTable') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230522031119_AddTransactionIdTable', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230531040123_AddPhoneNumberAndGlobalAccount') THEN

    ALTER TABLE `user_infos` ADD `global_account` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230531040123_AddPhoneNumberAndGlobalAccount') THEN

    ALTER TABLE `user_infos` ADD `phone_number` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230531040123_AddPhoneNumberAndGlobalAccount') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230531040123_AddPhoneNumberAndGlobalAccount', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230606102509_UpdateTransactionTable') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'transaction_ids');
    ALTER TABLE `transaction_ids` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230606102509_UpdateTransactionTable') THEN

    ALTER TABLE `transaction_ids` ADD `refer_id` varchar(255) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230606102509_UpdateTransactionTable') THEN

    ALTER TABLE `transaction_ids` ADD `customer_code` varchar(255) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230606102509_UpdateTransactionTable') THEN

    ALTER TABLE `transaction_ids` ADD CONSTRAINT `pk_transaction_ids` PRIMARY KEY (`prefix`, `date`, `refer_id`, `customer_code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230606102509_UpdateTransactionTable') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230606102509_UpdateTransactionTable', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230615121557_AlterTransactionIdTableChangePK') THEN

    CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'transaction_ids');
    ALTER TABLE `transaction_ids` DROP PRIMARY KEY;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230615121557_AlterTransactionIdTableChangePK') THEN

    ALTER TABLE `transaction_ids` DROP COLUMN `created_at`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230615121557_AlterTransactionIdTableChangePK') THEN

    ALTER TABLE `transaction_ids` DROP COLUMN `updated_at`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230615121557_AlterTransactionIdTableChangePK') THEN

    ALTER TABLE `transaction_ids` MODIFY COLUMN `customer_code` longtext CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230615121557_AlterTransactionIdTableChangePK') THEN

    ALTER TABLE `transaction_ids` MODIFY COLUMN `refer_id` varchar(16) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230615121557_AlterTransactionIdTableChangePK') THEN

    ALTER TABLE `transaction_ids` ADD CONSTRAINT `pk_transaction_ids` PRIMARY KEY (`prefix`, `date`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230615121557_AlterTransactionIdTableChangePK') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230615121557_AlterTransactionIdTableChangePK', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230616034057_AddReferIdIndexToTrasactionsTable') THEN

    CREATE INDEX `ix_transaction_ids_refer_id` ON `transaction_ids` (`refer_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230616034057_AddReferIdIndexToTrasactionsTable') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230616034057_AddReferIdIndexToTrasactionsTable', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230627135035_AddEmailColumnToUserInfo') THEN

    ALTER TABLE `user_infos` ADD `email` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230627135035_AddEmailColumnToUserInfo') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230627135035_AddEmailColumnToUserInfo', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230811052228_AddEmailHashColumnToUserInfo') THEN

    ALTER TABLE `user_infos` ADD `email_hash` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230811052228_AddEmailHashColumnToUserInfo') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230811052228_AddEmailHashColumnToUserInfo', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230908050949_AddTradingAccountAcctCode') THEN

    ALTER TABLE `trading_accounts` ADD `acct_code` longtext CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230908050949_AddTradingAccountAcctCode') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230908050949_AddTradingAccountAcctCode', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230927042437_AddBankAccAndDocumentTable') THEN

    CREATE TABLE `bank_accounts` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `user_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `account_no` longtext CHARACTER SET utf8mb4 NOT NULL,
        `account_no_hash` longtext CHARACTER SET utf8mb4 NOT NULL,
        `account_name` longtext CHARACTER SET utf8mb4 NOT NULL,
        `account_name_hash` longtext CHARACTER SET utf8mb4 NOT NULL,
        `bank_code` longtext CHARACTER SET utf8mb4 NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL,
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_bank_accounts` PRIMARY KEY (`id`),
        CONSTRAINT `fk_bank_accounts_user_infos_user_id` FOREIGN KEY (`user_id`) REFERENCES `user_infos` (`id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230927042437_AddBankAccAndDocumentTable') THEN

    CREATE TABLE `documents` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `user_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `document_type` int NOT NULL,
        `file_url` longtext CHARACTER SET utf8mb4 NOT NULL,
        `file_name` longtext CHARACTER SET utf8mb4 NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL,
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_documents` PRIMARY KEY (`id`),
        CONSTRAINT `fk_documents_user_infos_user_id` FOREIGN KEY (`user_id`) REFERENCES `user_infos` (`id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230927042437_AddBankAccAndDocumentTable') THEN

    CREATE INDEX `ix_bank_accounts_user_id` ON `bank_accounts` (`user_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230927042437_AddBankAccAndDocumentTable') THEN

    CREATE INDEX `ix_documents_user_id` ON `documents` (`user_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230927042437_AddBankAccAndDocumentTable') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230927042437_AddBankAccAndDocumentTable', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230928033655_AddColumnCitizenHashToUserInfo') THEN

    ALTER TABLE `user_infos` ADD `citizen_id_hash` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230928033655_AddColumnCitizenHashToUserInfo') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230928033655_AddColumnCitizenHashToUserInfo', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230928060901_UpdateDocTypeColumn') THEN

    ALTER TABLE `documents` MODIFY COLUMN `document_type` longtext CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230928060901_UpdateDocTypeColumn') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230928060901_UpdateDocTypeColumn', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    ALTER TABLE `user_infos` MODIFY COLUMN `updated_at` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    ALTER TABLE `trading_accounts` MODIFY COLUMN `updated_at` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    ALTER TABLE `trading_accounts` MODIFY COLUMN `trading_account_id` varchar(255) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    ALTER TABLE `notification_preferences` MODIFY COLUMN `updated_at` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    ALTER TABLE `documents` MODIFY COLUMN `updated_at` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    ALTER TABLE `documents` MODIFY COLUMN `file_url` varchar(200) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    ALTER TABLE `documents` MODIFY COLUMN `file_name` varchar(100) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    ALTER TABLE `devices` MODIFY COLUMN `updated_at` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    ALTER TABLE `cust_codes` MODIFY COLUMN `updated_at` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    ALTER TABLE `bank_accounts` MODIFY COLUMN `updated_at` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    CREATE TABLE `trading_accounts_v2` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `customer_code` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
        `trading_account_no` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
        `product_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `account_status` varchar(2) CHARACTER SET utf8mb4 NULL,
        `credit_line` decimal(16,2) NOT NULL,
        `credit_line_effective_date` date NULL,
        `credit_line_end_date` date NULL,
        `marketing_id` varchar(6) CHARACTER SET utf8mb4 NULL,
        `account_opening_date` date NULL,
        `user_id` char(36) COLLATE ascii_general_ci NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_trading_accounts_v2` PRIMARY KEY (`id`),
        CONSTRAINT `fk_trading_accounts_v2_user_infos_user_info_id` FOREIGN KEY (`user_id`) REFERENCES `user_infos` (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    CREATE INDEX `ix_trading_accounts_trading_account_id` ON `trading_accounts` (`trading_account_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    CREATE INDEX `ix_trading_accounts_v2_customer_code` ON `trading_accounts_v2` (`customer_code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    CREATE INDEX `ix_trading_accounts_v2_trading_account_no` ON `trading_accounts_v2` (`trading_account_no`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    CREATE INDEX `ix_trading_accounts_v2_user_id` ON `trading_accounts_v2` (`user_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20230930104253_CreateTradingAccountV2Table') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20230930104253_CreateTradingAccountV2Table', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20231003083201_AddExaminationTable') THEN

    CREATE TABLE `examinations` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `exam_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `user_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `exam_name` longtext CHARACTER SET utf8mb4 NOT NULL,
        `score` int NOT NULL,
        `expired_at` datetime(6) NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        `created_at` datetime(6) NOT NULL,
        `updated_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_examinations` PRIMARY KEY (`id`),
        CONSTRAINT `fk_examinations_user_infos_user_id` FOREIGN KEY (`user_id`) REFERENCES `user_infos` (`id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20231003083201_AddExaminationTable') THEN

    CREATE INDEX `ix_examinations_user_id` ON `examinations` (`user_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20231003083201_AddExaminationTable') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20231003083201_AddExaminationTable', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20231011042757_DeleteTradingAccountV2sTable') THEN

    DROP TABLE `trading_accounts_v2`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20231011042757_DeleteTradingAccountV2sTable') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20231011042757_DeleteTradingAccountV2sTable', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20231213093818_AddUserInfoPlaceOfBirth') THEN

    ALTER TABLE `user_infos` ADD `place_of_birth_city` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20231213093818_AddUserInfoPlaceOfBirth') THEN

    ALTER TABLE `user_infos` ADD `place_of_birth_country` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20231213093818_AddUserInfoPlaceOfBirth') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20231213093818_AddUserInfoPlaceOfBirth', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20240606041218_AddUserInfoPhoneNumberHash') THEN

    ALTER TABLE `user_infos` ADD `phone_number_hash` varchar(255) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20240606041218_AddUserInfoPhoneNumberHash') THEN

    CREATE UNIQUE INDEX `ix_user_infos_phone_number_hash` ON `user_infos` (`phone_number_hash`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20240606041218_AddUserInfoPhoneNumberHash') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20240606041218_AddUserInfoPhoneNumberHash', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20240807080054_AddBankBranchCodeInBankAccount') THEN

    ALTER TABLE `bank_accounts` ADD `bank_branch_code` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20240807080054_AddBankBranchCodeInBankAccount') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20240807080054_AddBankBranchCodeInBankAccount', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    ALTER TABLE `cust_codes` DROP FOREIGN KEY `fk_cust_codes_user_infos_user_info_id1`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    ALTER TABLE `devices` DROP FOREIGN KEY `fk_devices_user_infos_user_info_id1`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    ALTER TABLE `notification_preferences` DROP FOREIGN KEY `fk_notification_preferences_user_infos_user_info_id1`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    ALTER TABLE `trading_accounts` DROP FOREIGN KEY `fk_trading_accounts_user_infos_user_info_id1`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    ALTER TABLE `user_infos` ADD `address_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    ALTER TABLE `user_infos` ADD `kyc_id` char(36) COLLATE ascii_general_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE TABLE `addresses` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `place` varchar(100) CHARACTER SET utf8mb4 NULL,
        `home_no` varchar(100) CHARACTER SET utf8mb4 NULL,
        `town` varchar(100) CHARACTER SET utf8mb4 NULL,
        `building` varchar(100) CHARACTER SET utf8mb4 NULL,
        `village` varchar(100) CHARACTER SET utf8mb4 NULL,
        `floor` varchar(100) CHARACTER SET utf8mb4 NULL,
        `soi` varchar(100) CHARACTER SET utf8mb4 NULL,
        `road` varchar(100) CHARACTER SET utf8mb4 NULL,
        `sub_district` varchar(100) CHARACTER SET utf8mb4 NULL,
        `district` varchar(100) CHARACTER SET utf8mb4 NULL,
        `province` varchar(100) CHARACTER SET utf8mb4 NULL,
        `country` varchar(100) CHARACTER SET utf8mb4 NULL,
        `zip_code` varchar(10) CHARACTER SET utf8mb4 NULL,
        `country_code` varchar(3) CHARACTER SET utf8mb4 NULL,
        `province_code` varchar(10) CHARACTER SET utf8mb4 NULL,
        `user_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_addresses` PRIMARY KEY (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE TABLE `bank_account_v2s` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `account_no` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `hashed_account_no` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `account_name` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
        `bank_code` varchar(5) CHARACTER SET utf8mb4 NOT NULL,
        `branch_code` varchar(8) CHARACTER SET utf8mb4 NOT NULL,
        `payment_token` varchar(100) CHARACTER SET utf8mb4 NULL,
        `ats_effective_date` datetime(6) NULL,
        `status` int NOT NULL,
        `user_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `trade_account_bank_account_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_bank_account_v2s` PRIMARY KEY (`id`),
        CONSTRAINT `fk_bank_account_v2s_user_infos_user_id` FOREIGN KEY (`user_id`) REFERENCES `user_infos` (`id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE TABLE `kycs` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `review_date` date NOT NULL,
        `expired_date` date NOT NULL,
        `user_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_kycs` PRIMARY KEY (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE TABLE `suitability_tests` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `grade` varchar(1) CHARACTER SET utf8mb4 NOT NULL,
        `score` int NOT NULL,
        `version` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
        `review_date` date NOT NULL,
        `expired_date` date NOT NULL,
        `user_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_suitability_tests` PRIMARY KEY (`id`),
        CONSTRAINT `fk_suitability_tests_user_infos_user_id` FOREIGN KEY (`user_id`) REFERENCES `user_infos` (`id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE TABLE `user_accounts` (
        `id` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
        `user_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `user_id1` char(36) COLLATE ascii_general_ci NULL,
        `created_at` datetime(6) NOT NULL,
        `updated_at` datetime(6) NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_user_accounts` PRIMARY KEY (`id`),
        CONSTRAINT `fk_user_accounts_user_infos_user_id` FOREIGN KEY (`user_id`) REFERENCES `user_infos` (`id`) ON DELETE CASCADE,
        CONSTRAINT `fk_user_accounts_user_infos_user_id1` FOREIGN KEY (`user_id1`) REFERENCES `user_infos` (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE TABLE `trade_accounts` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `account_number` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
        `account_type` longtext CHARACTER SET utf8mb4 NOT NULL,
        `account_type_code` varchar(2) CHARACTER SET utf8mb4 NOT NULL,
        `exchange_market_id` varchar(2) CHARACTER SET utf8mb4 NOT NULL,
        `account_status` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
        `credit_line` decimal(16,2) NOT NULL,
        `credit_line_currency` varchar(3) CHARACTER SET utf8mb4 NOT NULL,
        `effective_date` date NULL,
        `end_date` date NULL,
        `marketing_id` varchar(10) CHARACTER SET utf8mb4 NULL,
        `sale_license` varchar(12) CHARACTER SET utf8mb4 NULL,
        `open_date` date NULL,
        `user_account_id` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
        `created_at` datetime(6) NOT NULL,
        `updated_at` datetime(6) NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_trade_accounts` PRIMARY KEY (`id`),
        CONSTRAINT `fk_trade_accounts_user_accounts_user_account_id` FOREIGN KEY (`user_account_id`) REFERENCES `user_accounts` (`id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE TABLE `external_accounts` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `value` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `provider_id` int NOT NULL,
        `trade_account_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `created_at` datetime(6) NOT NULL,
        `updated_at` datetime(6) NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_external_accounts` PRIMARY KEY (`id`),
        CONSTRAINT `fk_external_accounts_trade_accounts_id` FOREIGN KEY (`id`) REFERENCES `trade_accounts` (`id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE TABLE `trade_account_bank_accounts` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `rp_type` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
        `payment_type` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
        `transaction_type` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
        `bank_account_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `trade_account_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `row_version` timestamp(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `pk_trade_account_bank_accounts` PRIMARY KEY (`id`),
        CONSTRAINT `fk_trade_account_bank_accounts_bank_account_v2s_id` FOREIGN KEY (`id`) REFERENCES `bank_account_v2s` (`id`) ON DELETE CASCADE,
        CONSTRAINT `fk_trade_account_bank_accounts_trade_accounts_trade_account_id` FOREIGN KEY (`trade_account_id`) REFERENCES `trade_accounts` (`id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE INDEX `ix_user_infos_address_id` ON `user_infos` (`address_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE INDEX `ix_user_infos_kyc_id` ON `user_infos` (`kyc_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE INDEX `ix_addresses_user_id` ON `addresses` (`user_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE UNIQUE INDEX `ix_bank_account_v2s_hashed_account_no_bank_code` ON `bank_account_v2s` (`hashed_account_no`, `bank_code`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE INDEX `ix_bank_account_v2s_user_id` ON `bank_account_v2s` (`user_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE UNIQUE INDEX `ix_external_accounts_trade_account_id_provider_id` ON `external_accounts` (`trade_account_id`, `provider_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE INDEX `ix_kycs_user_id` ON `kycs` (`user_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE INDEX `ix_suitability_tests_user_id` ON `suitability_tests` (`user_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE INDEX `ix_trade_account_bank_accounts_bank_account_id` ON `trade_account_bank_accounts` (`bank_account_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE INDEX `ix_trade_account_bank_accounts_trade_account_id` ON `trade_account_bank_accounts` (`trade_account_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE INDEX `ix_trade_accounts_user_account_id` ON `trade_accounts` (`user_account_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE INDEX `ix_user_accounts_user_id` ON `user_accounts` (`user_id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    CREATE INDEX `ix_user_accounts_user_id1` ON `user_accounts` (`user_id1`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    ALTER TABLE `cust_codes` ADD CONSTRAINT `fk_cust_codes_user_infos_user_info_id` FOREIGN KEY (`user_info_id`) REFERENCES `user_infos` (`id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    ALTER TABLE `devices` ADD CONSTRAINT `fk_devices_user_infos_user_info_id` FOREIGN KEY (`user_info_id`) REFERENCES `user_infos` (`id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    ALTER TABLE `notification_preferences` ADD CONSTRAINT `fk_notification_preferences_user_infos_user_info_id` FOREIGN KEY (`user_info_id`) REFERENCES `user_infos` (`id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    ALTER TABLE `trading_accounts` ADD CONSTRAINT `fk_trading_accounts_user_infos_user_info_id` FOREIGN KEY (`user_info_id`) REFERENCES `user_infos` (`id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    ALTER TABLE `user_infos` ADD CONSTRAINT `fk_user_infos_addresses_address_id` FOREIGN KEY (`address_id`) REFERENCES `addresses` (`id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    ALTER TABLE `user_infos` ADD CONSTRAINT `fk_user_infos_kycs_kyc_id` FOREIGN KEY (`kyc_id`) REFERENCES `kycs` (`id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241021100226_AddUserDbNewStructure') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20241021100226_AddUserDbNewStructure', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241022081933_RefactorUserAccount') THEN

    ALTER TABLE `user_accounts` DROP FOREIGN KEY `fk_user_accounts_user_infos_user_id1`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241022081933_RefactorUserAccount') THEN

    ALTER TABLE `user_accounts` DROP INDEX `ix_user_accounts_user_id1`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241022081933_RefactorUserAccount') THEN

    ALTER TABLE `user_accounts` DROP COLUMN `user_id1`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241022081933_RefactorUserAccount') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20241022081933_RefactorUserAccount', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241107085050_AddMissingColumnOnUserInfo') THEN

    ALTER TABLE `user_infos` ADD `date_of_birth` date NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241107085050_AddMissingColumnOnUserInfo') THEN

    ALTER TABLE `user_accounts` ADD `user_account_type` varchar(20) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20241107085050_AddMissingColumnOnUserInfo') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20241107085050_AddMissingColumnOnUserInfo', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103055958_RemoveTradingAccountBankAccount') THEN

    DROP TABLE `trade_account_bank_accounts`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103055958_RemoveTradingAccountBankAccount') THEN

    ALTER TABLE `bank_account_v2s` DROP COLUMN `trade_account_bank_account_id`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103055958_RemoveTradingAccountBankAccount') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20250103055958_RemoveTradingAccountBankAccount', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103104350_FixExternalAccForeignKey') THEN

    ALTER TABLE `external_accounts` DROP FOREIGN KEY `fk_external_accounts_trade_accounts_id`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103104350_FixExternalAccForeignKey') THEN

    ALTER TABLE `external_accounts` ADD CONSTRAINT `fk_external_accounts_trade_accounts_trade_account_id` FOREIGN KEY (`trade_account_id`) REFERENCES `trade_accounts` (`id`) ON DELETE CASCADE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103104350_FixExternalAccForeignKey') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20250103104350_FixExternalAccForeignKey', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103113850_UpdateAddressPropertyLength') THEN

    ALTER TABLE `addresses` MODIFY COLUMN `village` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103113850_UpdateAddressPropertyLength') THEN

    ALTER TABLE `addresses` MODIFY COLUMN `town` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103113850_UpdateAddressPropertyLength') THEN

    ALTER TABLE `addresses` MODIFY COLUMN `sub_district` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103113850_UpdateAddressPropertyLength') THEN

    ALTER TABLE `addresses` MODIFY COLUMN `soi` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103113850_UpdateAddressPropertyLength') THEN

    ALTER TABLE `addresses` MODIFY COLUMN `road` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103113850_UpdateAddressPropertyLength') THEN

    ALTER TABLE `addresses` MODIFY COLUMN `province` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103113850_UpdateAddressPropertyLength') THEN

    ALTER TABLE `addresses` MODIFY COLUMN `place` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103113850_UpdateAddressPropertyLength') THEN

    ALTER TABLE `addresses` MODIFY COLUMN `home_no` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103113850_UpdateAddressPropertyLength') THEN

    ALTER TABLE `addresses` MODIFY COLUMN `floor` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103113850_UpdateAddressPropertyLength') THEN

    ALTER TABLE `addresses` MODIFY COLUMN `district` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103113850_UpdateAddressPropertyLength') THEN

    ALTER TABLE `addresses` MODIFY COLUMN `country` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103113850_UpdateAddressPropertyLength') THEN

    ALTER TABLE `addresses` MODIFY COLUMN `building` varchar(1000) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250103113850_UpdateAddressPropertyLength') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20250103113850_UpdateAddressPropertyLength', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250120045435_AddTempDob') THEN

    ALTER TABLE `user_infos` ADD `temp_date_of_birth` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250120045435_AddTempDob') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20250120045435_AddTempDob', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250120090722_RemoveDobColumn') THEN

    ALTER TABLE `user_infos` DROP COLUMN `date_of_birth`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250120090722_RemoveDobColumn') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20250120090722_RemoveDobColumn', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250120090850_UpdateTempDobToDob') THEN

    ALTER TABLE `user_infos` RENAME COLUMN `temp_date_of_birth` TO `date_of_birth`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250120090850_UpdateTempDobToDob') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20250120090850_UpdateTempDobToDob', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250125182212_AddWealthTypeToUserInfo') THEN

    ALTER TABLE `user_infos` ADD `wealth_type` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250125182212_AddWealthTypeToUserInfo') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20250125182212_AddWealthTypeToUserInfo', '8.0.7');

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
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250205034604_IncreaseColumnLengthInBankAccountV2') THEN

    ALTER TABLE `bank_account_v2s` MODIFY COLUMN `account_no` varchar(1000) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250205034604_IncreaseColumnLengthInBankAccountV2') THEN

    ALTER TABLE `bank_account_v2s` MODIFY COLUMN `account_name` varchar(1000) CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__UserDbContext` WHERE `migration_id` = '20250205034604_IncreaseColumnLengthInBankAccountV2') THEN

    INSERT INTO `__UserDbContext` (`migration_id`, `product_version`)
    VALUES ('20250205034604_IncreaseColumnLengthInBankAccountV2', '8.0.7');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;

DROP PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`;

