CREATE TABLE [identity].[Role] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_identity.Role] PRIMARY KEY CLUSTERED ([Id] ASC)
);

