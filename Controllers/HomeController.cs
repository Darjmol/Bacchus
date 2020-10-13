using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bacchus.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Bacchus.Data;

namespace Bacchus.Controllers
{
    public class HomeController : Controller
    {
        protected static string url = "http://uptime-auction-api.azurewebsites.net/api/Auction";

        public static List<Product> currentProducts = new List<Product>();
        public static List<string> categories = new List<string>();

        private readonly AuctionContext _context;

        public HomeController(AuctionContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string filter)
        {
            currentProducts = new List<Product>();
            await CreateListOfProducts();
            getCategories();

            if (!String.IsNullOrEmpty(filter))
            {
                currentProducts = currentProducts.Where(p => p.productCategory == filter).ToList();
            }

            return View(currentProducts.ToList());
        }

        // gets list of products off API
        async private static Task<List<Product>> CreateListOfProducts()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            HttpContent content = response.Content;

            string data = await content.ReadAsStringAsync();

            var products = JsonConvert.DeserializeObject<List<Product>>(data);

            foreach (var p in products)
            {
                currentProducts.Add(
                    new Product
                    {
                        productId = p.productId,
                        productName = p.productName,
                        productCategory = p.productCategory,
                        productDescription = p.productDescription,
                        biddingEndDate = p.biddingEndDate
                    }
                );
            };

            return currentProducts;
        }

        private void getCategories()
        {
            foreach(Product p in currentProducts)
            {
                if (!categories.Contains(p.productCategory)) {
                    categories.Add(p.productCategory);
                };     
            }

            ViewBag.categories = categories;
        }

        [HttpGet]
        public ActionResult Details(string id)
        {
            if (id == "")
            {
                return NotFound();
            }
            
            Product chosenProduct = currentProducts.Where(p => p.productId == id).FirstOrDefault();

            return View(chosenProduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(Product p)
        {
            Product chosenProduct = currentProducts.Where(prd => prd.productId == p.productId).FirstOrDefault();

            DateTime dateTime = DateTime.Now;
            if (dateTime >= DateTime.Parse(chosenProduct.biddingEndDate))
            {
                ModelState.AddModelError("lastBidderName", "Cannot make a bid, auction is over");
                return View(chosenProduct);
            }

            if (!ModelState.IsValid)
            {
                return View(chosenProduct);
            }

            if (Double.Parse(p.lastBidderSum) <= 0)
            {
                ModelState.AddModelError("lastBidderSum", "The entered amount cannot be 0.");
            }

            //if all is fine then
            chosenProduct.lastBidderName = p.lastBidderName;
            chosenProduct.lastBidderSum = p.lastBidderSum;
            chosenProduct.lastBidderDateTime = dateTime.ToString();

            DateTime d = DateTime.Parse(chosenProduct.biddingEndDate);

            BiddingInput(chosenProduct);

            return RedirectToAction("Index");
        }


        private void sumNotNull(string sum)
        {
            if (Double.Parse(sum) <= 0)
            {
                ModelState.AddModelError("lastBidderSum", "The entered amount cannot be 0.");
            }
        }

        //adds entry to db
        public async Task BiddingInput(Product p)
        {
            string product = p.productId;
            string bidder = p.lastBidderName;
            double sum = Double.Parse(p.lastBidderSum);
            string dateTime = p.lastBidderDateTime;

            ProductBidding newBid = new ProductBidding();

            newBid.id = Guid.NewGuid().ToString();
            newBid.productId = product;
            newBid.bidderName = bidder;
            newBid.bidderDateTime = dateTime;
            newBid.bidderSum = sum;

            _context.ProductBids.Add(newBid);
            _context.SaveChanges();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }       
    }
}
