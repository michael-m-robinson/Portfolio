using Portfolio.Models.Content;
using Portfolio.Models.ViewModels;

namespace Portfolio.Services.Interfaces;

public interface IMWSBlogEntity
{
    public Task CreateBlog(BlogCreateEditViewModel model);
    public Task EditBlog(BlogCreateEditViewModel model, Guid id);
    public Task<BlogPostViewModel> ListBlog(Blog currentBlog, int? page);
}