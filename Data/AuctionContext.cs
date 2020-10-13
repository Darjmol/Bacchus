using Bacchus.Models;
using Microsoft.EntityFrameworkCore;

namespace Bacchus.Data
{
    public class AuctionContext : DbContext
    {
        public AuctionContext(DbContextOptions<AuctionContext> options) : base(options) { }

        public DbSet<ProductBidding> ProductBids { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProductBidding>(
                entity =>
                {
                    entity.ToTable("ProductBids");
                });
        }
    }
}
