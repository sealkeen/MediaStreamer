CREATE TABLE [dbo].[Moderators] (
    [ModeratorID] UNIQUEIDENTIFIER NOT NULL,
    [UserID]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Moderator] PRIMARY KEY CLUSTERED ([ModeratorID] ASC),
    CONSTRAINT [FK_Moderator_User_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID])
);


GO
CREATE NONCLUSTERED INDEX [IX_Moderator_UserID]
    ON [dbo].[Moderators]([UserID] ASC);

