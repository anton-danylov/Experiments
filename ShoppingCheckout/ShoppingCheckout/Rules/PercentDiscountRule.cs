
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCheckout
{
    public class PercentDiscountRule : ICheckoutRule
    {
        decimal _percentDiscount;

        public PercentDiscountRule(decimal percentDiscount)
        {
            _percentDiscount = percentDiscount;
        }

        public bool IsExclusive => false;

        public decimal Apply(IEnumerable<Product> products)
        {
            decimal productsPrice = .0m;

            foreach (var product in products)
            {
                if (product.DiscountEligible)
                {
                    product.Accept(this);
                    productsPrice += product.Price;
                }
            }

            return productsPrice * (_percentDiscount * .01m);
        }
    }
}