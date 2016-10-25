using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Jenpbiz.Models
{
        public class JenpbizContext : DbContext
        {
            public JenpbizContext() : base("jenpbizDatabase")
            { }

            public DbSet<Product> Products { get; set; }
            public DbSet<Price> Prices { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasRequired(p => p.Price)
                .WithRequiredDependent(p => p.Product);

        }
    }
}
