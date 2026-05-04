using Microsoft.AspNetCore.Identity;
using ProiectII.Models;
using Microsoft.EntityFrameworkCore;

namespace ProiectII.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // ==========================================
            // 1. ROLURI
            // ==========================================
            var roles = new[] { "Admin", "User", "Employee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ==========================================
            // 2. UTILIZATORI (Admin, Angajat + 2 Useri pentru adopții)
            // ==========================================
            var usersToCreate = new List<(ApplicationUser User, string Password, string Role)>
            {
                (new ApplicationUser { UserName = "admin@fox.com", Email = "admin@fox.com", FirstName = "Victor", LastName = "Admin", BornDate = new DateOnly(1995, 5, 20), EmailConfirmed = true }, "SecurePass123!", "Admin"),
                (new ApplicationUser { UserName = "vet@fox.com", Email = "vet@fox.com", FirstName = "Maria", LastName = "Doctor", BornDate = new DateOnly(1988, 3, 15), EmailConfirmed = true }, "VetPass123!", "Employee"),
                (new ApplicationUser { UserName = "user1@fox.com", Email = "user1@fox.com", FirstName = "Ion", LastName = "Popescu", BornDate = new DateOnly(2000, 1, 1), EmailConfirmed = true }, "UserPass123!", "User"),
                (new ApplicationUser { UserName = "user2@fox.com", Email = "user2@fox.com", FirstName = "Elena", LastName = "Ionescu", BornDate = new DateOnly(2002, 7, 10), EmailConfirmed = true }, "UserPass123!", "User")
            };

            foreach (var u in usersToCreate)
            {
                if (await userManager.FindByEmailAsync(u.User.Email) == null)
                {
                    await userManager.CreateAsync(u.User, u.Password);
                    await userManager.AddToRoleAsync(u.User, u.Role);
                }
            }

            // ==========================================
            // 3. STATUSURI VULPI
            // ==========================================
            if (!context.Statuses.Any())
            {
                context.Statuses.AddRange(
                    new Status { Name = "Healthy", Description = "Ready for a new home", IsAdoptable = true, FoxStatus = FoxStatus.Healthy },
                    new Status { Name = "Under Treatment", Description = "In medical wing, recovering", IsAdoptable = false, FoxStatus = FoxStatus.Healthy }, // Păstrat enum-ul tău dacă nu ai altul
                    new Status { Name = "Quarantined", Description = "New arrival, observation period", IsAdoptable = false, FoxStatus = FoxStatus.Healthy }
                );
                await context.SaveChangesAsync();
            }

            // ==========================================
            // 4. LOCAȚII (Centre și puncte de raportare)
            // ==========================================
            if (!context.Locations.Any())
            {
                context.Locations.AddRange(
                    new Location { Name = "Enclosure Alpha Center", Coordinate = new Coordinate { Latitude = 46.7712m, Longitude = 23.5923m }, PrecisionRadius = 2.0 },
                    new Location { Name = "Enclosure Beta Center", Coordinate = new Coordinate { Latitude = 46.7720m, Longitude = 23.5930m }, PrecisionRadius = 2.0 },
                    new Location { Name = "North Forest Edge", Coordinate = new Coordinate { Latitude = 46.7800m, Longitude = 23.6000m }, PrecisionRadius = 10.5 },
                    new Location { Name = "Backyard Spotted", Coordinate = new Coordinate { Latitude = 46.7750m, Longitude = 23.5800m }, PrecisionRadius = 15.0 },
                    new Location { Name = "Highway Crossing", Coordinate = new Coordinate { Latitude = 46.7600m, Longitude = 23.6100m }, PrecisionRadius = 5.0 }
                );
                await context.SaveChangesAsync();
            }

            // ==========================================
            // 5. ȚARCURI & PUNCTE (2 Țarcuri distincte)
            // ==========================================
            if (!context.Enclosures.Any())
            {
                var locAlpha = context.Locations.First(l => l.Name == "Enclosure Alpha Center");
                var locBeta = context.Locations.First(l => l.Name == "Enclosure Beta Center");

                var alphaWing = new Enclosure { Name = "Alpha Wing", Description = "Main healthy area", ColorMaskHex = "#FF5733", Opacity = 0.6, CenterLocationId = locAlpha.Id };
                var betaWing = new Enclosure { Name = "Beta Quarantine", Description = "Strict quarantine area", ColorMaskHex = "#33FF57", Opacity = 0.8, CenterLocationId = locBeta.Id };

                context.Enclosures.AddRange(alphaWing, betaWing);
                await context.SaveChangesAsync();

                // Puncte pentru Alpha
                context.EnclosurePoints.AddRange(
                    new EnclosurePoint { EnclosureId = alphaWing.Id, DrawOrder = 1, Coordinate = new Coordinate { Latitude = 46.7710m, Longitude = 23.5920m } },
                    new EnclosurePoint { EnclosureId = alphaWing.Id, DrawOrder = 2, Coordinate = new Coordinate { Latitude = 46.7715m, Longitude = 23.5925m } },
                    new EnclosurePoint { EnclosureId = alphaWing.Id, DrawOrder = 3, Coordinate = new Coordinate { Latitude = 46.7712m, Longitude = 23.5930m } }
                );

                // Puncte pentru Beta
                context.EnclosurePoints.AddRange(
                    new EnclosurePoint { EnclosureId = betaWing.Id, DrawOrder = 1, Coordinate = new Coordinate { Latitude = 46.7720m, Longitude = 23.5930m } },
                    new EnclosurePoint { EnclosureId = betaWing.Id, DrawOrder = 2, Coordinate = new Coordinate { Latitude = 46.7725m, Longitude = 23.5935m } }
                );

                await context.SaveChangesAsync();
            }

            // ==========================================
            // 6. VULPI
            // ==========================================
            if (!context.Foxes.Any())
            {
                var healthy = context.Statuses.First(s => s.Name == "Healthy");
                var treatment = context.Statuses.First(s => s.Name == "Under Treatment");
                var quarantined = context.Statuses.First(s => s.Name == "Quarantined");

                var alphaWing = context.Enclosures.First(e => e.Name == "Alpha Wing");
                var betaWing = context.Enclosures.First(e => e.Name == "Beta Quarantine");

                var locForest = context.Locations.First(l => l.Name == "North Forest Edge");
                var locHighway = context.Locations.First(l => l.Name == "Highway Crossing");

                context.Foxes.AddRange(
                    new Fox { Name = "Foxy", Description = "Friendly and fast.", ImageUrl = "foxy.jpg", StatusId = healthy.Id, EnclosureId = alphaWing.Id, FirstSeenLocationId = locForest.Id },
                    new Fox { Name = "Shadow", Description = "Hard to spot.", ImageUrl = "shadow.jpg", StatusId = treatment.Id, EnclosureId = alphaWing.Id },
                    new Fox { Name = "Rusty", Description = "Found near the road, recovering.", ImageUrl = "rusty.jpg", StatusId = quarantined.Id, EnclosureId = betaWing.Id, FirstSeenLocationId = locHighway.Id },
                    new Fox { Name = "Luna", Description = "Very playful, loves apples.", ImageUrl = "luna.jpg", StatusId = healthy.Id, EnclosureId = alphaWing.Id }
                );
                await context.SaveChangesAsync();
            }

            // Preluăm instanțele de useri pentru a lega adopțiile/rapoartele
            var user1 = await userManager.FindByEmailAsync("user1@fox.com");
            var user2 = await userManager.FindByEmailAsync("user2@fox.com");

            // ==========================================
            // 7. ADOPȚII (Diverse statusuri)
            // ==========================================
            if (!context.Adoptions.Any())
            {
                var foxy = context.Foxes.First(f => f.Name == "Foxy");
                var luna = context.Foxes.First(f => f.Name == "Luna");

                context.Adoptions.AddRange(
                    new Adoption { FoxId = foxy.Id, UserId = user1.Id, AdoptionStatus = 0, RequestDate = DateTime.UtcNow, Reason = "I have a large garden and experience with rescues." }, // Pending
                    new Adoption { FoxId = luna.Id, UserId = user2.Id, AdoptionStatus = (AdoptionStatus)1, RequestDate = DateTime.UtcNow.AddDays(-5), Reason = "Always wanted a fox." } // Approved (presupunând că 1 = Approved)
                );
                await context.SaveChangesAsync();
            }

            // ==========================================
            // 8. RAPOARTE
            // ==========================================
            if (!context.Reports.Any())
            {
                var locBackyard = context.Locations.First(l => l.Name == "Backyard Spotted");
                var locHighway = context.Locations.First(l => l.Name == "Highway Crossing");

                context.Reports.AddRange(
                    new Report { Description = "Found a fox digging in my trash.", ReporterId = user1.Id, LocationId = locBackyard.Id, ReportStatus = (ReportStatus)0 }, // Pending
                    new Report { Description = "Injured fox near the barrier.", ReporterId = user2.Id, LocationId = locHighway.Id, ReportStatus = (ReportStatus)1 } // Investigating/Resolved
                );
                await context.SaveChangesAsync();
            }
        }
    }
}