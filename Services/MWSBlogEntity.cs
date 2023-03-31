using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Portfolio.Enums;
using Portfolio.Extensions;
using Portfolio.Models;
using Portfolio.Models.Content;
using Portfolio.Models.ViewModels;
using Portfolio.Services.Interfaces;
using SmartBreadcrumbs.Nodes;
using X.PagedList;

namespace Portfolio.Services;

public class MWSBlogEntity: IMWSBlogEntity
{
    private readonly IMWSImageService _imageService;
    private readonly IMWSCategoryService _categoryService;
    private readonly IMWSBlogService _blogService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMWSTagService _tagService;

    public MWSBlogEntity(IMWSImageService imageService, IMWSCategoryService categoryService, IHttpContextAccessor httpContextAccessor, IMWSBlogService blogService, IMWSTagService tagService)
    {
        _imageService = imageService;
        _categoryService = categoryService;
        _httpContextAccessor = httpContextAccessor;
        _blogService = blogService;
        _tagService = tagService;
    }

    public async Task CreateBlog(BlogCreateEditViewModel model)
    {
        model.Blog.Image = await _imageService.EncodeImageAsync(model.ImageFile);
        model.Blog.ImageType = model.ImageFile.ContentType;
        model.CategoryValues =
            await _categoryService.RemoveDuplicateCategoriesAsync(model.Blog.Id, model.CategoryValues);
        model.Blog.Created = DateTimeOffset.Now;
        model.Blog.AuthorId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        model.Blog.Slug = model.Blog.Name.Slugify();
            
        await _blogService.AddBlogAsync(model.Blog);
            
        model.CategoryValues?.Add("All Posts");
            
        if (model?.CategoryValues is not null)
            await _categoryService.AddCategoriesAsync(model.Blog.Id, model.CategoryValues);
    }

    public async Task EditBlog(BlogCreateEditViewModel model, Guid id)
    {
        var blogToUpdate = await _blogService.GetBlogAsync(id);
        blogToUpdate!.Updated = DateTimeOffset.Now;
        blogToUpdate.Created = model.Blog.Created;
        blogToUpdate.Name = model.Blog.Name;
        blogToUpdate.Description = model.Blog.Description;
        blogToUpdate.AuthorId = model.Blog.AuthorId;
        
        if (model.ImageFile is not null)
        {
            model.Blog.ImageType = model.ImageFile!.ContentType;
            model.Blog.Image = await _imageService.EncodeImageAsync(model.ImageFile);
            blogToUpdate.ImageType = model.Blog.ImageType;
            blogToUpdate.Image = model.Blog.Image;
        }
        
        blogToUpdate.Slug = model.Blog.Slug;
        
        var newCategoryEntries =
            await _categoryService.RemoveDuplicateCategoriesAsync(model.Blog.Id, model.CategoryValues);
        
        await _blogService.UpdateBlogAsync(blogToUpdate);
        if (newCategoryEntries.Any())
        {
            await _categoryService.RemoveStaleCategories(model.Blog);
            await _categoryService.AddCategoriesAsync(id, newCategoryEntries);
        }
    }

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
}