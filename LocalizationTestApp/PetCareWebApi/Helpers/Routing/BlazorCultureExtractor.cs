namespace Helpers.Routing
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Localization;

    namespace app.Middleware
    {
        public class BlazorCultureExtractor
        {
            private static string textFilePath = @"c:\temp\path.txt";

            //private readonly Regex BlazorRequestPattern = new Regex("^/(.*?)(/blazor.*)$");
            //private readonly Regex BlazorRequestPattern = new Regex("^/(.*?)(/_blazor.*)$");
            private readonly Regex BlazorRequestPattern = new Regex(".*blazor.*");
            public async Task Handle(HttpContext context, Func<Task> next)
            {
                using (var fileStream = new FileStream(textFilePath, FileMode.Append))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(context.Request.Path.Value);
                }

                var match = BlazorRequestPattern.Match(context.Request.Path.Value);

                //if (string.IsNullOrEmpty(context.Request.Path.Value) || match.Success == false)
                //{
                //    await next();
                //}

                if (match.Success)
                {

                    // If it's a request for a blazor endpoint
                    // Grab the culture from the URL and store it in RouteValues
                    // This allows IStringLocalizers to use the correct culture in Blazor components
                    context.Request.RouteValues["culture"] = "";// match.Groups[1].Value;
                    // Remove the /culture/ from the URL so that Blazor works properly
                    //context.Request.Path = match.Groups[1].Value;
                    //context.Request.Path = match.Groups[2].Value;

                    if (context.Request.Path == "/_framework/blazor.server.js")
                    {
                        context.Request.Path = "/blazor.server.js";
                    }

                    // Leaving this line uncommented removes the error at the bottom but click event does not fire.
                    //context.Request.Path = match.Groups[2].Value;
                }

                await next();
            }
        }
    }
}
