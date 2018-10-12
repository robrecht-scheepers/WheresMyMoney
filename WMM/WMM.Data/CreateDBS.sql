﻿BEGIN TRANSACTION;
DROP TABLE IF EXISTS `Transactions`;
CREATE TABLE IF NOT EXISTS `Transactions` (
	`Id`	BLOB NOT NULL,
	`[Date]`	REAL NOT NULL,
	`Category`	BLOB NOT NULL,
	`Amount`	NUMERIC NOT NULL,
	`Comments`	TEXT,
	`CreatedTime`	REAL NOT NULL,
	`CreatedAccount`	TEXT NOT NULL,
	`LastUpdateTime`	REAL NOT NULL,
	`LastUpdateAccount`	TEXT NOT NULL,
	`Deleted`	INTEGER NOT NULL,
	FOREIGN KEY(`Category`) REFERENCES `Categories`(`Id`)
);
DROP TABLE IF EXISTS `Categories`;
CREATE TABLE IF NOT EXISTS `Categories` (
	`Id`	BLOB NOT NULL,
	`Area`	BLOB NOT NULL,
	`Name`	TEXT,
	PRIMARY KEY(`Id`),
	FOREIGN KEY(`Area`) REFERENCES `Areas`(`Id`)
);
DROP TABLE IF EXISTS `Areas`;
CREATE TABLE IF NOT EXISTS `Areas` (
	`Id`	BLOB NOT NULL,
	`Name`	TEXT NOT NULL,
	PRIMARY KEY(`Id`)
);
DROP INDEX IF EXISTS `TransactionsDeletedIndex`;
CREATE INDEX IF NOT EXISTS `TransactionsDeletedIndex` ON `Transactions` (
	`Deleted`
);
DROP INDEX IF EXISTS `TransactionDateIndex`;
CREATE INDEX IF NOT EXISTS `TransactionDateIndex` ON `Transactions` (
	`[Date]`	ASC
);
COMMIT;
