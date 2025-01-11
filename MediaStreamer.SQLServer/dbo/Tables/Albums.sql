CREATE TABLE [dbo].[Albums] (
    [AlbumID]   UNIQUEIDENTIFIER NOT NULL,
    [AlbumName] NVARCHAR (512)   NOT NULL,
    [ArtistID]  UNIQUEIDENTIFIER NULL,
    [GenreID]   UNIQUEIDENTIFIER NOT NULL,
    [Year]      BIGINT           NULL,
    [Label]     NVARCHAR (50)    NULL,
    [Type]      NVARCHAR (50)    NULL,
    CONSTRAINT [PK_Album] PRIMARY KEY CLUSTERED ([AlbumID] ASC),
    CONSTRAINT [FK_Album_Artist_ArtistID] FOREIGN KEY ([ArtistID]) REFERENCES [dbo].[Artists] ([ArtistID]),
    CONSTRAINT [FK_Album_Genre_GenreID] FOREIGN KEY ([GenreID]) REFERENCES [dbo].[Genres] ([GenreID]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Album_ArtistID]
    ON [dbo].[Albums]([ArtistID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Album_GenreID]
    ON [dbo].[Albums]([GenreID] ASC);

