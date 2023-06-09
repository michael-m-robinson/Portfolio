﻿#region Imports

using System.Runtime.InteropServices;
using Portfolio.Extensions;
using Portfolio.Models.Content;
using Portfolio.Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

#endregion

namespace Portfolio.Services;

public class MWSOpenGraphService : IMWSOpenGraphService
{
    private readonly IWebHostEnvironment _hostEnvironment;

    public MWSOpenGraphService(IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    #region Add OpenGraph Post Image

    public async Task AddOpenGraphPostImageAsync(Post post, IFormFile file)
    {
        var contentRootPath = _hostEnvironment.ContentRootPath;
        var postFilePath = string.Empty;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            postFilePath = contentRootPath.Substring(0, contentRootPath.LastIndexOf("/", StringComparison.Ordinal));
            postFilePath = Path.Combine(postFilePath, "ArticleImages/PostImages/");
        }

        else
        {
            postFilePath = contentRootPath.Substring(0, contentRootPath.LastIndexOf("\\", StringComparison.Ordinal));
            postFilePath = Path.Combine(postFilePath, "ArticleImages\\PostImages\\");
        }


        if (Directory.Exists(postFilePath) == false) Directory.CreateDirectory(postFilePath);

        if (file.IsImage())
        {
            var filename = $"{post.Slug}.png";
            var image = await Image.LoadAsync(file.OpenReadStream());
            var completePath = Path.Combine(postFilePath, filename);
            if (File.Exists(completePath)) File.Delete(completePath);

            await image.SaveAsync(completePath, new PngEncoder());
        }
    }

    #endregion

    #region Add OpenGraph Project Image

    public async Task AddOpenGraphProjectImageAsync(Project project, IFormFile file)
    {
        var contentRootPath = _hostEnvironment.ContentRootPath;
        var projectFilePath = string.Empty;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            projectFilePath = contentRootPath.Substring(0, contentRootPath.LastIndexOf("/", StringComparison.Ordinal));
            projectFilePath = Path.Combine(projectFilePath, "ArticleImages/ProjectImages/");
        }

        else
        {
            projectFilePath = contentRootPath.Substring(0, contentRootPath.LastIndexOf("\\", StringComparison.Ordinal));
            projectFilePath = Path.Combine(projectFilePath, "ArticleImages\\ProjectImages\\");
        }

        if (Directory.Exists(projectFilePath) == false) Directory.CreateDirectory(projectFilePath);

        if (file.IsImage())
        {
            var filename = $"{project.Slug}.png";
            var image = await Image.LoadAsync(file.OpenReadStream());
            var completePath = Path.Combine(projectFilePath, filename);
            if (File.Exists(completePath)) File.Delete(completePath);

            await image.SaveAsync(completePath, new PngEncoder());
        }
    }

    #endregion

    #region Delete OpenGraph Post Image

    public void DeleteOpenGraphPostImage(Post post)
    {
        var contentRootPath = _hostEnvironment.ContentRootPath;
        var postFilePath = string.Empty;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            postFilePath = contentRootPath.Substring(0, contentRootPath.LastIndexOf("/", StringComparison.Ordinal));
            postFilePath = Path.Combine(postFilePath, "ArticleImages/PostImages/");
        }

        else
        {
            postFilePath = contentRootPath.Substring(0, contentRootPath.LastIndexOf("\\", StringComparison.Ordinal));
            postFilePath = Path.Combine(postFilePath, "ArticleImages\\PostImages\\");
        }

        var filename = $"{post.Slug}.png";
        var completePath = Path.Combine(postFilePath, filename);

        if (File.Exists(completePath)) File.Delete(completePath);
    }

    #endregion

    #region Delete OpenGraph Project Image

    public void DeleteOpenGraphProjectImage(Project project)
    {
        var contentRootPath = _hostEnvironment.ContentRootPath;
        var projectFilePath = string.Empty;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            projectFilePath = contentRootPath.Substring(0, contentRootPath.LastIndexOf("/", StringComparison.Ordinal));
            projectFilePath = Path.Combine(projectFilePath, "ArticleImages/ProjectImages/");
        }

        else
        {
            projectFilePath = contentRootPath.Substring(0, contentRootPath.LastIndexOf("\\", StringComparison.Ordinal));
            projectFilePath = Path.Combine(projectFilePath, "ArticleImages\\ProjectImages\\");
        }

        var filename = $"{project.Slug}.png";
        var completePath = Path.Combine(projectFilePath, filename);

        if (File.Exists(completePath)) File.Delete(completePath);
    }

    #endregion
}