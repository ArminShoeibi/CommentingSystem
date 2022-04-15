using CommentingSystem.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContextPool<CommentingSystemDbContext>(dbContextOptions =>
{
    dbContextOptions.UseSqlServer(builder.Configuration.GetConnectionString("CommentSystemConStr"));
});

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.MapDefaultControllerRoute();
await app.RunAsync();
