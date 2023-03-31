#nullable disable

#region Imports

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Models;
using Portfolio.Models.Content;
using Portfolio.Models.ViewModels;
using Portfolio.Services.Interfaces;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using X.PagedList;

#endregion

namespace Portfolio.Controllers;

[Breadcrumb("Blogs")]
public class BlogsController : Controller
{
    private readonly IMWSValidateService _validateService;
    private readonly IMWSBlogService _blogService;
    private readonly IMWSBlogEntityService _blogEntityService;
    private readonly IMWSCategoryService _categoryService;
    private readonly UserManager<BlogUser> _userManager;

    public BlogsController(UserManager<BlogUser> userManager,
        IMWSBlogService blogService,
        IMWSCategoryService categoryService,
        IMWSValidateService validateService, 
        IMWSBlogEntityService blogEntityService)
    {
        _userManager = userManager;
        _blogService = blogService;
        _categoryService = categoryService;
        _validateService = validateService;
        _blogEntityService = blogEntityService;
    }

    [TempData] public string StatusMessage { get; set; } = default!;

    #region All Blogs get action

    [HttpGet]
    [Route("/Blogs/")]
    [Route("/Blogs/Page/{page}")]
    public async Task<IActionResult> AllBlogs(int? page)
    {
        var blogList = await _blogEntityService.ListAllBlogs(page);
        return View("Index", blogList);
    }

    #endregion

    #region Blog articles get action

    public IActionResult Articles()
    {
        return NotFound();
    }

    #endregion

    #region Blog search post action

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BlogSearch(string term, int? page)
    {
        var pageNumber = page ?? 1;
        var pageSize = 6;
        var blogs = await _blogService.GetBlogsBySearchTerm(term);
        return View("Index", await blogs.ToPagedListAsync(pageNumber, pageSize));
    }

    #endregion

    #region Blog create get action

    [Authorize(Roles = "Administrator")]
    public IActionResult Create()
    {
        var model = new BlogCreateEditViewModel();
        return View(model);
    }

    #endregion

