using Microsoft.EntityFrameworkCore;
using TyzenR.Account;
using TyzenR.Account.Common;
using TyzenR.Account.Managers;
using TyzenR.Account.ServiceClients;
using TyzenR.Publisher.Shared;
using TyzenR.Taskman.Entity;
using TyzenR.Taskman.Managers;
using TyzenR.Taskman.Web.Components;

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

    builder.Services.AddTransient<IAccountServiceClient, AccountServiceClient>();
    builder.Services.AddTransient<IUserManager, UserManager>();
    builder.Services.AddScoped<IAppInfo, AppInfo>();
    builder.Services.AddTransient<ITaskManager, TaskManager>();

    var app = builder.Build();

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
}
catch (Exception ex)
{
    await SharedUtility.SendEmailToModertorAsync("Taskman.Program.Exception", ex.ToString());
}