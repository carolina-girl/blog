using Blog.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Security;

namespace Blog.Migrations
    {

        internal sealed class Configuration : DbMigrationsConfiguration<Blog.Models.ApplicationDbContext>
    {
        public object Password { get; private set; }
        public object FailureText { get; private set; }

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "mahburns@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "mahburns@gmail.com",
                    Email = "mahburns@gmail.com",
                    FirstName = "Mary",
                    LastName = "Burns",
                    DisplayName = "Mary Burns"
                }, "redhead46");
            }
            var userId = userManager.FindByEmail("mahburns@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            var roleManager1 = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            var userManager1 = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "moderator@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "moderator@coderfoundry.com",
                    Email = "moderator@coderfoundry.com",
                    DisplayName = "Moderator"
                }, "Password-1");
            }
            var userId1 = userManager.FindByEmail("moderator@coderfoundry.com").Id;
            userManager.AddToRole(userId, "Moderator");
        }
    }
}
        



            //  protected void LoginButton_Click(object sender, EventArgs e)
            // {
            ///****note: UserName and Password are textbox fields****/

            //object UserId1 = null;
            //if (Membership.ValidateUser(u.Email, Password))
            //{
            //    MembershipUser user = Membership.GetUser(UserId1);
            //    if (user == null)
            //    {
            //        FailureText = "Invalid password. Please try again.";
            //        return;
            //    }
            //    if (user.IsLockedOut)
            //        user.UnlockUser();

            //    /* this is the interesting part for you */
            //    if (user.LastPasswordChangedDate == user.CreationDate) //if true, that means user never changed their password before
            //    {
            //        //TODO: add your change password logic here
            //    }
            

    
        





        //  This method will be called after migrating to the latest version.

//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
//  to avoid creating duplicate seed data. E.g.
//
//    context.People.AddOrUpdate(
//      p => p.FullName,
//      new Person { FullName = "Andrew Peters" },
//      new Person { FullName = "Brice Lambson" },
//      new Person { FullName = "Rowan Miller" }
//    );
//


