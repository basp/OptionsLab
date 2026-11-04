using System.ComponentModel.DataAnnotations;

namespace OptionsLab.Options;

public sealed class NotificationOptions
{
    [Required]
    [RegularExpression("^(Email|Sms)$", ErrorMessage = "Provider must be Email or Sms.")]
    public string Provider { get; init; } = "Email";

    [Required]
    [StringLength(254, MinimumLength = 3)]
    public string SenderAddress { get; init; } = "notifications@example.test";

    [EmailAddress]
    public string? EmailFromAddress { get; init; }

    [StringLength(11, MinimumLength = 3)]
    public string? SmsSenderId { get; init; }
}
