#region Imports

using System.Security.Claims;
using Portfolio.Enums;
using Portfolio.Models.Content;
using Portfolio.Models.ViewModels;
using Portfolio.Services.Interfaces;
using X.PagedList;

#endregion

namespace Portfolio.Services;

public class MWSBlogEntityService : IMWSBlogEntityService
{
    private readonly IMWSBlogService _blogService;
    private readonly IMWSCategoryService _categoryService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMWSImageService _imageService;
    private readonly IMWSPostService _postService;
    private readonly IMWSTagService _tagService;

    public MWSBlogEntityService(IMWSImageService imageService, IMWSCategoryService categoryService,
        IHttpContextAccessor httpContextAccessor, IMWSBlogService blogService, IMWSTagService tagService,
        IMWSPostService postService)
    {
        _imageService = imageService;
        _categoryService = categoryService;
        _httpContextAccessor = httpContextAccessor;
        _blogService = blogService;
        _tagService = tagService;
        _postService = postService;
    }

    #region Create Blog

    public async Task CreateBlog(BlogCreateEditViewModel model)
    {
        model.Blog.Image = await _imageService.EncodeImageAsync(model.ImageFile!);
        model.Blog.ImageType = model.ImageFile!.ContentType;
        model.Blog.Created = DateTimeOffset.Now;
        model.Blog.AuthorId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        model.CategoryValues = model.CategoryValues == null ? new List<string>() : model.CategoryValues;

        await _blogService.AddBlogAsync(model.Blog);

        if (model.CategoryValues.Count == 0) model.CategoryValues.Add("All Posts");

        model.CategoryValues =
            await _categoryService.RemoveDuplicateCategoriesAsync(model.Blog.Id, model.CategoryValues);
        await _categoryService.AddCategoriesAsync(model.Blog.Id, model.CategoryValues);
    }

    #endregion

    #region Edit Blog

    public async Task EditBlog(BlogCreateEditViewModel model, Guid id)
    {
        var blogToUpdate = await _blogService.GetBlogAsync(id);
        var updatedBlog = UpdateBlogProperties(model.Blog, blogToUpdate);

        if (model.ImageFile is not null)
        {
            updatedBlog.ImageType = model.ImageFile!.ContentType;
            updatedBlog.Image = await _imageService.EncodeImageAsync(model.ImageFile);
        }

        await _blogService.UpdateBlogAsync(updatedBlog);
        var newCategoryEntries =
            await _categoryService.RemoveDuplicateCategoriesAsync(model.Blog.Id, model.CategoryValues!);
        await _categoryService.RemoveStaleCategories(model.Blog);
        await _categoryService.AddCategoriesAsync(id, newCategoryEntries);
    }

    #endregion

    #region List All Blog

    public async Task<IPagedList<Blog>> ListAllBlogs(int? page)
    {
        var pageNumber = page ?? 1;
        var pageSize = 6;
        var blogs = (await _blogService.GetAllBlogsAsync())
            .Where(b => b.Posts.Any(p => p.ReadyStatus == ReadyStatus.ProductionReady));

        var blogList = await blogs.ToPagedListAsync(pageNumber, pageSize);
        return blogList;
    }

    #endregion

    #region List Blog

    public async Task<BlogPostViewModel> ListBlog(Blog currentBlog, int? page)
    {
        var model = new BlogPostViewModel
        {
            Blog = currentBlog
        };

        var productionReadyArticles =
            await model.Blog.Posts.Where(p => p.ReadyStatus == ReadyStatus.ProductionReady).ToListAsync();

        var pageNumber = page ?? 1;
        var pageSize = 3;

        model.PaginatedPosts = await productionReadyArticles.ToPagedListAsync(pageNumber, pageSize);
        model.Tags = await _tagService.GetTopTwentyBlogTagsAsync(model.Blog.Id);
        model.CurrentAction = "Details";

        model.RecentArticles = await model.PaginatedPosts.OrderByDescending(p => p.Created)
            .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
            .Take(4).ToListAsync();

        return model;
    }

    #endregion

    #region List Blog By Category

    public async Task<BlogPostViewModel> ListBlogByCategory(Blog currentBlog, int? page, string categoryName)
    {
        var pageNumber = page ?? 1;
        var pageSize = 3;
        var model = new BlogPostViewModel
        {
            Blog = currentBlog
        };

        model.RecentArticles = await (await _postService.GetPostsByBlogId(model.Blog.Id))
            .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
            .OrderByDescending(p => p.Created).Take(4).ToListAsync();

        model.CategoryName = categoryName;
        model.Category = await _categoryService.GetCategoryByNameAsync(model.Blog.Id, categoryName);

        model.CurrentAction = "DetailsByCategory";

        model.Tags = await _tagService.GetTopTwentyBlogTagsAsync(model.Blog.Id);

        model.PaginatedPosts =
            await (await _postService.GetPostsByCategory(model.Blog.Id, model.Category))
                .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
                .ToPagedListAsync(pageNumber, pageSize);

        return model;
    }

    #endregion

    #region List Blog By Tag

    public async Task<BlogPostViewModel> ListBlogByTag(string tag, string slug, int? page)
    {
        var model = new BlogPostViewModel();
        var pageNumber = page ?? 1;
        var pageSize = 3;

        model.Blog = await _blogService.GetBlogBySlugAsync(slug);
        var tagPosts = (await _postService.GetPostsByTag(tag, model.Blog.Id))
            .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady);

        model.PaginatedPosts = await tagPosts.ToPagedListAsync(pageNumber, pageSize);
        model.RecentArticles = await _postService.GetTopFivePostsByDateAsync(model.Blog.Id);
        model.Tags = await _tagService.GetTopTwentyBlogTagsAsync(model.Blog.Id);
        model.CurrentAction = "Tag";

        return model;
    }

    #endregion

    #region List Blog Posts By Search

    public async Task<BlogPostViewModel> ListBlogPostsBySearch(string term, string slug, int? page)
    {
        var pageNumber = page ?? 1;
        var pageSize = 3;
        var model = new BlogPostViewModel();

        model.Blog = await _blogService.GetBlogBySlugAsync(slug);

        model.PaginatedPosts = await model.Blog.Posts
            .Where(p => p.Title.ToLower().Contains(term.ToLower()))
            .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
            .ToPagedListAsync(pageNumber, pageSize);

        model.RecentArticles = await (await _postService.GetPostsByBlogId(model.Blog.Id))
            .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
            .OrderByDescending(p => p.Created).Take(4)
            .ToListAsync();

        model.Tags = await _tagService.GetTopTwentyBlogTagsAsync(model.Blog.Id);
        model.CurrentAction = "PostSearch";

        return model;
    }

    #endregion

    private Blog UpdateBlogProperties(Blog baseBlog, Blog blogToUpdate)
    {
        blogToUpdate!.Updated = DateTimeOffset.Now;
        blogToUpdate.Created = baseBlog.Created;
        blogToUpdate.Name = baseBlog.Name;
        blogToUpdate.Description = baseBlog.Description;
        blogToUpdate.AuthorId = baseBlog.AuthorId;

        return blogToUpdate;
    }
}