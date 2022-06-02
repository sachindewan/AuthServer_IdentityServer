using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AuthServer;
using AuthServer.Data;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AuthServer
{
  public class SeedData
  {
    public static void EnsureSeedData(ConfigurationDbContext context, UserManager<IdentityUser> userMgr, RoleManager<IdentityRole> roleManager)
    {
        EnsureSeedData(context);
        EnsureUsers(userMgr, roleManager);
    }

    private static void EnsureUsers(UserManager<IdentityUser> userMgr, RoleManager<IdentityRole> roleManager)
    {
        if (!userMgr.Users.Any())
        {
            var roles = new List<IdentityRole>()
            {
                new IdentityRole(){Name ="Admin"},
                new IdentityRole(){ Name ="Member"}
            };
            foreach (var role in roles)
            {
                roleManager.CreateAsync(role).GetAwaiter().GetResult();
            }

        }
        var alice = userMgr.FindByNameAsync("alice").Result;
      if (alice == null)
      {
        alice = new IdentityUser
        {
          UserName = "alice",
          Email = "AliceSmith@email.com",
          EmailConfirmed = true,
        };
        var result = userMgr.CreateAsync(alice, "Pass123$").Result;
        if (!result.Succeeded)
        {
          throw new Exception(result.Errors.First().Description);
        }

        result = userMgr.AddClaimsAsync(alice, new Claim[]
        {
          new Claim(JwtClaimTypes.Name, "Alice Smith"),
          new Claim(JwtClaimTypes.GivenName, "Alice"),
          new Claim(JwtClaimTypes.FamilyName, "Smith"),
          new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
        }).Result;
        if (!result.Succeeded)
        {
          throw new Exception(result.Errors.First().Description);
        }
        var roleResult = userMgr.AddToRoleAsync(alice, "Admin").Result;
        if (!roleResult.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        } 
        Log.Debug("alice created");
      }
      else
      {
        Log.Debug("alice already exists");
      }

      var bob = userMgr.FindByNameAsync("bob").Result;
      if (bob == null)
      {
        bob = new IdentityUser
        {
          UserName = "bob",
          Email = "BobSmith@email.com",
          EmailConfirmed = true
        };
        var result = userMgr.CreateAsync(bob, "Pass123$").Result;
        if (!result.Succeeded)
        {
          throw new Exception(result.Errors.First().Description);
        }

        result = userMgr.AddClaimsAsync(bob, new Claim[]
        {
          new Claim(JwtClaimTypes.Name, "Bob Smith"),
          new Claim(JwtClaimTypes.GivenName, "Bob"),
          new Claim(JwtClaimTypes.FamilyName, "Smith"),
          new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
          new Claim("location", "somewhere"),
          new Claim(JwtClaimTypes.Role,"User"),
          new Claim(JwtClaimTypes.Role,"Tester")
        }).Result;

        if (!result.Succeeded)
        {
          throw new Exception(result.Errors.First().Description);
        }
        var result1 = userMgr.AddToRoleAsync(bob,"Admin").Result;
        if (!result1.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
        Log.Debug("bob created");
      }
      else
      {
        Log.Debug("bob already exists");
      }
    }


    private static void EnsureSeedData(ConfigurationDbContext context)
    {
      if (!context.Clients.Any())
      {
        Log.Debug("Clients being populated");
        foreach (var client in Config.Clients.ToList())
        {
          context.Clients.Add(client.ToEntity());
        }

        context.SaveChanges();
      }
      else
      {
          Log.Debug("Let see if there is any client configured..");
          if (context.Clients.ToList().Count < Config.Clients.Count())
          {
              Log.Debug("yes new client is configured lets seed it.");
              foreach (var client in Config.Clients.ToList())
              {
                  if (!context.Clients.Any(x => x.ClientName == client.ClientName))
                  {
                        context.Clients.Add(client.ToEntity());
                        context.SaveChanges();
                        Log.Debug("new client has been seeded now");
                  }
              }
          }
          else
          {
              Log.Debug("Clients already populated");
          }
      }

      if (!context.IdentityResources.Any())
      {
        Log.Debug("IdentityResources being populated");
        foreach (var resource in Config.IdentityResources.ToList())
        {
          context.IdentityResources.Add(resource.ToEntity());
        }

        context.SaveChanges();
      }
      else
      {
        Log.Debug("IdentityResources already populated");
      }

      if (!context.ApiScopes.Any())
      {
        Log.Debug("ApiScopes being populated");
        foreach (var resource in Config.ApiScopes.ToList())
        {
          context.ApiScopes.Add(resource.ToEntity());
        }

        context.SaveChanges();
      }
      else
      {
        Log.Debug("ApiScopes already populated");
      }

      if (!context.ApiResources.Any())
      {
        Log.Debug("ApiResources being populated");
        foreach (var resource in Config.ApiResources.ToList())
        {
          context.ApiResources.Add(resource.ToEntity());
        }

        context.SaveChanges();
      }
      else
      {
        Log.Debug("ApiScopes already populated");
      }
    }
  }
}