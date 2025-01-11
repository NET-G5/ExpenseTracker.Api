using Microsoft.AspNetCore.WebUtilities;

namespace ExpenseTracker.Application.Extensions;
public static class Helper
{
    public static string GetCallbackUrl(string clientUrl, string token, string email)
    {
        Dictionary<string, string> queryParams = new Dictionary<string, string>
        {
            { "email", email },
            { "token", token }
        };

        var callbackUrl = QueryHelpers.AddQueryString(clientUrl, queryParams);

        return callbackUrl;
    }
}
