using Blazored.Modal;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using System.Diagnostics;
using TyzenR.Account;
using TyzenR.Account.Common;
using TyzenR.Account.Managers;
using TyzenR.Account.ServiceClients;
using TyzenR.Publisher.Shared;
using TyzenR.Taskman.Entity;
using TyzenR.Taskman.Managers;
using TyzenR.Taskman.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

try
{
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

    AppSettings appSettings = new AppSettings();
    string publisherConnectionString = configuration.GetConnectionString("Publisher_ConnectionString");
    string accountConnectionString = configuration.GetConnectionString("Account_ConnectionString");
    builder.Configuration.GetSection("AppSettings").Bind(appSettings);
    builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

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
    builder.Services.AddBlazoredModal();
    builder.Services.AddSyncfusionBlazor();

    builder.Services.AddTransient<IAccountServiceClient, AccountServiceClient>();
    builder.Services.AddTransient<IUserManager, UserManager>();
    builder.Services.AddScoped<IAppInfo, AppInfo>();
    builder.Services.AddTransient<ITaskManager, TaskManager>();

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

    app.UseExceptionHandler("/Error");
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

    if (Debugger.IsAttached)
    {
        app.UseDeveloperExceptionPage();
    }

    app.Run();
}
catch (Exception ex)
{
    await SharedUtility.SendEmailToModertorAsync("Taskman.Program.Exception", ex.ToString());
}