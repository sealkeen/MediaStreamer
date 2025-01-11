CREATE TABLE [dbo].[Videos] (
    [VideoID]     UNIQUEIDENTIFIER NOT NULL,
    [XResolution] UNIQUEIDENTIFIER NULL,
    [YResolution] UNIQUEIDENTIFIER NULL,
    [FPS]         FLOAT (53)       NULL,
    [VariableFPS] BIT              NULL,
    [SizeKb]      UNIQUEIDENTIFIER NULL,
    [FilePath]    NVARCHAR (512)   NULL,
    CONSTRAINT [PK_Video] PRIMARY KEY CLUSTERED ([VideoID] ASC)
);

