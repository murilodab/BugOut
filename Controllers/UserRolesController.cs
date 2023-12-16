using BugOut.Extensions;
using BugOut.Models;
using BugOut.Models.ViewModels;
using BugOut.Services;
using BugOut.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugOut.Controllers
{
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
    }
}
