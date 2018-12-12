using Aden.Core.Data;
using Aden.Core.Dtos;
using Aden.Core.Models;
using Aden.Core.Repositories;
using Aden.Core.Services;
using Aden.Web.Filters;
using Aden.Web.ViewModels;
using Alsde.Extensions;
using ALSDE.Idem;
using ALSDE.Idem.Web.UI.AimBanner;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EAGetMail;
using Humanizer;
using System;
//using Independentsoft.Email.Mime;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Z.EntityFramework.Plus;


namespace Aden.Web.Controllers
{
    //[Authorize]
    [CustomAuthorize(Roles = "AdenAppUsers")]
    public class HomeController : AsyncController
    {
        private readonly AdenContext _context;
        private readonly IUnitOfWork _uow;
        private readonly IMembershipService _membership;

        public HomeController(AdenContext context, IUnitOfWork uow, IMembershipService membership)
        {
            _context = context;
            _uow = uow;
            _membership = membership;
        }

        [TrackViewName]
        [CustomAuthorize(Roles = Constants.FileSpecificationAdministratorGroup)]
        public ActionResult FileSpecifications()
        {
            return View();
        }

        [TrackViewName]
        public ActionResult Submissions()
        {
            return View();
        }

        [TrackViewName]
        public async Task<ActionResult> Reports(int datayear, string filenumber)
        {

            var dto = await _context.Reports
                .Where(f => (f.Submission.FileSpecification.FileNumber == filenumber && f.Submission.DataYear == datayear) || string.IsNullOrEmpty(filenumber))
                .Select(m =>
                    new ReportViewDto
                    {
                        Id = m.Id,
                        FileName = m.Submission.FileSpecification.FileName,
                        FileNumber = m.Submission.FileSpecification.FileNumber,
                        DataYear = m.Submission.FileSpecification.DataYear,
                        ReportState = m.ReportState,
                        ApprovedDate = m.ApprovedDate,
                        GeneratedDate = m.GeneratedDate,
                        SubmittedDate = m.SubmittedDate,
                        Documents = m.Documents.Select(d => new ReportDocumentViewDto()
                        {
                            Id = d.Id,
                            Filename = d.Filename,
                            Version = d.Version,
                            FileSize = d.FileSize,
                        }).ToList()
                    }
                )
                .ToListAsync();

            foreach (ReportViewDto item in dto)
            {
                item.Documents.ForEach(x =>
                {
                    x.FileSizeInMb = x.FileSize.ToFileSize();
                    x.FileSizeMb = x.FileSize / 1024;
                    x.FileSizeMb = x.FileSize.ConvertBytesToMega();
                });
            }

            return View(dto);
        }

        [TrackViewName]
        public ActionResult Assignments(string view, string username)
        {
            return View((object)User.Identity.Name);
        }

        public async Task<ActionResult> WorkHistory(int submissionId)
        {
            var isAdministrator = ((ClaimsPrincipal)User).Claims.Any(c => c.Value.ToLower().Contains(Constants.GlobalAdministratorGroup.ToLower()));

            ViewBag.IsSectionAdmin = isAdministrator;

            var dto = new SubmissionWorkHistoryViewDto()
            {
                WorkItemHistory = new List<WorkItemHistoryDto>(),
                SubmissionAudits = new List<SubmissionAudit>()
            };

            var report = _context.Reports.OrderByDescending(r => r.Id).FirstOrDefault(x => x.SubmissionId == submissionId);

            if (report == null) return PartialView("_WorkHistory", dto);

            dto.WorkItemHistory = _context.WorkItems.OrderByDescending(o => o.Id).Where(w => w.ReportId == report.Id)
                .ProjectTo<WorkItemHistoryDto>().Future().ToList();
            dto.SubmissionAudits = _context.SubmissionAudits.OrderByDescending(x => x.Id).Where(a => a.SubmissionId == submissionId).Future().ToList();

            return PartialView("_WorkHistory", dto);

        }

        public async Task<ActionResult> WorkItemImages(int workItemId)
        {
            var wi = await _context.WorkItems.Include(x => x.WorkItemImages).FirstOrDefaultAsync(x => x.Id == workItemId);

            return PartialView("_WorkItemImage", wi);
        }

        public async Task<ActionResult> Reassign(int workItemId)
        {
            var workItem = await _uow.WorkItems.GetByIdAsync(workItemId);

            var dto = Mapper.Map<ReassignmentDto>(workItem);

            return PartialView("_WorkItemReassignment", dto);
        }

