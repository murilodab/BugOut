using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugOut.Models.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public AppUser AppUser { get; set; }
        public MultiSelectList Roles { get; set; }
        public List <string> SelectedRoles { get; set; }

    }
}
