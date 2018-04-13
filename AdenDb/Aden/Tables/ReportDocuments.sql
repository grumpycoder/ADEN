CREATE TABLE [Aden].[ReportDocuments] (
    [ReportDocumentId] INT             IDENTITY (1, 1) NOT NULL,
    [ReportLevelId]    INT             NOT NULL,
    [FileData]         VARBINARY (MAX) NULL,
    [Filename]         VARCHAR (255)   NULL,
    [Version]          INT             NOT NULL,
    [ReportId]         INT             NOT NULL,
    CONSTRAINT [PK_Aden.ReportDocuments] PRIMARY KEY CLUSTERED ([ReportDocumentId] ASC),
    CONSTRAINT [FK_Aden.ReportDocuments_Aden.Reports_ReportId] FOREIGN KEY ([ReportId]) REFERENCES [Aden].[Reports] ([ReportId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ReportDocuments_ReportLevel] FOREIGN KEY ([ReportLevelId]) REFERENCES [Aden].[ReportLevels] ([ReportLevelId])
);


GO
CREATE NONCLUSTERED INDEX [IX_ReportId]
    ON [Aden].[ReportDocuments]([ReportId] ASC);

