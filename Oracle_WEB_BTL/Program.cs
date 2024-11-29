using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Oracle_WEB_BTL.Services;
using Oracle_WEB_BTL.Helpers;
using Oracle_WEB_BTL.Models;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ MVC
builder.Services.AddControllersWithViews();

// Thêm dịch vụ Email (nếu có)
builder.Services.AddTransient<IEmailService, EmailService>();

// Cấu hình DbContext sử dụng Oracle
builder.Services.AddDbContext<OracleContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleContext")));

// Cấu hình Authentication bằng Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Access/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(AppDefaults.TimeOut);
    });

// Thêm HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Cấu hình Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("IsAdmin", "True"));
});

var app = builder.Build();

// Cấu hình Middleware

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Sử dụng Static Files
app.UseStaticFiles();

// Routing
app.UseRouting();

// Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();

// Định nghĩa route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Access}/{action=Login}/{id?}");

// Chạy ứng dụng
app.Run();
