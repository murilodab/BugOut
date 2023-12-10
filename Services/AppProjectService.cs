using BugOut.Data;
using BugOut.Models;
using BugOut.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugOut.Services
{
    public class AppProjectService : IAppProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AppProjectService(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AddNewProjectAsync(Project project)
        {
            _context.Add(project);
            await _context.SaveChangesAsync();
        }

        public Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task ArchiveProjectAsync(Project project)
        {
            project.Archived = true;
            _context.Update(project);
            await _context.SaveChangesAsync();
        }

        public Task<List<AppUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            throw new NotImplementedException();
        }

        public Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AppUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project project = await _context.Projects
                                                    .Include(p => p.Tickets)
                                                    .Include(p=> p.Members)
                                                    .Include(p => p.ProjectPriority)
                                                    .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);   

            return project;
        }

        public Task<AppUser> GetProjectManagerASync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AppUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            throw new NotImplementedException();
        }

        public Task<List<AppUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Project>> GettAllProjectsByCompany(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AppUser>> GetUsersNotOnProjectAsync(int projectId, int comopanyId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserOnProject(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<int> LookupProjectPriorityId(string priorityName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }
    }
}
