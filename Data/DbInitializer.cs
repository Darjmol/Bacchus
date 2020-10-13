using System.Linq;

namespace Bacchus.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AuctionContext context)
        {
            context.Database.EnsureCreated();

            if (context.ProductBids.Any())
            {
                return;
            }
        }
    }
}
