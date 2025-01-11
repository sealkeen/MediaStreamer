CREATE TABLE [dbo].[AlbumGenres] (
    [GenreID] UNIQUEIDENTIFIER NOT NULL,
    [AlbumID] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_AlbumGenre] PRIMARY KEY CLUSTERED ([AlbumID] ASC, [GenreID] ASC),
    CONSTRAINT [FK_AlbumGenre_Album_AlbumID] FOREIGN KEY ([AlbumID]) REFERENCES [dbo].[Albums] ([AlbumID]),
    CONSTRAINT [FK_AlbumGenre_Genre_GenreID] FOREIGN KEY ([GenreID]) REFERENCES [dbo].[Genres] ([GenreID])
);


GO
CREATE NONCLUSTERED INDEX [IX_AlbumGenre_GenreID]
    ON [dbo].[AlbumGenres]([GenreID] ASC);

