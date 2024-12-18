using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestLocalization.Helpers.Routing;

public class CustomRouteDataRequestCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {

        var routeCulture = (string)httpContext.GetRouteValue("culture");
        var urlCulture = httpContext.Request.Path.Value.Split('/')[1];

        var container = new List<string>() { routeCulture, urlCulture };

        string cultureCode = Options.SupportedCultures.Select(c => c.TwoLetterISOLanguageName).SingleOrDefault(c => container.Contains(c) );

        if (string.IsNullOrWhiteSpace(cultureCode) == false)
        {
            return Task.FromResult(new ProviderCultureResult(cultureCode));
        }

        // if no match, return 404
        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        return Task.FromResult(new ProviderCultureResult(Options.DefaultRequestCulture.Culture.TwoLetterISOLanguageName));
    }
}
