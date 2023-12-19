﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProiectDAW.Data;

namespace ProiectDAW.Models
{
    public static class SeedData //static ca o mai folosim altundeva
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService
                <DbContextOptions<ApplicationDbContext>>()))
            {
                // Verificam daca in baza de date exista cel putin rol
                // insemnand ca a fost rulat codul
                // De aceea facem return pentru a nu insera inca o data
                // Acesta metoda trebuie sa se execute o singura data
                if (context.Roles.Any())
                {
                    return; // baza de date contine deja roluri
                }
                // CREAREA ROLURILOR IN BD
                // daca nu contine roluri, acestea se vor crea
                context.Roles.AddRange(
                    new IdentityRole { Id = "2c5e174e-3b0e-446f-86af483d56fd7210", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                    new IdentityRole { Id = "2c5e174e-3b0e-446f-86af483d56fd7211", Name = "Manager", NormalizedName = "Manager".ToUpper() },
                    new IdentityRole { Id = "2c5e174e-3b0e-446f-86af483d56fd7212", Name = "User", NormalizedName = "User".ToUpper() }
                );
                // o noua instanta pe care o vom utiliza pentru crearea parolelor utilizatorilor
                // parolele sunt de tip hash
                var hasher = new PasswordHasher<AppUser>();
                // CREAREA USERILOR IN BD
                // Se creeaza cate un user pentru fiecare rol
                context.Users.AddRange(
                    new AppUser
                    {
                        Id = "8e445865-a24d-4543-a6c6-9443d048cdb0",
                        UserName = "admin@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@TEST.COM",
                        Email = "admin@test.com",
                        NormalizedUserName = "ADMIN@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Admin1!")
                    },
                   new AppUser
                   {

                       Id = "8e445865-a24d-4543-a6c6-9443d048cdb1",
                       UserName = "manager@test.com",
                       EmailConfirmed = true,
                       NormalizedEmail = "MANAGER@TEST.COM",
                       Email = "manager@test.com",
                       NormalizedUserName = "MANAGER@TEST.COM",
                       PasswordHash = hasher.HashPassword(null, "Manager1!")
                   },
                    new AppUser
                    {
                        Id = "8e445865-a24d-4543-a6c6-9443d048cdb2",
                        UserName = "user@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "USER@TEST.COM",
                        Email = "user@test.com",
                        NormalizedUserName = "USER@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "User1!")
                    }
                );
                // ASOCIEREA USER-ROLE
                context.UserRoles.AddRange(
                    new IdentityUserRole<string>
                    {
                        RoleId = "2c5e174e-3b0e-446f-86af483d56fd7210",
                        UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0"
                    },
                   new IdentityUserRole<string>
                   {
                       RoleId = "2c5e174e-3b0e-446f-86af483d56fd7211",
                       UserId = "8e445865-a24d-4543-a6c6-9443d048cdb1"
                   },
                   new IdentityUserRole<string>
                   {
                       RoleId = "2c5e174e-3b0e-446f-86af483d56fd7212",
                       UserId = "8e445865-a24d-4543-a6c6-9443d048cdb2"
                   }
                );
                context.SaveChanges();
            }
        }
    }
}