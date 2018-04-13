CREATE TABLE [Aden].[FileSpecifications] (
    [FileSpecificationId] INT           IDENTITY (1, 1) NOT NULL,
    [FileNumber]          VARCHAR (8)   NULL,
    [FileName]            VARCHAR (250) NULL,
    [Department]          VARCHAR (255) NULL,
    [GenerationUserGroup] VARCHAR (255) NULL,
    [ApprovalUserGroup]   VARCHAR (255) NULL,
    [SubmissionUserGroup] VARCHAR (255) NULL,
    [IsRetired]           BIT           NULL,
    [DueDate]             SMALLDATETIME NULL,
    [DataYear]            INT           NULL,
    [FileNameFormat]      VARCHAR (255) NULL,
    [ReportAction]        VARCHAR (255) NULL,
    CONSTRAINT [PK_Aden.FileSpecifications] PRIMARY KEY CLUSTERED ([FileSpecificationId] ASC)
);

