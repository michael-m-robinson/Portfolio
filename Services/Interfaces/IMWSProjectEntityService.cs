#region Imports

using Portfolio.Models.ViewModels;

#endregion

namespace Portfolio.Services.Interfaces;

public interface IMWSProjectEntityService
{
    public Task CreateProject(ProjectCreateViewModel model, IFormFileCollection files);
    public Task EditProject(ProjectEditViewModel model, IFormFileCollection files);
}