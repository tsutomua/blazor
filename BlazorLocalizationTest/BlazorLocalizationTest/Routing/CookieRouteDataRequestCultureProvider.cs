using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(routeCulture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(routeCulture);

            var cookieName = CookieRequestCultureProvider.DefaultCookieName;
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);
            httpContext.Response.Cookies.Append(cookieName, cookieValue);

            return Task.FromResult(new ProviderCultureResult(routeCulture));
        }
        // Culture provided in URL
        else if (IsSupportedCulture(urlCulture))
        {
            return Task.FromResult(new ProviderCultureResult(urlCulture));
        }
        else
        // Use default culture
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            var myCookie = httpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            var splitCookieValue = myCookie.Split("|");
            var v2 = splitCookieValue.Select(s => s.Split("=")).ToDictionary(s => s[0], s => s[1]);

            return Task.FromResult(new ProviderCultureResult(v2["uic"]));
            //return Task.FromResult(new ProviderCultureResult(DefaultCulture));
            //return Task.FromResult<ProviderCultureResult>(null);
        }
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

    private string DefaultCulture => Options.DefaultRequestCulture.Culture.TwoLetterISOLanguageName;
}

