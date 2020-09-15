using System;

using Android.Views;
using Android.Widget;

using AndroidX.RecyclerView.Widget;

namespace SIRLDemo.Retail.ShoppingList
{
    public class ShoppingListItemHolder : RecyclerView.ViewHolder
    {
        public TextView qty { get; private set; }
        public TextView productName { get; private set; }

        public ShoppingListItemHolder(View itemView) : base(itemView)
        {
            // Locate and cache view references:
            qty = itemView.FindViewById<TextView>(Resource.Id.shopping_item_quantity);
            productName = itemView.FindViewById<TextView>(Resource.Id.shopping_item_name);
        }
    }
}
