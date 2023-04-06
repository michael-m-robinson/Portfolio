using Portfolio.Models.ViewModels;
using Portfolio.Services.Interfaces;

namespace Portfolio.Services;

public class MWSValidateService: IMWSValidateService
{
    private readonly IMWSCivilityService _civilityService;
    private readonly IMWSBlogService _blogService;
    private readonly IMWSCategoryService _categoryService;
    
    public MWSValidateService(IMWSCivilityService civilityService, IMWSBlogService blogService, IMWSCategoryService categoryService)
    {
        _civilityService = civilityService;
        _blogService = blogService;
        _categoryService = categoryService;
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
}