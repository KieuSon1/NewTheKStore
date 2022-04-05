using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace NewTheKStore.Models
{
    public partial class TheKStore : DbContext
    {
        public TheKStore()
            : base("name=TheKStore1")
        {
        }

        public virtual DbSet<account> accounts { get; set; }
        public virtual DbSet<cart> carts { get; set; }
        public virtual DbSet<product> products { get; set; }
        public virtual DbSet<cartinfo> cartinfoes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<account>()
                .Property(e => e.phone)
                .IsFixedLength();

            modelBuilder.Entity<cart>()
                .Property(e => e.location)
                .IsFixedLength();

            modelBuilder.Entity<cart>()
                .HasMany(e => e.cartinfoes)
                .WithRequired(e => e.cart)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<product>()
                .Property(e => e.price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<product>()
                .HasMany(e => e.cartinfoes)
                .WithRequired(e => e.product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<cartinfo>()
                .Property(e => e.price)
                .HasPrecision(19, 4);
        }
    }
}
