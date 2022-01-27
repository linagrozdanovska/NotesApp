using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotesApp.Data;
using System;
using System.Linq;

namespace NotesApp.Tests.IntegrationTesting
{
    public class FakeStartup : Startup
    {
        public FakeStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);

            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                if (dbContext.Database.GetDbConnection().ConnectionString.ToLower().Contains("database.windows.net"))
                {
                    throw new Exception("LIVE SETTINGS IN TESTS!");
                }

                try
                {
                    dbContext.Database.EnsureCreated();
                }
                catch
                {
                    throw;
                }

                if (!dbContext.Users.Any(u => u.Id == UserSettings.UserId))
                {

                    var user = new IdentityUser();
                    user.ConcurrencyStamp = DateTime.Now.Ticks.ToString();
                    user.Email = UserSettings.UserEmail;
                    user.EmailConfirmed = true;
                    user.Id = UserSettings.UserId;
                    user.NormalizedEmail = user.Email;
                    user.NormalizedUserName = user.Email;
                    user.PasswordHash = Guid.NewGuid().ToString();
                    user.UserName = user.Email;

                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
