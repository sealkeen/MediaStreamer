CREATE TABLE [dbo].[ListenedCompositions] (
    [ListenDate]    DATETIME         NOT NULL,
    [CompositionID] UNIQUEIDENTIFIER NOT NULL,
    [UserID]        UNIQUEIDENTIFIER NOT NULL,
    [StoppedAt]     FLOAT (53)       NOT NULL,
    [CountOfPlays]  BIGINT           NULL,
    CONSTRAINT [FK_ListenedComposition_User_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID])
);


GO
CREATE CLUSTERED INDEX [IX_CompositionID_UserId]
    ON [dbo].[ListenedCompositions]([CompositionID] ASC, [UserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ListenedComposition_CompositionID]
    ON [dbo].[ListenedCompositions]([CompositionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ListenedComposition_UserID]
    ON [dbo].[ListenedCompositions]([UserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ListenedComposition_ListenDate]
    ON [dbo].[ListenedCompositions]([ListenDate] ASC);

