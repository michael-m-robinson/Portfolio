#region Imports

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portfolio.Extensions;
using Portfolio.Models;
using Portfolio.Models.Content;
using Portfolio.Models.ViewModels;
using Portfolio.Services.Interfaces;
using X.PagedList;

#endregion

namespace Portfolio.Controllers;

public class ProjectsController : Controller
{
    private readonly IMWSImageService _imageService;
    private readonly IMWSOpenGraphService _openGraphService;
    private readonly IMWSProjectEntityService _projectEntity;
    private readonly IMWSProjectService _projectService;
    private readonly UserManager<BlogUser> _userManager;
    private readonly IMWSValidateService _validateService;

    public ProjectsController(IMWSProjectService projectService,
        IMWSImageService imageService,
        IMWSOpenGraphService openGraphService,
        UserManager<BlogUser> userManager,
        IMWSValidateService validateService,
        IMWSProjectEntityService projectEntity)
    {
        _projectService = projectService;
        _imageService = imageService;
        _openGraphService = openGraphService;
        _userManager = userManager;
        _validateService = validateService;
        _projectEntity = projectEntity;
    }

    [TempData] public string StatusMessage { get; set; } = default!;

    #region All author projects get action

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> AllAuthorProjects(int? page)
    {
        var pageNumber = page ?? 1;
        var pageSize = 10;
        var projects = await _projectService.GetAllProjectsAsync();

        return View("AuthorIndex", await projects.ToPagedListAsync(pageNumber, pageSize));
    }

    #endregion

    #region Author Index get action

    public IActionResult AuthorIndex()
    {
        return NotFound();
    }

    #endregion

    #region Create project get action

    [Authorize(Roles = "Administrator")]
    public IActionResult Create()
    {
        var model = new ProjectCreateViewModel();
        model.ProjectSelectListItems = model.ProjectSelectListItems!.CreateProjectSelectListItems();
        ViewBag.ProjectMultiSelectList = new MultiSelectList(model.ProjectSelectListItems, "Value", "Text");
        return View(model);
    }

    #endregion

    #region Create project post action

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create([FromForm] ProjectCreateViewModel model)
    {
        var categoryList = model.Project.Categories.First().Split(',').ToList();
        List<SelectListItem>? categorySelectItems;
        if (ModelState.IsValid)
        {
            model.Project.Slug = model.Project.Title.Slugify();
            var files = HttpContext.Request.Form.Files;

            //Start validation
            var errorList = await _validateService.ValidateProjectCreateModel(model, files);
            if (errorList.Count > 0)
            {
                foreach (var error in errorList) ModelState.AddModelError(error.Key, error.Value);
                categorySelectItems = GetProjectCategoryList(categoryList);
                model.ProjectSelectListItems = GetProjectSelectListItems(categorySelectItems);
                return StatusCode(400, ModelState.AllErrors());
            }

            await _projectEntity.CreateProject(model, files);
            return StatusCode(201);
        }

        GetProjectCategoryList(categoryList);

        categorySelectItems = GetProjectCategoryList(categoryList);
        model.ProjectSelectListItems = GetProjectSelectListItems(categorySelectItems);
        return StatusCode(404, ModelState.AllErrors());
    }

    #endregion

