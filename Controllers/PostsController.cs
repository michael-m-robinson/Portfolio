#nullable disable

#region Imports

using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Portfolio.Extensions;
using Portfolio.Models;
using Portfolio.Models.Content;
using Portfolio.Models.ViewModels;
using Portfolio.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using X.PagedList;
using static System.Runtime.InteropServices.JavaScript.JSType;

#endregion

namespace Portfolio.Controllers;

[Breadcrumb]
public class PostsController : Controller
{
    private readonly IMWSBlogService _blogService;
    private readonly IMWSCategoryService _categoryService;
    private readonly IMWSCivilityService _civilityService;
    private readonly IMWSCommentService _commentService;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IMWSImageService _imageService;
    private readonly IMWSOpenGraphService _openGraphService;
    private readonly IMWSPostService _postService;
    private readonly IMWSTagService _tagService;
    private readonly UserManager<BlogUser> _userManager;
    private readonly IMWSValidateService _validateService;
    private readonly IMWSPostEntityService _postEntityService;

    public PostsController(UserManager<BlogUser> userManager,
        IMWSPostService postService,
        IMWSBlogService blogService,
        IMWSImageService imageService,
        IMWSTagService tagService,
        IMWSCommentService commentService,
        IMWSCivilityService civilityService,
        IMWSCategoryService categoryService,
        IMWSOpenGraphService openGraphService,
        IWebHostEnvironment hostEnvironment,
        IMWSValidateService validateService, 
        IMWSPostEntityService postEntityService)
    {
        _userManager = userManager;
        _postService = postService;
        _blogService = blogService;
        _imageService = imageService;
        _tagService = tagService;
        _commentService = commentService;
        _civilityService = civilityService;
        _categoryService = categoryService;
        _openGraphService = openGraphService;
        _hostEnvironment = hostEnvironment;
        _validateService = validateService;
        _postEntityService = postEntityService;
    }

    [TempData] public string StatusMessage { get; set; } = default!;

    #region Add Comment Post Action

