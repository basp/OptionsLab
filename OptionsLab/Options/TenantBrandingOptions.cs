using System.ComponentModel.DataAnnotations;

namespace OptionsLab.Options;

public sealed class TenantBrandingOptions
{
    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string TenantName { get; set; } = "Options Lab";

    [Required]
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "PrimaryColor must be a six-digit hex color.")]
    public string PrimaryColor { get; set; } = "#2563EB";

    [Url]
    public string? SupportUrl { get; set; }
}
