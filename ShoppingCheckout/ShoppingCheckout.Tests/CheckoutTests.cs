using System;
using Xunit;

namespace ShoppingCheckout.Tests
{
    public class CheckoutTests
    {
        [Fact]
        public void Empty()
        {
            ShoppingCart cart = new ShoppingCart();

            decimal total = cart.GetTotal();

            Assert.Equal(.0m, total);
        }
    }
}
