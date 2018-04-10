using System.Collections.Generic;
using System.Linq;

namespace ShoppingCheckout
{
    public class Discount3for2Rule : ICheckoutRule
    {
        private string _productName;
        private readonly int _discountedIndex = 3;

        public Discount3for2Rule(string productName)
        {
            this._productName = productName;
        }

        public bool IsExclusive => true;

        public decimal Apply(IEnumerable<Product> products)
        {
            int counter = 0;
            decimal discountedPrice = .0m;
            foreach (var product in products)
            {
                if (product.Name != _productName)
                {
                    continue;
                }

                product.Accept(this);

                if (++counter == _discountedIndex)
                {
                    discountedPrice += product.Price;
                    counter = 0;
                }
            }

            return discountedPrice;
        }
    }
}