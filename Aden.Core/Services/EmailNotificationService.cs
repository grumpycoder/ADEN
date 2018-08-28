using Aden.Core.Helpers;
using Aden.Core.Models;
using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Web;

namespace Aden.Core.Services
{
    public class EmailNotificationService : INotificationService
    {

        public string Url
        {
            get
            {
                var env = Constants.CurrentEnvironment != "Production" ? Constants.CurrentEnvironment : string.Empty;
                return $"https://{env.ToLower()}aden.alsde.edu/assignments";
            }
        }

        public void SendWorkNotification(WorkItem workItem)
        {
            var s = AppSettings.Get<string>("ASPNET_ENV");
            try
            {
                var client = new SmtpClient();
                var message = new MailMessage(Constants.ReplyAddress, workItem.AssignedUser)
                {
                    Subject =
                        $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Assignment",
                    Body = $"You have been assigned a {workItem.WorkItemAction.GetDisplayName()} task for {workItem.Report.Submission.FileSpecification.FileName} to be completed by {workItem.Report.Submission.DueDate}. You can view your assignments at {Url}."
                };

                client.Send(message);
            }
            catch (Exception e)
            {
                //TODO: Log sending error and queue message
                Debug.WriteLine("Error sending email message", e);
            }
        }

        public void SendWorkCancelNotification(WorkItem workItem)
        {
            try
            {
                var client = new SmtpClient();
                var message = new MailMessage(Constants.ReplyAddress, workItem.AssignedUser)
                {
                    Subject =
                        $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Assignment Cancelled",
                    Body =
                        $"You're assignment of {workItem.WorkItemAction.GetDisplayName()} task for {workItem.Report.Submission.FileSpecification.FileName} has been cancelled."
                };

                client.Send(message);
            }
            catch (Exception e)
            {
                //TODO: Log sending error and queue message
                Debug.WriteLine("Error sending email message", e);
            }
        }

        public void SendWorkReassignmentNotification(WorkItem workItem)
        {
            try
            {
                var client = new SmtpClient();
                var message = new MailMessage(Constants.ReplyAddress, workItem.AssignedUser)
                {
                    Subject =
                        $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Assignment Cancelled",
                    Body =
                        $"You're assignment of {workItem.WorkItemAction.GetDisplayName()} task for {workItem.Report.Submission.FileSpecification.FileName} has been reassigned. You can view your assignments at {Url}."
                };

                client.Send(message);
            }
            catch (Exception e)
            {
                //TODO: Log sending error and queue message
                Debug.WriteLine("Error sending email message", e);
            }
        }

        //public void SendWorkErrorNotification(WorkItem workItem, List<byte[]> files = null)
        public void SendWorkErrorNotification(WorkItem workItem, HttpPostedFileBase[] files = null)
        {
            try
            {
                var client = new SmtpClient();
                var message = new MailMessage(Constants.ReplyAddress, workItem.AssignedUser)
                {
                    Subject =
                        $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Submission Error",
                    Body =
                        $"{workItem.Report.Submission.FileSpecification.FileName} submission has generated an error. {Environment.NewLine}" +
                        $"Notes: {Environment.NewLine} {workItem.Notes}"
                };

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        message.Attachments.Add(new Attachment(file.InputStream, file.FileName));
                    }
                }

                client.Send(message);
            }
            catch (Exception e)
            {
                //TODO: Log sending error and queue message
                Debug.WriteLine("Error sending email message", e);
            }

            //Send email to helpdesk
            try
            {
                var client = new SmtpClient();
                var bodyText = $"Place below text into ticket and attach any images and assign to IS Programmers. Thanks {Environment.NewLine}";
                var line = new string('-', 25);
                bodyText += $"{line}{Environment.NewLine}";

                bodyText +=
                    $"{workItem.Report.Submission.FileSpecification.FileName} submission has generated an error. {Environment.NewLine}";
                bodyText += $"Notes: {Environment.NewLine} {workItem.Notes}";

                var message = new MailMessage(Constants.ReplyAddress, Constants.HelpDeskEmail)
                {
                    Subject =
                        $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Submission Error",
                    Body = bodyText
                };

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        message.Attachments.Add(new Attachment(file.InputStream, file.FileName));
                    }
                }

                client.Send(message);
            }
            catch (Exception e)
            {
                //TODO: Log sending error and queue message
                Debug.WriteLine("Error sending email message", e);
            }

        }
    }
}
