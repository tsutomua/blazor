namespace Helpers.Routing
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Localization;

    namespace app.Middleware
    {
        public class BlazorCultureExtractor
        {
            //private readonly Regex BlazorRequestPattern = new Regex("^/(.*?)(/_blazor.*)$");
            private readonly Regex BlazorRequestPattern = new Regex(".*blazor.*");
            public async Task Handle(HttpContext context, Func<Task> next)
            {
                var match = BlazorRequestPattern.Match(context.Request.Path.Value);

                if (string.IsNullOrEmpty(context.Request.Path.Value) || match.Success == false)
                {
                    await next();
                }

                // If it's a request for a blazor endpoint
                // Grab the culture from the URL and store it in RouteValues
                // This allows IStringLocalizers to use the correct culture in Blazor components
                context.Request.RouteValues["culture"] = match.Groups[1].Value;
                // Remove the /culture/ from the URL so that Blazor works properly

                context.Request.Path = match.Groups[2].Value;


                await next();
            }
        }
    }
}
