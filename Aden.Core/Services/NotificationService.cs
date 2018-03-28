using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using Aden.Core.Helpers;
using Aden.Core.Models;

namespace Aden.Core.Services
{
    public class NotificationService
    {
        public NotificationService()
        {

        }

        public static void SendWorkNotification(WorkItem workItem)
        {
            var client = new SmtpClient();
            var message = new MailMessage("noreplay@alsde.edu", workItem.AssignedUser);
            message.Subject = string.Format("{0} {1} Assigned", workItem.Report.Submission.FileSpecification.FileName, workItem.WorkItemAction.GetDisplayName());
            var bodyText = string.Format("You have been assigned a {0} task for {1} to be completed by {2}", workItem.WorkItemAction.GetDisplayName(), workItem.Report.Submission.FileSpecification.FileName, workItem.Report.Submission.DueDate);

            message.Body = bodyText;
            try
            {
                client.Send(message);
            }
            catch (Exception e)
            {
                //TODO: Log sending error and queue message
                Debug.WriteLine("Error sending email message", e);
            }
        }

        public static void SendCancelWorkNotification(WorkItem workItem)
        {
            var client = new SmtpClient();
            var message = new MailMessage("noreplay@alsde.edu", workItem.AssignedUser);

            message.Subject = string.Format("{0} {1} Assignment cancelled", workItem.Report.Submission.FileSpecification.FileName, workItem.WorkItemAction.GetDisplayName());
            var bodyText = string.Format("You're assignment of {0} task for {1} has been cancelled", workItem.WorkItemAction.GetDisplayName(), workItem.Report.Submission.FileSpecification.FileName);

            message.Body = bodyText;

            try
            {
                client.Send(message);
            }
            catch (Exception e)
            {
                //TODO: Log sending error and queue message
                Debug.WriteLine("Error sending email message", e);
            }

        }

        public static void SendWorkItemError(WorkItem workItem, string notes, string filePath)
        {
            using (var client = new SmtpClient())
            {
                using (var message = new MailMessage("noreplay@alsde.edu", workItem.AssignedUser))
                {
                    message.Subject = string.Format("{0} {1} Submission Error", workItem.Report.Submission.FileSpecification.FileName, workItem.WorkItemAction.GetDisplayName());
                    var bodyText = string.Format("{0} submission has generated an error. {1}", workItem.Report.Submission.FileSpecification.FileName, Environment.NewLine);
                    bodyText += string.Format("Notes: {0} {1}", Environment.NewLine, notes);
                    message.Body = bodyText;

                    foreach (var f in Directory.GetFiles(filePath))
                    {
                        message.Attachments.Add(new Attachment(f));
                    }

                    try
                    {
                        client.Send(message);
                    }
                    catch (Exception e)
                    {
                        //TODO: Log sending error and queue message
                        Debug.WriteLine("Error sending email message", e);
                    }
                }
            }

            using (var client = new SmtpClient())
            {
                using (var message = new MailMessage("noreplay@alsde.edu", workItem.AssignedUser))
                {
                    message.Subject = string.Format("{0} {1} Submission Error", workItem.Report.Submission.FileSpecification.FileName, workItem.WorkItemAction.GetDisplayName());

                    var bodyText = string.Format("Place below text into ticket and attach any images. Thanks {0}", Environment.NewLine);
                    var line = new String('-', 25);
                    bodyText += string.Format("{0}{1}", line, Environment.NewLine);

                    bodyText += string.Format("{0} submission has generated an error. {1}", workItem.Report.Submission.FileSpecification.FileName, Environment.NewLine);
                    bodyText += string.Format("Notes: {0} {1}", Environment.NewLine, notes);
                    message.Body = bodyText;

                    foreach (var f in Directory.GetFiles(filePath))
                    {
                        message.Attachments.Add(new Attachment(f));
                    }

                    try
                    {
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
    }
}