using Aden.Core.Models;

namespace Aden.Core
{
    public class WorkItemFactory
    {
        public static WorkItem CreateNextWorkItem(WorkItem workItem)
        {
            if (workItem == null) return WorkItem.Create(WorkItemAction.Generate);

            switch (workItem.WorkItemAction)
            {
                case WorkItemAction.Generate:
                    return WorkItem.Create(WorkItemAction.Review);
                case WorkItemAction.Review:
                    return WorkItem.Create(WorkItemAction.Approve);
                case WorkItemAction.Approve:
                    return WorkItem.Create(WorkItemAction.Submit);
                case WorkItemAction.SubmitWithError:
                    return WorkItem.Create(WorkItemAction.ReviewError);
                case WorkItemAction.ReviewError:
                    return WorkItem.Create(WorkItemAction.Generate);
                default:
                    return WorkItem.Create(WorkItemAction.Review);
            }
        }
    }
}
