CREATE TABLE [identity].[UserRole] (
    [Id]     INT IDENTITY (1, 1) NOT NULL,
    [UserId] INT NOT NULL,
    [RoleId] INT NOT NULL,
    CONSTRAINT [PK_identity.UserRole] PRIMARY KEY CLUSTERED ([Id] ASC)
);

