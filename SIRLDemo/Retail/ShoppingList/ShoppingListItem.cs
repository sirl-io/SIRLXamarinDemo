using System;
namespace SIRLDemo.Retail.ShoppingList
{
    public class ShoppingListItem
    {
        protected string name;
        protected string sku;
        protected int quantity;

        public ShoppingListItem(string name, string sku)
        {
            this.name = name;
            this.sku = sku;
            this.SetQuantity(1);
        }

        public ShoppingListItem(string name, string sku, int q)
        {
            this.name = name;
            this.sku = sku;
            this.SetQuantity(q);
        }

        public string GetSku()
        {
            return sku;
        }

        public int GetQuantity()
        {
            return quantity;
        }

        public void SetQuantity(int quantity)
        {
            if (quantity < 1) this.quantity = 1;
            else this.quantity = quantity;
        }

        public void IncrementQuantity()
        {
            quantity += 1;
        }

        public void DecrementQuantity()
        {
            if (quantity > 0) quantity--;
        }

        public String GetName()
        {
            return name;
        }

        public ShoppingListItem()
        {
        }
    }
}
