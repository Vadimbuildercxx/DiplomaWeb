using Diploma.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Diploma.Controllers;
using Diploma.Hubs;
using Pomelo.EntityFrameworkCore.MySql;
using Diploma;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

string connectionString = builder.Configuration.GetConnectionString("DBConnectionString");
    

builder.Services.AddDbContext<DBContext>(options => 
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                options => options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: System.TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null)
                ), ServiceLifetime.Scoped);

builder.Services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>,ApplicationUserClaimsPrincipalFactory>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<DBContext>().AddRoles<IdentityRole>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/Login");

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.AccessDeniedPath = "/Error";
});

//builder.Services.AddScoped<Diploma.Controllers.RabbitMQBusService>();
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(365);

    options.LoginPath = "/Login";
    //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});


//Add RMQ message Broker
builder.Services.AddHostedService<RabbitMQBusService>();


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

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapHub<MessageHub>("/chat");
});
app.MapRazorPages();

app.Run();

