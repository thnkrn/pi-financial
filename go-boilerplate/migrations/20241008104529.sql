-- Modify "examples" table
ALTER TABLE `examples` MODIFY COLUMN `id` varchar(191) NOT NULL, ADD COLUMN `versioning` bigint NULL;
