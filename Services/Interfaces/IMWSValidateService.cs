#region Imports

using Portfolio.Models.ViewModels;

#endregion

namespace Portfolio.Services.Interfaces;

public interface IMWSValidateService
{
    public Task<Dictionary<string, string>> ValidateBlogCreateModel(BlogCreateEditViewModel model);
    public Task<Dictionary<string, string>> ValidateBlogEditModel(BlogCreateEditViewModel model);
    public Task<Dictionary<string, string>> ValidatePostCreateModel(PostCreateViewModel model);
    public Task<Dictionary<string, string>> ValidatePostEditModel(PostEditViewModel model);

    public Task<Dictionary<string, string>> ValidateProjectCreateModel(ProjectCreateViewModel model,
        IFormFileCollection files);

    public Task<Dictionary<string, string>> ValidateProjectEditModel(ProjectEditViewModel model,
        IFormFileCollection files);
}