using BugOut.Data;
using BugOut.Models;
using BugOut.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BugOut.Services
{
    public class AppCompanyInfoService : IAppCompanyInfoService
    {
        private readonly ApplicationDbContext _context;

        public AppCompanyInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AppUser>> GetAllMembersAsync(int? companyId)
        {
            List<AppUser> members = await _context.Users.Where(m => m.CompanyId == companyId).ToListAsync();

            return members;
        }

        public async Task<List<Project>> GetAllProjectsAsync(int? companyId)
        {
            List<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                            .Include(p => p.Members)
                                                            .Include(p => p.Tickets)
                                                                .ThenInclude(p => p.Comments)
                                                            .Include(p => p.Tickets)
                                                                .ThenInclude(p => p.Attachments)
                                                            .Include(p => p.Tickets)
                                                                .ThenInclude(p => p.History)
                                                            .Include(p => p.Tickets)
                                                                .ThenInclude(p => p.DeveloperUser)
                                                            .Include(p => p.Tickets)
                                                                .ThenInclude(p => p.OwnerUser)
                                                            .Include(p => p.Tickets)
                                                                .ThenInclude(p => p.Notifications)
                                                            .Include(p => p.Tickets)
                                                                .ThenInclude(p => p.TicketStatus)
                                                             .Include(p => p.Tickets)
                                                                .ThenInclude(p => p.TicketPriority)
                                                             .Include(p => p.Tickets)
                                                                .ThenInclude(p => p.TicketType)
                                                            .Include(p => p.ProjectPriority)
                                                            .ToListAsync();

            return projects;
        }

        public async Task<List<Ticket>> GetAllTicketsAsync(int? companyId)
        {
            List<Ticket> tickets = new();

            List<Project> projects = await GetAllProjectsAsync(companyId);

            tickets = projects.SelectMany(p => p.Tickets).ToList();

            return tickets;
        }

        public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {
            Company companyInfo = new();

            if (companyId != null)
            {
                companyInfo = await _context.Companies
                    .Include(c => c.Members)
                    .Include(c => c.Projects)
                    .Include(c => c.Invites)
                    .FirstOrDefaultAsync(c => c.Id == companyId);
            }

            return companyInfo;
        }
    }
}
