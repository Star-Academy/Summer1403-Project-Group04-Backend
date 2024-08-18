﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Integration.Test.Controllers;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }


            services.AddDbContext<ApplicationDbContext>(options => { options.UseInMemoryDatabase(_databaseName).UseLazyLoadingProxies(); });


            var serviceProvider = services.BuildServiceProvider();


            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var dbContext = scopedServices.GetRequiredService<ApplicationDbContext>();


            dbContext.Database.EnsureCreated();
            SeedDatabase(dbContext);
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
            RoleId = 1,
            Role = role,
            UserId = 1,
            User = user
        };
        dbContext.UserRoles.Add(userRole);
        user.UserRoles.Add(userRole);
        role.UserRoles.Add(userRole);
        dbContext.Users.Add(user);
        dbContext.Roles.Add(role);
        dbContext.SaveChanges();
    }
}