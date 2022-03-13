using FileTracking.Models;
using FileTracking.ServiceHelper;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string dbConn = builder.Configuration.GetSection("AppSettings").GetSection("ConnectionString").Value;
builder.Configuration.GetSection("AppSettings");
builder.Services.AddDbContext<dbFileTrackerContext>(
        options => options.UseSqlServer(dbConn));
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//app.MapRazorPages();
app.MapControllerRoute(name: "default", pattern: "{controller=Dashboard}/{action=Search}/{id?}");
app.Run();
