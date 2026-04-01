-- 1. Create the Users Table
CREATE TABLE [dbo].[Users] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Username] NVARCHAR(50) NOT NULL UNIQUE
);

-- 2. Create the Wallets Table
CREATE TABLE [dbo].[Wallets] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] INT NOT NULL FOREIGN KEY REFERENCES [Users]([Id]),
    [CurrencyCode] NVARCHAR(3) NOT NULL,
    [Balance] DECIMAL(18, 4) NOT NULL DEFAULT 0.00
);

-- 3. Create the Transactions Table
CREATE TABLE [dbo].[Transactions] (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UserId] INT NOT NULL FOREIGN KEY REFERENCES [Users]([Id]),
    [BoughtCurrency] NVARCHAR(3) NOT NULL,
    [SoldCurrency] NVARCHAR(3) NOT NULL,
    [ExchangeRate] DECIMAL(18, 4) NOT NULL,
    [Amount] DECIMAL(18, 4) NOT NULL,
    [TransactionDate] DATETIME NOT NULL DEFAULT GETDATE()
);

-- 4. Add a test user with some starting money (10,000 PLN) so we have data to test with!
INSERT INTO [dbo].[Users] ([Username]) VALUES ('TestUser');
INSERT INTO [dbo].[Wallets] ([UserId], [CurrencyCode], [Balance]) VALUES (1, 'PLN', 10000.00);
INSERT INTO [dbo].[Wallets] ([UserId], [CurrencyCode], [Balance]) VALUES (1, 'USD', 0.00);