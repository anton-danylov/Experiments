using System;

namespace ShoppingCheckout
{
    public class Product
    {
        private string _name;
        private decimal _price;

        public string Name => _name;
        public decimal Price => _price;

        public bool DiscountEligible { get; private set; } = true;

        public Product(string name, decimal price)
        {
            this._name = name;
            this._price = price;
        }

        public void Accept(ICheckoutRule rule)
        {
            if (rule.IsExclusive)
            {
                DiscountEligible = false;
            }
        }
    }
}