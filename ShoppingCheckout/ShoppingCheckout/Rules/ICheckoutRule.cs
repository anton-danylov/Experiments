using System.Collections.Generic;

namespace ShoppingCheckout
{
    public interface ICheckoutRule
    {
        decimal Apply(IEnumerable<Product> products);
    }
}