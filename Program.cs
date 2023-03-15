#region Imports

using System.Reflection;
using System.Runtime.InteropServices;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Portfolio.Data;
using Portfolio.Models;
using Portfolio.Models.Settings;
using Portfolio.Models.ViewModels;
using Portfolio.Services;
using Portfolio.Services.Interfaces;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SmartBreadcrumbs.Extensions;
using System.IO;

#endregion

var webRootDirectory = "ArticleImages";
var webRootPath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions() 
{
    WebRootPath = Path.Combine(webRootPath, webRootDirectory)
});

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

var connectionString = configuration.GetConnectionString("Production");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<BlogUser>>(TokenOptions.DefaultProvider)
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = configuration.GetSection("GoogleAccount").GetSection("client_id").Value!;
        options.ClientSecret = configuration.GetSection("GoogleAccount").GetSection("client_secret").Value!;
        options.ClaimActions.MapJsonKey("GivenName", "given_name");
        options.ClaimActions.MapJsonKey("Surname", "family_name");
        options.ClaimActions.MapJsonKey("picture", "picture");
    });

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<DataService>();

var mailSettings = configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettingsViewModel>(mailSettings);
builder.Services.AddReCaptcha(configuration.GetSection("ReCaptcha"));


builder.Services.AddScoped<IMWSBlogService, MWSBlogService>();
builder.Services.AddScoped<IMWSPostService, MWSPostService>();
builder.Services.AddScoped<IMWSCategoryService, MWSCategoryService>();
builder.Services.AddScoped<IMWSCivilityService, MWSCivilityService>();
builder.Services.AddScoped<IMWSCommentService, MWSCommentService>();
builder.Services.AddScoped<IMWSEmailService, MWSEmailService>();
builder.Services.AddScoped<IMWSImageService, MWSImageService>();
builder.Services.AddScoped<IMWSProjectService, MWSProjectService>();
builder.Services.AddScoped<IMWSProjectImageService, MWSProjectImageService>();
builder.Services.AddScoped<IMWSTagService, MWSTagService>();
builder.Services.AddScoped<IMWSOpenGraphService, MWSOpenGraphService>();
builder.Services.AddImageSharp();
builder.Services.AddBreadcrumbs(Assembly.GetExecutingAssembly(), options =>
{
    options.TagName = "div";
    options.TagClasses = "py-5";
    options.OlClasses = "hp-bre h-list";
    options.LiClasses = string.Empty;
    options.ActiveLiClasses = "text-info";
    options.SeparatorElement = string.Empty;
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseStatusCodePagesWithRedirects("/Error/{0}");
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithRedirects("/Error/{0}");
    app.UseHsts();
}

app.UseResponseCompression();
app.UseHttpsRedirection();

app.UseDefaultFiles();

app.UseImageSharp();

var env = app.Environment;

var osDirectory = string.Empty;
if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) 
    || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    Console.WriteLine("We're on Unix!");
    osDirectory = "wwwroot";
}
else
{
    Console.WriteLine("We're on Windows!");
    osDirectory = "wwwroot";
}

var contentPath = Path.Combine(env.ContentRootPath, osDirectory);

app.Environment.WebRootFileProvider = new CompositeFileProvider(new PhysicalFileProvider(env.WebRootPath), new PhysicalFileProvider(contentPath));

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    "Post-details",
    "Blog/{blogSlug}/Post/{slug}",
    new { Controller = "Posts", Action = "Details" });

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    "delete-category",
    "blogs/DeleteCategory/{categoryName}",
    new { Controller = "Blogs", Action = "DeleteCategory" });


app.MapRazorPages();

var dataService = app.Services
    .CreateScope()
    .ServiceProvider
    .GetRequiredService<DataService>();

await dataService.ManageDataAsync();

app.Run();