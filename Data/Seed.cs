using Microsoft.AspNetCore.Identity;
using navision.api.Models;
using System.Text.Json;

namespace navision.api.Data
{
  public class Seed
    {
        public static void SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager){

            if(!userManager.Users.Any()){
                var userData = System.IO.File.ReadAllText("Data/seedUsers.json");
                var users = JsonSerializer.Deserialize<List<User>>(userData);
                // var users = JsonConvert.DeserializeObject<List<User>>(userData);
                
                var roles = new List<Role>{
                    new Role{Name = "Admin"},
                    new Role{Name = "User"}
                };

                foreach (var role in roles)
                {
                    roleManager.CreateAsync(role).Wait();
                }

                foreach (var user in users!)
                {
                    userManager.CreateAsync(user, "password").Wait();
                    userManager.AddToRoleAsync(user, "User").Wait();
                }

                var admin = new User{
                    UserName = "Admin"
                };

                var result = userManager.CreateAsync(admin, "0comma5!").Result;

                if(result.Succeeded){
                    var user = userManager.FindByNameAsync("Admin").Result;
                    userManager.AddToRoleAsync(admin, "Admin").Wait();
                }
            }
        }
    }
}