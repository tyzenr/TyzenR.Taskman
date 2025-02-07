using TyzenR.Account.Common;
using TyzenR.Taskman.Managers;
using TyzenR.Taskman.Web.Components;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IAppInfo, AppInfo>();
builder.Services.AddTransient<ITaskManager, TaskManager>();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
