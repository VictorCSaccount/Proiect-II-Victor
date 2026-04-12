using Microsoft.AspNetCore.Identity;
using ProiectII.Models;
using Microsoft.EntityFrameworkCore;

namespace ProiectII.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Roluri
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // 2. Utilizatori (Admin și un User normal pentru testarea adopțiilor/rapoartelor)
            ApplicationUser adminUser = await userManager.FindByEmailAsync("admin@fox.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@fox.com",
                    Email = "admin@fox.com",
                    FirstName = "Victor",
                    LastName = "Admin",
                    BornDate = new DateOnly(1995, 5, 20),
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "SecurePass123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            ApplicationUser normalUser = await userManager.FindByEmailAsync("user@fox.com");
            if (normalUser == null)
            {
                normalUser = new ApplicationUser
                {
                    UserName = "user@fox.com",
                    Email = "user@fox.com",
                    FirstName = "Ion",
                    LastName = "Popescu",
                    BornDate = new DateOnly(2000, 1, 1),
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(normalUser, "UserPass123!");
                await userManager.AddToRoleAsync(normalUser, "User");
            }

            // 3. Statusuri
            if (!context.Statuses.Any())
            {
                context.Statuses.AddRange(
                    new Status { Name = "Healthy", Description = "Ready for a new home", IsAdoptable = true, FoxStatus = FoxStatus.Healthy },
                    new Status { Name = "Under Treatment", Description = "In medical wing", IsAdoptable = false, FoxStatus = FoxStatus.Healthy },
                    new Status { Name = "Quarantined", Description = "New arrival", IsAdoptable = false, FoxStatus = FoxStatus.Healthy }
                );
                await context.SaveChangesAsync();
            }

            // 4. Locații (Pentru Centru Țarc, Vulpile văzute și Rapoarte)
            if (!context.Locations.Any())
            {
                context.Locations.AddRange(
                    new Location { Name = "Enclosure Alpha Center", Coordinate = new Coordinate { Latitude = 46.7712m, Longitude = 23.5923m }, PrecisionRadius = 2.0 },
                    new Location { Name = "North Forest Edge", Coordinate = new Coordinate { Latitude = 46.7800m, Longitude = 23.6000m }, PrecisionRadius = 10.5 },
                    new Location { Name = "Backyard Spotted", Coordinate = new Coordinate { Latitude = 46.7750m, Longitude = 23.5800m }, PrecisionRadius = 15.0 }
                );
                await context.SaveChangesAsync();
            }

            // 5. Țarcuri și Puncte (Relație 1:N)
            if (!context.Enclosures.Any())
            {
                var center = context.Locations.First(l => l.Name == "Enclosure Alpha Center");
                var enclosure = new Enclosure
                {
                    Name = "Alpha Wing",
                    Description = "Main recovery area",
                    ColorMaskHex = "#FF5733",
                    Opacity = 0.6,
                    CenterLocationId = center.Id
                };
                context.Enclosures.Add(enclosure);
                await context.SaveChangesAsync();

                context.EnclosurePoints.AddRange(
                    new EnclosurePoint { EnclosureId = enclosure.Id, DrawOrder = 1, Coordinate = new Coordinate { Latitude = 46.7710m, Longitude = 23.5920m } },
                    new EnclosurePoint { EnclosureId = enclosure.Id, DrawOrder = 2, Coordinate = new Coordinate { Latitude = 46.7715m, Longitude = 23.5925m } }
                );
                await context.SaveChangesAsync();
            }

            // 6. Vulpi (Relații cu Status, Enclosure și Locations)
            if (!context.Foxes.Any())
            {
                var healthy = context.Statuses.First(s => s.Name == "Healthy");
                var sick = context.Statuses.First(s => s.Name == "Under Treatment");
                var enclosure = context.Enclosures.First();
                var loc1 = context.Locations.First(l => l.Name == "North Forest Edge");

                context.Foxes.AddRange(
                    new Fox
                    {
                        Name = "Foxy",
                        Description = "Friendly and fast.",
                        ImageUrl = "foxy.jpg",
                        StatusId = healthy.Id,
                        EnclosureId = enclosure.Id,
                        FirstSeenLocationId = loc1.Id
                    },
                    new Fox
                    {
                        Name = "Shadow",
                        Description = "Hard to spot.",
                        ImageUrl = "shadow.jpg",
                        StatusId = sick.Id,
                        EnclosureId = enclosure.Id
                    }
                );
                await context.SaveChangesAsync();
            }

            // 7. Interacțiuni: Adopții, Comentarii, Rapoarte
            if (!context.Adoptions.Any())
            {
                var fox = context.Foxes.First(f => f.Name == "Foxy");
                context.Adoptions.Add(new Adoption
                {
                    FoxId = fox.Id,
                    UserId = normalUser.Id,
                    AdoptionStatus = 0, // Pending
                    RequestDate = DateTime.UtcNow,
                    Reason = "I love foxes."
                });
            }

            if (!context.Comments.Any())
            {
                var fox = context.Foxes.First(f => f.Name == "Foxy");
                context.Comments.Add(new Comment
                {
                    Content = "Is she eating well?",
                    CreatedAt = DateTime.UtcNow,
                    UserId = adminUser.Id,
                    FoxId = fox.Id
                });
            }

            if (!context.Reports.Any())
            {
                var loc = context.Locations.First(l => l.Name == "Backyard Spotted");
                context.Reports.Add(new Report
                {
                    Description = "Found a fox near the trash can.",
                    ReporterId = normalUser.Id,
                    LocationId = loc.Id,
                    ReportStatus = 0 // Pending
                });
            }

            await context.SaveChangesAsync();
        }
    }
}