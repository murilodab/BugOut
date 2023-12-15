using BugOut.Data;
using BugOut.Models;
using BugOut.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugOut.Services
{
    public class AppRolesService : IAppRolesService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public AppRolesService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        #region Add User To Role
        public async Task<bool> AddUserToRoleAsync(AppUser user, string roleName)
        {
            bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;

            return result;
        } 
        #endregion

        #region Get Roles
        public async Task<List<IdentityRole>> GetRolesAsync()
        {
            try
            {
                List<IdentityRole> result = new();

                result = await _context.Roles.ToListAsync();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        } 
        #endregion

        public async Task<string> GetRoleNameByIdAsync(string roleId)
        {
            IdentityRole role = _context.Roles.Find(roleId);
            string result = await _roleManager.GetRoleNameAsync(role);

            return result;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(AppUser user)
        {
            IEnumerable<string> result = await _userManager.GetRolesAsync(user);

            return result;
        }

        public async Task<List<AppUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            List<AppUser> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();

            return users.Where(u => u.CompanyId == companyId).ToList();
        }

        public async Task<List<AppUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
        {
            List<string> usersIdsInRole = (await _userManager.GetUsersInRoleAsync(roleName)).Select(u => u.Id).ToList();

            List<AppUser> usersNotInRole= _context.Users.Where(u => !usersIdsInRole.Contains(u.Id)).ToList();


            return usersNotInRole.Where(u => u.CompanyId == companyId).ToList();
        }

        public async Task<bool> IsUserInRoleAsync(AppUser user, string roleName)
        {
            bool result = await _userManager.IsInRoleAsync(user, roleName);

            return result;
        }

        public async Task<bool> RemoveUserFromRoleAsync(AppUser user, string roleName)
        {
            bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;

            return result;
        }

        public async Task<bool> RemoveUserFromRolesAsync(AppUser user, IEnumerable<string> roles)
        {
            bool result = (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;

            return result;
        }
    }
}
