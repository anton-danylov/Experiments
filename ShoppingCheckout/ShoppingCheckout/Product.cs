using System;

namespace ShoppingCheckout
{
    public class Product
    {
        private string _name;
        private decimal _price;

        public string Name => _name;
        public decimal Price => _price;

        public Product(string name, decimal price)
        {
            this._name = name;
            this._price = price;
        }
    }
}