CREATE TABLE [dbo].[Genres] (
    [GenreID]   UNIQUEIDENTIFIER NOT NULL,
    [GenreName] NVARCHAR (256)   NULL,
    [StyleId]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Genre] PRIMARY KEY CLUSTERED ([GenreID] ASC),
    FOREIGN KEY ([StyleId]) REFERENCES [dbo].[Styles] ([StyleId])
);

