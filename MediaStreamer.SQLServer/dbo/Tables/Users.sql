CREATE TABLE [dbo].[Users] (
    [UserID]       UNIQUEIDENTIFIER NOT NULL,
    [UserName]     NVARCHAR (128)   NOT NULL,
    [Password]     NVARCHAR (128)   NOT NULL,
    [Email]        NVARCHAR (256)   NOT NULL,
    [DateOfSignUp] DATETIME         NOT NULL,
    [VKLink]       NVARCHAR (256)   NULL,
    [FaceBookLink] NVARCHAR (256)   NULL,
    [Bio]          NVARCHAR (512)   NULL,
    [AspNetUserId] NVARCHAR (450)   NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserID] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_User_Email]
    ON [dbo].[Users]([Email] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_User_UserName]
    ON [dbo].[Users]([UserName] ASC);

