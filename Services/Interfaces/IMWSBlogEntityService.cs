using Portfolio.Models.Content;
using Portfolio.Models.ViewModels;
using X.PagedList;

namespace Portfolio.Services.Interfaces;

public interface IMWSBlogEntityService
{
    public Task CreateBlog(BlogCreateEditViewModel model);
    public Task EditBlog(BlogCreateEditViewModel model, Guid id);
    public Task<IPagedList<Blog>> ListAllBlogs(int? page);
    public Task<BlogPostViewModel> ListBlog(Blog currentBlog, int? page);
    public Task<BlogPostViewModel> ListBlogByCategory(Blog currentBlog, int? page, string categoryName);
    public Task<BlogPostViewModel> ListBlogPostsBySearch(string term, string slug, int? page);
    public Task<BlogPostViewModel> ListBlogByTag(string tag, string slug, int? page);
}