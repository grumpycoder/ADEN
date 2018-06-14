using Aden.Core.Models;
using System.Web;

namespace Aden.Core.Services
{
    public interface INotificationService
    {
        void SendWorkNotification(WorkItem workItem);
        void SendWorkCancelNotification(WorkItem workItem);
        void SendWorkReassignmentNotification(WorkItem workItem);
        void SendWorkErrorNotification(WorkItem workItem, HttpPostedFileBase[] files);
    }
}
