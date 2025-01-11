CREATE TABLE [dbo].[Administrators] (
    [AdministratorID] UNIQUEIDENTIFIER NOT NULL,
    [ModeratorID]     UNIQUEIDENTIFIER NULL,
    [UserID]          UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Administrator] PRIMARY KEY CLUSTERED ([AdministratorID] ASC),
    CONSTRAINT [FK_Administrator_Moderator_ModeratorID] FOREIGN KEY ([ModeratorID]) REFERENCES [dbo].[Moderators] ([ModeratorID]),
    CONSTRAINT [FK_Administrator_User_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID])
);


GO
CREATE NONCLUSTERED INDEX [IX_Administrator_ModeratorID]
    ON [dbo].[Administrators]([ModeratorID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Administrator_UserID]
    ON [dbo].[Administrators]([UserID] ASC);

