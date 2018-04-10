using System.Collections.Generic;

namespace ShoppingCheckout
{
    public interface ICheckoutRule
    {
        bool IsExclusive { get; }

        decimal Apply(IEnumerable<Product> products);
    }
}