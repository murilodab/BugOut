using BugOut.Data;
using BugOut.Models;
using BugOut.Models.Enums;
using BugOut.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Construction;
using Microsoft.EntityFrameworkCore;

namespace BugOut.Services
{
    public class AppProjectService : IAppProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppRolesService _rolesService;
       

        public AppProjectService(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IAppRolesService roleService)
        {
            _context = context;
            _rolesService = roleService;
        }

        #region Add New Project
        public async Task AddNewProjectAsync(Project project)
        {
            try
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Add Project Manager
        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            AppUser projectManager = await GetProjectManagerAsync(projectId);

            if (projectManager != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"**** ERROR **** - Failed to Remove the Current Project Manager. --> {ex.Message}");
                    return false;
                }
            }

            try
            {
                await AddUserToProjectAsync(userId, projectId);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** ERROR **** - Failed to Add the New Project Manager. --> {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Add User To Project
        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            AppUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                if (!await IsUserOnProjectAsync(userId, projectId))
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
        #endregion

        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {
                project.Archived = true;
                await UpdateProjectAsync(project);

                //Archive the Tickets for the Project
                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = true;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<AppUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            List<AppUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
            List<AppUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
            List<AppUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

            List<AppUser> teamMembers = developers.Concat(submitters).Concat(admins).ToList();

            return teamMembers;
        }

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> projects = await GetAllProjectsByCompanyAsync(companyId);

            int priorityId = await LookupProjectPriorityId(priorityName);

            return projects.Where(p => p.ProjectPriorityId == priorityId).ToList();
        }

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            try
            {
                List<Project> projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == true)
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
            catch (Exception)
            {

                throw;
            }

        }

        public Task<List<AppUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        #region Get Project By Id
        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project project = await _context.Projects
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketPriority)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketStatus)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.TicketType)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.DeveloperUser)
                                                    .Include(p => p.Tickets)
                                                        .ThenInclude(t => t.OwnerUser)
                                                    .Include(p => p.Members)
                                                    .Include(p => p.ProjectPriority)
                                                    .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

            return project;
        } 
        #endregion

        public async Task<AppUser> GetProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            foreach(AppUser user in project?.Members)
            {
                if (await _rolesService.IsUserInRoleAsync(user, Roles.ProjectManager.ToString()))
                {
                    return user;
                }
            }

            return null;
        }

        public async Task<List<AppUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = await _context.Projects
                                                .Include(p => p.Members)
                                                .FirstOrDefaultAsync(p => p.Id == projectId);

            List<AppUser> members = new();

            foreach(var user in project.Members)
            {
                if(await _rolesService.IsUserInRoleAsync(user, role))
                {
                    members.Add(user);
                }
            }

            return members;
        }

        public Task<List<AppUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
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

        #region Get User Projects
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
                            .ThenInclude(t => t.TicketPriority)
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
        #endregion

        public async Task<List<AppUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            List<AppUser> users = await _context.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToListAsync();

            return users.Where(u => u.CompanyId == companyId).ToList();
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

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            Project project = await _context.Projects
                                            .Include(p => p.Members)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);

            try
            {
                foreach(AppUser user in project?.Members)
                {
                    if(await _rolesService.IsUserInRoleAsync(user, Roles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(user.Id, projectId);
                    }
                }

            }catch(Exception)
            {
                throw;
            }
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

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                List<AppUser> members = await GetProjectMembersByRoleAsync(projectId, role);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                foreach(AppUser user in members)
                {
                    try
                    {
                        project.Members.Remove(user);
                        
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"**** ERROR **** - Failed to Remove Users From Project. --> {ex.Message}");
                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _context.Update(project);
            await _context.SaveChangesAsync();
        }

        public async Task RestoreProjectAsync(Project project)
        {
            try
            {
                project.Archived = false;
                await UpdateProjectAsync(project);

                //Archive the Tickets for the Project
                foreach (Ticket ticket in project.Tickets)
                {
                    ticket.ArchivedByProject = false;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Is Assigned Project Manager
        public async Task<bool> IsAssignedProjectManagerAsync(string userId, int projectId)
        {
            try
            {
                string projectManagerId = (await GetProjectManagerAsync(projectId))?.Id;

                if(projectManagerId == userId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        } 
        #endregion
    }
}
