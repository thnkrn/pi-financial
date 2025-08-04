CREATE TABLE IF NOT EXISTS `__DataSeedingDbContext` (
    `migration_id` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `product_version` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `pk___data_seeding_db_context` PRIMARY KEY (`migration_id`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231006055913_AddResponseCodes') THEN

    truncate table response_codes;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231006055913_AddResponseCodes') THEN

    truncate table response_code_actions;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231006055913_AddResponseCodes') THEN

    INSERT INTO `response_codes` (`id`, `description`, `machine`, `product_type`, `state`, `suggestion`)
    VALUES ('0ac1e94d-990d-4d3c-9fac-8ae42e39351e', 'Waiting for Fund', 'Deposit', NULL, 'DepositWaitingForPayment', 'Inquire Payment Status'),
    ('0e1158b2-569d-4916-a68c-508c6813cb79', 'FX Transfer Fail', 'Deposit', 'GlobalEquity', 'FXTransferFailed', 'Manual Allocation Required'),
    ('1395482b-939f-46f5-a039-4bd3bdf3edd8', 'CCY Allocation Transfer Fail', 'Deposit', 'GlobalEquity', 'ManualAllocationFailed', 'Contact Technical Team'),
    ('15a9160b-a5eb-4754-98b2-3cfea4c4e0d2', 'Insufficient Fund @ Master Account', 'Deposit', 'GlobalEquity', 'FXTransferInsufficientBalance', 'Top up balance is required'),
    ('2203f732-3fbe-4738-95d2-1c0f70603914', 'Refund Success', 'Deposit', NULL, 'RefundSucceed', NULL),
    ('220ed567-701a-4903-a8fe-ad5d3cfc43c1', 'Refund Fail', 'Deposit', NULL, 'RefundFailed', 'Contact Technical Support and Manual Refund Required'),
    ('222d19bd-92b9-4c40-bcea-3b404a14146a', 'Amount Mismatch', 'Deposit', NULL, 'DepositFailedAmountMismatch', 'Refund Required'),
    ('2258bbbc-2dbf-4519-9d40-3bfa7e4b6609', 'Revert Transfer Fail', 'Withdraw', 'ThaiEquity', 'RevertTransferFailed', 'Contact Technical Team and check Customer Trading Account Balance'),
    ('23f0b465-57dc-4b07-be8d-9db340bb5cc0', 'OTP Required', 'Withdraw', 'ThaiEquity', 'CashWithdrawWaitingForOtpValidation', 'Waiting for Customer OTP'),
    ('3070a7c3-5ef4-4898-b0c2-92efd83f8e9d', 'Waiting for SBA Callback', 'Deposit', 'ThaiEquity', 'CashDepositWaitingForGateway', NULL),
    ('5afa5a4e-d054-4377-a9f4-e808c1c3706f', 'Revert Transfer Success', 'Withdraw', 'ThaiEquity', 'RevertTransferSuccess', NULL),
    ('60245f07-190e-4c94-b2db-bda11e4f8fa1', 'Updating SBA', 'Deposit', 'ThaiEquity', 'CashDepositTradingPlatformUpdating', NULL),
    ('6862d9de-1e1c-4055-b45e-8fc6845dbc94', 'Revert Transfer Success', 'Withdraw', 'GlobalEquity', 'RevertTransferSuccess', NULL),
    ('6a5113c7-5381-40a8-b49f-b1751c44d22b', 'Name Mismatch (Thai Equity)', 'Deposit', 'ThaiEquity', 'DepositFailedNameMismatch', 'Investigate Name'),
    ('6e865244-d493-4635-b0f6-7b9b6717d20b', 'Invalid Source', 'Deposit', NULL, 'DepositFailedInvalidSource', 'Investigate Source of Fund'),
    ('723f4edf-fb08-42da-9bd9-60cf7eaead9c', 'Trading Account Deposit Fail', 'Deposit', 'ThaiEquity', 'CashDepositFailed', 'Contact Technical Support and manual update Fund to SBA'),
    ('76c845bf-fb3a-490f-928c-54811f0a8739', 'FX Failed', 'Deposit', 'GlobalEquity', 'FXFailed', 'Refund Required'),
    ('77ff1567-da72-4305-a2cc-428ed3f88913', 'OTP Required', 'Withdraw', NULL, 'RequestingOtpValidationFailed', NULL),
    ('90c3ae3e-c42b-40cb-8573-0a35096d9272', 'Updating Settrade', 'Deposit', 'ThaiEquity', 'CashDepositWaitingForTradingPlatform', NULL),
    ('975cd24f-b648-402f-a17f-65fd053c9e72', 'Revert Transfer Fail', 'Withdraw', 'GlobalEquity', 'RevertTransferFailed', 'Manual Re-allocation Required'),
    ('a151f28d-7439-411c-9928-6ca26d7ec82f', 'Fail to Deposit Fund', 'Deposit', NULL, 'TransferRequestFailed', 'Contact Technical Team'),
    ('a8b817d4-a320-4970-973b-5b403b9c8e1a', 'Deposit Completed', 'Deposit', 'GlobalEquity', 'Final', NULL),
    ('c1a73f39-b127-427b-806c-206952194ff4', 'Withdraw Failed - Pending Revert', 'Withdraw', 'ThaiEquity', 'WithdrawalFailed', NULL),
    ('c9e76eeb-f77f-4f8c-ad06-1cd285eca1bd', 'Transfer Request Fail', 'Withdraw', 'GlobalEquity', 'TransferRequestFailed', 'Contact Technical Team'),
    ('f28492e9-1ee4-4ea7-bfb2-a965eb8cb107', 'Transfer Request Fail', 'Withdraw', 'ThaiEquity', 'TransferRequestFailed', 'Contact Technical Team'),
    ('f2cc09c8-a739-4797-b00a-492e503f7c8d', 'FX Rate Compare Fail', 'Deposit', 'GlobalEquity', 'FXRateCompareFailed', 'Refund Required'),
    ('f3092460-34af-4c24-9b87-d24df13a2872', 'Fail to Deposit Fund', 'Deposit', NULL, 'DepositFailed', 'Contact Technical Team'),
    ('f3c9ed99-0978-4797-ae3d-3d0ef7854caa', 'Name Mismatch (Global Equity)', 'Deposit', 'GlobalEquity', 'DepositFailedNameMismatch', 'Refund Required'),
    ('f4432ba1-6019-44e2-ae9c-c3d3ac64a2ab', 'CCY Allocation Transfer Fail', 'Withdraw', 'GlobalEquity', 'ManualAllocationFailed', 'Contact Technical Team'),
    ('f8ef36bb-1b95-4405-8263-c8e31c86f5f2', 'Withdraw Completed', 'Withdraw', NULL, 'Final', NULL),
    ('fec7cb30-7a63-4248-a7c0-c6d2f9c0cf1a', 'Trading Account Deposit Completed', 'Deposit', 'ThaiEquity', 'CashDepositCompleted', NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231006055913_AddResponseCodes') THEN

    INSERT INTO `response_code_actions` (`id`, `action`, `response_code_id`)
    VALUES ('46d926c8-acb4-46bf-af49-d56bef3da2eb', 'CcyAllocationTransfer', '0e1158b2-569d-4916-a68c-508c6813cb79'),
    ('5939094e-9c8d-4442-9b1e-59bbc4d35c6b', 'CcyAllocationTransfer', '975cd24f-b648-402f-a17f-65fd053c9e72'),
    ('600ee264-c301-4a09-87fb-8e0296ef29be', 'CcyAllocationTransfer', 'f4432ba1-6019-44e2-ae9c-c3d3ac64a2ab'),
    ('6fd67285-f01a-4b7d-bf6e-aacf753c4bca', 'CcyAllocationTransfer', '15a9160b-a5eb-4754-98b2-3cfea4c4e0d2'),
    ('74ec6520-a058-4501-b09f-feb9322894c7', 'Approve', '6a5113c7-5381-40a8-b49f-b1751c44d22b'),
    ('7f24e643-3d74-4104-868d-1caeeffb7574', 'Refund', '6a5113c7-5381-40a8-b49f-b1751c44d22b'),
    ('a2a50061-4a3b-4066-a08c-640fa5453bc3', 'Refund', '222d19bd-92b9-4c40-bcea-3b404a14146a'),
    ('c4e2df1b-1525-422b-9810-2485592708a5', 'Refund', 'f3c9ed99-0978-4797-ae3d-3d0ef7854caa'),
    ('c921d71f-cf50-4b91-b015-de1d7167747f', 'CcyAllocationTransfer', '1395482b-939f-46f5-a039-4bd3bdf3edd8'),
    ('ccd35bfe-13cc-4871-941c-4e1d3569ba31', 'Refund', 'f2cc09c8-a739-4797-b00a-492e503f7c8d'),
    ('d23fdea9-7bb6-4a30-afea-29940ce32615', 'Refund', '76c845bf-fb3a-490f-928c-54811f0a8739');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231006055913_AddResponseCodes') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20231006055913_AddResponseCodes', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231011061531_UpdateResponseCodes') THEN

    DELETE FROM `response_codes`
    WHERE `id` = '2258bbbc-2dbf-4519-9d40-3bfa7e4b6609';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231011061531_UpdateResponseCodes') THEN

    DELETE FROM `response_codes`
    WHERE `id` = '5afa5a4e-d054-4377-a9f4-e808c1c3706f';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231011061531_UpdateResponseCodes') THEN

    UPDATE `response_codes` SET `product_type` = 'GlobalEquity'
    WHERE `id` = '2203f732-3fbe-4738-95d2-1c0f70603914';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231011061531_UpdateResponseCodes') THEN

    UPDATE `response_codes` SET `product_type` = 'GlobalEquity'
    WHERE `id` = '220ed567-701a-4903-a8fe-ad5d3cfc43c1';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231011061531_UpdateResponseCodes') THEN

    UPDATE `response_codes` SET `suggestion` = 'Contact Technical Team and check Customer Trading Account Balance'
    WHERE `id` = 'f28492e9-1ee4-4ea7-bfb2-a965eb8cb107';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231011061531_UpdateResponseCodes') THEN

    INSERT INTO `response_codes` (`id`, `description`, `machine`, `product_type`, `state`, `suggestion`)
    VALUES ('06b0657c-9338-4db1-baf0-95351bec3de2', 'Refunding', 'Deposit', 'GlobalEquity', 'Refunding', NULL),
    ('b6bedce3-9801-419a-8b3d-e1b726ba9607', 'Refund Success', 'Deposit', 'ThaiEquity', 'DepositRefundSucceed', NULL),
    ('ca7f7e31-69f1-4d33-a1b6-9f9645f26d00', 'Refund Fail', 'Deposit', 'ThaiEquity', 'DepositRefundFailed', 'Contact Technical Support and Manual Refund Required'),
    ('d53c7aca-bed8-409b-a163-40f33180960d', 'Refunding', 'Deposit', 'ThaiEquity', 'DepositRefunding', NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231011061531_UpdateResponseCodes') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20231011061531_UpdateResponseCodes', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012062041_UpdateDuplicateMigration') THEN

    UPDATE `response_codes` SET `suggestion` = 'Manual Re-allocation Required'
    WHERE `id` = '1395482b-939f-46f5-a039-4bd3bdf3edd8';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012062041_UpdateDuplicateMigration') THEN

    UPDATE `response_codes` SET `description` = 'Transfer Request Failed', `product_type` = 'GlobalEquity'
    WHERE `id` = 'a151f28d-7439-411c-9928-6ca26d7ec82f';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012062041_UpdateDuplicateMigration') THEN

    UPDATE `response_codes` SET `description` = 'Fail to Deposit'
    WHERE `id` = 'f3092460-34af-4c24-9b87-d24df13a2872';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012062041_UpdateDuplicateMigration') THEN

    UPDATE `response_codes` SET `suggestion` = 'Manual Re-allocation Required'
    WHERE `id` = 'f4432ba1-6019-44e2-ae9c-c3d3ac64a2ab';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012062041_UpdateDuplicateMigration') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20231012062041_UpdateDuplicateMigration', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012080217_UpdateResponseCodesV37') THEN

    UPDATE `response_codes` SET `description` = 'Manual allocation in XNT', `suggestion` = 'Manual Allocation Required due to failed allocation'
    WHERE `id` = '0e1158b2-569d-4916-a68c-508c6813cb79';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012080217_UpdateResponseCodesV37') THEN

    UPDATE `response_codes` SET `description` = 'Insufficient Balance', `suggestion` = 'Alert Finance Team on fund top up'
    WHERE `id` = '15a9160b-a5eb-4754-98b2-3cfea4c4e0d2';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012080217_UpdateResponseCodesV37') THEN

    UPDATE `response_codes` SET `description` = 'Incorrect Source'
    WHERE `id` = '6e865244-d493-4635-b0f6-7b9b6717d20b';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012080217_UpdateResponseCodesV37') THEN

    UPDATE `response_codes` SET `description` = 'Unable to FX'
    WHERE `id` = '76c845bf-fb3a-490f-928c-54811f0a8739';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012080217_UpdateResponseCodesV37') THEN

    UPDATE `response_codes` SET `description` = 'Unfavourable FX (rate over)'
    WHERE `id` = 'f2cc09c8-a739-4797-b00a-492e503f7c8d';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012080217_UpdateResponseCodesV37') THEN

    UPDATE `response_codes` SET `description` = 'Fail to Deposit Fund'
    WHERE `id` = 'f3092460-34af-4c24-9b87-d24df13a2872';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012080217_UpdateResponseCodesV37') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20231012080217_UpdateResponseCodesV37', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012081339_UpdateResponseCodesV37.2') THEN

    UPDATE `response_codes` SET `description` = 'Manual allocation in XNT'
    WHERE `id` = '975cd24f-b648-402f-a17f-65fd053c9e72';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231012081339_UpdateResponseCodesV37.2') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20231012081339_UpdateResponseCodesV37.2', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231016090609_ChangeDepositRefundToCommon') THEN

    UPDATE `response_codes` SET `product_type` = NULL
    WHERE `id` = 'b6bedce3-9801-419a-8b3d-e1b726ba9607';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231016090609_ChangeDepositRefundToCommon') THEN

    UPDATE `response_codes` SET `product_type` = NULL
    WHERE `id` = 'ca7f7e31-69f1-4d33-a1b6-9f9645f26d00';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231016090609_ChangeDepositRefundToCommon') THEN

    UPDATE `response_codes` SET `product_type` = NULL
    WHERE `id` = 'd53c7aca-bed8-409b-a163-40f33180960d';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231016090609_ChangeDepositRefundToCommon') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20231016090609_ChangeDepositRefundToCommon', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231018050706_UpdateWithdrawGEStateOTP') THEN

    UPDATE `response_codes` SET `state` = 'AwaitingOtpValidation'
    WHERE `id` = '77ff1567-da72-4305-a2cc-428ed3f88913';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231018050706_UpdateWithdrawGEStateOTP') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20231018050706_UpdateWithdrawGEStateOTP', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231018071414_FixProductTypeForOTPGE') THEN

    UPDATE `response_codes` SET `product_type` = 'GlobalEquity'
    WHERE `id` = '77ff1567-da72-4305-a2cc-428ed3f88913';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231018071414_FixProductTypeForOTPGE') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20231018071414_FixProductTypeForOTPGE', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231024035838_UpdateNameMismatchDescription') THEN

    UPDATE `response_codes` SET `description` = 'Name Mismatch'
    WHERE `id` = '6a5113c7-5381-40a8-b49f-b1751c44d22b';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231024035838_UpdateNameMismatchDescription') THEN

    UPDATE `response_codes` SET `description` = 'Name Mismatch'
    WHERE `id` = 'f3c9ed99-0978-4797-ae3d-3d0ef7854caa';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231024035838_UpdateNameMismatchDescription') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20231024035838_UpdateNameMismatchDescription', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025073446_UpdateResponseCodesAndRemoveUnused') THEN

    DELETE FROM `response_codes`
    WHERE `id` = 'c1a73f39-b127-427b-806c-206952194ff4';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025073446_UpdateResponseCodesAndRemoveUnused') THEN

    UPDATE `response_codes` SET `description` = 'Manual Allocation in XNT Failed'
    WHERE `id` = '1395482b-939f-46f5-a039-4bd3bdf3edd8';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025073446_UpdateResponseCodesAndRemoveUnused') THEN

    UPDATE `response_codes` SET `description` = 'Unfavorable FX (rate over)'
    WHERE `id` = 'f2cc09c8-a739-4797-b00a-492e503f7c8d';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025073446_UpdateResponseCodesAndRemoveUnused') THEN

    UPDATE `response_codes` SET `description` = 'Manual Allocation in XNT Failed'
    WHERE `id` = 'f4432ba1-6019-44e2-ae9c-c3d3ac64a2ab';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025073446_UpdateResponseCodesAndRemoveUnused') THEN

    UPDATE `response_codes` SET `description` = 'Deposit Completed'
    WHERE `id` = 'fec7cb30-7a63-4248-a7c0-c6d2f9c0cf1a';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025073446_UpdateResponseCodesAndRemoveUnused') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20231025073446_UpdateResponseCodesAndRemoveUnused', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = FALSE
    WHERE `id` = '06b0657c-9338-4db1-baf0-95351bec3de2';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = FALSE
    WHERE `id` = '0ac1e94d-990d-4d3c-9fac-8ae42e39351e';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = '0e1158b2-569d-4916-a68c-508c6813cb79';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = '1395482b-939f-46f5-a039-4bd3bdf3edd8';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = '15a9160b-a5eb-4754-98b2-3cfea4c4e0d2';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = '2203f732-3fbe-4738-95d2-1c0f70603914';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = '220ed567-701a-4903-a8fe-ad5d3cfc43c1';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = '222d19bd-92b9-4c40-bcea-3b404a14146a';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = FALSE
    WHERE `id` = '23f0b465-57dc-4b07-be8d-9db340bb5cc0';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = FALSE
    WHERE `id` = '3070a7c3-5ef4-4898-b0c2-92efd83f8e9d';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = FALSE
    WHERE `id` = '60245f07-190e-4c94-b2db-bda11e4f8fa1';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = '6862d9de-1e1c-4055-b45e-8fc6845dbc94';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = '6a5113c7-5381-40a8-b49f-b1751c44d22b';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = '6e865244-d493-4635-b0f6-7b9b6717d20b';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = '723f4edf-fb08-42da-9bd9-60cf7eaead9c';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = '76c845bf-fb3a-490f-928c-54811f0a8739';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = FALSE
    WHERE `id` = '77ff1567-da72-4305-a2cc-428ed3f88913';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = FALSE
    WHERE `id` = '90c3ae3e-c42b-40cb-8573-0a35096d9272';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = '975cd24f-b648-402f-a17f-65fd053c9e72';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = 'a151f28d-7439-411c-9928-6ca26d7ec82f';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = 'a8b817d4-a320-4970-973b-5b403b9c8e1a';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = 'b6bedce3-9801-419a-8b3d-e1b726ba9607';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = 'c9e76eeb-f77f-4f8c-ad06-1cd285eca1bd';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = 'ca7f7e31-69f1-4d33-a1b6-9f9645f26d00';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = FALSE
    WHERE `id` = 'd53c7aca-bed8-409b-a163-40f33180960d';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = 'f28492e9-1ee4-4ea7-bfb2-a965eb8cb107';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = 'f2cc09c8-a739-4797-b00a-492e503f7c8d';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = 'f3092460-34af-4c24-9b87-d24df13a2872';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = 'f3c9ed99-0978-4797-ae3d-3d0ef7854caa';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = 'f4432ba1-6019-44e2-ae9c-c3d3ac64a2ab';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = 'f8ef36bb-1b95-4405-8263-c8e31c86f5f2';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    UPDATE `response_codes` SET `is_filterable` = TRUE
    WHERE `id` = 'fec7cb30-7a63-4248-a7c0-c6d2f9c0cf1a';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231025094212_PopulateFilterableResponseCodes') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20231025094212_PopulateFilterableResponseCodes', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231103023145_POR446UpdateCashDepositFailedSuggestion') THEN

    UPDATE `response_codes` SET `suggestion` = 'Contact Technical Team and check Customer Trading Account Balance'
    WHERE `id` = '723f4edf-fb08-42da-9bd9-60cf7eaead9c';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231103023145_POR446UpdateCashDepositFailedSuggestion') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20231103023145_POR446UpdateCashDepositFailedSuggestion', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231218032110_AUDI261UpdateResponseCodeAndActionForSBAManualTransfer') THEN

    INSERT INTO `response_code_actions` (`id`, `action`, `response_code_id`)
    VALUES ('9462554b-f394-4572-b29f-0e5830b03889', 'SbaAllocationTransfer', '723f4edf-fb08-42da-9bd9-60cf7eaead9c');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231218032110_AUDI261UpdateResponseCodeAndActionForSBAManualTransfer') THEN

    UPDATE `response_codes` SET `suggestion` = 'Check Customer Trading Account Balance, before Manual Allocate'
    WHERE `id` = '723f4edf-fb08-42da-9bd9-60cf7eaead9c';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20231218032110_AUDI261UpdateResponseCodeAndActionForSBAManualTransfer') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20231218032110_AUDI261UpdateResponseCodeAndActionForSBAManualTransfer', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240320075644_AddNewActionForWalletV2') THEN

    UPDATE `response_codes` SET `product_type` = NULL
    WHERE `id` = '723f4edf-fb08-42da-9bd9-60cf7eaead9c';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240320075644_AddNewActionForWalletV2') THEN

    INSERT INTO `response_codes` (`id`, `description`, `is_filterable`, `machine`, `product_type`, `state`, `suggestion`)
    VALUES ('f534e848-9441-40bb-8878-1b9b5b3a801f', 'Trading Account Deposit Fail', TRUE, 'Deposit', NULL, 'UpBackFailedAwaitingActionSba', 'Check Customer Trading Account Balance, before Manual Allocate'),
    ('f9e00911-c580-48f2-9302-d7e10388507f', 'SetTrade Trading Account Deposit Fail', TRUE, 'Deposit', NULL, 'UpBackFailedAwaitingActionSetTrade', 'Check MT4, Check Customer SetTrade Account Balance, before Manual Allocate');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240320075644_AddNewActionForWalletV2') THEN

    INSERT INTO `response_code_actions` (`id`, `action`, `response_code_id`)
    VALUES ('b0c5da2a-0526-41b1-a5ee-bc6ab2b5090a', 'SetTradeAllocationTransfer', 'f9e00911-c580-48f2-9302-d7e10388507f'),
    ('bf0c2940-1080-453d-a7ca-6718a880cda2', 'SbaAllocationTransfer', 'f534e848-9441-40bb-8878-1b9b5b3a801f');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240320075644_AddNewActionForWalletV2') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20240320075644_AddNewActionForWalletV2', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240326062040_AddConfirmCallbackAction') THEN

    INSERT INTO `response_codes` (`id`, `description`, `is_filterable`, `machine`, `product_type`, `state`, `suggestion`)
    VALUES ('b05dbb35-7e00-4ada-9811-4ece7d7e1625', 'Waiting for receiving kkp response', TRUE, 'Deposit', NULL, 'WaitingForPayment', 'Check kkp report, Check Customer Trading Account balance,  before kkp callback confirm'),
    ('caa1b189-aa54-4c5d-933f-8eb8bb066aa2', 'Waiting to receiving freewill ats response', TRUE, 'Deposit', NULL, 'WaitingForAtsGatewayConfirmation', 'Check freewill report, Check Customer Trading Account balance , before deposit sba ats'),
    ('d9fef26d-a2ef-470e-82be-9f64f2c32d90', 'Waiting for receiving freewill response', TRUE, 'Deposit', NULL, 'DepositWaitingForGateway', 'Check freewill, Check Customer Trading Account balance , before deposit sba');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240326062040_AddConfirmCallbackAction') THEN

    INSERT INTO `response_code_actions` (`id`, `action`, `response_code_id`)
    VALUES ('586c694a-dfb1-41b2-af6b-b2ff1fe63fbd', 'SbaDepositAtsCallbackConfirm', 'caa1b189-aa54-4c5d-933f-8eb8bb066aa2'),
    ('b89a1eba-eedd-4509-b427-2c26456f2755', 'SbaDepositConfirm', 'd9fef26d-a2ef-470e-82be-9f64f2c32d90'),
    ('eb19e2ce-4565-4ea5-9c1b-177f14f582cb', 'DepositKkpConfirm', 'b05dbb35-7e00-4ada-9811-4ece7d7e1625');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240326062040_AddConfirmCallbackAction') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20240326062040_AddConfirmCallbackAction', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240327095435_ModifiedStateNameForWalletV2') THEN

    UPDATE `response_codes` SET `state` = 'UpBackFailedRequireActionSba'
    WHERE `id` = 'f534e848-9441-40bb-8878-1b9b5b3a801f';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240327095435_ModifiedStateNameForWalletV2') THEN

    UPDATE `response_codes` SET `state` = 'UpBackFailedRequireActionSetTrade'
    WHERE `id` = 'f9e00911-c580-48f2-9302-d7e10388507f';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240327095435_ModifiedStateNameForWalletV2') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20240327095435_ModifiedStateNameForWalletV2', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240415044811_UpdateCallToActionDesc') THEN

    UPDATE `response_codes` SET `description` = 'Waiting for ats response', `suggestion` = 'Check freewill report, before approve front'
    WHERE `id` = 'caa1b189-aa54-4c5d-933f-8eb8bb066aa2';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240415044811_UpdateCallToActionDesc') THEN

    INSERT INTO `response_codes` (`id`, `description`, `is_filterable`, `machine`, `product_type`, `state`, `suggestion`)
    VALUES ('4bc8fc98-a84c-4bc0-97cc-99cca7c24e5b', 'Pending Revert Transaction', TRUE, 'Withdraw', NULL, 'WithdrawFailedRequireActionRecovery', 'Check kkp report, contact IT to revert transaction'),
    ('532d2838-9610-4704-a267-4e609032adf9', 'Pending Revert Transaction', TRUE, 'Withdraw', NULL, 'UpBackFailedRequireActionRevert', 'Check freewill or settrade, Contact IT to revert, Before mark as fail');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240415044811_UpdateCallToActionDesc') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20240415044811_UpdateCallToActionDesc', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240423093903_AddChangeStatusAction') THEN

    INSERT INTO `response_code_actions` (`id`, `action`, `response_code_id`)
    VALUES ('04bffc06-1405-4809-b32f-09115d66c08d', 'ChangeSetTradeStatusToSuccess', '3070a7c3-5ef4-4898-b0c2-92efd83f8e9d'),
    ('074d958f-1639-417a-a21b-36130c84e39a', 'ChangeStatusToFail', '222d19bd-92b9-4c40-bcea-3b404a14146a'),
    ('0d201e4b-94f8-42aa-b708-a618cbbb14d8', 'ChangeStatusToFail', 'f534e848-9441-40bb-8878-1b9b5b3a801f'),
    ('0f2ef9b5-6c84-4ceb-b794-b74e12bba61e', 'ChangeStatusToSuccess', 'f534e848-9441-40bb-8878-1b9b5b3a801f'),
    ('1e5f4d1c-f65b-4033-ad7d-0a5c3196d3e5', 'ChangeSetTradeStatusToSuccess', '723f4edf-fb08-42da-9bd9-60cf7eaead9c'),
    ('2a3eff05-64bd-4a4b-a58e-aef6ffbcd80a', 'ChangeStatusToFail', 'd9fef26d-a2ef-470e-82be-9f64f2c32d90'),
    ('2e9176ba-9dfe-48ba-836e-92059f1c9488', 'ChangeStatusToSuccess', '975cd24f-b648-402f-a17f-65fd053c9e72'),
    ('3921769a-c1c7-48a3-9920-f06e9770775b', 'ChangeStatusToSuccess', '76c845bf-fb3a-490f-928c-54811f0a8739'),
    ('4272eef0-736c-4f3a-bb81-5f7e2391f789', 'ChangeStatusToFail', '532d2838-9610-4704-a267-4e609032adf9'),
    ('45828eb2-052a-4c36-9c95-c4268077817e', 'ChangeSetTradeStatusToFail', '723f4edf-fb08-42da-9bd9-60cf7eaead9c'),
    ('518c4ce1-6cda-4b07-911a-bf3d55666e4d', 'ChangeStatusToSuccess', '3070a7c3-5ef4-4898-b0c2-92efd83f8e9d'),
    ('524fe165-d582-4f6e-89f6-f815a02309e5', 'ChangeStatusToSuccess', '6e865244-d493-4635-b0f6-7b9b6717d20b'),
    ('5f747a23-eb7e-4dba-8442-38b9b4cb6fa1', 'ChangeStatusToSuccess', '532d2838-9610-4704-a267-4e609032adf9'),
    ('744ba0f4-040b-4bb0-ab4e-54cd21f2a4e5', 'ChangeStatusToSuccess', 'f9e00911-c580-48f2-9302-d7e10388507f'),
    ('8303a6a4-657e-4937-a8f5-8c2f6c58958d', 'ChangeStatusToSuccess', 'd9fef26d-a2ef-470e-82be-9f64f2c32d90'),
    ('907361e9-fc23-4d25-b32d-9bf12f75a9ec', 'ChangeStatusToFail', '975cd24f-b648-402f-a17f-65fd053c9e72'),
    ('9807126c-e2b4-4b83-a530-579a5a291a36', 'ChangeStatusToFail', 'caa1b189-aa54-4c5d-933f-8eb8bb066aa2'),
    ('a55878c9-9a74-42bb-85c4-fb27298730bf', 'ChangeStatusToFail', 'f9e00911-c580-48f2-9302-d7e10388507f'),
    ('a679f695-898d-4ba3-94ee-c963a6b38ee7', 'ChangeStatusToSuccess', '0e1158b2-569d-4916-a68c-508c6813cb79'),
    ('a6f84686-6ee2-432f-955f-fa8c570b210e', 'ChangeStatusToFail', '4bc8fc98-a84c-4bc0-97cc-99cca7c24e5b'),
    ('ac2b76d7-37c4-45af-a796-d8ecce065baf', 'ChangeStatusToSuccess', '723f4edf-fb08-42da-9bd9-60cf7eaead9c'),
    ('b5ef4ff0-97b8-4d08-8b3f-e215d8fb8f59', 'ChangeStatusToSuccess', 'f2cc09c8-a739-4797-b00a-492e503f7c8d'),
    ('b7b356f2-85c9-4fbe-ac86-7759610e3bc6', 'ChangeStatusToFail', '0e1158b2-569d-4916-a68c-508c6813cb79'),
    ('c34a1032-49f5-497e-aab7-891529ca7363', 'ChangeStatusToFail', '3070a7c3-5ef4-4898-b0c2-92efd83f8e9d'),
    ('ccc455ce-0ccb-46fe-a729-371c6aa20b28', 'ChangeStatusToFail', 'f2cc09c8-a739-4797-b00a-492e503f7c8d'),
    ('cddf81a7-c5f7-434d-870a-198d653b83dc', 'ChangeStatusToSuccess', '4bc8fc98-a84c-4bc0-97cc-99cca7c24e5b'),
    ('d3b6668f-63e4-40d9-b282-ead905076903', 'ChangeSetTradeStatusToFail', '3070a7c3-5ef4-4898-b0c2-92efd83f8e9d'),
    ('d69c72b1-7eda-4fc6-9bd8-5fcc308444d0', 'ChangeStatusToFail', '723f4edf-fb08-42da-9bd9-60cf7eaead9c'),
    ('d7cb0c26-e1f8-43e9-ba4f-6449789535d6', 'ChangeStatusToFail', '6a5113c7-5381-40a8-b49f-b1751c44d22b'),
    ('da0a09cb-6003-4879-83e6-24876d3ca863', 'ChangeStatusToFail', '76c845bf-fb3a-490f-928c-54811f0a8739'),
    ('eb96e5ce-5dfd-4f7b-ae00-66ecefa3c4bb', 'ChangeStatusToSuccess', '222d19bd-92b9-4c40-bcea-3b404a14146a'),
    ('f797b251-e55b-4077-99cf-4f4268788754', 'ChangeStatusToSuccess', 'caa1b189-aa54-4c5d-933f-8eb8bb066aa2'),
    ('f938909d-7b69-473b-93a1-4b2b171a2fd5', 'ChangeStatusToSuccess', '6a5113c7-5381-40a8-b49f-b1751c44d22b'),
    ('fe29778c-4c72-46c1-aaa8-89efb10b6059', 'ChangeStatusToFail', '6e865244-d493-4635-b0f6-7b9b6717d20b');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240423093903_AddChangeStatusAction') THEN

    INSERT INTO `response_codes` (`id`, `description`, `is_filterable`, `machine`, `product_type`, `state`, `suggestion`)
    VALUES ('3dc91a3c-402b-45b5-80a5-b96c75e24391', 'Waiting for ats response', TRUE, 'Withdraw', NULL, 'WaitingForAtsGatewayConfirmation', 'Change transaction status'),
    ('69aeef24-d4fe-45b5-b0e8-cce920f67936', 'SetTrade Trading Account Deposit Fail', TRUE, 'Deposit', NULL, 'UpBackFailedRequireActionSetTrade', 'Change transaction status'),
    ('d8fa6460-36cf-458f-9f12-f43cdd9c6b28', 'Waiting for receiving freewill response', TRUE, 'Withdraw', NULL, 'WithdrawWaitingForGateway', 'Change transaction status'),
    ('fe785ac2-09af-4cb0-a679-030310809bee', 'SetTrade Trading Account Deposit Fail', TRUE, 'Deposit', NULL, 'TfexCashDepositFailed', 'Change transaction status');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240423093903_AddChangeStatusAction') THEN

    INSERT INTO `response_code_actions` (`id`, `action`, `response_code_id`)
    VALUES ('103152af-c45c-462e-8eb8-e3b0cf77990e', 'ChangeStatusToFail', 'fe785ac2-09af-4cb0-a679-030310809bee'),
    ('15cf9ccf-054e-4061-8c8d-bc4e602191e0', 'ChangeStatusToFail', '3dc91a3c-402b-45b5-80a5-b96c75e24391'),
    ('1ff86f44-9818-44f7-a421-89105a3a4a7c', 'ChangeStatusToFail', '69aeef24-d4fe-45b5-b0e8-cce920f67936'),
    ('6d98f3cf-5654-44f8-850f-7cca6257af00', 'ChangeStatusToSuccess', 'd8fa6460-36cf-458f-9f12-f43cdd9c6b28'),
    ('a01bf92f-f69d-4a01-9349-325a2d2a296b', 'ChangeStatusToSuccess', '3dc91a3c-402b-45b5-80a5-b96c75e24391'),
    ('aadb8248-f18d-438e-bd9e-c33753e71fee', 'ChangeStatusToSuccess', 'fe785ac2-09af-4cb0-a679-030310809bee'),
    ('ab6bab02-cd95-4ac1-846f-c3acfcc39541', 'ChangeSetTradeStatusToFail', 'fe785ac2-09af-4cb0-a679-030310809bee'),
    ('b43288c1-b3e1-413b-a0e2-fe52781ee1b0', 'ChangeStatusToFail', 'd8fa6460-36cf-458f-9f12-f43cdd9c6b28'),
    ('bbfeb1ac-453b-4bd3-b3e7-b07c61bb6538', 'ChangeStatusToSuccess', '69aeef24-d4fe-45b5-b0e8-cce920f67936'),
    ('f9ba7739-b55c-49de-8beb-ccc3d08beba8', 'ChangeSetTradeStatusToSuccess', 'fe785ac2-09af-4cb0-a679-030310809bee');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240423093903_AddChangeStatusAction') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20240423093903_AddChangeStatusAction', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240430083935_UpdateRefundResponseCodes') THEN

    DELETE FROM `response_codes`
    WHERE `id` = '2203f732-3fbe-4738-95d2-1c0f70603914';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240430083935_UpdateRefundResponseCodes') THEN

    DELETE FROM `response_codes`
    WHERE `id` = '220ed567-701a-4903-a8fe-ad5d3cfc43c1';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240430083935_UpdateRefundResponseCodes') THEN

    DELETE FROM `response_codes`
    WHERE `id` = 'ca7f7e31-69f1-4d33-a1b6-9f9645f26d00';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240430083935_UpdateRefundResponseCodes') THEN

    UPDATE `response_codes` SET `state` = 'RefundSuccess'
    WHERE `id` = 'b6bedce3-9801-419a-8b3d-e1b726ba9607';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240430083935_UpdateRefundResponseCodes') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20240430083935_UpdateRefundResponseCodes', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240515065348_UpdateSbaCallbackAction') THEN

    UPDATE `response_code_actions` SET `action` = 'SbaConfirm'
    WHERE `id` = 'b89a1eba-eedd-4509-b427-2c26456f2755';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240515065348_UpdateSbaCallbackAction') THEN

    INSERT INTO `response_code_actions` (`id`, `action`, `response_code_id`)
    VALUES ('b06c1068-7d65-4784-822f-5c471a89a21d', 'SbaConfirm', 'd8fa6460-36cf-458f-9f12-f43cdd9c6b28');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240515065348_UpdateSbaCallbackAction') THEN

    UPDATE `response_codes` SET `suggestion` = 'Check freewill, Before confirm SBA'
    WHERE `id` = 'd8fa6460-36cf-458f-9f12-f43cdd9c6b28';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240515065348_UpdateSbaCallbackAction') THEN

    UPDATE `response_codes` SET `suggestion` = 'Check freewill, Check Customer Trading Account balance , Before confirm SBA'
    WHERE `id` = 'd9fef26d-a2ef-470e-82be-9f64f2c32d90';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240515065348_UpdateSbaCallbackAction') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20240515065348_UpdateSbaCallbackAction', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240613033026_RemoveDuplicateState') THEN

    DELETE FROM `response_code_actions`
    WHERE `id` = '1ff86f44-9818-44f7-a421-89105a3a4a7c';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240613033026_RemoveDuplicateState') THEN

    DELETE FROM `response_code_actions`
    WHERE `id` = 'bbfeb1ac-453b-4bd3-b3e7-b07c61bb6538';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240613033026_RemoveDuplicateState') THEN

    DELETE FROM `response_codes`
    WHERE `id` = '69aeef24-d4fe-45b5-b0e8-cce920f67936';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240613033026_RemoveDuplicateState') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20240613033026_RemoveDuplicateState', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240808090024_AddRetryWithdrawActions') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20240808090024_AddRetryWithdrawActions', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240821075730_AddDefaultResponseCodeUpdateTransaction') THEN

    INSERT INTO `response_codes` (`id`, `description`, `is_filterable`, `machine`, `product_type`, `state`, `suggestion`)
    VALUES ('961c1faf-963c-40ed-87e1-01ebfdebb0a0', 'Update Transaction', FALSE, 'Deposit', NULL, '', NULL),
    ('bba989b7-8103-4841-802b-acc9146b0bd9', 'Update Transaction', FALSE, 'Withdraw', NULL, '', NULL);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240821075730_AddDefaultResponseCodeUpdateTransaction') THEN

    INSERT INTO `response_code_actions` (`id`, `action`, `response_code_id`)
    VALUES ('22872725-cdba-4ab8-9b04-c2748be6e2cb', 'ChangeStatusToFail', '961c1faf-963c-40ed-87e1-01ebfdebb0a0'),
    ('362f0108-15c5-4bba-899f-92fd33a6f6ae', 'ChangeStatusToFail', 'bba989b7-8103-4841-802b-acc9146b0bd9'),
    ('b4bab9c1-fd63-4b41-83d6-2465226ed4d6', 'ChangeStatusToSuccess', 'bba989b7-8103-4841-802b-acc9146b0bd9'),
    ('f1f75678-b77f-4091-8267-cc284484cb0c', 'ChangeStatusToSuccess', '961c1faf-963c-40ed-87e1-01ebfdebb0a0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240821075730_AddDefaultResponseCodeUpdateTransaction') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20240821075730_AddDefaultResponseCodeUpdateTransaction', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240902050200_AddInvalidSourceEmailAction') THEN

    INSERT INTO `response_codes` (`id`, `description`, `is_filterable`, `machine`, `product_type`, `state`, `suggestion`)
    VALUES ('8246f2c5-e8dc-4876-956a-9c9fb2361610', 'Incorrect Source - Email not success', TRUE, 'Deposit', NULL, 'InvalidSourceSendEmailFailed', 'Manually email to customer for documents before proceed next step'),
    ('fc8220ca-5728-468f-85f8-406e1c2f0ff4', 'Incorrect Source - Email success', TRUE, 'Deposit', NULL, 'InvalidSourceSendEmailSuccess', 'Waiting customer document before proceed next step');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20240902050200_AddInvalidSourceEmailAction') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20240902050200_AddInvalidSourceEmailAction', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20241203091115_AddTransferCashActions') THEN

    INSERT INTO `response_codes` (`id`, `description`, `is_filterable`, `machine`, `product_type`, `state`, `suggestion`)
    VALUES ('9621da04-db2a-4286-a792-8b7000897f43', 'SetTrade Trading Account Transfer Fail', FALSE, 'TransferCash', NULL, 'TransferCashFailedRequireActionSetTrade', 'Check MT4, check customer SetTrade account balance, before manual allocate');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20241203091115_AddTransferCashActions') THEN

    INSERT INTO `response_code_actions` (`id`, `action`, `response_code_id`)
    VALUES ('5c329989-d81e-4a81-8e4c-140de3454417', 'ChangeStatusToSuccess', '9621da04-db2a-4286-a792-8b7000897f43'),
    ('641da5c8-cf45-4f3c-812e-c9b13047bca0', 'ChangeStatusToFail', '9621da04-db2a-4286-a792-8b7000897f43');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20241203091115_AddTransferCashActions') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20241203091115_AddTransferCashActions', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20250127073059_AddBillPaymentActions') THEN

    UPDATE `response_codes` SET `state` = 'TransferFailedRequireActionSetTrade'
    WHERE `id` = '9621da04-db2a-4286-a792-8b7000897f43';
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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20250127073059_AddBillPaymentActions') THEN

    INSERT INTO `response_codes` (`id`, `description`, `is_filterable`, `machine`, `product_type`, `state`, `suggestion`)
    VALUES ('a10a5dfb-265b-457d-996b-4858ad450bd5', 'Account Number Mismatch (Ref1)', FALSE, 'Deposit', NULL, 'BillPaymentRequestInvalid', 'Look up sender bank name and verify with customers before input correct account number');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20250127073059_AddBillPaymentActions') THEN

    INSERT INTO `response_code_actions` (`id`, `action`, `response_code_id`)
    VALUES ('6952e94c-38aa-44eb-9f2b-9fb2fb280907', 'UpdateBillPaymentReference', 'a10a5dfb-265b-457d-996b-4858ad450bd5'),
    ('c54b668a-32a0-478f-bd57-ae40fda48a00', 'ChangeStatusToSuccess', 'a10a5dfb-265b-457d-996b-4858ad450bd5'),
    ('fe1f553b-c87e-43ec-868d-d19cd05f6e28', 'ChangeStatusToFail', 'a10a5dfb-265b-457d-996b-4858ad450bd5');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20250127073059_AddBillPaymentActions') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20250127073059_AddBillPaymentActions', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20250214031814_AddTransferCashFailedState') THEN

    INSERT INTO `response_codes` (`id`, `description`, `is_filterable`, `machine`, `product_type`, `state`, `suggestion`)
    VALUES ('f4c3f9ab-1873-4b1b-b301-9002b5aaf85f', 'SetTrade Trading Account Transfer Fail', FALSE, 'TransferCash', NULL, 'TransferCashFailedRequireActionSetTrade', 'Check MT4, check customer SetTrade account balance, before manual allocate');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20250214031814_AddTransferCashFailedState') THEN

    INSERT INTO `response_code_actions` (`id`, `action`, `response_code_id`)
    VALUES ('0c08be70-9370-44ba-bd4a-8e6404392a2f', 'ChangeStatusToSuccess', 'f4c3f9ab-1873-4b1b-b301-9002b5aaf85f'),
    ('53cd518f-6f3a-4d7d-b211-097fce7873a5', 'ChangeStatusToFail', 'f4c3f9ab-1873-4b1b-b301-9002b5aaf85f');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20250214031814_AddTransferCashFailedState') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20250214031814_AddTransferCashFailedState', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20250219083155_AddBillPaymentFailedNameMismatchState') THEN

    INSERT INTO `response_codes` (`id`, `description`, `is_filterable`, `machine`, `product_type`, `state`, `suggestion`)
    VALUES ('aa6e979e-7304-4731-b535-52c3ec111ca4', 'Name Mismatch', TRUE, 'Deposit', 'ThaiEquity', 'BillPaymentFailedNameMismatch', 'Investigate Name');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20250219083155_AddBillPaymentFailedNameMismatchState') THEN

    INSERT INTO `response_code_actions` (`id`, `action`, `response_code_id`)
    VALUES ('124f57f9-d44b-41e3-adba-5d75f9f26ae1', 'ChangeStatusToSuccess', 'aa6e979e-7304-4731-b535-52c3ec111ca4'),
    ('651698ea-702a-41df-883e-fb9e56691264', 'Approve', 'aa6e979e-7304-4731-b535-52c3ec111ca4'),
    ('7045ca97-1700-4d5e-a8e7-51aa67ea8ea1', 'ChangeStatusToFail', 'aa6e979e-7304-4731-b535-52c3ec111ca4');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20250219083155_AddBillPaymentFailedNameMismatchState') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20250219083155_AddBillPaymentFailedNameMismatchState', '7.0.5');

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
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20250717085315_AddUpBackFailedProposeMismatchState') THEN

    INSERT INTO `response_codes` (`id`, `description`, `is_filterable`, `machine`, `product_type`, `state`, `suggestion`)
    VALUES ('e7267f12-7e3d-4181-8bf2-ef87e8903842', 'Pending for manual in SBA', TRUE, 'Deposit', NULL, 'UpBackFailedPurposeMismatch', 'manual in SBA');

END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20250717085315_AddUpBackFailedProposeMismatchState') THEN

    INSERT INTO `response_code_actions` (`id`, `action`, `response_code_id`)
    VALUES ('1400e565-e420-40ed-88b7-2f300590e14c', 'ChangeStatusToFail', 'e7267f12-7e3d-4181-8bf2-ef87e8903842'),
    ('8ff5de2a-d090-4648-8482-dd096f104259', 'ChangeStatusToSuccess', 'e7267f12-7e3d-4181-8bf2-ef87e8903842');

END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__DataSeedingDbContext` WHERE `migration_id` = '20250717085315_AddUpBackFailedProposeMismatchState') THEN

    INSERT INTO `__DataSeedingDbContext` (`migration_id`, `product_version`)
    VALUES ('20250717085315_AddUpBackFailedProposeMismatchState', '7.0.5');

END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;