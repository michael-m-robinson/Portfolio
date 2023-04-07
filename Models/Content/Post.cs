#region Imports

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Portfolio.Enums;
using Portfolio.Models.Filters;

#endregion

namespace Portfolio.Models.Content;

public class Post
{
    [Key] public Guid Id { get; set; }
    [Display(Name = "Blog Name")] public Guid BlogId { get; set; } = new();
    public Guid CategoryId { get; set; } = new();
    public string AuthorId { get; set; } = string.Empty;
    [JsonIgnore] public ReadyStatus ReadyStatus { get; set; }
    public string Slug { get; set; } = string.Empty;
    public byte[] Image { get; set; } = new byte[] { };
    public byte[] ThumbNail { get; set; } = new byte[] {};
    public string ImageType { get; set; } = string.Empty;
    [NotMapped] public string? Base64PostPicture { get; set; }

    [Required]
    [StringLength(75, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long.",
        MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(200, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long.",
        MinimumLength = 2)]
    public string Abstract { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Created Date")]
    public DateTimeOffset Created { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Updated Date")]
    public DateTimeOffset? Updated { get; set; }

    // Navigation Properties

    //1:M - One blog has many posts
    [Display(Name = "Blog")] public virtual Blog Blog { get; set; } = new();

    //1:M - One user can have many blogs
    [Display(Name = "Author")] public virtual BlogUser Author { get; set; } = new();

    //1:1 - One post has one category.
    public virtual Category? Category { get; set; }

    //1:M - One post has many tags.
    [JsonIgnore] public virtual ICollection<Tag>? Tags { get; set; }

    //1:M - One post has many comments
    public virtual ICollection<Comment>? Comments { get; set; }
}