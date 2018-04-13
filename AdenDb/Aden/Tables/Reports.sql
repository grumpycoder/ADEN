CREATE TABLE [Aden].[Reports] (
    [ReportId]      INT           IDENTITY (1, 1) NOT NULL,
    [GeneratedDate] SMALLDATETIME NULL,
    [ApprovedDate]  SMALLDATETIME NULL,
    [SubmittedDate] SMALLDATETIME NULL,
    [GeneratedUser] VARCHAR (255) NULL,
    [ApprovedUser]  VARCHAR (75)  NULL,
    [SubmittedUser] VARCHAR (75)  NULL,
    [ReportStateId] TINYINT       NOT NULL,
    [DataYear]      INT           NULL,
    [SubmissionId]  INT           NULL,
    CONSTRAINT [PK_Aden.Reports] PRIMARY KEY CLUSTERED ([ReportId] ASC),
    CONSTRAINT [FK_Reports_ReportState] FOREIGN KEY ([ReportStateId]) REFERENCES [Aden].[ReportStates] ([ReportStateId]),
    CONSTRAINT [FK_Reports_Submissions] FOREIGN KEY ([SubmissionId]) REFERENCES [Aden].[Submissions] ([SubmissionId])
);

