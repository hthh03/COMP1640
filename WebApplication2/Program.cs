﻿using WebApplication2.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using Microsoft.AspNetCore.Identity;
using WebApplication2.Utilities;
using WebApplication2.Hubs;
using Microsoft.AspNetCore.Http.Features;
using WebApplication2.Services;
using WebApplication2.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();


// Cấu hình DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Fix")));

// Cấu hình Identity
builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Thêm Authorization
builder.Services.AddAuthorization();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Thêm Controllers với Views
builder.Services.AddControllersWithViews();


var app = builder.Build();
app.MapHub<MeetingHub>("/meetingHub");






// Cấu hình Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}




app.MapHub<MessageHub>("/messageHub");  // Đăng ký SignalR Hub
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/notificationHub");
});

// Khởi tạo dữ liệu ban đầu (vai trò và tài khoản Admin mặc định)
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<Users>>();

    var roleNames = new[] { "Admin", "Student", "Teacher" };
    foreach (var roleName in roleNames)
    {
        if (!roleManager.RoleExistsAsync(roleName).Result)
        {
            roleManager.CreateAsync(new IdentityRole(roleName)).Wait();
        }
    }

    string adminEmail = "admin@example.com";
    string adminPassword = "Admin123456";

    var adminUser = userManager.FindByEmailAsync(adminEmail).Result;
    if (adminUser == null)
    {
        adminUser = new Users
        {
            FullName = adminEmail,
            Email = adminEmail,
            UserName = adminEmail, // ✅ Thêm dòng này để tránh lỗi khi tạo tài khoản
            EmailConfirmed = true,
            Role = "Admin"
        };

        var result = userManager.CreateAsync(adminUser, adminPassword).Result;
        if (result.Succeeded)
        {
            userManager.AddToRoleAsync(adminUser, "Admin").Wait();
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error creating admin user: {error.Description}");
            }
        }
    }
}

// Cấu hình định tuyến
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=GetStarted}/{id?}");
});



app.Run();
