using BlazorLocalizationTest.Components;
using BlazorLocalizationTest.Localization;
using BlazorLocalizationTest.Resources;
using BlazorLocalizationTest.Routing;
using Core;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

Utils.ILogger logger;

// Add services to the container.

string correlationId = Guid.NewGuid().ToString();
//IdentityModelEventSource.ShowPII = true;

// During development, Application Insights should not used to save costs. Change below to log to Application Insights.
logger = new TestLogger();


builder.Services.AddSingleton(logger);

try
{
    builder.Services.AddHttpContextAccessor();

    // Add services to the container.
    builder.Services.AddRazorPages(options =>
    {
        // decorate all page routes with {culture} e.g. @page "/{culture}..."

        // https://www.learnrazorpages.com/razor-pages/routing#register-additional-routes
        // Commenting out the following line causes pages not to be localized, but the bottom error message disappears.
        options.Conventions.Add(new CultureTemplatePageRouteModelConvention());
    });

    builder.Services.AddServerSideBlazor();
    builder.Services.AddHttpClient();
    builder.Services.AddControllers();
}
catch (Exception ex)
{
    //logger.TrackException(ex, "Error occurred before registering ExceptionHandler.", correlationId);
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
    options.ConstraintMap.Add("cultureConstraint", typeof(CultureConstraint));
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

    // Count functionality works, but pages are not localized.
    options.RequestCultureProviders.Insert(0, new CookieRouteDataRequestCultureProvider()
    {
        Options = options,
    });
});


var app = builder.Build();

app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
