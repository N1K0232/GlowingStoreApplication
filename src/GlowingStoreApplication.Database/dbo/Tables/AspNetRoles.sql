CREATE TABLE [dbo].[AspNetRoles] (
    [Id]               UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Name]             NVARCHAR (256)   NOT NULL,
    [NormalizedName]   NVARCHAR (256)   NOT NULL,
    [ConcurrencyStamp] NVARCHAR (MAX)   NOT NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_RoleName]
    ON [dbo].[AspNetRoles]([NormalizedName] ASC);

