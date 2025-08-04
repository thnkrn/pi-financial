-- Create "bo_change_requests" table
CREATE TABLE `bo_change_requests` (
  `id` varchar(36) NOT NULL,
  `user_id` varchar(36) NULL,
  `info_type` varchar(50) NULL,
  `status` varchar(10) NULL,
  `maker_id` varchar(36) NULL,
  `maker_name` varchar(100) NULL,
  `checker_id` varchar(36) NULL,
  `checker_name` varchar(100) NULL,
  `created_at` datetime(3) NULL,
  `updated_at` datetime(3) NULL,
  PRIMARY KEY (`id`)
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
-- Create "bo_audit_logs" table
CREATE TABLE `bo_audit_logs` (
  `id` varchar(36) NOT NULL,
  `change_request_id` varchar(36) NULL,
  `action` varchar(10) NULL,
  `actor` varchar(100) NULL,
  `note` text NULL,
  `action_time` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `fk_bo_audit_logs_change_request` (`change_request_id`),
  CONSTRAINT `fk_bo_audit_logs_change_request` FOREIGN KEY (`change_request_id`) REFERENCES `bo_change_requests` (`id`) ON UPDATE NO ACTION ON DELETE NO ACTION
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
-- Create "bo_change_request_infos" table
CREATE TABLE `bo_change_request_infos` (
  `id` varchar(36) NOT NULL,
  `change_request_id` varchar(36) NULL,
  `field_name` varchar(50) NULL,
  `current_value` varchar(255) NULL,
  `change_value` varchar(255) NULL,
  `created_at` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  INDEX `fk_bo_change_request_infos_change_request` (`change_request_id`),
  CONSTRAINT `fk_bo_change_request_infos_change_request` FOREIGN KEY (`change_request_id`) REFERENCES `bo_change_requests` (`id`) ON UPDATE NO ACTION ON DELETE NO ACTION
) CHARSET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
