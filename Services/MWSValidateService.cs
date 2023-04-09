using Microsoft.AspNetCore.Html;
using Portfolio.Extensions;
using Portfolio.Models.ViewModels;
using Portfolio.Services.Interfaces;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Portfolio.Services;

public class MWSValidateService : IMWSValidateService
{
    private readonly IMWSCivilityService _civilityService;
    private readonly IMWSBlogService _blogService;
    private readonly IMWSCategoryService _categoryService;
    private readonly IMWSPostService _postService;
    private readonly IMWSProjectService _projectService;

    public MWSValidateService(IMWSCivilityService civilityService,
        IMWSBlogService blogService,
        IMWSCategoryService categoryService,
        IMWSPostService postService,
        IMWSProjectService projectService)
    {
        _civilityService = civilityService;
        _blogService = blogService;
        _categoryService = categoryService;
        _postService = postService;
        _projectService = projectService;
    }

    #region Validate Blog Create Model

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

    #endregion

    #region Validate Blog Edit Model

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

    #endregion

    #region Validate Post Create Model

    public async Task<Dictionary<string, string>> ValidatePostCreateModel(PostCreateViewModel model)
    {
        var errorList = new Dictionary<string, string>();

        errorList.Add("ImageFile", "All Posts must have an image.");


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

        var noHtmlContent =
            WebUtility.HtmlDecode(Regex.Replace(model.Post.Content.RemoveHtmlTags(), "<[^>]*(>|$)", string.Empty));
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
                    break;
                }

        return errorList;
    }

    #endregion

    #region Validate Post Edit Model

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

        if (string.IsNullOrEmpty(model.Post?.Content))
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
                    break;
                }

        return errorList;
    }

    #endregion

    #region Validate Project Create Model

    public async Task<Dictionary<string, string>> ValidateProjectCreateModel(ProjectCreateViewModel model,
        IFormFileCollection files)
    {
        var errorList = new Dictionary<string, string>();

        if (_civilityService.IsCivil(model.Project.Title).Verdict == false)
        {
            errorList.Add("Project.Title",
                "There is an error with your title, please check it and try again.");
        }

        if (await _projectService.IsUniqueAsync(model.Project.Title.Slugify()) == false)
        {
            errorList.Add("Project.Title",
                "Your title is not unique, please change it and try again.");
        }

        if (_civilityService.IsCivil(model.Project.ProjectUrl).Verdict == false)
        {
            errorList.Add("Project.ProjectUrl",
                "There is an error with your URL, please check it and try again.");
        }

        if (_civilityService.IsCivil(model.Project.Description).Verdict == false)
        {
            errorList.Add("Project.Description",
                "There is an error with your Description, please check it and try again.");
        }

        var enumSelectListItems = new List<SelectListItem>().CreateProjectSelectListItems();
        var categoryList = model.Project.Categories.First().Split(',').ToList();
        if (!categoryList.Any() ||
            categoryList.Count > enumSelectListItems.Count)
        {
            errorList.Add("Project.Categories",
                "Please make sure you have at least one category.");
        }

        foreach (var projectCategory in categoryList)
            if (_civilityService.IsCivil(projectCategory).Verdict == false)
            {
                errorList.Add("Project.Categories",
                    "There is an error with at least one of your category name, please check it and try again.");
                break;
            }

        var masterCategoryList = enumSelectListItems.Select(e => e.Text).ToList();
        var numberOfDifferences = categoryList.Count(cl => masterCategoryList.All(mcl => mcl != cl));
        if (numberOfDifferences > 0)
        {
            errorList.Add(string.Empty, "The form has an error; please refresh the page.");
        }

        foreach (var file in files)
            if (file.IsImage() == false)
            {
                errorList.Add("Project.ProjectImages",
                    "There is an error with your images, please check them and try again.");
                break;
            }

        return errorList;
    }

    #endregion

    #region Validate Project Edit Model

        public async Task<Dictionary<string, string>> ValidateProjectEditModel(ProjectEditViewModel model,
        IFormFileCollection files)
    {
        var errorList = new Dictionary<string, string>();

        if (_civilityService.IsCivil(model.Project.Title).Verdict == false)
        {
            errorList.Add("Project.Title",
                "There is an error with your title, please check it and try again.");
        }

        if (model.Project.Slug != (await _projectService.GetProjectAsync(model.Project.Id)).Slug &&
            await _projectService.IsUniqueAsync(model.Project.Slug!) == false)
        {
            errorList.Add("Project.Title",
                "Your title is not unique, please change it and try again.");
        }

        if (_civilityService.IsCivil(model.Project.ProjectUrl).Verdict == false)
        {
            errorList.Add("Project.ProjectUrl",
                "There is an error with your URL, please check it and try again.");
        }

        if (_civilityService.IsCivil(model.Project.Description).Verdict == false)
        {
            errorList.Add("Project.Description",
                "There is an error with your Description, please check it and try again.");
        }

        var enumSelectListItems = new List<SelectListItem>().CreateProjectSelectListItems();
        var categoryList = model.Project.Categories.First().Split(',').ToList();
        if (!categoryList.Any() ||
            categoryList.Count > enumSelectListItems.Count)
        {
            errorList.Add("Project.Categories",
                "Please make sure you have at least one category.");
        }

        foreach (var projectCategory in categoryList)
            if (_civilityService.IsCivil(projectCategory).Verdict == false)
            {
                errorList.Add("Project.Categories",
                    "There is an error with at least one of your category name, please check it and try again.");
                break;
            }

        foreach (var file in files.OrderByDescending(f => f.FileName))
            if (file.IsImage() == false)
            {
                errorList.Add("Project.ProjectImages",
                    "There is an error with your images, please check them and try again.");
                break;
            }

        var masterCategoryList = enumSelectListItems.Select(e => e.Text).ToList();
        var numberOfDifferences = categoryList.Count(cl => masterCategoryList.All(mcl => mcl != cl));
        if (numberOfDifferences > 0)
        {
            errorList.Add(string.Empty, "The form has an error; please refresh the page.");
        }

        return errorList;
    }

    #endregion
    
}