    #region Delete project get action

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null) return NotFound();
        var project = await _projectService.GetProjectAsync(id.Value);
        if (project == new Project()) return NotFound();

        return View(project);
    }

    #endregion

    #region Delete project post action

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if (await ProjectExists(id) == false) return NotFound();
        var project = await _projectService.GetProjectAsync(id);
        _openGraphService.DeleteOpenGraphProjectImage(project);
        await _projectService.DeleteProjectAsync(project);

        return RedirectToAction(nameof(Index), "Home");
    }

    #endregion

    #region Project Details Action

    [Route("/Project/{slug}")]
    public async Task<IActionResult> Details(string slug)
    {
        var project = await _projectService.GetProjectBySlugAsync(slug);
        if (project.Id == new Guid()) return NotFound();

        var projects = (await _projectService.GetAllProjectsAsync())
            .OrderBy(pj => pj.Title)
            .ToList();

        var nextProjectIndex = -1;
        var lastProjectIndex = -1;
        var currentProjectIndex = projects.IndexOf(project);

        if (currentProjectIndex != projects.IndexOf(projects.Last())) nextProjectIndex = projects.IndexOf(project) + 1;
        if (currentProjectIndex != projects.IndexOf(projects.First())) lastProjectIndex = projects.IndexOf(project) - 1;
        if (nextProjectIndex != -1) ViewBag.NextProjectIndex = projects[nextProjectIndex].Slug!;
        if (lastProjectIndex != -1) ViewBag.LastProjectIndex = projects[lastProjectIndex].Slug!;

        return View(project);
    }

    #endregion

    #region Edit project get action

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit(Guid id)
    {
        if (await ProjectExists(id) == false) return NotFound();
        var model = new ProjectEditViewModel
        {
            Project = await _projectService.GetProjectAsync(id)
        };

        //get base64 version of project images
        foreach (var projectImage in model.Project.ProjectImages!)
        {
            var base64Image = _imageService.DecodeImage(projectImage.File, projectImage.FileContentType);
            base64Image = base64Image.Substring(base64Image.IndexOf(",", StringComparison.Ordinal) + 1);
            var imageDictionary = new Dictionary<string, string>();
            imageDictionary.Add("Image", base64Image);
            imageDictionary.Add("Name", projectImage.Name);
            model.Base64Images.Add(imageDictionary);
        }

        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        ViewBag.ImageList = JsonSerializer.Serialize(model.Base64Images, options);
        ViewBag.Images = model.Base64Images;

        model.ProjectSelectListItems = model.ProjectSelectListItems!.CreateProjectSelectListItems();
        var selectedCategories = model.Project.ProjectCategories?.Select(pc => pc.Text).ToList();
        ViewBag.ProjectMultiSelectList =
            new MultiSelectList(model.ProjectSelectListItems, "Value", "Text", selectedCategories);

        return View(model);
    }

    #endregion

    #region Edit project post action

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit(Guid id, [FromForm] ProjectEditViewModel model)
    {
        var categoryList = model.Project.Categories.First().Split(',').ToList();
        if (ModelState.IsValid)
        {
            if (await ProjectExists(id) == false) return NotFound();
            model.Project.Id = id;
            model.Project.Slug = model.Project.Title.Slugify();
            SetBase64ProjectImages(model);

            //Start validation
            var files = HttpContext.Request.Form.Files;
            var errorList = await _validateService.ValidateProjectEditModel(model, files);
            if (errorList.Count > 0)
            {
                foreach (var error in errorList) ModelState.AddModelError(error.Key, error.Value);
                return StatusCode(400, ModelState.AllErrors());
            }

            await _projectEntity.EditProject(model, files);
            return StatusCode(200);
        }

        ModelState.AddModelError("", "An error has occurred, if this persists, please contact the administrator.");
        return StatusCode(400, ModelState.AllErrors());
    }

    #endregion

    #region Project Search Action

    public async Task<IActionResult> Search(string term, int? page)
    {
        var blogUserId = _userManager.GetUserId(User);
        var pageNumber = page ?? 1;
        var pageSize = 10;
        var projects = await _projectService.GetAllProjectsAsync();

        var result = projects
            .Where(p => p.Title.ToLower().Contains(term.ToLower()));

        //return posts to the author index view.
        return View("AuthorIndex", await result.ToPagedListAsync(pageNumber, pageSize));
    }

    #endregion

    private List<SelectListItem> GetProjectCategoryList(List<string> categoryList)
    {
        var selectItemList = new List<SelectListItem>();
        foreach (var category in categoryList)
        {
            var selectListItem = new SelectListItem
            {
                Text = category,
                Value = category
            };
            selectItemList.Add(selectListItem);
        }

        return selectItemList;
    }

    private List<SelectListItem> GetProjectSelectListItems(List<SelectListItem> categoryList)
    {
        //generate project select list items
        var result = new List<SelectListItem>();
        result.CreateProjectSelectListItems();

        //init project multi select list to view bag
        ViewBag.ProjectMultiSelectList =
            new MultiSelectList(result, "Value", "Text", categoryList);

        //return selectList
        return result;
    }

    private async Task<bool> ProjectExists(Guid id)
    {
        var project = await _projectService.GetProjectAsync(id);
        if (project.Id == new Guid()) return false;
        return true;
    }

    private void SetBase64ProjectImages(ProjectEditViewModel model)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        ViewBag.ImageList = JsonSerializer.Serialize(model.Base64Images, options);
        ViewBag.Images = model.Base64Images;
    }
}