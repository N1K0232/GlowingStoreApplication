CREATE TABLE [dbo].[Products]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
    [CategoryId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(256) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [Quantity] INTEGER NOT NULL,
    [Price] DECIMAL(8, 2) NOT NULL,
    [DiscountPercentage] FLOAT NULL,
    [CreationDate] DATETIME NOT NULL DEFAULT getutcdate(),
    [LastModificationDate] DATETIME NULL,
    [IsDeleted] BIT NOT NULL DEFAULT (0),
    [DeletedDate] DATETIME NULL,
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Product]
ON [dbo].[Products]([CategoryId],[Name],[Price]);