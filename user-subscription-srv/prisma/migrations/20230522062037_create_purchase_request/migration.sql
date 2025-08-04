-- CreateTable
CREATE TABLE `purcahse_request` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `reference_code` VARCHAR(191) NOT NULL,
    `customer_code` VARCHAR(191) NOT NULL,
    `product` ENUM('mt5') NOT NULL,
    `plan_id` INTEGER NOT NULL,
    `amount` INTEGER NOT NULL,
    `status` ENUM('CREATED', 'GENERATED_PAYMENT', 'RECEIVED_PAYMENT', 'COMPLETED', 'REJECTED') NOT NULL DEFAULT 'CREATED',
    `payment_generated_id` INTEGER NULL,
    `payment_received_id` INTEGER NULL,
    `created_at` DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
    `updated_at` DATETIME(3) NOT NULL,

    UNIQUE INDEX `purcahse_request_reference_code_key`(`reference_code`),
    UNIQUE INDEX `purcahse_request_payment_generated_id_key`(`payment_generated_id`),
    UNIQUE INDEX `purcahse_request_payment_received_id_key`(`payment_received_id`),
    PRIMARY KEY (`id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `payment_generated` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `transaction_no` VARCHAR(191) NOT NULL,
    `transaction_ref_code` VARCHAR(191) NOT NULL,
    `amount` INTEGER NOT NULL,
    `qr_value` VARCHAR(191) NOT NULL,
    `created_at` DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
    `updated_at` DATETIME(3) NOT NULL,

    PRIMARY KEY (`id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateTable
CREATE TABLE `payment_received` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `transaction_no` VARCHAR(191) NOT NULL,
    `transaction_ref_code` VARCHAR(191) NOT NULL,
    `customer_name` VARCHAR(191) NOT NULL,
    `customer_bank_account` VARCHAR(191) NOT NULL,
    `customer_bank_code` VARCHAR(191) NOT NULL,
    `amount` INTEGER NOT NULL,
    `payment_type` VARCHAR(191) NOT NULL,
    `payment_date_time` VARCHAR(191) NOT NULL,
    `created_at` DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
    `updaupdated_at` DATETIME(3) NOT NULL,

    PRIMARY KEY (`id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- AddForeignKey
ALTER TABLE `purcahse_request` ADD CONSTRAINT `purcahse_request_plan_id_fkey` FOREIGN KEY (`plan_id`) REFERENCES `plan`(`id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `purcahse_request` ADD CONSTRAINT `purcahse_request_payment_generated_id_fkey` FOREIGN KEY (`payment_generated_id`) REFERENCES `payment_generated`(`id`) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `purcahse_request` ADD CONSTRAINT `purcahse_request_payment_received_id_fkey` FOREIGN KEY (`payment_received_id`) REFERENCES `payment_received`(`id`) ON DELETE SET NULL ON UPDATE CASCADE;
