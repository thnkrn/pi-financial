/*
  Warnings:

  - Added the required column `qr_expiration_time` to the `payment_generated` table without a default value. This is not possible if the table is not empty.

*/
-- AlterTable
ALTER TABLE `payment_generated` ADD COLUMN `qr_expiration_time` DATETIME(3) NOT NULL;
