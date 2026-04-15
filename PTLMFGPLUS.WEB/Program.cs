using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PTLMFGPLUS.SERVICE;
using PTLMFGPLUS.WEB;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation(); 
builder.Services.AddHttpClient<FacebookService>();
builder.Services.AddMvc();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAppDI();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";   // Redirect here if user is not authenticated
        options.LogoutPath = "/Home/Logout"; // Optional
        options.AccessDeniedPath = "/Home/AccessDenied"; // Optional
        options.ExpireTimeSpan = TimeSpan.FromHours(1);     // Cookie expiration
        options.SlidingExpiration = true;                  // Renew cookie if close to expiration
        options.Cookie.HttpOnly = true;
        options.Cookie.Name = "MFGAppAuth";           // Optional: customize cookie name
    });


builder.Services.Configure<SecurityStampValidatorOptions>(o => o.ValidationInterval = TimeSpan.FromHours(2));
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 209715200; // 200 MB
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 209715200; // 200 MB
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.MaxDepth = 64;
});
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueCountLimit = int.MaxValue; // default is 1024
    options.ValueLengthLimit = int.MaxValue; // default is 4096
    options.MultipartBodyLengthLimit = long.MaxValue; // default is 134217728 (128 MB)
});


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseDeveloperExceptionPage();
//Middle Ware

app.UseHttpsRedirection();
//app.UseResponseCompression();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
//app.MapHub<NotificationHub>("/notificationHub");
//app.MapHub<SalesHub>("/salesHub");
app.MapControllers();
app.MapControllerRoute(
          name: "areas",
          pattern: "{area:exists}/{controller=Dashboard}/{action=DashboardIndex}/{id?}");
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();