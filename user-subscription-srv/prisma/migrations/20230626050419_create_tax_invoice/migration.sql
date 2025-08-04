/*
  Warnings:

  - You are about to alter the column `amount` on the `payment_generated` table. The data in that column could be lost. The data in that column will be cast from `Int` to `Double`.
  - You are about to alter the column `amount` on the `payment_received` table. The data in that column could be lost. The data in that column will be cast from `Int` to `Double`.
  - You are about to alter the column `price` on the `plan` table. The data in that column could be lost. The data in that column will be cast from `Int` to `Double`.
  - You are about to alter the column `amount` on the `purcahse_request` table. The data in that column could be lost. The data in that column will be cast from `Int` to `Double`.
  - A unique constraint covering the columns `[tax_invoice_id]` on the table `purcahse_request` will be added. If there are existing duplicate values, this will fail.

*/
-- AlterTable
ALTER TABLE `payment_generated` MODIFY `amount` DOUBLE NOT NULL;

-- AlterTable
ALTER TABLE `payment_received` MODIFY `amount` DOUBLE NOT NULL;

-- AlterTable
ALTER TABLE `plan` MODIFY `price` DOUBLE NOT NULL;

-- AlterTable
ALTER TABLE `purcahse_request` ADD COLUMN `tax_invoice_id` INTEGER NULL,
    MODIFY `amount` DOUBLE NOT NULL;

-- CreateTable
CREATE TABLE `tax_invoice` (
    `id` INTEGER NOT NULL AUTO_INCREMENT,
    `tax_invoice_no` VARCHAR(191) NOT NULL,
    `amount` DOUBLE NOT NULL,
    `amount_ex_vat` DOUBLE NOT NULL,
    `vat` DOUBLE NOT NULL,
    `customer_name` VARCHAR(191) NOT NULL,
    `customer_id` VARCHAR(191) NOT NULL,
    `createdAt` DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
    `updatedAt` DATETIME(3) NOT NULL,

    UNIQUE INDEX `tax_invoice_tax_invoice_no_key`(`tax_invoice_no`),
    PRIMARY KEY (`id`)
) DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- CreateIndex
CREATE UNIQUE INDEX `purcahse_request_tax_invoice_id_key` ON `purcahse_request`(`tax_invoice_id`);

-- AddForeignKey
ALTER TABLE `purcahse_request` ADD CONSTRAINT `purcahse_request_tax_invoice_id_fkey` FOREIGN KEY (`tax_invoice_id`) REFERENCES `tax_invoice`(`id`) ON DELETE SET NULL ON UPDATE CASCADE;
