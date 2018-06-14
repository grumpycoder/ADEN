using Aden.Core.Helpers;
using Aden.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using System.Web;

namespace Aden.Core.Services
{
    public class EmailNotificationService : INotificationService
    {
        private const string HelpDeskEmail = "HelpDeskEmail@alsde.edu";
        private const string ReplyAddress = "noreply@alsde.edu";

        public void SendWorkNotification(WorkItem workItem)
        {
            try
            {
                var client = new SmtpClient();
                var message = new MailMessage(ReplyAddress, workItem.AssignedUser)
                {
                    Subject =
                        $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Assignment",
                    Body = $"You have been assigned a {workItem.WorkItemAction.GetDisplayName()} task for {workItem.Report.Submission.FileSpecification.FileName} to be completed by {workItem.Report.Submission.DueDate}"
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
                var message = new MailMessage(ReplyAddress, workItem.AssignedUser)
                {
                    Subject =
                        $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Assignment Cancelled",
                    Body =
                        $"You're assignment of {workItem.WorkItemAction.GetDisplayName()} task for {workItem.Report.Submission.FileSpecification.FileName} has been cancelled"
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
                var message = new MailMessage(ReplyAddress, workItem.AssignedUser)
                {
                    Subject =
                        $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Assignment Cancelled",
                    Body =
                        $"You're assignment of {workItem.WorkItemAction.GetDisplayName()} task for {workItem.Report.Submission.FileSpecification.FileName} has been ressigned"
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
                var message = new MailMessage(ReplyAddress, workItem.AssignedUser)
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

                var message = new MailMessage(ReplyAddress, HelpDeskEmail)
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
