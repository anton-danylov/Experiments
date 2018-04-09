using System;
using System.Collections.Generic;
using System.Linq;


namespace ShoppingCheckout
{
    public class ShoppingCart
    {
        private List<Product> _products = new List<Product>();
        private readonly IEnumerable<ICheckoutRule> _rules;

        public ShoppingCart(IEnumerable<ICheckoutRule> rules)
        {
            _rules = rules;
        }
        public decimal GetTotal()
        {
            return _products.Sum(p => p.Price) - CalculateDiscount();
        }


        private decimal CalculateDiscount()
        {
            if (_rules == null)
            {
                return .0m;
            }

            decimal totalDiscount = .0m;
            
            foreach(var rule in _rules)
            {
                decimal currentDiscount = rule.Apply(_products);
                totalDiscount += currentDiscount;
            }

            return totalDiscount;
        }
        public void Add(Product product)
        {
            _products.Add(product);
        }
    }
}
