using System;
using System.Collections.Generic;
using Xunit;

namespace ShoppingCheckout.Tests
{
    public class CheckoutTests
    {
        [Fact]
        public void Empty()
        {
            ShoppingCart cart = new ShoppingCart(null);

            decimal total = cart.GetTotal();

            Assert.Equal(.0m, total);
        }

        [Fact]
        public void SimpleSum()
        {
            ShoppingCart cart = new ShoppingCart(null);

            cart.Add(new Product("TypeA", 10.0m));
            cart.Add(new Product("TypeB", 20.0m));

            decimal total = cart.GetTotal();
            Assert.Equal(30.0m, total);
        }

        [Fact]
        public void DiscountOn3for2()
        {
            List<ICheckoutRule> rules = new List<ICheckoutRule>();
            rules.Add(new Discount3for2Rule("TypeA"));

            ShoppingCart cart = new ShoppingCart(rules);

            cart.Add(new Product("TypeA", 10.0m));
            cart.Add(new Product("TypeA", 10.0m));
            cart.Add(new Product("TypeA", 10.0m));


            decimal total = cart.GetTotal();
            Assert.Equal(20.0m, total);
        }

        [Fact]
        public void PercentDiscount()
        {
            List<ICheckoutRule> rules = new List<ICheckoutRule>();
            rules.Add(new PercentDiscountRule(10));

            ShoppingCart cart = new ShoppingCart(rules);

            cart.Add(new Product("TypeA", 20.0m));
            cart.Add(new Product("TypeB", 100.0m));


            decimal total = cart.GetTotal();
            Assert.Equal(120.0m - 12m, total);
        }

        [Fact]
        public void MutuallyExclusiveDiscounts()
        {
            List<ICheckoutRule> rules = new List<ICheckoutRule>();
            rules.Add(new Discount3for2Rule("TypeA"));
            rules.Add(new PercentDiscountRule(10));

            ShoppingCart cart = new ShoppingCart(rules);

            cart.Add(new Product("TypeA", 10.0m));
            cart.Add(new Product("TypeA", 10.0m));
            cart.Add(new Product("TypeA", 10.0m));
            cart.Add(new Product("TypeB", 100.0m));


            decimal total = cart.GetTotal();
            Assert.Equal(110.0m, total);
        }
    }
}
