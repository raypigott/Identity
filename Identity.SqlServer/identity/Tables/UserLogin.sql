CREATE TABLE [identity].[UserLogin] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [LoginProvider] NVARCHAR (128) NOT NULL,
    [ProviderKey]   NVARCHAR (128) NOT NULL,
    [UserId]        INT            NOT NULL,
    CONSTRAINT [PK_identity.UserLogin] PRIMARY KEY CLUSTERED ([Id] ASC)
);

