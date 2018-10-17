using Aden.Core.Helpers;
using Aden.Core.Models;
using ALSDE.Idem;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;


namespace Aden.Core.Services
{
    public class EmailNotificationService : INotificationService
    {

        private const string EmailTemplateResource = "Aden.Core.EmailTemplate.html";
        public string Url
        {
            get
            {
                var env = Constants.CurrentEnvironment != "Production" ? Constants.CurrentEnvironment : string.Empty;
                return $"https://{env.ToLower()}aden.alsde.edu/assignments";
            }
        }

        public string Template
        {
            get
            {
                return File.ReadAllText("EmailTemplate.html");
            }
        }

        public void SendWorkNotification(WorkItem workItem)
        {
            try
            {
                var heading = "Congratulations!";
                var subject =
                    $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Assignment";
                var title = $"You have been assigned a {workItem.WorkItemAction.GetDisplayName()} task";
                var messageBody =
                    $"A task of {workItem.WorkItemAction.GetDisplayName()} for {workItem.Report.Submission.FileSpecification.FileName} needs to be completed before {workItem.Report.Submission.DueDate.GetValueOrDefault():M/d/yyy}.";
                var additionalNotes = "";
                var assignmentsUrlText = "View Your Assignments";

                var imagePath = @"https://png.icons8.com/ios/50/000000/realtime-protection.png";

                switch (workItem.WorkItemAction)
                {
                    case WorkItemAction.Generate:
                        imagePath = @"https://png.icons8.com/ios/50/000000/graph-report-filled.png";
                        break;
                    case WorkItemAction.Review:
                        imagePath = @"https://png.icons8.com/ios/50/000000/checked-filled.png";
                        break;
                    case WorkItemAction.Approve:
                        imagePath = @"https://png.icons8.com/ios/50/000000/good-quality-filled.png";
                        break;
                    case WorkItemAction.Submit:
                        imagePath = @"https://png.icons8.com/ios/50/000000/upload-to-cloud-filled.png";
                        break;
                    case WorkItemAction.SubmitWithError:
                        imagePath = @"https://png.icons8.com/ios/50/000000/error-filled.png";
                        break;
                    case WorkItemAction.ReviewError:
                        imagePath = @"https://png.icons8.com/ios/50/000000/error-cloud-filled.png";
                        break;
                    case WorkItemAction.Nothing:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }



                var body = GetTemplateResourceBody()
                    .Replace(@"{Heading}", $"{heading}")
                    .Replace(@"{Title}", $"{title}")
                    .Replace(@"{MessageBody}", $"{messageBody}")
                    .Replace(@"{AdditionalNotes}", $"{additionalNotes}")
                    .Replace(@"{AssignmentsUrlText}", $"{assignmentsUrlText}")
                    .Replace(@"{AssignmentsUrl}", $"{Constants.Url}")
                    .Replace(@"{ImagePath}", $"{imagePath}").Replace("..", ".");

                var message = new MailMessage(Constants.ReplyAddress, workItem.AssignedUser)
                {
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    Subject = subject,
                    Body = body
                };
                var client = new SmtpClient();
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

                var subject =
                    $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Assignment Cancelled";
                var title = $"Your assignment of {workItem.WorkItemAction.GetDisplayName()} task has been cancelled.";
                var messageBody =
                    $"A task for {workItem.Report.Submission.FileSpecification.FileName} to be cancelled.";
                var imagePath = @"https://png.icons8.com/ios/50/000000/delete-message-filled.png";


                var body = GetTemplateResourceBody()
                    .Replace(@"{Title}", $"{title}")
                    .Replace(@"{MessageBody}", $"{messageBody}")
                    .Replace(@"{ImagePath}", $"{imagePath}").Replace("..", ".");

                var message = new MailMessage(Constants.ReplyAddress, workItem.AssignedUser)
                {
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    Subject = subject,
                    Body = body
                };
                var client = new SmtpClient();
                client.Send(message);



                //var client = new SmtpClient();
                //var message = new MailMessage(Constants.ReplyAddress, workItem.AssignedUser)
                //{
                //    Subject =
                //        $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Assignment Cancelled",
                //    Body =
                //        $"You're assignment of {workItem.WorkItemAction.GetDisplayName()} task for {workItem.Report.Submission.FileSpecification.FileName} has been cancelled."
                //};

                //client.Send(message);
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
                var subject =
                    $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Assignment Cancelled";
                var title = $"Your assignment of {workItem.WorkItemAction.GetDisplayName()} task has been cancelled.";
                var messageBody =
                    $"A task for {workItem.Report.Submission.FileSpecification.FileName} has been reassigned.";
                var imagePath = @"https://png.icons8.com/ios/50/000000/about-filled.png";


                var body = GetTemplateResourceBody()
                    .Replace(@"{Title}", $"{title}")
                    .Replace(@"{MessageBody}", $"{messageBody}")
                    .Replace(@"{ImagePath}", $"{imagePath}").Replace("..", ".");

                var message = new MailMessage(Constants.ReplyAddress, workItem.AssignedUser)
                {
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    Subject = subject,
                    Body = body
                };
                var client = new SmtpClient();
                client.Send(message);

                //var client = new SmtpClient();
                //var message = new MailMessage(Constants.ReplyAddress, workItem.AssignedUser)
                //{
                //    Subject =
                //        $"{workItem.Report.Submission.FileSpecification.FileName} {workItem.WorkItemAction.GetDisplayName()} Assignment Cancelled",
                //    Body =
                //        $"You're assignment of {workItem.WorkItemAction.GetDisplayName()} task for {workItem.Report.Submission.FileSpecification.FileName} has been reassigned. You can view your assignments at {Url}."
                //};

                //client.Send(message);
            }
            catch (Exception e)
            {
                //TODO: Log sending error and queue message
                Debug.WriteLine("Error sending email message", e);
            }
        }

