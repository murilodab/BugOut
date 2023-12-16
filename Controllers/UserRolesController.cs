using BugOut.Extensions;
using BugOut.Models;
using BugOut.Models.ViewModels;
using BugOut.Services;
using BugOut.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugOut.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IAppRolesService _rolesService;
        private readonly IAppCompanyInfoService _companyInfoService;

        public UserRolesController(IAppRolesService rolesService, IAppCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }


        //GET
        #region Manage User Roles
        [HttpGet]       
        public async Task<IActionResult> ManageUserRoles()
        {
            //Add an instance of the ViewModel as a list
            List<ManageUserRolesViewModel> model = new();

            //Get CompanyId
            int companyId = User.Identity.GetCompanyId().Value;

            //Get all company users
            List<AppUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            //Loop over th eusers to populate the ViewModel
            //- instantiate ViewModel
            // - use -rolesService
            // - Create multi-selects
            foreach (AppUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.AppUser = user;
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);

            }

            //Return the Model to the view
            return View(model);
        }
        #endregion

        //POST
        #region Manage User Roles
        [HttpPost]    
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            //Get the CompanyId
            int companyId = User.Identity.GetCompanyId().Value;

            //Instantiate the AppUser
            AppUser user = (await _companyInfoService.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.AppUser.Id);

            //Get Roles for the User
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(user);

            //Get the Selected Role
            string userRole = member.SelectedRoles.FirstOrDefault();

            if (!string.IsNullOrEmpty(userRole))
            {
                //Remove User from their roles
                if (await _rolesService.RemoveUserFromRolesAsync(user, roles))
                {
                    //Add User to new role
                    await _rolesService.AddUserToRoleAsync(user, userRole);
                }


            }
            //Navigate back to the View
            return RedirectToAction(nameof(ManageUserRoles));

        } 
        #endregion
    }
}
