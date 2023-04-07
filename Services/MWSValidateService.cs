using Microsoft.AspNetCore.Html;
using Portfolio.Extensions;
using Portfolio.Models.ViewModels;
using Portfolio.Services.Interfaces;
using System.Net;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Portfolio.Services;

public class MWSValidateService: IMWSValidateService
{
    private readonly IMWSCivilityService _civilityService;
    private readonly IMWSBlogService _blogService;
    private readonly IMWSCategoryService _categoryService;
    private readonly IMWSPostService _postService;
    
    public MWSValidateService(IMWSCivilityService civilityService, 
        IMWSBlogService blogService, 
        IMWSCategoryService categoryService, 
        IMWSPostService postService)
    {
        _civilityService = civilityService;
        _blogService = blogService;
        _categoryService = categoryService;
        _postService = postService;
    }
    
    public async Task<Dictionary<string, string>> ValidateBlogCreateModel(BlogCreateEditViewModel model)
    {
        var errorList = new Dictionary<string, string>();
        if (model.ImageFile is null)
        {
            errorList.Add("ImageFile", "All blogs must have an image.");
        }

        if (_civilityService.IsCivil(model.Blog.Description).Verdict == false)
        {
            errorList.Add("Blog.Description", "We found inappropriate language in the description");
        }

        if (_civilityService.IsCivil(model.Blog.Name).Verdict == false)
        {
            errorList.Add("Blog.Name", "We found inappropriate language in the name of the blog");
        }
        
        if (model.Blog?.Slug is not null &&
            _blogService.IsSlugUniqueAsync(model.Blog.Slug) == false)
        {
            errorList.Add("Blog.Name", "Your blog name is taken, please choose another.");
        }
        
        if (model.CategoryValues is not null)
        {
            model.CategoryValues =
                await _categoryService.RemoveDuplicateCategoriesAsync(model.Blog!.Id, model.CategoryValues);

            foreach (var category in model.CategoryValues)
                if (_civilityService.IsCivil(category).Verdict == false ||
                    (await _categoryService.IsCategoryUnique(model.Blog.Id, category) == false &&
                     category != "All Posts"))
                {
                    errorList.Add("CategoryValues", "There was a problem with your list, please revise.");
                    break;
                }
        }

        return errorList;
    }

    public async Task<Dictionary<string, string>> ValidateBlogEditModel(BlogCreateEditViewModel model)
    {
        var errorList = new Dictionary<string, string>();
        if (_civilityService.IsCivil(model.Blog.Description).Verdict == false)
        {
            errorList.Add("Blog.Description", "We found inappropriate language in the description");
        }

        if (_civilityService.IsCivil(model.Blog.Name).Verdict == false)
        {
            errorList.Add("Blog.Name", "We found inappropriate language in the name of the blog");
        }
        
        if (model.CategoryValues is not null)
        {
            model.CategoryValues =
                await _categoryService.RemoveDuplicateCategoriesAsync(model.Blog.Id, model.CategoryValues);

            foreach (var category in model.CategoryValues)
                if (_civilityService.IsCivil(category).Verdict == false ||
                    (await _categoryService.IsCategoryUnique(model.Blog.Id, category) == false &&
                     category != "All Posts"))
                {
                    errorList.Add("CategoryValues", "There was a problem with your list, please revise.");
                    break;
                }
        }
        
        if ((await _blogService.GetBlogAsync(model.Blog.Id)).Slug != model.Blog.Slug &&
            _blogService.IsSlugUniqueAsync(model.Blog.Slug!) == false)
        {
            errorList.Add("Blog.Name", "Your blog name is taken, please choose another.");
        }

        return errorList;
    }

    public async Task<Dictionary<string, string>> ValidatePostCreateModel(PostCreateViewModel model)
    {
        var errorList = new Dictionary<string,string>();
        if (model.ImageFile is null)
        {
            errorList.Add("ImageFile", "All Posts must have an image.");
        }

        if (model.ImageFile!.IsImage() == false)
        {
            errorList.Add("ImageFile",
                "The file must be an image, please check your entry and try again.");
        }

        if (model.Post.Slug is not null &&
                await _postService.IsSlugUniqueAsync(model.Post.Slug, model.Post.BlogId) == false)
        {
            errorList.Add("Post.Title", "Your post title is taken, please choose another.");
        }

        if (_civilityService.IsCivil(model.Post.Title).Verdict == false)
        {
            errorList.Add("Post.Title", "There is foul language in the title, please revise.");
        }

        if (_civilityService.IsCivil(model.Post.Abstract).Verdict == false)
        {
            errorList.Add("Post.Abstract",
                "There is foul language in the abstract section of this post, please revise.");
        }
        
        var noHtmlContent = WebUtility.HtmlDecode(Regex.Replace(model.Post.Content.RemoveHtmlTags(), "<[^>]*(>|$)", string.Empty));
        if (_civilityService.IsCivil(noHtmlContent).Verdict == false)
        {
            errorList.Add("Post.Content",
                "There is foul language in the body of this post, please revise.");
        }

        if (model.TagValues is not null)
            foreach (var tag in model.TagValues)
                if (_civilityService.IsCivil(tag).Verdict == false)
                {
                    errorList.Add("TagValues",
                        "There is foul language in one of your tags, please revise.");
                }

        return errorList;
    }

    public async Task<Dictionary<string, string>> ValidatePostEditModel(PostEditViewModel model)
    {
        var errorList = new Dictionary<string, string>();
        if (model.ImageFile is not null && model.ImageFile.IsImage())
        {
            errorList.Add("ImageFile",
                "The file must be an image, please check your entry and try again.");
        }
        
        var newSlug = model.Post!.Title.Slugify();
        if (await _postService.IsSlugUniqueAsync(newSlug!, model.Post!.BlogId) == false &&
            newSlug != (await _postService.GetPostByIdAsync(model.Post.Id)).Slug)
        {
            errorList.Add("Post.Title", "Your title is already taken, please choose another.");
        }
        
        if (_civilityService.IsCivil(model.Post.Title).Verdict == false)
        {
            errorList.Add("Post.Title", "There is foul language in the title, please revise.");
        }
        
        var noHtmlContent = WebUtility.HtmlDecode(Regex.Replace(model.Post.Content.RemoveHtmlTags(), 
            "<[^>]*(>|$)", string.Empty));
        if (_civilityService.IsCivil(noHtmlContent).Verdict == false)
        {
            errorList.Add("Post.Content",
                "There is foul language in the body of this post, please revise.");
        }
        
        if(string.IsNullOrEmpty(model.Post?.Content))
        {
            errorList.Add("Post.Content",
                "The body of the post cannot be empty, please revise.");
        }
        
        if (_civilityService.IsCivil(model.Post!.Abstract).Verdict == false)
        {
            errorList.Add("Post.Abstract",
                "There is foul language in the abstract section of this post, please revise.");
        }
        
        if (model.TagValues is not null)
            foreach (var tag in model.TagValues)
                if (_civilityService.IsCivil(tag).Verdict == false)
                {
                    errorList.Add("TagValues",
                        "There is foul language in one of your tags, please revise.");
                }

        return errorList;
    }
}