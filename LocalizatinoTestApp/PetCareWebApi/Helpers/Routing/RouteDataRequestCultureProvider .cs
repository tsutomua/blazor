using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestLocalization.Helpers.Routing;

public class RouteDataRequestCultureProvider : RequestCultureProvider
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
            return Task.FromResult(new ProviderCultureResult(DefaultCulture));
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

