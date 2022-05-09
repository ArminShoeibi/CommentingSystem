using CommentingSystem.Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViewsConfiguration();

builder.Services.AddDbContextConfiguration(builder.Configuration);

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapDefaultControllerRoute();

await app.RunAsync();