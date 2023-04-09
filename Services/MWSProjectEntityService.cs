#region Imports

using Portfolio.Models.Content;
using Portfolio.Models.ViewModels;
using Portfolio.Services.Interfaces;

#endregion

namespace Portfolio.Services;

public class MWSProjectEntityService : IMWSProjectEntityService
{
    private readonly IMWSImageService _imageService;
    private readonly IMWSOpenGraphService _openGraphService;
    private readonly IMWSProjectImageService _projectImageService;
    private readonly IMWSProjectService _projectService;

    public MWSProjectEntityService(IMWSProjectService projectService,
        IMWSOpenGraphService openGraphService,
        IMWSProjectImageService projectImageService,
        IMWSImageService imageService)
    {
        _projectService = projectService;
        _openGraphService = openGraphService;
        _projectImageService = projectImageService;
        _imageService = imageService;
    }

    #region Create Project

    public async Task CreateProject(ProjectCreateViewModel model, IFormFileCollection files)
    {
        var categoryList = model.Project.Categories.First().Split(',').ToList();
        model.Project.Categories = categoryList;
        model.Project.Created = DateTime.Now.ToUniversalTime();
        await _projectService.AddProjectAsync(model.Project);
        await _projectService.AddProjectCategoriesAsync(model.Project);
        var projectSplashFile = files.OrderBy(f => f.FileName).Last();
        await _openGraphService.AddOpenGraphProjectImageAsync(model.Project, projectSplashFile);
        await _projectImageService.AddProjectImagesAsync(model.Project, files.ToList());
    }

    #endregion

    #region Edit Project

    public async Task EditProject(ProjectEditViewModel model, IFormFileCollection files)
    {
        var projectToUpdate = await _projectService.GetProjectAsync(model.Project.Id);
        var updatedProject = UpdateProjectProperties(model.Project, projectToUpdate);
        updatedProject.Slug = model.Project.Slug;
        var categoryList = model.Project.Categories.First().Split(',').ToList();

        //get base64 version of project images
        foreach (var projectImage in updatedProject.ProjectImages!)
        {
            var base64Image = _imageService.DecodeImage(projectImage.File, projectImage.FileContentType);
            var imageDictionary = new Dictionary<string, string>
            {
                { "Image", base64Image },
                { "Name", projectImage.Name }
            };
            model.Base64Images.Add(imageDictionary);
        }

        updatedProject.Categories = categoryList;
        await _projectService.UpdateProjectAsync(updatedProject);
        await _projectService.RemoveStaleCategoriesAsync(updatedProject);
        await _projectService.AddProjectCategoriesAsync(updatedProject);
        await _projectImageService.RemoveStaleProjectImagesAsync(updatedProject);
        await _projectImageService.AddProjectImagesAsync(updatedProject, files.ToList());
    }

    #endregion

    private Project UpdateProjectProperties(Project baseProject, Project projectToUpdate)
    {
        projectToUpdate.Description = baseProject.Description;
        projectToUpdate.Title = baseProject.Title;
        projectToUpdate.ProjectUrl = baseProject.ProjectUrl;

        return projectToUpdate;
    }
}