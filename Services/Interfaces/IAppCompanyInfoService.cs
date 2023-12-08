using BugOut.Models;

namespace BugOut.Services.Interfaces
{
    public interface IAppCompanyInfoService
    {
        public Task<Company> GetCompanyInfoByIdAsync (int? companyId);
        public Task<List<AppUser>> GetAllMembersAsync(int? companyId);
        public Task<List<Project>> GetAllProjectsAsync(int? companyId);
        public Task<List<Ticket>> GetAllTicketsAsync(int? companyId);

    }
}
