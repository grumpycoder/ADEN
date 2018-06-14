using ALSDE.Idem;
using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aden.Core.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string AssignedUser { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Notes { get; set; }

        public WorkItemAction WorkItemAction { get; set; }
        public WorkItemState WorkItemState { get; set; }

        public Report Report { get; set; }

        public bool CanCancel
        {
            get
            {
                //TODO: Cleanup
                if (WorkItemState == WorkItemState.Cancelled) return false;

                if (Report.ReportState == ReportState.Complete || Report.ReportState == ReportState.CompleteWithError)
                    return false;

                var items = Report.WorkItems.OrderBy(x => x.Id).ToList();
                var item = items.LastOrDefault();
                if (item == null) return false;

                if (item.WorkItemAction == WorkItemAction.Generate) return false;

                var p = GetPrevious(items, item);

                if (p == null) return false;

                if (p.Id == Id) return true;

                return false;
            }
        }

        private WorkItem() { }

        private WorkItem(WorkItemState state, string assignee)
        {
            AssignedUser = assignee;
            WorkItemState = state;
        }

        public static WorkItem Create(WorkItemAction action, string assignee)
        {
            return new WorkItem(action, assignee);
        }

        public void Finish()
        {
            CompletedDate = DateTime.Now;
            WorkItemState = WorkItemState.Completed;
        }

        public static WorkItemAction Next(WorkItem workItem)
        {
            switch (workItem.WorkItemAction)
            {
                case WorkItemAction.Generate:
                    return WorkItemAction.Review;
                case WorkItemAction.Review:
                    return WorkItemAction.Approve;
                case WorkItemAction.Approve:
                    return WorkItemAction.Submit;
                case WorkItemAction.SubmitWithError:
                    return WorkItemAction.ReviewError;
                case WorkItemAction.ReviewError:
                    return WorkItemAction.Generate;
                case WorkItemAction.Submit:
                    return WorkItemAction.Nothing;
                default:
                    return WorkItemAction.Nothing;
            }

        }

        public void Cancel()
        {
            WorkItemState = WorkItemState.Cancelled;
            CompletedDate = DateTime.Now;
        }
        public void Reassign(string assignee)
        {
            AssignedUser = assignee;
        }

        //REFACTOR BELOW 


        private void CreateNextWorkItem(WorkItemAction currentWorkItem)
        {
            WorkItem wi;

            //TODO: Refactor to factory method
            //switch (currentWorkItem)
            //{
            //    case WorkItemAction.Generate:
            //        Report.GeneratedDate = DateTime.Now;
            //        Report.GeneratedUser = AssignedUser;
            //        Report.ReportState = ReportState.AssignedForReview;
            //        Report.Submission.SubmissionState = SubmissionState.AssignedForReview;
            //        wi = WorkItem.Create(WorkItemAction.Review, Report.Submission.FileSpecification.GenerationUserGroup);
            //        Report.AddWorkItem(wi);
            //        break;
            //    case WorkItemAction.Review:
            //        Report.ReportState = ReportState.AwaitingApproval;
            //        Report.Submission.SubmissionState = SubmissionState.AwaitingApproval;
            //        wi = WorkItem.Create(WorkItemAction.Approve, Report.Submission.FileSpecification.ApprovalUserGroup);
            //        Report.AddWorkItem(wi);
            //        break;
            //    case WorkItemAction.Approve:
            //        Report.ApprovedDate = DateTime.Now;
            //        Report.ApprovedUser = AssignedUser;
            //        Report.ReportState = ReportState.AssignedForSubmission;
            //        Report.Submission.SubmissionState = SubmissionState.AssignedForSubmission;
            //        wi = WorkItem.Create(WorkItemAction.Submit, Report.Submission.FileSpecification.SubmissionUserGroup);
            //        Report.AddWorkItem(wi);
            //        break;
            //    case WorkItemAction.SubmitWithError:
            //        Report.SubmittedDate = DateTime.Now;
            //        Report.SubmittedUser = AssignedUser;
            //        Report.ReportState = ReportState.CompleteWithError;
            //        Report.Submission.SubmissionState = SubmissionState.CompleteWithError;
            //        wi = WorkItem.Create(WorkItemAction.ReviewError, Report.Submission.FileSpecification.ApprovalUserGroup);
            //        Report.AddWorkItem(wi);
            //        break;
            //    case WorkItemAction.Submit:
            //        Report.SubmittedDate = DateTime.Now;
            //        Report.SubmittedUser = AssignedUser;
            //        Report.ReportState = ReportState.Complete;
            //        Report.Submission.SubmissionState = SubmissionState.Complete;
            //        break;
            //    case WorkItemAction.ReviewError:
            //        Report.StartNewWork();
            //        break;
            //}
        }

        private WorkItem(WorkItemAction action, string assignee)
        {
            WorkItemAction = action;
            AssignedUser = assignee;
            AssignedDate = DateTime.Now;
            WorkItemState = WorkItemState.NotStarted;
        }


        //public static Result<WorkItem> Create(WorkItemAction action, string assignee)
        //{
        //    if (string.IsNullOrWhiteSpace(assignee)) return Result.Fail<WorkItem>("Assignee should not be empty");

        //    var workItem = new WorkItem(action, assignee);

        //    return Result.Ok(workItem);
        //}

        //public static Result<WorkItem> Create(WorkItemAction action, string groupName)
        //{
        //    if (string.IsNullOrWhiteSpace(groupName)) return Result.Fail<WorkItem>("Assignment Group should not be empty");

        //    var memberOrError = GetAssigneeFromGroup(groupName);

        //    if (memberOrError.IsFailure) return Result.Fail<WorkItem>(memberOrError.Value);

        //    var workItem = new WorkItem(action, memberOrError.Value);

        //    return Result.Ok(workItem);
        //}

        private static Result<string> GetAssigneeFromGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName)) return Result.Fail<string>("Group Name should not be empty");

            var groupMembers = GroupHelper.GetGroupMembers(groupName);

            if (groupMembers == null) return Result.Fail<string>($"No members defined for group {groupName}");

            var members = groupMembers.Select(m => m.EmailAddress).ToList();

            //var assignmentService = new AssignmentService();
            //Result<string> assigneeOrError = assignmentService.GetAssigneeFromMemberList(members);

            //if (assigneeOrError.IsFailure) return Result.Fail<string>(assigneeOrError.Value);

            //return Result.Ok(assigneeOrError.Value);
            return Result.Ok<string>("AssigneeTest");
        }

        //public static WorkItem Create(WorkItemAction action, string assignment, bool isIndividual = false)
        //{

        //try
        //{

        //    var assignee = string.Empty;
        //    var members = new List<string>();

        //    if (isIndividual) assignee = assignment;
        //    if (!isIndividual)
        //    {
        //        var groupMembers = GroupHelper.GetGroupMembers(assignment);

        //        if (groupMembers == null) throw new Exception(string.Format("No group {0} defined or no members assigned", assignment));

        //        members = groupMembers.Select(m => m.EmailAddress).ToList();
        //    }

        //    //var workItem = new WorkItem();

        //    ////TODO: This doesn't belong here. Coupled to data source. Should not reference uow or group helper

        //    //assignee = isIndividual ? assignee : _uow.WorkItems.GetUserWithLeastAssignments(members);

        //    var wi = new WorkItem(action, assignee);
        //    return wi;
        //}
        //catch (ArgumentNullException e)
        //{
        //    throw new Exception(e.Message, e);
        //}
        //catch (Exception ex)
        //{
        //    if (ex.InnerException.Message.Contains("Login failed"))
        //    {
        //        throw new Exception($"User unable to connect to database. ", ex);
        //    }
        //    throw new Exception($"{assignment} Group not defined or no members assigned. ", ex);
        //}

        //}



        private static T GetNext<T>(IEnumerable<T> list, T current)
        {
            try
            {
                return list.SkipWhile(x => !x.Equals(current)).Skip(1).First();
            }
            catch
            {
                return default(T);
            }
        }

        private static T GetPrevious<T>(IEnumerable<T> list, T current)
        {
            try
            {
                return list.TakeWhile(x => !x.Equals(current)).Last();
            }
            catch
            {
                return default(T);
            }
        }

        public void SetAction(WorkItemAction action)
        {
            WorkItemAction = action;
        }


    }
}
