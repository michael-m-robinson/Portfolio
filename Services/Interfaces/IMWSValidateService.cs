using Portfolio.Models.ViewModels;

namespace Portfolio.Services.Interfaces;

public interface IMWSValidateService
{
    public Task<Dictionary<string, string>> ValidateBlogCreateModel(BlogCreateEditViewModel model);
    public Task<Dictionary<string, string>> ValidateBlogEditModel(BlogCreateEditViewModel model);
}