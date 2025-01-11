CREATE TABLE [dbo].[Styles] (
    [StyleId]   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [StyleName] VARCHAR (256)    NOT NULL,
    PRIMARY KEY CLUSTERED ([StyleId] ASC),
    UNIQUE NONCLUSTERED ([StyleName] ASC)
);

