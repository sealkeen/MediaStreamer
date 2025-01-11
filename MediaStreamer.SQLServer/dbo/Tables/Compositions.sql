CREATE TABLE [dbo].[Compositions] (
    [CompositionID]   UNIQUEIDENTIFIER NOT NULL,
    [CompositionName] NVARCHAR (512)   NOT NULL,
    [ArtistID]        UNIQUEIDENTIFIER NULL,
    [AlbumID]         UNIQUEIDENTIFIER NULL,
    [Duration]        BIGINT           NULL,
    [FilePath]        NVARCHAR (324)   NULL,
    [Lyrics]          NVARCHAR (3600)  NULL,
    [About]           NVARCHAR (512)   NULL,
    CONSTRAINT [PK_Composition] PRIMARY KEY CLUSTERED ([CompositionID] ASC),
    CONSTRAINT [FK_Composition_Album_AlbumID] FOREIGN KEY ([AlbumID]) REFERENCES [dbo].[Albums] ([AlbumID]),
    CONSTRAINT [FK_Composition_Artist_ArtistID] FOREIGN KEY ([ArtistID]) REFERENCES [dbo].[Artists] ([ArtistID])
);


GO
CREATE NONCLUSTERED INDEX [IX_Composition_AlbumID]
    ON [dbo].[Compositions]([AlbumID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Composition_ArtistID]
    ON [dbo].[Compositions]([ArtistID] ASC);

