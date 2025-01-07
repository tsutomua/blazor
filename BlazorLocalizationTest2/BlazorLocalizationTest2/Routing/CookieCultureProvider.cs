using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace BlazorLocalizationTest.Routing;

public class CookieCultureProvider 
{
    public CookieCultureProvider(List<CultureInfo> supportedCultures)
    {
        SupportedCultures = supportedCultures;
    }

    public string GetCultureFromCookie(HttpContext httpContext)
    {
        string cultureFromCookieOrDefault;
        string? cultureCookie = httpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];

        if (string.IsNullOrEmpty(cultureCookie))
        {
            cultureFromCookieOrDefault = DefaultCulture;
        }
        else
        {
            Dictionary<string, string> cookieCultureDictionary = cultureCookie.Split("|")
                .Select(s => s.Split("="))
                .ToDictionary(s => s[0], s => s[1]);

            if (cookieCultureDictionary.ContainsKey("uic") && IsSupportedCulture(cookieCultureDictionary["uic"]))
            {
                cultureFromCookieOrDefault = cookieCultureDictionary["uic"];
            }
            else if (cookieCultureDictionary.ContainsKey("c") && IsSupportedCulture(cookieCultureDictionary["c"]))
            {
                cultureFromCookieOrDefault = cookieCultureDictionary["c"];
            }
            else
            {
                cultureFromCookieOrDefault = DefaultCulture;
            }
        }

        return cultureFromCookieOrDefault;
    }

    /**
     * Culture must be in the list of supported cultures
     */
    private bool IsSupportedCulture(string cultureCode) =>
        !string.IsNullOrEmpty(cultureCode)
        && SupportedCultures.Any(x =>
            x.TwoLetterISOLanguageName.Equals(
                cultureCode,
                StringComparison.InvariantCultureIgnoreCase
            )
        );

    private string DefaultCulture => "en-US";

    public List<CultureInfo> SupportedCultures { get; }
}