        public void SendWorkErrorNotification(WorkItem workItem, HttpPostedFileBase[] files = null)
        {
            // Send Generation User email notification
            // Send Global Admins email notification
            // Send Developers email notification
            var globalAdmins = GroupHelper.GetGroupMembers(Constants.GlobalAdministrators).Select(x => x.EmailAddress);


            var heading = "Submission Error!";
            var subject =
                $"ADEN {workItem.Report.Submission.FileSpecification.FileName} ({workItem.Report.Submission.FileSpecification.FileNumber}) File Submission Error";
            var title = $"{workItem.Report.Submission.FileSpecification.FileName} file submission has generated an error";
            var messageBody = $"<p>A submission error has occurred when sending file to EDEN for {workItem.Report.Submission.FileSpecification.FileName} ({workItem.Report.Submission.FileSpecification.FileNumber}). Management and Development have also been notified. </p>";
            var additionalNotes = $"<p><strong>Error Description:</strong> <hr />{workItem.Description}</p>";
            var imagePath = @"https://png.icons8.com/ios/50/000000/error-filled.png";
            var assignmentsUrlText = "View Your Assignments";

            var body = GetTemplateResourceBody()
                .Replace(@"{Heading}", $"{heading}")
                .Replace(@"{Title}", $"{title}")
                .Replace(@"{MessageBody}", $"{messageBody}")
                .Replace(@"{AdditionalNotes}", $"{additionalNotes}")
                .Replace(@"{AssignmentsUrlText}", $"{assignmentsUrlText}")
                .Replace(@"{AssignmentsUrl}", $"{Constants.Url}")
                .Replace(@"{ImagePath}", $"{imagePath}").Replace("..", ".");

            var message = new MailMessage(Constants.ReplyAddress, workItem.AssignedUser)
            {
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                Subject = subject,
                Body = body
            };
            message.CC.Add(Constants.Development);
            foreach (var admin in globalAdmins)
            {
                message.CC.Add(admin);
            }

            if (files != null)
            {
                foreach (var file in files)
                {
                    file.InputStream.Position = 0;
                    message.Attachments.Add(new Attachment(file.InputStream, file.FileName));
                }
            }

            var client = new SmtpClient();
            client.Send(message);
        }

        private static string GetTemplateResourceBody()
        {
            var copyRightFooter = $"Copyright © {DateTime.Now:yyyy} Alabama State Department of Education. All rights reserved.";
            var dateHeader = $"{DateTime.Now:d}";

            string body;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(EmailTemplateResource))
            using (var reader = new StreamReader(stream))
            {
                body = reader.ReadToEnd()
                    .Replace(@"{DateHeader}", $"{dateHeader}")
                    .Replace(@"{CopyRightFooter}", $"{copyRightFooter}")
                    .Replace(@"{AssignmentLinkUrl}", $"{Constants.Url}");
            }

            return body;
        }

    }
}
