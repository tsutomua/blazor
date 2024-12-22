using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace BlazorLocalizationTest.Routing;

public class CookieRouteDataRequestCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        string previousRouteCulture;
        string routeCulture = (string)httpContext.Request.RouteValues["culture"];
        string urlCulture = httpContext.Request.Path.Value.Split('/')[1];

        // Culture provided in route values
        if (IsSupportedCulture(routeCulture))
        {
            previousRouteCulture = routeCulture;
            RequestCulture requestCulture = new RequestCulture(routeCulture, routeCulture);

            SetThreadCulture(routeCulture);

            string cookieName = CookieRequestCultureProvider.DefaultCookieName;
            string cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);
            httpContext.Response.Cookies.Append(cookieName, cookieValue);

            return Task.FromResult(new ProviderCultureResult(routeCulture));
        }
        // Culture provided in URL
        else if (IsSupportedCulture(urlCulture))
        {
            SetThreadCulture(urlCulture);

            return Task.FromResult(new ProviderCultureResult(urlCulture));
        }
        else
        {
            CookieCultureProvider cookieCultureProvider = new CookieCultureProvider(Options.SupportedCultures.ToList());
            string cultureFromCookieOrDefault = cookieCultureProvider.GetCultureFromCookie(httpContext);
            SetThreadCulture(cultureFromCookieOrDefault);

            return Task.FromResult(new ProviderCultureResult(cultureFromCookieOrDefault));
        }
    }

    private static void SetThreadCulture(string routeCulture)
    {
        CultureInfo cultureInfo = CultureInfo.GetCultureInfo(routeCulture);
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(routeCulture);
    }

    /**
     * Culture must be in the list of supported cultures
     */
    private bool IsSupportedCulture(string cultureCode) =>
        !string.IsNullOrEmpty(cultureCode)
        && Options.SupportedCultures.Any(x =>
            x.TwoLetterISOLanguageName.Equals(
                cultureCode,
                StringComparison.InvariantCultureIgnoreCase
            )
        );
}

