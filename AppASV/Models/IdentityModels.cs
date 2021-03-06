﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AppASV.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

		public IEnumerable<SelectListItem> AllRoles { get; set; }
	}

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

		public DbSet<Episode> Episodes { get; set; }
		public DbSet<Actor> Actors { get; set; }
		public DbSet<Series> Series { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<Role> Characters { get; set; }
		public DbSet<Genre> Genres { get; set; }
		public DbSet<SeriesGenre> SeriesGenres { get; set; }
		public DbSet<FavouriteSeries> FavouriteSeries { get; set; }
		public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}