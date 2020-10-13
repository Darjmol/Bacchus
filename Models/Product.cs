using System.ComponentModel.DataAnnotations;

namespace Bacchus.Models
{
    public class Product
    {
        public string productId { get; set; }

        [Display(Name = "Product name")]
        public string productName { get; set; }

        [Display(Name = "Description")]
        public string productDescription { get; set; }

        [Display(Name = "Category")]
        public string productCategory { get; set; }

        [Display(Name = "Bidding end date:")]
        public string biddingEndDate { get; set; }

        [Display(Name = "Enter your name:")]
        [Required(ErrorMessage ="Please fill")]
        [RegularExpression(@"^[A-Za-z]+(\s([A-Za-z]+))+$", ErrorMessage = "Enter your fullname using only alphabetic letters and no special symbols.")]
        public string lastBidderName { get; set; }
        public string lastBidderDateTime { get; set; }

        [Display(Name = "Enter the price (€):")]
        [Required(ErrorMessage = "Please fill")]
        [RegularExpression("^(0|([1-9][0-9]*))(\\.[0-9]+)?$", ErrorMessage = "Only numbers and dots alowed!")]
        public string lastBidderSum { get; set; }
    }
}
