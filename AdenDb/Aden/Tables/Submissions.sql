CREATE TABLE [Aden].[Submissions] (
    [SubmissionId]          INT             IDENTITY (1, 1) NOT NULL,
    [FileSpecificationId]   INT             NOT NULL,
    [ReportStateId]         TINYINT         NOT NULL,
    [DueDate]               SMALLDATETIME   NULL,
    [DataYear]              INT             NULL,
    [IsSEA]                 BIT             NULL,
    [IsLEA]                 BIT             NULL,
    [IsSCH]                 BIT             NULL,
    [SpecificationDocument] VARBINARY (MAX) NULL,
    CONSTRAINT [PK_Submissions] PRIMARY KEY CLUSTERED ([SubmissionId] ASC),
    CONSTRAINT [FK_Submissions_FileSpecifications] FOREIGN KEY ([FileSpecificationId]) REFERENCES [Aden].[FileSpecifications] ([FileSpecificationId]),
    CONSTRAINT [FK_Submissions_ReportStates] FOREIGN KEY ([ReportStateId]) REFERENCES [Aden].[ReportStates] ([ReportStateId])
);

