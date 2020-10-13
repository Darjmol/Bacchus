namespace Bacchus.Models
{
    public class ProductBidding
    {
        public string id { get; set; }
        public string productId { get; set; }
        public string bidderName { get; set; }
        public string bidderDateTime { get; set; }
        public double bidderSum { get; set; }
    }
}
