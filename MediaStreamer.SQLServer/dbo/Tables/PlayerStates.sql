CREATE TABLE [dbo].[PlayerStates] (
    [StateID]     UNIQUEIDENTIFIER NOT NULL,
    [StateTime]   DATETIME         NOT NULL,
    [VolumeLevel] NUMERIC (38, 17) NOT NULL,
    CONSTRAINT [PK_PlayerStates] PRIMARY KEY CLUSTERED ([StateID] ASC)
);

