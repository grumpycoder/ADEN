using Aden.Core.Repositories;
using ALSDE.Idem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aden.Core.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public int? ReportId { get; set; }
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



        private static IUnitOfWork _uow;
        private WorkItem(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public void Complete()
        {
            CompletedDate = DateTime.Now;
            WorkItemState = WorkItemState.Completed;
            CreateNextWorkItem(WorkItemAction);
        }

        private void CreateNextWorkItem(WorkItemAction currentWorkItem)
        {
            WorkItem wi;

            //TODO: Refactor to factory method
            switch (currentWorkItem)
            {
                case WorkItemAction.Generate:
                    Report.GeneratedDate = DateTime.Now;
                    Report.GeneratedUser = AssignedUser;
                    Report.ReportState = ReportState.AssignedForReview;
                    Report.Submission.SubmissionState = SubmissionState.AssignedForReview;
                    wi = WorkItem.Create(WorkItemAction.Review, Report.Submission.FileSpecification.GenerationUserGroup);
                    Report.AddWorkItem(wi);
                    break;
                case WorkItemAction.Review:
                    Report.ReportState = ReportState.AwaitingApproval;
                    Report.Submission.SubmissionState = SubmissionState.AwaitingApproval;
                    wi = WorkItem.Create(WorkItemAction.Approve, Report.Submission.FileSpecification.ApprovalUserGroup);
                    Report.AddWorkItem(wi);
                    break;
                case WorkItemAction.Approve:
                    Report.ApprovedDate = DateTime.Now;
                    Report.ApprovedUser = AssignedUser;
                    Report.ReportState = ReportState.AssignedForSubmission;
                    Report.Submission.SubmissionState = SubmissionState.AssignedForSubmission;
                    wi = WorkItem.Create(WorkItemAction.Submit, Report.Submission.FileSpecification.SubmissionUserGroup);
                    Report.AddWorkItem(wi);
                    break;
                case WorkItemAction.SubmitWithError:
                    Report.SubmittedDate = DateTime.Now;
                    Report.SubmittedUser = AssignedUser;
                    Report.ReportState = ReportState.CompleteWithError;
                    Report.Submission.SubmissionState = SubmissionState.CompleteWithError;
                    wi = WorkItem.Create(WorkItemAction.ReviewError, Report.Submission.FileSpecification.ApprovalUserGroup);
                    Report.AddWorkItem(wi);
                    break;
                case WorkItemAction.Submit:
                    Report.SubmittedDate = DateTime.Now;
                    Report.SubmittedUser = AssignedUser;
                    Report.ReportState = ReportState.Complete;
                    Report.Submission.SubmissionState = SubmissionState.Complete;
                    break;
                case WorkItemAction.ReviewError:
                    Report.StartNewWork();
                    break;
            }
        }

        //public WorkItem Reassign(string assignedUser)
        //{
        //    var wi = Create(WorkItemAction, assignedUser, true);
        //    wi.Report = Report;
        //    Report.AddWorkItem(wi);

        //    WorkItemState = WorkItemState.Reassigned;

        //    return wi;
        //}

        private WorkItem(WorkItemAction action, string assignee)
        {
            WorkItemAction = action;
            AssignedUser = assignee;
            AssignedDate = DateTime.Now;
            WorkItemState = WorkItemState.NotStarted;
        }

        public static WorkItem Create(WorkItemAction action, string assignment, bool isIndividual = false)
        {
            try
            {

                var assignee = string.Empty;

                if (isIndividual)
                {
                    assignee = assignment;
                }
                else
                {
                    var groupMembers = GroupHelper.GetGroupMembers(assignment);

                    if (groupMembers == null) throw new Exception(string.Format("No group {0} defined or no members assigned", assignment));

                    var members = groupMembers.Select(m => m.EmailAddress).ToList();
                    //TODO: This doesn't belong here. Coupled to data source. Should not reference uow
                    assignee = _uow.WorkItems.GetUserWithLeastAssignments(members);
                }

                var wi = new WorkItem(action, assignee);
                return wi;
            }
            catch (ArgumentNullException e)
            {
                throw new Exception(e.Message, e);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("Login failed"))
                {
                    throw new Exception($"User unable to connect to database. ", ex);
                }
                throw new Exception($"{assignment} Group not defined or no members assigned. ", ex);
            }

        }

        public void Cancel()
        {
            WorkItemState = WorkItemState.Cancelled;
            CompletedDate = DateTime.Now;
        }


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