    #region Blog create post action

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] BlogCreateEditViewModel model)
    {
        if (ModelState.IsValid)
        {
            var errorList = await _validateService.ValidateBlogCreateModel(model);
            if (errorList.Count > 0)
            {
                foreach (var error in errorList)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                    model = GetBlogCreateEditViewModelData(model);
                    return View(model);
                }
            }
            await _blogEntityService.CreateBlog(model);
            return RedirectToAction(nameof(Index));
        }

        //Model state is not valid, return user to create view.
        
        ModelState.AddModelError("",
            "There has been an error if this continues, please contact the administrator.");
        model = GetBlogCreateEditViewModelData(model);
        return View(model);
    }

    #endregion

    #region Blog delete action

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if ((await _blogService.GetBlogAsync(id)).Id == new Guid()) return NotFound();
        var blog = await _blogService.GetBlogAsync(id);
        return View(blog);
    }

    #endregion

    #region Category delete get action

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    [Route("/Blogs/DeleteCategory/{name}/Blog/{blogId}")]
    public async Task<IActionResult> DeleteCategory(string name, Guid blogId)
    {
        if (blogId == new Guid() || blogId == Guid.Empty ||
            (await _blogService.GetBlogAsync(blogId)).Id == new Guid()) return NotFound();

        var category = await _categoryService.GetCategoryByNameAsync(blogId, name);
        if (category.Id == new Guid() || category.Id == Guid.Empty) return NotFound();

        if (category.Name.ToLower() == "all posts") return RedirectToAction("Edit", new { id = category.BlogId });

        return View(category);
    }

    #endregion

    #region Category delete post action

    [HttpPost]
    [ActionName("DeleteCategory")]
    [Route("/Blogs/DeleteCategory/{id}/Blog/{blogId}")]
    [Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategoryConfirm(Guid id, Guid blogId)
    {
        if (blogId == new Guid() || blogId == Guid.Empty || id == new Guid() || id == Guid.Empty) return NotFound();
        var categoryToDelete = await _categoryService.GetCategoryByIdAsync(blogId, id);
        if (categoryToDelete.Id == new Guid()) return NotFound();

        await _categoryService.MovePostsToDefaultCategoryAsync(blogId, categoryToDelete);
        await _categoryService.DeleteCategoryAsync(categoryToDelete);

        return RedirectToAction("Edit", new { id = blogId });
    }

    #endregion

    #region Blog delete post action

    [HttpPost]
    [ActionName("Delete")]
    [Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if ((await _blogService.GetBlogAsync(id)).Id == new Guid()) return NotFound();

        var blog = await _blogService.GetBlogAsync(id);
        await _blogService.DeleteBlogAsync(blog);

        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region Blog details get action

    [HttpGet]
    [Route("/Blog/{slug}")]
    [Route("/Blog/{slug}/Page/{page}")]
    public async Task<IActionResult> Details(string slug, int? page)
    {
        var currentBlog = await _blogService.GetBlogBySlugAsync(slug);
        if (currentBlog.Id == new Guid()) return NotFound();
        
        SetBlogDetailsBreadCrumbs(currentBlog);
        var model = await _blogEntityService.ListBlog(currentBlog, page);
        
        return View(model);
    }

    #endregion

    #region Blog category details get action

    [HttpGet]
    [Route("/Blog/{slug}/Category/{categoryName}/Page/{page}")]
    [Breadcrumb("Category")]
    public async Task<IActionResult> DetailsByCategory(string slug, int? page, string categoryName)
    {
        var currentBlog = await _blogService.GetBlogBySlugAsync(slug);
        var model = await _blogEntityService.ListBlogByCategory(currentBlog, page, categoryName);
        
        if (ModelState.IsValid)
        {
            if (currentBlog.Id == new Guid()) return NotFound();
            if (string.IsNullOrEmpty(categoryName)) return NotFound();
            SetBlogCategoryDetailsBreadCrumbs(model.Blog, slug, page, categoryName);
            return View("Details", model);
        }

        TempData["StatusMessage"] = "There is an error, if this continues, please contact the administrator.";
        return View("Details", model);
    }

    #endregion

    #region Blog edit get action

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit(Guid id)
    {
        if ((await _blogService.GetBlogAsync(id)).Id == new Guid()) return NotFound();
        var model = new BlogCreateEditViewModel
        {
            Blog = await _blogService.GetBlogAsync(id)
        };

        model.CategoryValues = model.Blog.Categories!.Select(c => c.Name).OrderBy(c => c).ToList();
        model.AuthorId = _userManager.GetUserId(User);
        return View(model);
    }

    #endregion

    #region Blog edit post action

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [FromForm] BlogCreateEditViewModel model)
    {
        if ((await _blogService.GetBlogAsync(id)).Id == new Guid()) return NotFound();
        if (ModelState.IsValid)
        {
            model.Blog.Id = id;
            var errorList = await _validateService.ValidateBlogEditModel(model);
            if (errorList.Count > 0)
            {
                foreach (var error in errorList)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                    model = GetBlogCreateEditViewModelData(model);
                    return View(model);
                }
            }
            await _blogEntityService.EditBlog(model, id);
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("",
            "There has been an error if this continues, please contact the administrator.");
        model = GetBlogCreateEditViewModelData(model);
        return View(model);
    }

    #endregion

    #region Blog author index get action

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAuthorBlogs(int? page)
    {
        var pageNumber = page ?? 1;
        var pageSize = 6;
        var currentUserId = _userManager.GetUserId(User);
        var blogs = await _blogService.GetBlogsByAuthorAsync(currentUserId!);

        return View("Index", await blogs.ToPagedListAsync(pageNumber, pageSize));
    }

    #endregion

    #region Blog categories get action

    [HttpGet]
    public async Task<IActionResult> GetCategories(Guid id)
    {
        var categoryList = await _categoryService.GetCategoriesAsync(id);
        return Json(new { categoryListJson = categoryList });
    }

    #endregion

    #region Blog index get action

    [HttpGet]
    public IActionResult Index()
    {
        return NotFound();
    }

    #endregion

    #region Blog post-search get action

    [HttpGet]
    [Route("Blogs/PostSearch")]
    [Route("Blogs/{slug}/PostSearch/{term}/Page/{page}")]
    public async Task<IActionResult> PostSearch(string term, string slug, int? page)
    {
        if (term == null) return BadRequest();
        var model = await _blogEntityService.ListBlogPostsBySearch(term, slug, page);
        if (model.Blog.Id == new Guid()) return NotFound();
        SetBlogPostSearchBreadCrumbs(model.Blog, slug, page, term);
        ViewBag.SearchTerm = term;
        
        return View("Details", model);
    }

    #endregion

    #region Blog post-search post action

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("PostSearch")]
    public async Task<IActionResult> PostSearchPost(string term, string slug, int? page)
    {
        if (term == null) return BadRequest();
        var model = await _blogEntityService.ListBlogPostsBySearch(term, slug, page);
        if (model.Blog.Id == new Guid()) return NotFound();
        SetBlogPostSearchBreadCrumbs(model.Blog, slug, page, term);
        ViewBag.SearchTerm = term;

        return View("Details", model);
    }

    #endregion

    #region Blog Tag get action

    [HttpGet]
    [Route("/Blog/{slug}/Tag/{tag}/Page/{page}")]
    public async Task<IActionResult> Tag(int? page, string tag, string slug)
    {
        ViewBag.Tag = tag;
        var model = await _blogEntityService.ListBlogByTag(tag, slug, page);
        SetBlogTagBreadCrumbs(model.Blog, tag, slug);
        return View("Details", model);
    }

    #endregion

    private BlogCreateEditViewModel GetBlogCreateEditViewModelData(BlogCreateEditViewModel model)
    {
        model.ImageFile = default!;
        model.Blog.Image = null;
        model.Blog.ImageType = null;

        return model;
    }

    private void SetBlogDetailsBreadCrumbs(Blog model)
    {
        var blogsNode = new MvcBreadcrumbNode("AllBlogs", "Blogs", "Blogs");
        var blogNode = new MvcBreadcrumbNode("Details", "Blogs", model.Name)
        {
            RouteValues = new { slug = model.Slug },
            Parent = blogsNode
        };
        
        ViewData["BreadcrumbNode"] = blogNode;
    }

    private void SetBlogCategoryDetailsBreadCrumbs(Blog model, string slug, int? page, string categoryName)
    {
        var blogsNode = new MvcBreadcrumbNode("AllBlogs", "Blogs", "Blogs");

        var blogNode = new MvcBreadcrumbNode("Details", "Blogs", model.Name)
        {
            RouteValues = new { slug = model.Slug },
            Parent = blogsNode
        };

        var categoryNode = new MvcBreadcrumbNode("DetailsByCategory", "Blogs", categoryName)
        {
            RouteValues = new { slug, page, categoryName },
            Parent = blogNode
        };

        ViewData["BreadcrumbNode"] = categoryNode;
    }

    private void SetBlogPostSearchBreadCrumbs(Blog model, string slug, int? page, string term)
    {
        var blogNode = new MvcBreadcrumbNode("Details", "Blogs", model.Name)
        {
            RouteValues = new { slug = model.Slug }
        };

        var searchNode = new MvcBreadcrumbNode("PostSearch", "Blogs", term)
        {
            RouteValues = new { term, slug, page },
            Parent = blogNode
        };

        ViewData["BreadcrumbNode"] = searchNode;
    }

    private void SetBlogTagBreadCrumbs(Blog model, string tag, string slug)
    {
        var blogsNode = new MvcBreadcrumbNode("AllBlogs", "Blogs", "Blogs");

        var blogNode = new MvcBreadcrumbNode("Details", "Blogs", model.Name)
        {
            RouteValues = new { slug = model.Slug },
            Parent = blogsNode
        };

        var tagNode = new MvcBreadcrumbNode("Tag", "Blogs", tag)
        {
            RouteValues = new { tag, slug },
            Parent = blogNode
        };

        ViewData["BreadcrumbNode"] = tagNode;
    }
}