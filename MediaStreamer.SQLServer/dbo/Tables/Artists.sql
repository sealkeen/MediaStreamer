﻿CREATE TABLE [dbo].[Artists] (
    [ArtistID]   UNIQUEIDENTIFIER NOT NULL,
    [ArtistName] NVARCHAR (256)   NOT NULL,
    CONSTRAINT [PK_Artist] PRIMARY KEY CLUSTERED ([ArtistID] ASC)
);

