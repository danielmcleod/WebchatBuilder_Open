using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebChatBuilderModels.Models;

namespace WebChatBuilderModels
{
    ////For adding migrations, switch back to original > add > update > switch back - not doing this results in a connection error to the db
    ////Need to look at EF source for CreateDatabaseIfNotExist and see how migrations are created/handled
    //public class RepositoryInitializer : CreateDatabaseIfNotExists<Repository>
    public class RepositoryInitializer : CreateOrMigrateDatabaseInitializer<Repository,Migrations.Configuration>
    {
        protected override void Seed(Repository context)
        {
            //base.Seed(context);
            //context.SaveChanges();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            
            if (!userManager.Users.Any())
            {
                var user = new ApplicationUser
                {
                    UserName = "WcbAdmin",
                    FirstName = "Wcb",
                    LastName = "Admin",
                    DisplayName = "Wcb Admin",
                    Title = "Admin",
                    IsActive = true,
                    Email = "WcbAdmin",
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                };

                userManager.Create(user, "W3bCh@tB!dr2020"); //set a default password here - do not leave this as the default
            }

            if (!roleManager.Roles.Any())
            {
                var userAdmin = new IdentityRole
                {
                    Name = "UserAdmin"
                };
                var dashboardAdmin = new IdentityRole
                {
                    Name = "DashboardAdmin"
                };
                var settingsAdmin = new IdentityRole
                {
                    Name = "SettingsAdmin"
                };
                var alertAdmin = new IdentityRole
                {
                    Name = "AlertAdmin"
                };
                var profileAdmin = new IdentityRole
                {
                    Name = "ProfileAdmin"
                };
                var supervisor = new IdentityRole
                {
                    Name = "Supervisor"
                };
                roleManager.Create(userAdmin);
                roleManager.Create(dashboardAdmin);
                roleManager.Create(settingsAdmin);
                roleManager.Create(alertAdmin);
                roleManager.Create(profileAdmin);
                roleManager.Create(supervisor);

                var user = userManager.FindByName("WcbAdmin");
                if (user != null)
                {
                    userManager.AddToRole(user.Id, userAdmin.Name);
                    userManager.AddToRole(user.Id, dashboardAdmin.Name);
                    userManager.AddToRole(user.Id, settingsAdmin.Name);
                    userManager.AddToRole(user.Id, alertAdmin.Name);
                    userManager.AddToRole(user.Id, profileAdmin.Name);
                    userManager.AddToRole(user.Id, supervisor.Name);
                }
            }
        }

    }
}
