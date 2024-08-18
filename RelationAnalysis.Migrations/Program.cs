﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;

namespace RelationAnalysis.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Applying migrations");
            var webHost = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<ConsoleStartup>()
                .Build();

            using (var context = (ApplicationDbContext) webHost.Services.GetService(typeof(ApplicationDbContext)))
            {
                context.Database.Migrate();
            }
            Console.WriteLine("Done");
        }
    }
}