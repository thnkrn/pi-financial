CREATE TABLE IF NOT EXISTS `__NumberGeneratorDbContext` (
    `migration_id` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `product_version` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk___number_generator_db_context` PRIMARY KEY (`migration_id`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__NumberGeneratorDbContext` WHERE `migration_id` = '20240703085047_InitNumberGeneratorTable') THEN

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
    IF NOT EXISTS(SELECT 1 FROM `__NumberGeneratorDbContext` WHERE `migration_id` = '20240703085047_InitNumberGeneratorTable') THEN

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
    IF NOT EXISTS(SELECT 1 FROM `__NumberGeneratorDbContext` WHERE `migration_id` = '20240703085047_InitNumberGeneratorTable') THEN

    INSERT INTO `__NumberGeneratorDbContext` (`migration_id`, `product_version`)
    VALUES ('20240703085047_InitNumberGeneratorTable', '8.0.7');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

