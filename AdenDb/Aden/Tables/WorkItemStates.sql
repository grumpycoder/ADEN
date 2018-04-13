CREATE TABLE [Aden].[WorkItemStates] (
    [WorkItemStateId] INT          NOT NULL,
    [Name]            VARCHAR (50) NULL,
    CONSTRAINT [PK_WorkItemState] PRIMARY KEY CLUSTERED ([WorkItemStateId] ASC)
);

