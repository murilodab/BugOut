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

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            AppUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if(user != null)
            {
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                if(!await IsUserOnProjectAsync(userId, projectId))
                {
                    try
                    {
                        project.Members.Add(user);
                        await _context.SaveChangesAsync();
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
            else
            {
                return false;
            }
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

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> projects = await GetAllProjectsByCompany(companyId);

            int priorityId = await LookupProjectPriorityId(priorityName);

            return projects.Where(p => p.ProjectPriorityId == priorityId).ToList();
        }

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            List<Project> projects = await GetAllProjectsByCompany(companyId);

            return projects.Where(p => p.Archived == true).ToList();

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

        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            List<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == false)
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

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users
                    .Include(u => u.Projects)
                        .ThenInclude(p => p.Company)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.DeveloperUser)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.OwnerUser)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t=> t.TicketPriority)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.TicketStatus)
                    .Include(u => u.Projects)
                        .ThenInclude(t => t.Tickets)
                            .ThenInclude(t => t.TicketType)
                    .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();

                return userProjects;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** ERROR ***** - Failed to Get User Projects. ---> {ex.Message}");
                throw;
            }
        }

        public Task<List<AppUser>> GetUsersNotOnProjectAsync(int projectId, int comopanyId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await _context.Projects
                .Include( p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            bool result = false;

            if(project != null)
            {
                result = project.Members.Any(m => m.Id == userId);
            }

            return result;

        }

        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;

            return priorityId;
        }

        public Task RemoveProjectManagerAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                AppUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                try
                {
                    if (await IsUserOnProjectAsync(userId, projectId))
                    {
                        project.Members.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** ERROR ***** - Failed to Remove User from Project. ---> {ex.Message}");
            }
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
