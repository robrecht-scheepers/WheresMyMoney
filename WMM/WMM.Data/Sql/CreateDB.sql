BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS `Transactions` (
	`Id`	BLOB NOT NULL,
	`Date`	NUMERIC,
	`Category`	NUMERIC NOT NULL,
	`Amount`	NUMERIC NOT NULL,
	`Comments`	TEXT,
	`CreatedTime`	TEXT NOT NULL,
	`CreatedAccount`	TEXT NOT NULL,
	`LastUpdateTime`	TEXT NOT NULL,
	`LastUpdateAccount`	TEXT NOT NULL,
	`Deleted`	INTEGER NOT NULL,
	`Recurring`	INTEGER NOT NULL,
	FOREIGN KEY(`Category`) REFERENCES `Categories`(`Id`),
	PRIMARY KEY(`Id`)
);
CREATE TABLE IF NOT EXISTS `Goals` (
	`Id`	BLOB NOT NULL,
	`Name`	TEXT NOT NULL UNIQUE,
	`Description`	TEXT,
	`CategoryCriteria`	TEXT NOT NULL,
	`AreaCriteria`	TEXT NOT NULL,
	`CategoryTypeCriteria`	TEXT NOT NULL,
	`Limit`	NUMERIC NOT NULL,
	PRIMARY KEY(`Id`)
);
CREATE TABLE IF NOT EXISTS `Categories` (
	`Id`	BLOB NOT NULL,
	`Area`	BLOB NOT NULL,
	`Name`	TEXT NOT NULL UNIQUE,
	`ForecastType`	INTEGER,
	FOREIGN KEY(`Area`) REFERENCES `Areas`(`Id`),
	PRIMARY KEY(`Id`)
);
CREATE TABLE IF NOT EXISTS `Areas` (
	`Id`	BLOB NOT NULL,
	`Name`	TEXT NOT NULL UNIQUE,
	PRIMARY KEY(`Id`)
);
CREATE INDEX IF NOT EXISTS `TransactionsDeletedIndex` ON `Transactions` (
	`Deleted`
);
CREATE INDEX IF NOT EXISTS `TransactionDateIndex` ON `Transactions` (
	`Date`	ASC
);
CREATE INDEX IF NOT EXISTS `TransactionCategoryIndex` ON `Transactions` (
	`Category`
);
COMMIT;
