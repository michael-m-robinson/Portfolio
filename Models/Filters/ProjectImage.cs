﻿#region Imports

using Portfolio.Models.Content;
using System.ComponentModel.DataAnnotations;

#endregion

namespace Portfolio.Models.Filters;

public class ProjectImage
{
    [Key] public Guid Id { get; set; }

    public byte[] File { get; set; } = default!;
    public string FileContentType { get; set; } = default!;
    public string Name { get; set; } = default!;
    public Guid ProjectId { get; set; }

    //Navigational Properties
    public virtual Project Project { get; set; } = default!;
}