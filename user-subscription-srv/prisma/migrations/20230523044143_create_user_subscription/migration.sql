-- CreateTable
CREATE TABLE `user_subscription` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `customer_code` VARCHAR(191) NOT NULL,
    `plan_id` INTEGER NULL,
    `purchase_request_id` INTEGER NULL,
    `active_date` DATE NOT NULL,
    `expired_Date` DATE NOT NULL,
    `createdAt` DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
    `updatedAt` DATETIME(3) NOT NULL,

    UNIQUE INDEX `user_subscription_purchase_request_id_key`(`purchase_request_id`),
    PRIMARY KEY (`id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- AddForeignKey
ALTER TABLE `user_subscription` ADD CONSTRAINT `user_subscription_plan_id_fkey` FOREIGN KEY (`plan_id`) REFERENCES `plan`(`id`) ON DELETE SET NULL ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE `user_subscription` ADD CONSTRAINT `user_subscription_purchase_request_id_fkey` FOREIGN KEY (`purchase_request_id`) REFERENCES `purcahse_request`(`id`) ON DELETE SET NULL ON UPDATE CASCADE;
