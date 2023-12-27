using BugOut.Extensions;
using BugOut.Models;
using BugOut.Models.ChartModels;
using BugOut.Models.Enums;
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
            _projectService = projectService;
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

        [HttpPost]
        public async Task<JsonResult> GglProjectTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);

            List<object> chartData = new();
            chartData.Add(new object[] { "ProjectName", "TicketCount" });

            foreach (Project prj in projects)
            {
                chartData.Add(new object[] { prj.Name, prj.Tickets.Where(t => t.Archived == false).Count() });
            }

            return Json(chartData);
        }

        [HttpPost]
        public async Task<JsonResult> GglProjectPriority()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);

            List<object> chartData = new();
            chartData.Add(new object[] { "Priority", "Count" });


            foreach (string priority in Enum.GetNames(typeof(AppProjectPriority)))
            {
                int priorityCount = (await _projectService.GetAllProjectsByPriority(companyId, priority)).Where(p=>p.Archived==false).Count();
                chartData.Add(new object[] { priority, priorityCount });
            }

            return Json(chartData);
        }

        [HttpPost]
        public async Task<JsonResult> AmCharts()
        {

            AmChartData amChartData = new();
            List<AmItem> amItems = new();

            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = (await _companyInfoService.GetAllProjectsAsync(companyId)).Where(p => p.Archived == false).ToList();

            foreach (Project project in projects)
            {
                AmItem item = new();

                item.Project = project.Name;
                item.Tickets = project.Tickets.Where(t => t.Archived == false).Count();
                item.Developers = (await _projectService.GetProjectMembersByRoleAsync(project.Id, nameof(Roles.Developer))).Count();

                amItems.Add(item);
            }

            amChartData.Data = amItems.ToArray();


            return Json(amChartData.Data);
        }

    }
}
