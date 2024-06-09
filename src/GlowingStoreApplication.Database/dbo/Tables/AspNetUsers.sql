CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                         UNIQUEIDENTIFIER   DEFAULT (newid()) NOT NULL,
    [UserName]                   NVARCHAR (256)     NOT NULL,
    [NormalizedUserName]         NVARCHAR (256)     NOT NULL,
    [FirstName]                  NVARCHAR (256)     NOT NULL,
    [LastName]                   NVARCHAR (256)     NULL,
    [Email]                      NVARCHAR (256)     NOT NULL,
    [NormalizedEmail]            NVARCHAR (256)     NOT NULL,
    [EmailConfirmed]             BIT                NOT NULL,
    [PasswordHash]               NVARCHAR (MAX)     NOT NULL,
    [SecurityStamp]              NVARCHAR (MAX)     NULL,
    [ConcurrencyStamp]           NVARCHAR (MAX)     NULL,
    [PhoneNumber]                NVARCHAR (256)     NULL,
    [PhoneNumberConfirmed]       BIT                NOT NULL,
    [TwoFactorEnabled]           BIT                NOT NULL,
    [LockoutEnd]                 DATETIMEOFFSET (7) NULL,
    [LockoutEnabled]             BIT                NOT NULL,
    [AccessFailedCount]          INT                NOT NULL,

    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Email]
    ON [dbo].[AspNetUsers]([NormalizedEmail] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserName]
    ON [dbo].[AspNetUsers]([NormalizedUserName] ASC);