        public async Task<ActionResult> Document(int id)
        {
            //var connectionString =
            //ConfigurationManager.ConnectionStrings["AdenContext"].ConnectionString;

            //var selectStatement = "SELECT [id], [filedata] FROM [Aden].[ReportDocuments] WHERE Id = " + id;

            //using (var con = new SqlConnection(connectionString))
            //{
            //    await con.OpenAsync();
            //    return await con.QueryAsync<ReportDocumentDto>(selectStatement, commandType: CommandType.Text);
            //}

            //var connectionString =
            //ConfigurationManager.ConnectionStrings["AdenContext"].ConnectionString;

            //var selectStatement = "SELECT [id], [filedata] FROM [Aden].[ReportDocuments] WHERE Id = " + id; 

            //var asyncConnectionString = new SqlConnectionStringBuilder(connectionString)
            //{
            //    AsynchronousProcessing = true
            //}.ToString();

            //using (var conn = new SqlConnection(asyncConnectionString))
            //{
            //    using (var cmd = new SqlCommand())
            //    {

            //        cmd.Connection = conn;
            //        cmd.CommandText = selectStatement;
            //        cmd.CommandType = CommandType.Text;

            //        conn.Open();

            //        using (var reader = await cmd.ExecuteReaderAsync())
            //        {

            //            return reader.Select(r => new ReportDocumentDto()
            //            {
            //                FileData = r["FileData"].ToString()
            //            }
            //            ); 
            //        }
            //    }
            //}

            //using (SqlConnection connection = new SqlConnection(_context.Database.Connection.ConnectionString))
            //{
            //    await connection.OpenAsync();
            //    using (SqlCommand command = new SqlCommand("SELECT [id], [filedata] FROM [Aden].[ReportDocuments] WHERE Id = " + id, connection))
            //    {

            //        // The reader needs to be executed with the SequentialAccess behavior to enable network streaming  
            //        // Otherwise ReadAsync will buffer the entire text document into memory which can cause scalability issues or even OutOfMemoryExceptions  
            //        using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
            //        {
            //            while (await reader.ReadAsync())
            //            {
            //                Console.Write("{0}: ", reader.GetInt32(0));

            //                if (await reader.IsDBNullAsync(1))
            //                {
            //                    Console.Write("(NULL)");
            //                }
            //                else
            //                {
            //                    char[] buffer = new char[4096];
            //                    int charsRead = 0;
            //                    using (TextReader data = reader.GetTextReader(1))
            //                    {
            //                        do
            //                        {
            //                            // Grab each chunk of text and write it to the console  
            //                            // If you are writing to a TextWriter you should use WriteAsync or WriteLineAsync  
            //                            charsRead = await data.ReadAsync(buffer, 0, buffer.Length);
            //                            Console.Write(buffer, 0, charsRead);
            //                        } while (charsRead > 0);
            //                    }
            //                }

            //                Console.WriteLine();
            //            }
            //        }
            //    }
            //}


            //var document = await _uow.Documents.GetByIdAsync(id);





            //var dto = await _context.ReportDocuments.Where(d => d.Id == id).Select(r => new ReportDocumentDto()
            //{
            //    Filename = r.Filename,
            //    Version = r.Version,
            //    Id = r.Id, 
            //    FileData = r.FileData

            //}).FirstOrDefaultAsync();

            //dto.Content = Encoding.UTF8.GetString(dto.FileData).ToString();

            ////var dto = Mapper.Map<ReportDocumentDto>(document);

            var dto = new ReportDocumentDto() { Id = id };

            return PartialView(dto);
        }

        public async Task<FileResult> Download(int id)
        {
            var document = await _uow.Documents.GetByIdAsync(id);
            return File(document.FileData, System.Net.Mime.MediaTypeNames.Application.Octet, document.Filename);
        }

        public ActionResult EditFileSpecification(int id)
        {
            var spec = _uow.FileSpecifications.GetById(id);

            var dataGroups = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = "Data", Text = "Data"},
                new SelectListItem(){ Value = "Development", Text = "Development"},
                new SelectListItem(){ Value = "Section", Text = "Section"}
            };

