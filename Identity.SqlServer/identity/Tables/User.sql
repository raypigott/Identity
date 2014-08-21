CREATE TABLE [identity].[User] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [Email]                  NVARCHAR (256) NULL,
    [IsEmailConfirmed]       BIT            NOT NULL,
    [PasswordHash]           NVARCHAR (MAX) NULL,
    [SecurityStamp]          NVARCHAR (MAX) NULL,
    [PhoneNumber]            NVARCHAR (MAX) NULL,
    [IsPhoneNumberConfirmed] BIT            NOT NULL,
    [IsTwoFactorEnabled]     BIT            NOT NULL,
    [LockoutEndDateUtc]      DATETIME       NULL,
    [IsLockoutEnabled]       BIT            NOT NULL,
    [AccessFailedCount]      INT            NOT NULL,
    [UserName]               NVARCHAR (256) NOT NULL,
    [IsAccountActive]        BIT            NOT NULL,
    CONSTRAINT [PK_identity.User] PRIMARY KEY CLUSTERED ([Id] ASC)
);

