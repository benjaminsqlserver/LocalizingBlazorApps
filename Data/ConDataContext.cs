using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using MyFriendsApp.Models.ConData;

namespace MyFriendsApp.Data
{
    public partial class ConDataContext : DbContext
    {
        public ConDataContext()
        {
        }

        public ConDataContext(DbContextOptions<ConDataContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MyFriendsApp.Models.ConData.Friend>()
              .Property(p => p.DateOfBirth)
              .HasColumnType("datetime");
            this.OnModelBuilding(builder);
        }

        public DbSet<MyFriendsApp.Models.ConData.Friend> Friends { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    
    }
}