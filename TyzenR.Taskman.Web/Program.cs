using Blazored.Modal;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using TyzenR.Account;
using TyzenR.Account.Common;
using TyzenR.Account.Managers;
using TyzenR.Account.ServiceClients;
using TyzenR.Publisher.Shared;
using TyzenR.Taskman.Entity;
using TyzenR.Taskman.Managers;

try
{
    var builder = WebApplication.CreateBuilder(args);
    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    IConfigurationRoot configuration;
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    string file = string.Empty;
    if (environmentName == "Development")
    {
        file = "appsettings.Dev.json";
        configuration = new ConfigurationBuilder()
            .AddJsonFile(file, optional: false)
            .Build();
    }
    else
    {
        file = "appsettings.PROD.json";
        configuration = new ConfigurationBuilder()
            .AddJsonFile(file, false)
            .Build();
    }

    var appSettings = new AppSettings();
    configuration.GetSection("AppSettings").Bind(appSettings); // Load from dynamic config NOT builder.Services.Configuration
    builder.Services.AddSingleton<AppSettings>(appSettings);
    appSettings.File = file;

    builder.Configuration.GetSection("AppSettings").Bind(appSettings);
    builder.Services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

    builder.Services.AddDbContext<EntityContext>(options =>
    {
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        options.UseSqlServer(appSettings.PublisherConnectionString);
    }, ServiceLifetime.Transient);

    builder.Services.AddDbContext<AccountContext>(options =>
    {
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        options.UseSqlServer(appSettings.AccountConnectionString);
    }, ServiceLifetime.Transient);

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddTyzenRAccountAuthentication(builder, appSettings);

    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddBlazoredModal();
    builder.Services.AddSyncfusionBlazor();

    builder.Services.AddTransient<IAccountServiceClient, AccountServiceClient>();
    builder.Services.AddTransient<IUserManager, UserManager>();
    builder.Services.AddScoped<IAppInfo, AppInfo>();
    builder.Services.AddTransient<ITaskManager, TaskManager>();
    builder.Services.AddTransient<IAttachmentManager, AttachmentManager>();
    builder.Services.AddTransient<IActionTrackerManager, ActionTrackerManager>();

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

    app.UseDeveloperExceptionPage(); // Uncomment for viewing exception

    app.Run();
}
catch (Exception ex)
{
    await SharedUtility.SendEmailToModeratorAsync("Taskman.Program.Exception", ex.ToString());
}