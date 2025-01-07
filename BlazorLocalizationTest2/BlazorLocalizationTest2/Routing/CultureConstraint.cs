using System.Globalization;
using Utils;

namespace BlazorLocalizationTest.Routing;

public class CultureConstraint : IRouteConstraint
{
    public bool Match(
        HttpContext? httpContext, IRouter? route, string routeKey,
        RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (!values.TryGetValue(routeKey, out var routeValue))
        {
            return false;
        }


        List<string> supportedCultures = httpContext.RequestServices.GetService<IConfiguration>()
            .GetValue<string>(Constants.SupportedCultureCodes)
            .ToStringListFromCommaDelimitedStringEmptyNotAllowed();

        var routeValueString = Convert.ToString(routeValue, CultureInfo.InvariantCulture);

        return supportedCultures.Any(c => routeValueString == c);
    }
}