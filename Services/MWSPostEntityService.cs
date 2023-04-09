#region Imports

using System.Net;
using System.Web;
using Portfolio.Extensions;
using Portfolio.Models.Content;
using Portfolio.Models.ViewModels;
using Portfolio.Services.Interfaces;
using X.PagedList;

#endregion

namespace Portfolio.Services;

public class MWSPostEntityService : IMWSPostEntityService
{
    private readonly IMWSImageService _imageService;
    private readonly IMWSOpenGraphService _openGraphService;
    private readonly IMWSPostService _postService;
    private readonly IMWSTagService _tagService;

    public MWSPostEntityService(IMWSImageService imageService,
        IMWSOpenGraphService openGraphService,
        IMWSPostService postService,
        IMWSTagService tagService)
    {
        _imageService = imageService;
        _openGraphService = openGraphService;
        _postService = postService;
        _tagService = tagService;
    }

    #region Create Post

    public async Task CreatePost(PostCreateViewModel model)
    {
        model.Post.Slug = model.Post.Title.Slugify();
        model.Post.Created = DateTimeOffset.Now;
        model.Post.Image = await _imageService.EncodeImageAsync(model.ImageFile);
        model.Post.ThumbNail = await _imageService.CreateThumbnailAsync(model.ImageFile);
        model.Post.ImageType = model.ImageFile.ContentType;
        model.Post.Content = HttpUtility.HtmlEncode(model.Post.Content);

        await _openGraphService.AddOpenGraphPostImageAsync(model.Post, model.ImageFile);
        await _postService.AddPostAsync(model.Post);
        await _tagService.AddTagsAsync(model.Post, model.TagValues!);
    }

    #endregion

    #region Edit Post

    public async Task EditPost(PostEditViewModel model, Guid id)
    {
        var postToUpdate = await _postService.GetPostByIdAsync(id);
        var updatedPost = UpdatePostProperties(model.Post!, postToUpdate);

        if (model.ImageFile is not null)
        {
            updatedPost.Image = await _imageService.EncodeImageAsync(model.ImageFile);
            updatedPost.ThumbNail = await _imageService.CreateThumbnailAsync(model.ImageFile);
            updatedPost.ImageType = model.ImageFile.ContentType;
        }

        updatedPost.Slug = model.Post!.Title.Slugify();
        if (model.ImageFile is not null)
            await _openGraphService.AddOpenGraphPostImageAsync(updatedPost, model.ImageFile);

        await _postService.UpdatePostAsync(updatedPost);
        await _tagService.RemoveStaleTagsAsync(updatedPost);
        await _tagService.AddTagsAsync(updatedPost, model.TagValues!);
    }

    #endregion

    #region List Post

    public async Task<PostIndexViewModel> ListPost(string slug, string blogSlug)
    {
        var model = new PostIndexViewModel
        {
            Post = await _postService.GetPostBySlugAsync(slug, blogSlug)
        };

        model.PostId = model.Post.Id;
        model.Post.Content = WebUtility.HtmlDecode(model.Post.Content);
        model.RecentArticles = await _postService.GetTopFivePostsByDateAsync(model.Post.BlogId);
        var tagList = await _tagService.GetTopTwentyBlogTagsAsync(model.Post.BlogId);
        model.BlogTags = await tagList.Select(t => t.Text).Distinct().ToListAsync();

        return model;
    }

    #endregion

    private Post UpdatePostProperties(Post basePost, Post postToUpdate)
    {
        postToUpdate.Updated = DateTimeOffset.Now;
        postToUpdate.Content = basePost.Content;
        postToUpdate.Abstract = basePost.Abstract;
        postToUpdate.AuthorId = basePost.AuthorId;
        postToUpdate.CategoryId = basePost.CategoryId;
        postToUpdate.BlogId = basePost.BlogId;
        postToUpdate.ReadyStatus = basePost.ReadyStatus;
        postToUpdate.Title = basePost.Title;
        postToUpdate.Id = basePost.Id;
        postToUpdate.Tags = basePost.Tags;

        return postToUpdate;
    }
}