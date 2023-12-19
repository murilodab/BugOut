using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugOut.Data;
using BugOut.Models;
using BugOut.Extensions;
using BugOut.Models.ViewModels;
using BugOut.Services.Interfaces;
using BugOut.Models.Enums;
using Org.BouncyCastle.Bcpg;
using Microsoft.AspNetCore.Identity;

namespace BugOut.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly IAppLookupService _lookupService;
        private readonly ApplicationDbContext _context;
        private readonly IAppRolesService _rolesService;
        private readonly IAppFileService _fileService;
        private readonly IAppProjectService _projectService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAppCompanyInfoService _companyInfoService;

        public ProjectsController(ApplicationDbContext context, 
            IAppRolesService rolesService, 
            IAppLookupService lookupService, 
            IAppFileService fileService, 
            IAppProjectService projectService, 
            UserManager<AppUser> userManager,
            IAppCompanyInfoService companyInfoService)
        {
            _context = context;
            _rolesService = rolesService;
            _lookupService = lookupService;
            _fileService = fileService;
            _projectService = projectService;
            _userManager = userManager;
            _companyInfoService = companyInfoService;
           
        }

        // GET: Projects
        #region Index
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Projects.Include(p => p.Company).Include(p => p.ProjectPriority);
            return View(await applicationDbContext.ToListAsync());
        }
        #endregion

        //GET: My Projects
        #region My Projects
        public async Task<IActionResult> MyProjects()
        {
            string userId = _userManager.GetUserId(User);

            List<Project> projects = await _projectService.GetUserProjectsAsync(userId);

            return View(projects);
        }
        #endregion

        //GET: All Projects
        #region All Projects
        public async Task<IActionResult> AllProjects()
        {


            List<Project> projects = new();

            int companyId = User.Identity.GetCompanyId().Value;

            if(User.IsInRole(nameof(Roles.Admin)) || User.IsInRole(nameof(Roles.ProjectManager)))
            {
                projects = await _companyInfoService.GetAllProjectsAsync(companyId);
            }
            else
            {
                projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            }

            return View(projects);
        }
        #endregion

        //GET: My Projects
        #region Archived Projects
        public async Task<IActionResult> ArchivedProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetArchivedProjectsByCompany(companyId);

            return View(projects);
        }
        #endregion

        // GET: Projects/Details/5
        #region Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Company)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }
        #endregion
        // GET: Projects/Create
        #region Create
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            //Add ViewModel Instance
            AddProjectWithPMViewModel model = new();

            //Load SelectList with data, PMLists and PriorityList
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");


            return View(model);
        } 
        #endregion

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        #region Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
          if(model != null)
            {
                int companyId = User.Identity.GetCompanyId().Value;

                try
                {
                    if(model.Project.ImageFormFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageContentType = model.Project.ImageFormFile.ContentType;
                    }

                    model.Project.CompanyId = companyId;

                    await _projectService.AddNewProjectAsync(model.Project);

                    //Add PM if one was chosen
                    if (!string.IsNullOrEmpty(model.PMId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PMId, model.Project.Id);
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                //TODO: Redirect to All Projects
                return RedirectToAction("Index");
            }
             
         
            return RedirectToAction("Create");
        }
        #endregion

        // GET: Projects/Edit/5
        #region Edit
        public async Task<IActionResult> Edit(int? id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            //Add ViewModel Instance
            AddProjectWithPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            //Load SelectList with data, PMLists and PriorityList
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");


            return View(model);
        }
        #endregion

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        #region Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
        {
            if (model != null)
            {

                try
                {
                    if (model.Project.ImageFormFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageContentType = model.Project.ImageFormFile.ContentType;
                    }



                    await _projectService.UpdateProjectAsync(model.Project);

                    //Add PM if one was chosen
                    if (!string.IsNullOrEmpty(model.PMId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PMId, model.Project.Id);
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                //TODO: Redirect to All Projects
                return RedirectToAction("Index");
            }


            return RedirectToAction("Edit");
        }
        #endregion

        // GET: Projects/Delete/5
        #region Archive
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;

            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }
        #endregion

        // POST: Projects/Delete/5
        #region Archive Confirmed
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            var project = await _projectService.GetProjectByIdAsync(id, companyId);

            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }
        #endregion

        // GET: Projects/Delete/5
        #region Restore
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId().Value;

            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }
        #endregion

        // POST: Projects/Delete/5
        #region Restore Confirmed
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            var project = await _projectService.GetProjectByIdAsync(id, companyId);

            await _projectService.RestoreProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }
        #endregion

        private bool ProjectExists(int id)
        {
          return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