    [ValidateReCaptcha]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPostComment([Bind("Id,PostId,AuthorId,Body")] Comment comment,
        string redirectPostSlug, string redirectBlogSlug)
    {
        if (User.Identity!.IsAuthenticated)
        {
            if (ModelState.ContainsKey("Recaptcha") &&
                ModelState["Recaptcha"]!.ValidationState == ModelValidationState.Invalid)
            {
                StatusMessage = "Error: Error verifying reCaptcha, please try again.";
                TempData["Comment"] = comment.Body;
                return RedirectToAction(nameof(Details), new { slug = redirectPostSlug, blogSlug = redirectBlogSlug });
            }

            if (string.IsNullOrEmpty(comment.Body))
            {
                StatusMessage = "Error: A comment cannot be empty";
                TempData["Comment"] = comment.Body;
                return RedirectToAction(nameof(Details), new { slug = redirectPostSlug, blogSlug = redirectBlogSlug });
            }

            comment.Created = DateTimeOffset.Now;
            await _commentService.AddCommentAsync(comment);
            return RedirectToAction(nameof(Details), new { slug = redirectPostSlug, blogSlug = redirectBlogSlug });
        }

        StatusMessage = "Error: you must be logged in to post a comment.";
        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region All author posts get action

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> AllAuthorPosts(int? page)
    {
        var currentUserId = _userManager.GetUserId(User);
        var pageNumber = page ?? 1;
        var pageSize = 10;
        var posts = await _postService.GetPostsByUserIdAsync(currentUserId);

        return View("AuthorIndex", await posts.ToPagedListAsync(pageNumber, pageSize));
    }

    #endregion

    #region Author index action

    public IActionResult AuthorIndex()
    {
        return NotFound();
    }

    #endregion

    #region Post create get action

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create()
    {
        var model = new PostCreateViewModel();
        var userId = _userManager.GetUserId(User);
        var blog = await _blogService.GetBlogsByAuthorAsync(userId);
        if (blog.Any())
        {
            model.BlogSelectList = new SelectList(blog, "Id", "Name", blog.FirstOrDefault());
            model.CategorySelectList =
                new SelectList(await _categoryService.GetCategoriesAsync(blog.FirstOrDefault()!.Id), "Id", "Name");

            return View(model);
        }

        return NotFound();
    }

    #endregion

    #region Post create post action

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] PostCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            var errorList = await _validateService.ValidatePostCreateModel(model);
            if (errorList.Count > 0)
            {
                foreach (var error in errorList)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                    model = await GetPostCreateViewModelData(model);
                    return View(model);
                }
            }

            await _postEntityService.CreatePost(model);
            return RedirectToAction(nameof(Index));
        }

        model = await GetPostCreateViewModelData(model);
        return View(model);
    }

    #endregion

    #region Post delete get action

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if ((await _postService.GetPostByIdAsync(id)).Id == new Guid()) return NotFound();
        var post = await _postService.GetPostByIdAsync(id);
        return View(post);
    }

    #endregion

    #region Post delete post action

    [HttpPost]
    [ActionName("Delete")]
    [Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        if ((await _postService.GetPostByIdAsync(id)).Id == new Guid()) return NotFound();
        var post = await _postService.GetPostByIdAsync(id);
        await _postService.DeletePostAsync(post);
        _openGraphService.DeleteOpenGraphPostImage(post);
        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region Post details get action

    [HttpGet]
    public async Task<IActionResult> Details(string slug, string blogSlug)
    {
        if (string.IsNullOrEmpty(slug)) return BadRequest();
        if ((await _postService.GetPostBySlugAsync(slug, blogSlug)).Id == new Guid()) return NotFound();

        var model = await _postEntityService.ListPost(slug, blogSlug);
        SetPostDetailsBreadCrumbs(model, slug, blogSlug);
        
        return View(model);
    }

    #endregion

    #region Post edit get action

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit(Guid id)
    {
        if ((await _postService.GetPostByIdAsync(id)).Id == new Guid()) return NotFound();

        var model = new PostEditViewModel();
        var userId = _userManager.GetUserId(User);
        var blog = await _blogService.GetBlogsByAuthorAsync(userId!);
        model.Post = await _postService.GetPostByIdAsync(id);
        if (blog.FirstOrDefault() is not null)
        {
            if (model.Post?.Tags is not null)
            {
                foreach (var tag in model.Post.Tags) model.TagValues!.Add(tag.Text);

                model.Tags = string.Join(",", model.TagValues!);
            }

            return View(model);
        }
        return NotFound();
    }

    #endregion

    #region Post edit post action

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [FromForm] PostEditViewModel model)
    {
        if ((await _postService.GetPostByIdAsync(id)).Id == new Guid()) return NotFound();
        if (ModelState.IsValid)
        {
            var errorList = await _validateService.ValidatePostEditModel(model);
            if (errorList.Count > 0)
            {
                foreach (var error in errorList)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                    model = GetPostEditViewModelData(model);
                    return View(model);
                }
            }

            await _postEntityService.EditPost(model, id);
            return RedirectToAction(nameof(Index));
        }

        model = GetPostEditViewModelData(model);
        return View(model);
    }

    #endregion

    #region Post index get action

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Index(int? page)
    {
        var pageNumber = page ?? 1;
        var pageSize = 6;
        var posts = await _postService.GetAllPosts();
        var model = new PostIndexViewModel
        {
            Posts = await posts.ToPagedListAsync(pageNumber, pageSize),
            RecentArticles = await posts.OrderByDescending(p => p.Created).Take(9).ToListAsync()
        };

        return View(model);
    }

    #endregion

    #region Search post action

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Search(string term, int? page)
    {
        var blogUserId = _userManager.GetUserId(User);
        var pageNumber = page ?? 1;
        var pageSize = 10;
        var posts = await _postService.GetPostsByUserIdAsync(blogUserId!);

        var result = posts
            .Where(p => p.Title.ToLower().Contains(term.ToLower()));

        //return posts to the author index view.
        return View("AuthorIndex", await result.ToPagedListAsync(pageNumber, pageSize));
    }

    #endregion

    #region UploadPostImageFile post action

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadPostImageFile(IFormFile file)
    {
        var contentRootPath = _hostEnvironment.ContentRootPath;
        var postFilePath = contentRootPath.Substring(0, contentRootPath.LastIndexOf("\\", StringComparison.Ordinal));
        postFilePath = Path.Combine(postFilePath, "ArticleImages\\Repository\\");

        if (Directory.Exists(postFilePath) == false) Directory.CreateDirectory(postFilePath);

        if (file.IsImage())
            try
            {
                var guid = Guid.NewGuid();
                var filename = $"{guid}.png";
                var image = await Image.LoadAsync(file.OpenReadStream());
                var imageUrl = $"https://{HttpContext.Request.Host}/ArticleImages/Repository/{filename}";
                var completePath = Path.Combine(postFilePath, filename);
                await image.SaveAsync(completePath, new PngEncoder());
                var result = new { success = 1, location = imageUrl };

                return Json(result);
            }
            catch
            {
                var result = new { success = 0, location = string.Empty };
                return Json(result);
            }

        return Json(new { success = 0, file = new { url = string.Empty } });
    }

    #endregion

    private async Task<PostCreateViewModel> GetPostCreateViewModelData(PostCreateViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        var blog = await _blogService.GetBlogsByAuthorAsync(userId!);
        var lastSelectedBlog = await _blogService.GetBlogAsync(model.Post!.BlogId);

        model.BlogSelectList = new SelectList(blog, "Id", "Name", lastSelectedBlog);

        model.CategorySelectList =
            new SelectList(await _categoryService.GetCategoriesAsync(lastSelectedBlog.Id), "Id", "Name");

        model.ImageFile = default!;

        return model;
    }

    private PostEditViewModel GetPostEditViewModelData(PostEditViewModel model)
    {
        model.ImageFile = default!;
        return model;
    }

    private void SetPostDetailsBreadCrumbs(PostIndexViewModel model, string slug, string blogSlug)
    {
        var blogsNode = new MvcBreadcrumbNode("AllBlogs", "Blogs", "Blogs");

        var blogNode = new MvcBreadcrumbNode("Details", "Blogs", model.Post.Blog?.Name)
        {
            RouteValues = new { slug = model.Post.Blog?.Slug },
            Parent = blogsNode
        };

        var postNode = new MvcBreadcrumbNode("Details", "Posts", model.Post.Title)
        {
            RouteValues = new { slug, blogSlug },
            Parent = blogNode
        };

        ViewData["BreadcrumbNode"] = postNode;
    }
}