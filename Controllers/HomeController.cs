using BugOut.Extensions;
using BugOut.Models;
using BugOut.Models.ViewModels;
using BugOut.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BugOut.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAppCompanyInfoService _companyInfoService;
        private readonly IAppProjectService _projectService;
        private readonly IAppTicketService _ticketService;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IAppCompanyInfoService companyInfoService, IAppProjectService projectService, IAppTicketService ticketService, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _companyInfoService = companyInfoService;
            
        }

        public async Task<IActionResult> Dashboard()
        {
            DashBoardViewModel model = new();
            int companyId = User.Identity.GetCompanyId().Value;

            model.Company = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);
            model.Projects = (await _companyInfoService.GetAllProjectsAsync(companyId)).Where(p => p.Archived == false).ToList();
            model.Tickets = model.Projects.SelectMany(p => p.Tickets).Where(t => t.Archived == false).ToList();
            model.Members = model.Company.Members.ToList();


            return View(model);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
