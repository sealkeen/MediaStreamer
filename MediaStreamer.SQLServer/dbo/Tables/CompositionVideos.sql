CREATE TABLE [dbo].[CompositionVideos] (
    [VideoID]       UNIQUEIDENTIFIER NOT NULL,
    [CompositionID] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompositionVideo] PRIMARY KEY CLUSTERED ([VideoID] ASC, [CompositionID] ASC),
    CONSTRAINT [FK_CompositionVideo_Composition_CompositionID] FOREIGN KEY ([CompositionID]) REFERENCES [dbo].[Compositions] ([CompositionID]),
    CONSTRAINT [FK_CompositionVideo_Video_VideoID] FOREIGN KEY ([VideoID]) REFERENCES [dbo].[Videos] ([VideoID]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_CompositionVideo_CompositionID]
    ON [dbo].[CompositionVideos]([CompositionID] ASC);

