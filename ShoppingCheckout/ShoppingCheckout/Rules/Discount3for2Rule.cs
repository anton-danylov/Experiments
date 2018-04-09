using System.Collections.Generic;
using System.Linq;

namespace ShoppingCheckout
{
    public class Discount3for2Rule : ICheckoutRule
    {
        private string _productName;

        public Discount3for2Rule(string productName)
        {
            this._productName = productName;
        }

        public decimal Apply(IEnumerable<Product> products)
        {
            int count = products.Count(p => p.Name == _productName);

            int discountedCount = count / 3;

            var productPrice = products.Select(p => p.Price).FirstOrDefault();

            return productPrice * discountedCount;
        }
    }
}