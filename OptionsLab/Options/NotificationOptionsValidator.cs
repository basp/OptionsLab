using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace OptionsLab.Options;

public sealed class NotificationOptionsValidator : IValidateOptions<NotificationOptions>
{
    public ValidateOptionsResult Validate(string? name, NotificationOptions options)
    {
        var failures = new List<string>();

        if (options.Provider == "Email")
        {
            if (string.IsNullOrWhiteSpace(options.EmailFromAddress))
            {
                failures.Add("Email profiles require EmailFromAddress.");
            }
            else if (!new EmailAddressAttribute().IsValid(options.EmailFromAddress))
            {
                failures.Add("EmailFromAddress must be a valid email address.");
            }

            if (!string.IsNullOrWhiteSpace(options.SmsSenderId))
            {
                failures.Add("Email profiles cannot specify SmsSenderId.");
            }
        }

        if (options.Provider == "Sms")
        {
            if (string.IsNullOrWhiteSpace(options.SmsSenderId))
            {
                failures.Add("SMS profiles require SmsSenderId.");
            }

            if (!string.IsNullOrWhiteSpace(options.EmailFromAddress))
            {
                failures.Add("SMS profiles cannot specify EmailFromAddress.");
            }
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
