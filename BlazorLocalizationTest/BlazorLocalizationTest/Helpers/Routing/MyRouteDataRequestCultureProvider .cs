using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestLocalization.Helpers.Routing;

public class MyRouteDataRequestCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        string routeCulture = (string)httpContext.Request.RouteValues["culture"];
        string urlCulture = httpContext.Request.Path.Value.Split('/')[1];

        // Culture provided in route values
        if (IsSupportedCulture(routeCulture))
        {
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
            var culture = Thread.CurrentThread.CurrentCulture;

            return Task.FromResult(new ProviderCultureResult("es"));
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