            var collections = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = "Accumulator", Text = "Accumulator"},
                new SelectListItem(){ Value = "Assessment", Text = "Assessment"},
                new SelectListItem(){ Value = "AnnualDataReport", Text = "Annual Data Report"},
                new SelectListItem(){ Value = "Application", Text = "Application"},
                new SelectListItem(){ Value = "ChildCount", Text = "Child Count"},
                new SelectListItem(){ Value = "Financials", Text = "Financials"},
                new SelectListItem(){ Value = "EOY-9thMonth", Text = "EOY - 9th Month"},
                new SelectListItem(){ Value = "Fall-20Day", Text = "Fall-20 Day"},
                new SelectListItem(){ Value = "Manual", Text = "Manual"},
                new SelectListItem(){ Value = "SIR", Text = "SIR"},
                new SelectListItem(){ Value = "Schedules", Text = "Schedules"}
            };

            ViewBag.GenerationGroupMemberCount = GroupHelper.GetGroupMembers(spec.GenerationUserGroup)?.Count ?? 0;
            ViewBag.ApprovalGroupMemberCount = GroupHelper.GetGroupMembers(spec.ApprovalUserGroup)?.Count ?? 0;
            ViewBag.Applications = IdemApplications.Applications.ConvertAll(a => new SelectListItem() { Text = a.Title, Value = a.Title });
            ViewBag.DataGroups = dataGroups;
            ViewBag.Collections = collections;

            var model = Mapper.Map<UpdateFileSpecificationDto>(spec);
            return PartialView("_FileSpecificationForm", model);

        }

        public ActionResult EditGroupMembership(int id, string groupName)
        {
            var displayGroupName = groupName.Humanize().ToTitleCase().RemoveExactWord("App");
            ViewBag.GroupName = groupName;
            ViewBag.DisplayGroupName = displayGroupName;
            ViewBag.IsGroupDefined = true;

            var groupExists = _membership.GroupExists(groupName);

            if (!groupExists)
            {
                ViewBag.IsGroupDefined = false;
                return PartialView("_GroupMembershipForm");
            }

            var membersResult = _membership.GetGroupMembers(groupName);

            if (membersResult.IsSuccess) ViewBag.Members = membersResult.Value;


            if (membersResult.IsFailure) ViewBag.IsGroupDefined = false;

            return PartialView("_GroupMembershipForm");
        }

        public async Task<ActionResult> ErrorReport(int id)
        {
            var wi = await _uow.WorkItems.GetByIdAsync(id);

            var model = Mapper.Map<WorkItemDto>(wi);
            return PartialView("_ErrorReportForm", model);
        }

        public async Task<ActionResult> UploadReport(int id)
        {
            var wi = await _uow.WorkItems.GetByIdAsync(id);

            var model = Mapper.Map<ReportUploadDto>(wi);
            return PartialView("_ReportUploadForm", model);
        }

        public ActionResult Error(string message)
        {
            return View();
        }

        public ActionResult Audit(int id)
        {
            var audit = new SubmissionAuditEntryDto() { SubmissionId = id };
            return PartialView("_SubmissionAuditEntry", audit);
        }

        public ActionResult ReopenAudit(int id)
        {
            var audit = new SubmissionReopenAuditEntryDto() { SubmissionId = id };
            return PartialView("_SubmissionReopenAuditEntry", audit);
        }

        [TrackViewName]
        public ActionResult Mail()
        {
            var vm = new List<MailViewModel>();
            var path = HostingEnvironment.MapPath(@"/App_Data");
            foreach (var file in Directory.GetFiles($@"{path}", "*.eml"))
            {

                var msg = new Mail("TryIt");
                msg.Load(file, false);
                vm.Add(new MailViewModel()
                {
                    Id = Path.GetFileNameWithoutExtension(file),
                    Sent = msg.ReceivedDate,
                    To = msg.To.Select(s => s.Address.ToString()),
                    CC = msg.Cc.Select(s => s.Address.ToString()),

                    From = msg.From.Address,
                    Subject = msg.Subject.Replace("(Trial Version)", ""),
                    Body = msg.HtmlBody,
                    Attachments = msg.Attachments.ToList()
                });
            }

            return View(vm.OrderByDescending(x => x.Sent));
        }

    }


    public static class Extensions
    {
        public static string ToFileSize(this int source)
        {
            return ToFileSize(Convert.ToInt64(source));
        }

        public static string ToFileSize(this long source)
        {
            const int byteConversion = 1024;
            double bytes = Convert.ToDouble(source);

            if (bytes >= Math.Pow(byteConversion, 3)) //GB Range
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 3), 2), " GB");
            }
            else if (bytes >= Math.Pow(byteConversion, 2)) //MB Range
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 2), 2), " MB");
            }
            else if (bytes >= byteConversion) //KB Range
            {
                return string.Concat(Math.Round(bytes / byteConversion, 2), " KB");
            }
            else //Bytes
            {
                return string.Concat(bytes, " Bytes");
            }
        }



        public static double ConvertBytesToMegabytes(this long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        public static long ConvertBytesToMega(this long bytes)
        {
            return Convert.ToInt64((bytes / 1024f) / 1024f);
        }

        public static IEnumerable<T> Select<T>(
            this SqlDataReader reader, Func<SqlDataReader, T> projection)
        {

            while (reader.Read())
            {
                yield return projection(reader);
            }
        }
    }

}