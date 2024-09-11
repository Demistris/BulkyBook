using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public void Initialize()
        {
            //Migrations if they are not applied
            try
            {
                if(_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //Create roles if they are not created
            if (!_roleManager.RoleExistsAsync(SD.ROLE_ADMIN).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.ROLE_ADMIN)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.ROLE_EMPLOYEE)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.ROLE_USER_INDIVIDUAL)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.ROLE_USER_COMPANY)).GetAwaiter().GetResult();

                //Create Admin
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "Admin",
                    Email = "admin@gmail.com",
                    Name = "Kaja Więckowska",
                    PhoneNumber = "111222333",
                    StreetAddress = "Test Street",
                    PostalCode = "12-123",
                    City = "Cracow",
                    Country = "Poland",
                    EmailConfirmed = true,
                }, "Admin123!").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@dotnetmastery.com");

                if (user != null)
                {
                    _userManager.AddToRoleAsync(user, SD.ROLE_ADMIN).GetAwaiter().GetResult();
                }
            }

            return;
        }
    }
}
