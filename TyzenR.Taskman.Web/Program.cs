using Microsoft.EntityFrameworkCore;
using Radzen;
using TyzenR.Account;
using TyzenR.Account.Common;
using TyzenR.Account.Managers;
using TyzenR.Publisher.Shared;
using TyzenR.Taskman.Entity;
using TyzenR.Taskman.Managers;
using TyzenR.Taskman.Web;

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
    // Add services to the container.
    builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

app.Run();
}
catch (Exception ex)
{
    await SharedUtility.SendEmailToModertorAsync("Taskman.Program.Exception", ex.ToString());
}

