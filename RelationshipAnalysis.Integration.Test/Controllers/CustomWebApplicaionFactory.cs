using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models;

namespace RelationshipAnalysis.Integration.Test.Controllers;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private readonly string _databaseName;

    public CustomWebApplicationFactory()
    {
        _databaseName = Guid.NewGuid().ToString(); // استفاده از یک نام پایگاه داده منحصر به فرد
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // حذف DbContext موجود
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // اضافه کردن DbContext با پایگاه داده In-Memory جدید
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            // ساخت ارائه‌دهنده خدمات
            var serviceProvider = services.BuildServiceProvider();

            // ایجاد دامنه برای دسترسی به DbContext
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var dbContext = scopedServices.GetRequiredService<ApplicationDbContext>();

                // اطمینان از ایجاد پایگاه داده و تزریق داده‌ها
                dbContext.Database.EnsureCreated();
                SeedDatabase(dbContext);
            }
        });
    }

    private void SeedDatabase(ApplicationDbContext dbContext)
    {
        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com"
        };
        var role = new Role
        {
            Id = 1,
            Name = "admin",
            Permissions = "[\"AdminPermissions\"]",
        };
        var userRole = new UserRole
        {
            Id = 1,
            Role = role,
            User = user
        };
        user.UserRoles.Add(userRole);
        role.UserRoles.Add(userRole);
        dbContext.Users.Add(user);
        dbContext.Roles.Add(role);
        dbContext.UserRoles.Add(userRole);
        dbContext.SaveChanges();
    }
}
