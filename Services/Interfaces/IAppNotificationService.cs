using BugOut.Models;

namespace BugOut.Services.Interfaces
{
    public interface IAppNotificationService
    {
        public Task AddNotificationAsync(Notification notification);
        public Task<List<Notification>> GetReceivedNotificationsAsync(string userId);
        public Task<List<Notification>> GetSentNotificationsAsync(string userId);
        public Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role);
        public Task SendMembersEmailNotificationsAsync(Notification notification, List<AppUser> members);
        public Task SendEmailNotificationsAsync(Notification notification, string emailSubject);
    }
}
