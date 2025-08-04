CREATE TABLE IF NOT EXISTS `__BackofficeDbContext` (
    `migration_id` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `product_version` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk___backoffice_db_context` PRIMARY KEY (`migration_id`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230530042832_AddUserTable') THEN

    CREATE TABLE `users` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `iam_user_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `first_name` varchar(255) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `last_name` varchar(255) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `email` varchar(255) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `created_at` datetime(6) NOT NULL,
        CONSTRAINT `pk_users` PRIMARY KEY (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230530042832_AddUserTable') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230530042832_AddUserTable', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230601042558_AddErrorTable') THEN

    CREATE TABLE `bank` (
        `code` varchar(255) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `name` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `abbr` varchar(255) COLLATE utf8mb4_0900_ai_ci NOT NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230601042558_AddErrorTable') THEN

    CREATE TABLE `error_handler_actions` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `error_mapping_id` char(36) COLLATE ascii_general_ci NOT NULL,
        `action` varchar(50) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        CONSTRAINT `pk_error_handler_actions` PRIMARY KEY (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230601042558_AddErrorTable') THEN

    CREATE TABLE `error_mappings` (
        `id` char(36) COLLATE ascii_general_ci NOT NULL,
        `machine` varchar(100) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `state` varchar(100) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `suggestion` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `description` longtext COLLATE utf8mb4_0900_ai_ci NOT NULL,
        CONSTRAINT `pk_error_mappings` PRIMARY KEY (`id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230601042558_AddErrorTable') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230601042558_AddErrorTable', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230601042633_AddBankTable') THEN

    ALTER TABLE `bank` RENAME `banks`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230601042633_AddBankTable') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230601042633_AddBankTable', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230601044726_UpdateBankColumnName') THEN

    ALTER TABLE `banks` RENAME COLUMN `abbr` TO `abbreviation`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230601044726_UpdateBankColumnName') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230601044726_UpdateBankColumnName', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230601050531_AddPopulateScript') THEN

    INSERT INTO `banks` (`code`, `name`, `abbreviation`)
    VALUES ('002', 'Bangkok Bank Public Company Limited', 'BBL'),
    ('004', 'Kasikornbank Public Company Limited', 'KBNK'),
    ('006', 'Krung Thai Bank Public Company Limited', 'KTB'),
    ('011', 'TMBThanachart Bank Public Company Limited', 'TTB'),
    ('014', 'Siam Commercial Bank Public Company Limited', 'SCB'),
    ('017', 'Citibank, N.A', 'CITI'),
    ('018', 'Sumitomo Mitsui Banking Corporation', 'SMBC'),
    ('020', 'Standard Chartered Bank (THAI) Public Company Limited', 'SCBT'),
    ('022', 'CIMB Thai Bank Public Company Limited', 'CIMBT'),
    ('024', 'United Overseas Bank (THAI) Public Company Limited', 'UOBT'),
    ('025', 'Bank of Ayudhya Public Company Limited', 'BAY'),
    ('029', 'Indian Overseas Bank', 'IOBA'),
    ('030', 'Government Saving Bank', 'GSB'),
    ('031', 'Hong Kong & Shanghai Corporation Limited', 'HSBC'),
    ('032', 'Deutsche Bank Aktiengesellschaft', 'DBBK'),
    ('033', 'Government Housing Bank', 'GHBA'),
    ('034', 'Bank of Agriculture and Agricultural Cooperatives', 'BAAC'),
    ('039', 'Mizuho Bank Bangkok Branch', 'MHBC'),
    ('045', 'BNP Paribas, Bangkok Branch', 'BNPP'),
    ('052', 'Bank of China Limited', 'BOCB'),
    ('066', 'Islamic Bank of Thailand', 'ISBT'),
    ('067', 'Tisco Bank Public Company Limited', 'TSCO'),
    ('069', 'Kiatnakin Bank Public Company Limited', 'KKP'),
    ('070', 'Industrial and Commercial Bank of China (THAI) Public Company Limited', 'ICBC'),
    ('071', 'Thai Credit Retail Bank Public Company Limited', 'TCRB'),
    ('073', 'Land and Houses Bank Public Company Limited', 'LH BA'),
    ('080', 'Sumitomo Mitsui Trust Bank (THAI) PCL', 'SMTB');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230601050531_AddPopulateScript') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230601050531_AddPopulateScript', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230620081831_UpdateErrorMappingTable') THEN

    ALTER TABLE `error_mappings` MODIFY COLUMN `suggestion` varchar(255) COLLATE utf8mb4_0900_ai_ci NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230620081831_UpdateErrorMappingTable') THEN

    ALTER TABLE `error_mappings` MODIFY COLUMN `description` varchar(255) COLLATE utf8mb4_0900_ai_ci NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230620081831_UpdateErrorMappingTable') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230620081831_UpdateErrorMappingTable', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230620082222_AddPopulateErrorCode') THEN

    INSERT INTO `error_mappings` (`id`, `machine`, `state`, `description`, `suggestion`)
    VALUES ('70772bb6-b7c7-4f66-8573-2217ec88aa26', 'Deposit', 'DepositGenerateQRCodeFailed', 'Fail to Generate QR', 'Contact Technical Team'),
    ('2c6b0daa-221a-4766-9ef8-eab63f23280a', 'Deposit', 'DepositFailedNameMismatch', 'Name Mismatch', 'Investigate Name'),
    ('f789328c-7859-44d9-8755-ad9de22c3774', 'Deposit', 'DepositFailedInvalidSource', 'Invalid Source', 'Investigate Source of Fund'),
    ('94615811-5391-4655-9112-61e93f80a98b', 'Deposit', 'NotApprovedFrontOffice', 'Can not Approve to Front Office', 'Contact Technical Team'),
    ('68bedff6-bb54-46ca-b751-efa015c1d8d5', 'Deposit', 'LockTableBackOffice', 'Lock table in Back Office', 'Contact Technical Team'),
    ('9112a77a-0c73-4798-898e-5aa2badac09a', 'Deposit', 'ConnectionTimeOut', 'Connection timed out', 'Contact Technical Team'),
    ('f1199c64-f629-4134-80dd-d5b463bc9e19', 'Deposit', 'InternalServerError', 'Internal server error', 'Contact Technical Team'),
    ('b78ea701-410d-4530-a60b-623d301a31d6', 'Deposit', 'DepositFailed', 'Unexpected error occurred', 'Contact Technical Team'),
    ('1109d446-37ac-44f1-8783-cc2b25ddafe4', 'Deposit', 'DepositFailedAmountMismatch', 'Amount Mismatch', 'Investigate Amount'),
    ('912ae161-26f0-4c98-8965-f48429658d01', 'Deposit', 'DepositRefundFailed', 'Unable to Refund', 'Contact Technical Team');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230620082222_AddPopulateErrorCode') THEN

    INSERT INTO `error_handler_actions` (`id`, `error_mapping_id`, `action`)
    VALUES ('b06af7d6-39be-4668-b2ed-b3f12935dd6d', '2c6b0daa-221a-4766-9ef8-eab63f23280a', 'Approve'),
    ('2a5f197f-b036-46d6-8c7a-d08aed5b5164', '2c6b0daa-221a-4766-9ef8-eab63f23280a', 'Refund');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230620082222_AddPopulateErrorCode') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230620082222_AddPopulateErrorCode', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230725072826_UpdateStateName') THEN

    UPDATE `error_mappings` SET `state` = 'DepositFailedNameMismatch'
    WHERE `state` = 'DepositFailedNameMismatched';
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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230725072826_UpdateStateName') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230725072826_UpdateStateName', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230725074614_AddErrorType') THEN

    ALTER TABLE `error_mappings` ADD `error_type` varchar(100) COLLATE utf8mb4_0900_ai_ci NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230725074614_AddErrorType') THEN

    UPDATE `error_mappings` SET `error_type` = 'ThaiEquity'
    WHERE `state` = 'DepositFailedNameMismatch';
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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230725074614_AddErrorType') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230725074614_AddErrorType', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230725075056_PopulateErrorMappingGE') THEN

    INSERT INTO `error_mappings` (`id`, `machine`, `state`, `error_type`, `description`, `suggestion`)
    VALUES ('76ca8656-4dfb-4cd8-9545-8659390eaadf', 'Deposit', 'DepositFailedNameMismatch', 'GlobalEquity', 'Name Mismatch', 'Refund Required'),
    ('6c2dec69-d5f0-4417-8632-b5313e59a37d', 'Deposit', 'FXTransferFailed', 'GlobalEquity', 'FX Transfer Fail', 'Manual Allocation Required'),
    ('b6c9f8ff-020a-4dde-8fda-538ea923ee7d', 'Deposit', 'FXTransferInsufficientBalance', 'GlobalEquity', 'Insufficient Fund in Master Account', 'Top up balance is required'),
    ('6fcb096a-de64-44d7-8964-dae8767269e0', 'Deposit', 'FXFailed', 'GlobalEquity', 'Fail to convert FX', 'Full Refund'),
    ('9bed58a9-5ab1-4164-86a6-46d023d25f71', 'Deposit', 'FXRateCompareFailed', 'GlobalEquity', 'Unfavourable FX', 'Full Refund');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230725075056_PopulateErrorMappingGE') THEN

    INSERT INTO `error_handler_actions` (`id`, `error_mapping_id`, `action`)
    VALUES ('9db3ec80-2084-499f-995a-e52c2ab90ea5', '76ca8656-4dfb-4cd8-9545-8659390eaadf', 'Refund'),
    ('2a78e677-8350-42ba-87ae-8d970f7059e7', '6c2dec69-d5f0-4417-8632-b5313e59a37d', 'CcyAllocationTransfer'),
    ('420ccdc5-dd7d-4c0c-ada9-c8cc1714313d', '6fcb096a-de64-44d7-8964-dae8767269e0', 'Refund'),
    ('1629f0f3-6d9e-4234-a9d0-3dc8d446d5f4', '9bed58a9-5ab1-4164-86a6-46d023d25f71', 'Refund'),
    ('b14cbe01-49ce-4555-93fd-2bd2bcdedfbc', 'b6c9f8ff-020a-4dde-8fda-538ea923ee7d', 'CcyAllocationTransfer');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230725075056_PopulateErrorMappingGE') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230725075056_PopulateErrorMappingGE', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230804094853_AddWithdrawError') THEN

    INSERT INTO `error_mappings` (`id`, `machine`, `state`, `error_type`, `description`, `suggestion`)
    VALUES ('d923aa4d-3561-46bc-bec0-25a8e925905f', 'Withdraw', 'FXTransferFailed', 'GlobalEquity', 'FX Transfer Fail', NULL),
    ('030341d6-31b3-4a98-b496-7782803d4e15', 'Withdraw', 'KKPWithdrawalFailed', 'GlobalEquity', 'Withdraw Fail', NULL),
    ('ee6606e9-b22d-4464-b7eb-4a684900d798', 'Withdraw', 'RevertTransferFailed', 'GlobalEquity', 'Withdraw Request Fail', 'Manual Re-allocation Required');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230804094853_AddWithdrawError') THEN

    INSERT INTO `error_handler_actions` (`id`, `error_mapping_id`, `action`)
    VALUES ('84a61396-c8e7-4383-b0db-d297c1526c70', 'ee6606e9-b22d-4464-b7eb-4a684900d798', 'CcyAllocationTransfer');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230804094853_AddWithdrawError') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230804094853_AddWithdrawError', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230807100736_AddResponseCode') THEN

    ALTER TABLE `error_mappings` RENAME `response_codes`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230807100736_AddResponseCode') THEN

    ALTER TABLE `error_handler_actions` RENAME `response_code_actions`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230807100736_AddResponseCode') THEN

    ALTER TABLE `response_codes` RENAME COLUMN `error_type` TO `product_type`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230807100736_AddResponseCode') THEN

    ALTER TABLE `response_code_actions` RENAME COLUMN `error_mapping_id` TO `response_code_id`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230807100736_AddResponseCode') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230807100736_AddResponseCode', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230807102900_UpdateProductTypeToNull') THEN

    ALTER TABLE `response_codes` MODIFY COLUMN `product_type` longtext NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230807102900_UpdateProductTypeToNull') THEN

    UPDATE `response_codes` SET `product_type` = NULL
    WHERE `product_type` = 'Common';
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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230807102900_UpdateProductTypeToNull') THEN

    UPDATE `response_codes` SET `product_type` = NULL
    WHERE `product_type` = '';
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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230807102900_UpdateProductTypeToNull') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230807102900_UpdateProductTypeToNull', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230816033704_PopulateResponseCodes') THEN

    INSERT INTO `response_codes` (`id`, `machine`, `state`, `product_type`, `description`, `suggestion`)
    VALUES ('91cc38c2-9aba-4c91-962f-f557cd192255', 'Deposit', 'DepositCompleted', NULL, 'Deposit Completed', NULL),
    ('de72a4c3-7226-4788-8d67-bff2c60e6151', 'Deposit', 'DepositWaitingForPayment', NULL, 'Waiting for Fund', 'Inquire Payment Status'),
    ('1f29dad6-2166-4c45-8c19-6c2f33e41db9', 'Deposit', 'TransferRequestFailed', 'GlobalEquity', 'Unable To Process Global Transfer Payment', NULL),
    ('dd8ca1cd-22d6-4f6c-8d8d-098be62f1f39', 'Withdraw', 'TransferRequestFailed', 'GlobalEquity', 'Transfer Request Fail', 'Contact Technical Team'),
    ('0c5cb836-b1b5-4ce7-be30-69914e87cf33', 'Withdraw', 'WithdrawalFailed', 'GlobalEquity', 'Withdraw Fail', NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230816033704_PopulateResponseCodes') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230816033704_PopulateResponseCodes', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230824035405_PopulateResponseCodesForRefund') THEN

    INSERT INTO `response_codes` (`id`, `machine`, `state`, `product_type`, `description`, `suggestion`)
    VALUES ('e6cf7119-98fe-4764-bc01-ccb1096a1462', 'Deposit', 'DepositRefundSucceed', NULL, 'Refund Success', NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230824035405_PopulateResponseCodesForRefund') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230824035405_PopulateResponseCodesForRefund', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230828094744_PopulateResponseCodesForManualAllocation') THEN

    INSERT INTO `response_codes` (`id`, `machine`, `state`, `product_type`, `description`, `suggestion`)
    VALUES ('816538ea-faed-4651-8963-0097b1ec994d', 'Deposit', 'ManualAllocationFailed', 'GlobalEquity', 'CCY Allocation Transfer Failed', 'Contact Technical Team');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230828094744_PopulateResponseCodesForManualAllocation') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230828094744_PopulateResponseCodesForManualAllocation', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230904064800_PopulateResponseCodesForFinalState') THEN

    INSERT INTO `response_codes` (`id`, `machine`, `state`, `product_type`, `description`, `suggestion`)
    VALUES ('f3884d07-5025-4c8f-8ad0-8900e6dec7af', 'Deposit', 'Final', 'GlobalEquity', 'Deposit Completed', NULL),
    ('0ce1ee39-b53c-4c27-b111-31c24ab2d88a', 'Withdraw', 'Final', NULL, 'Withdraw Completed', NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230904064800_PopulateResponseCodesForFinalState') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230904064800_PopulateResponseCodesForFinalState', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230914081932_RemoveResponseCodes') THEN

    DELETE FROM `response_codes`
    WHERE `state` = 'Received' AND `product_type` IS NULL;
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'Failed' AND `product_type` IS NULL;
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositPaymentReceived' AND `product_type` IS NULL;
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositPaymentSourceValidating' AND `product_type` IS NULL;
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositPaymentNameValidating' AND `product_type` IS NULL;
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositQRCodeGenerating' AND `product_type` IS NULL;
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'TransactionNoGenerating' AND `product_type` IS NULL;
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositFailed' AND `product_type` IS NULL;
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositFailedAmountMismatch' AND `product_type` IS NULL;
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositFailed' AND `product_type` IS NULL;
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositRefundFailed' AND `product_type` IS NULL;
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositWaitingForPayment' AND `product_type` IS NULL;
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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230914081932_RemoveResponseCodes') THEN

    INSERT INTO `response_codes` (`id`, `machine`, `state`, `product_type`, `description`, `suggestion`)
    VALUES ('aecd798f-6085-4156-89a1-2e11bb36d3a7', 'Deposit', 'DepositFailed', NULL, 'Unexpected error occurred', 'Contact Technical Team'),
    ('c2abca1d-78ac-4704-a9e3-1d8ce1df5feb', 'Deposit', 'DepositFailedAmountMismatch', NULL, 'Amount Mismatch', 'Investigate Amount'),
    ('5386c145-ac2a-4f96-b46f-ebba27ed23ac', 'Deposit', 'DepositRefundFailed', NULL, 'Unable to Refund', 'Contact Technical Team');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230914081932_RemoveResponseCodes') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230914081932_RemoveResponseCodes', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230914083930_RemoveResponseCodesForProd') THEN

    DELETE FROM `response_codes`
    WHERE `state` = 'Received' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'Failed' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositWaitingForPayment' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositPaymentReceived' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositPaymentSourceValidating' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositPaymentNameValidating' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositQRCodeGenerating' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'TransactionNoGenerating' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositWaitingforPayment' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositCompleted' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositFailed' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositFailedAmountMismatch' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositFailed' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositRefundFailed' AND `product_type` = '';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositWaitingForPayment' AND `product_type` = '';
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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230914083930_RemoveResponseCodesForProd') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230914083930_RemoveResponseCodesForProd', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230914084646_AddResponseCodeForProd') THEN

    INSERT INTO `response_codes` (`id`, `machine`, `state`, `product_type`, `description`, `suggestion`)
    VALUES ('b6cb7883-3e90-4803-ac5a-91ad4b80eefb', 'Deposit', 'DepositWaitingForPayment', NULL, 'Waiting for Fund', 'Inquire Payment Status');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230914084646_AddResponseCodeForProd') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230914084646_AddResponseCodeForProd', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230921032833_AddResponseCodeForCashDeposit') THEN

    INSERT INTO `response_codes` (`id`, `machine`, `state`, `product_type`, `description`, `suggestion`)
    VALUES ('fd878e60-0776-4c84-a288-08f419a1cb61', 'Deposit', 'CashDepositTradingPlatformUpdating', 'ThaiEquity', 'Updating Back', NULL),
    ('9cc00ad4-5dfb-4873-8407-0e630c137044', 'Deposit', 'CashDepositWaitingForGateway', 'ThaiEquity', 'Waiting for Response from Back', NULL),
    ('9ad97a4c-d06f-46dd-b8fe-8e9d9eb4f0c9', 'Deposit', 'CashDepositWaitingForTradingPlatform', 'ThaiEquity', 'Updating Settrade TFEX ', NULL),
    ('fb8d318e-ffd4-44e4-b3a9-974c279ff75b', 'Deposit', 'CashDepositCompleted', 'ThaiEquity', 'Trading Account Deposit Completed', NULL),
    ('9c11fc8d-e3fa-4815-bd34-79348969eb18', 'Deposit', 'CashDepositFailed', 'ThaiEquity', 'Trading Account Deposit Fail', 'Contact Technical Support');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230921032833_AddResponseCodeForCashDeposit') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230921032833_AddResponseCodeForCashDeposit', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230927052009_UpdateKBankAbbr') THEN

    UPDATE `banks` SET `abbreviation` = 'KBANK'
    WHERE `abbreviation` = 'KBNK';
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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230927052009_UpdateKBankAbbr') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230927052009_UpdateKBankAbbr', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230929033318_RemoveOutdatedResponseCodes') THEN

    DELETE FROM `response_codes`
    WHERE `state` = 'NotApprovedFrontOffice';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'LockTableBackOffice';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'ConnectionTimeOut';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'DepositGenerateQrCodeFailed';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'InternalServerError';
    SELECT ROW_COUNT();

    DELETE FROM `response_codes`
    WHERE `state` = 'KKPWithdrawalFailed';
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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230929033318_RemoveOutdatedResponseCodes') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230929033318_RemoveOutdatedResponseCodes', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230929041409_AddRefundingResponseCode') THEN

    INSERT INTO `response_codes` (`id`, `machine`, `state`, `product_type`, `description`, `suggestion`)
    VALUES ('d1c60765-c4d0-4846-8d9e-928c5be7e648', 'Deposit', 'DepositRefunding', NULL, 'Deposit Refunding', NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230929041409_AddRefundingResponseCode') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230929041409_AddRefundingResponseCode', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230929041658_AddThaiWithdrawResponseCode') THEN

    INSERT INTO `response_codes` (`id`, `machine`, `state`, `product_type`, `description`, `suggestion`)
    VALUES ('0cb5682c-1a8c-4b05-a26c-cfaedbf0a91d', 'Withdraw', 'CashWithdrawWaitingForOtpValidation', 'ThaiEquity', 'OTP Required', NULL),
    ('f6963e61-323a-41c2-b83d-5344222491dc', 'Withdraw', 'TransferRequestFailed', 'ThaiEquity', 'Transfer Request Fail', 'Contact Technical Team'),
    ('1cf08033-a029-434f-907a-9a7d88787b7f', 'Withdraw', 'RevertTransferFailed', 'ThaiEquity', 'Revert Transaction Fail', 'Contact Technical Team and check both front and back'),
    ('001fb893-7f55-4721-b34d-175ca4cf6a82', 'Withdraw', 'RevertTransferSuccess', 'ThaiEquity', 'Revert Transaction Success', NULL),
    ('c6b59d2c-c75c-4f52-8114-97926a61bcf0', 'Withdraw', 'WithdrawalFailed', 'ThaiEquity', 'Withdraw Failed - Pending Revert', NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20230929041658_AddThaiWithdrawResponseCode') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20230929041658_AddThaiWithdrawResponseCode', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030414_AddBankChannelsTable') THEN

    CREATE TABLE `bank_channels` (
        `bank_code` varchar(255) COLLATE utf8mb4_0900_ai_ci NOT NULL,
        `channel` varchar(255) COLLATE utf8mb4_0900_ai_ci NOT NULL
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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030414_AddBankChannelsTable') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20231003030414_AddBankChannelsTable', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '002');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '004');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '006');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '011');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '014');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '017');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '018');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '020');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '022');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '024');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '025');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '029');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '030');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '031');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '032');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '033');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '034');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '039');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '045');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '052');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '066');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '067');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '069');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '070');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '071');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '073');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('OnlineTransfer', '080');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('AtsBatch', '002');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('AtsBatch', '004');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('AtsBatch', '006');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('AtsBatch', '011');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('AtsBatch', '014');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('AtsBatch', '025');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('AtsBatch', '073');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('Odd', '002');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('Odd', '004');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('Odd', '014');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('SetTrade', '002');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('SetTrade', '004');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('SetTrade', '006');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('SetTrade', '014');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '002');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '004');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '006');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '011');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '014');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '017');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '018');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '020');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '022');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '024');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '025');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '029');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '030');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '031');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '032');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '033');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '034');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '039');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '045');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '052');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '066');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '067');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '069');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '070');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '071');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '073');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('QR', '080');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231003030446_PopulateBankChannels') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20231003030446_PopulateBankChannels', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231010090218_AddMissingBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('AtsBatch', '022');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231010090218_AddMissingBankChannels') THEN

    INSERT INTO `bank_channels` (`channel`, `bank_code`)
    VALUES ('AtsBatch', '024');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231010090218_AddMissingBankChannels') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20231010090218_AddMissingBankChannels', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231025094031_AddIsFilterableToResponseCode') THEN

    ALTER TABLE `response_codes` ADD `is_filterable` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__BackofficeDbContext` WHERE `migration_id` = '20231025094031_AddIsFilterableToResponseCode') THEN

    INSERT INTO `__BackofficeDbContext` (`migration_id`, `product_version`)
    VALUES ('20231025094031_AddIsFilterableToResponseCode', '7.0.5');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

