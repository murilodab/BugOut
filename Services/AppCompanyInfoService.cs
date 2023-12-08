using BugOut.Data;
using BugOut.Models;
using BugOut.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            List<Ticket> tickets = await _context.Tickets.Where(t => t.Project.CompanyId == companyId).ToListAsync();

            return tickets;
        }

        public Task<Company> GetCompanyInfoByIdAsync(int? companyId)
        {

        }
    }
}
