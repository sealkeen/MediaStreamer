CREATE TABLE [dbo].[ArtistGenres] (
    [ArtistID] UNIQUEIDENTIFIER NOT NULL,
    [GenreID]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ArtistGenre] PRIMARY KEY CLUSTERED ([ArtistID] ASC, [GenreID] ASC),
    CONSTRAINT [FK_ArtistGenre_Artist_ArtistID] FOREIGN KEY ([ArtistID]) REFERENCES [dbo].[Artists] ([ArtistID]),
    CONSTRAINT [FK_ArtistGenre_Genre_GenreID] FOREIGN KEY ([GenreID]) REFERENCES [dbo].[Genres] ([GenreID])
);


GO
CREATE NONCLUSTERED INDEX [IX_ArtistGenre_GenreID]
    ON [dbo].[ArtistGenres]([GenreID] ASC);

