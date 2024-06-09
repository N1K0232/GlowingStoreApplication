CREATE TABLE [dbo].[Images]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
    [Path] NVARCHAR(512) NOT NULL,
    [ContentType] NVARCHAR(50) NOT NULL,
    [Length] BIGINT NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [CreationDate] DATETIME NOT NULL DEFAULT getutcdate(),
    [LastModificationDate] DATETIME NULL,

    PRIMARY KEY([Id])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Path]
ON [dbo].[Images]([Path]);