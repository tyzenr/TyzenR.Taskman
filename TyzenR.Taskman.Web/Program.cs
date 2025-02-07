using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Diagnostics;
using TyzenR.Account.Managers;
using TyzenR.Account.ServiceClients;
using TyzenR.Investor.Managers.Extensions.EmailService;
using TyzenR.Investor.Shared;
using TyzenR.Taskman.Entity;
using TyzenR.Taskman.Managers;

var builder = WebApplication.CreateBuilder(args);
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

IConfigurationRoot configuration;

if (environmentName == "Development")
{
    configuration = new ConfigurationBuilder()
         .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
         .Build();
}
else
{
    configuration = new ConfigurationBuilder()
         .AddJsonFile($"appsettings.PROD.json")
         .Build();
}

try
{
    AppSettings appSettings = new AppSettings();
    string publisherConnectionString = configuration.GetConnectionString("Publisher_ConnectionString");
    string accountConnectionString = configuration.GetConnectionString("Account_ConnectionString");
    if (publisherConnectionString == null)
    {
        throw new ApplicationException("Publisher_ConnectionString is not found in appsettings.json");
    }
    if (accountConnectionString == null)
    {
        throw new ApplicationException("Account_ConnectionString is not found in appsettings.json");
    }

    builder.Configuration.GetSection("AppSettings").Bind(appSettings);
    builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

    builder.Services.Configure<EmailConfig>(
          configuration.GetSection(nameof(EmailConfig)));

    builder.Services.AddSingleton<IEmailConfig>(sp =>
        sp.GetRequiredService<IOptions<EmailConfig>>().Value);

    builder.Services.AddDbContext<EntityContext>(options =>
    {
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        options.UseSqlServer(publisherConnectionString);
    }, ServiceLifetime.Transient);

    builder.Services.AddDbContext<AccountContext>(options =>
    {
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        options.UseSqlServer(accountConnectionString);
    }, ServiceLifetime.Transient);

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddTyzenrAccountAuthentication(builder);

    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();

    builder.Services.AddMemoryCache();

    builder.Services.AddScoped<IAppInfo, AppInfo>();
    builder.Services.AddScoped<DialogService>();
    builder.Services.AddScoped<NotificationService>();

    builder.Services.AddTransient<IUserManager, UserManager>();
    builder.Services.AddTransient<IAccountServiceClient, AccountServiceClient>();

    builder.Services.AddCors(options =>
    {
        var origin = appSettings.CorsOrigin;
        options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.WithOrigins(origin)
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    var app = builder.Build();

    if (!Debugger.IsAttached)
    {
        app.UseExceptionHandler("/Error");
    }

    app.UseHsts();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCors("CorsPolicy");
    app.UseCookiePolicy(
        new CookiePolicyOptions()
        {
            MinimumSameSitePolicy = SameSiteMode.Lax
        });

    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();
}
catch (Exception ex)
{
    await SharedUtility.SendEmailToModertorAsync("Investor.Program.Exception", ex.ToString());
}
