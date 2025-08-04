DROP TABLE IF EXISTS `portfolio_global_equity_depositwithdraw_daily_snapshot`;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'SequelizeMeta' AND TABLE_SCHEMA = 'portfolio_summary_db';
SHOW INDEX FROM `SequelizeMeta`;
DELETE FROM `SequelizeMeta` WHERE `name` = '20250122080831-create-portfolio-global-equity-depositwithdraw-table.js';
