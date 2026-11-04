using Microsoft.Extensions.Options;

namespace OptionsLab.Options;

public sealed class TenantBrandingPostConfigure : IPostConfigureOptions<TenantBrandingOptions>
{
    public void PostConfigure(string? name, TenantBrandingOptions options)
    {
        options.TenantName = options.TenantName.Trim();
        options.PrimaryColor = options.PrimaryColor.ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(options.SupportUrl))
        {
            var slug = string.Concat(options.TenantName
                .ToLowerInvariant()
                .Select(character => char.IsLetterOrDigit(character) ? character : '-'))
                .Trim('-');

            options.SupportUrl = $"https://support.example.test/{slug}";
        }
    }
}
