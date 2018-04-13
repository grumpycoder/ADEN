CREATE TABLE [Aden].[WorkItems] (
    [WorkItemId]       INT           IDENTITY (1, 1) NOT NULL,
    [ReportId]         INT           NULL,
    [AssignedUser]     VARCHAR (75)  NULL,
    [AssignedDate]     DATETIME2 (7) NOT NULL,
    [CompletedDate]    SMALLDATETIME NULL,
    [WorkItemActionId] INT           NOT NULL,
    [WorkItemStateId]  INT           NOT NULL,
    [Notes]            VARCHAR (500) NULL,
    CONSTRAINT [PK_WorkItems] PRIMARY KEY CLUSTERED ([WorkItemId] ASC),
    CONSTRAINT [FK_Aden.WorkItems_Aden.Reports_ReportId] FOREIGN KEY ([ReportId]) REFERENCES [Aden].[Reports] ([ReportId]),
    CONSTRAINT [FK_WorkItems_WorkItemAction] FOREIGN KEY ([WorkItemActionId]) REFERENCES [Aden].[WorkItemActions] ([WorkItemActionId]),
    CONSTRAINT [FK_WorkItems_WorkItemState] FOREIGN KEY ([WorkItemStateId]) REFERENCES [Aden].[WorkItemStates] ([WorkItemStateId])
);


GO
CREATE NONCLUSTERED INDEX [IX_ReportId]
    ON [Aden].[WorkItems]([ReportId] ASC);

