using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Logging;
using Utils;
using PetCareWebApi.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using TestLocalization.Helpers.Routing;
using Core;
using Helpers.Routing.app.Middleware;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ILogger logger;

        // Add services to the container.

        string correlationId = Guid.NewGuid().ToString();
        IdentityModelEventSource.ShowPII = true;

        // During development, Application Insights should not used to save costs. Change below to log to Application Insights.
        logger = new TestLogger();


        builder.Services.AddSingleton(logger);
        logger.TrackEvent("In ConfigureServices...", correlationId);

        try
        {
            builder.Services.AddHttpContextAccessor();

            // Add services to the container.
            builder.Services.AddRazorPages(options =>
            {
                // decorate all page routes with {culture} e.g. @page "/{culture}..."
                options.Conventions.Add(new CultureTemplatePageRouteModelConvention());
            });

            builder.Services.AddServerSideBlazor();
            builder.Services.AddHttpClient();
            builder.Services.AddControllers();
        }
        catch (Exception ex)
        {
            logger.TrackException(ex, "Error occurred before registering ExceptionHandler.", correlationId);
            throw;
        }

        // AddLocalization must be before builder.Build().
        builder.Services.AddLocalization();

        IStringLocalizer localizer = builder.Services.BuildServiceProvider().GetService<IStringLocalizer<ApplicationResource>>();
        builder.Services.AddSingleton<ITranslationProvider>(c => new ResXTranslationProvider(localizer));

        builder.Services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.AppendTrailingSlash = false;
            //options.ConstraintMap.Add("cultureConstraint", typeof(CultureConstraint));
        });

        List<string> supportedCultureCodes = builder.Configuration.GetStringValueFromConfig(Constants.SupportedCultureCodes)
            .ToStringListFromCommaDelimitedStringEmptyNotAllowed();

        var supportedCultures = supportedCultureCodes.Select(c => new CultureInfo(c)).ToList();
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(culture: "en-us", uiCulture: "en-us");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.FallBackToParentCultures = true;        
            options.RequestCultureProviders.Clear();

            // Error at the bottom
            options.RequestCultureProviders.Insert(0, new RouteDataRequestCultureProvider()
            {
                Options = options,
            });

            //// Sidebar not displayed.
            options.RequestCultureProviders.Insert(0, new CustomRouteDataRequestCultureProvider() { Options = options });
        });


        var app = builder.Build();

        app.Use(new BlazorCultureExtractor().Handle);
        app.UseRequestLocalization();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}