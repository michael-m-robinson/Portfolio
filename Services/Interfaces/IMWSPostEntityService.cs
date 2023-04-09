#region Imports

using Portfolio.Models.ViewModels;

#endregion

namespace Portfolio.Services.Interfaces;

public interface IMWSPostEntityService
{
    public Task CreatePost(PostCreateViewModel model);
    public Task EditPost(PostEditViewModel model, Guid id);
    public Task<PostIndexViewModel> ListPost(string slug, string blogSlug);
}