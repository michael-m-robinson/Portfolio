#region Imports

using System.ComponentModel.DataAnnotations;

#endregion

namespace Portfolio.Enums;

public enum ProjectCategories
{
    [Display(Name = "JavaScript")] Html,
    [Display(Name = "DOT-NET")] Net,
    [Display(Name = "Python")] Python
}