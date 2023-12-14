using BugOut.Data;
using BugOut.Models;
using BugOut.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace BugOut.Services
{
    public class AppNotificationService : IAppNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IAppRolesService _rolesService;

        public AppNotificationService(ApplicationDbContext context, IEmailSender emailSender, IAppRolesService rolesService)
        {
            _context = context;
            _emailSender = emailSender;
            _rolesService = rolesService;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            try
            {
                await _context.AddAsync(notification);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications
                                                                 .Include(n => n.Recipient)
                                                                 .Include(n => n.Sender)
                                                                 .Include(n => n.Ticket)
                                                                    .ThenInclude(t => t.Project)
                                                                 .Where(n => n.RecipientId == userId).ToListAsync();
                return notifications;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            try
            {
                List<Notification> notifications = await _context.Notifications
                                                                 .Include(n => n.Recipient)
                                                                 .Include(n => n.Sender)
                                                                 .Include(n => n.Ticket)
                                                                    .ThenInclude(t => t.Project)
                                                                 .Where(n => n.SenderId == userId).ToListAsync();
                return notifications;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject)
        {
            AppUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == notification.RecipientId);

            if(user != null)
            {
                string userEmail = user.Email;
                string message = notification.Message;

                //Send Email
                try
                {
                    await _emailSender.SendEmailAsync(userEmail, emailSubject, message);
                    return true;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role)
        {
            try
            {
                List<AppUser> members = await _rolesService.GetUsersInRoleAsync(role, companyId);

                foreach(AppUser user in members)
                {
                    notification.RecipientId = user.Id;

                    await SendEmailNotificationAsync (notification, notification.Title);            
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SendMembersEmailNotificationsAsync(Notification notification, List<AppUser> members)
        {
            try
            {
                foreach (AppUser user in members)
                {
                    notification.RecipientId = user.Id;

                    await SendEmailNotificationAsync(notification, notification.Title);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
