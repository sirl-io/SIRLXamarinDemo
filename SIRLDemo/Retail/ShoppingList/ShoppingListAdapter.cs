using System;
using System.Collections.Generic;

using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace SIRLDemo.Retail.ShoppingList
{
    public class ShoppingListAdapter: RecyclerView.Adapter
    {
        IList<ShoppingListItem> items;

        public ShoppingListAdapter( IList<ShoppingListItem> shoppingListItems)
        {
            items = shoppingListItems;
        }

        public override int ItemCount => items.Count;

        //public override View GetView(int position, View convertView, ViewGroup parent)
        //{
        //    //else if (convertView.FindViewById(Resource.Id.) == null)
        //    else {
        //        return convertView;
        //    }
        //    //else
        //    //{
        //    //    Log.d(TAG, "Convertview is not Null: " + convertView.toString());
        //    //}

        //    ShoppingListItem item = GetItem(position);
        //    TextView productName = convertView.FindViewById<TextView>(Resource.Id.item_name);
        //    productName.Text = item.GetName();

        //    //ItemListener listener = new ItemListener(pickingListItem);
        //    //productName.setOnLongClickListener(listener);

        //    TextView qty = convertView.FindViewById<TextView>(Resource.Id.shopping_item_quantity);
        //    if(qty != null)
        //    {
        //        qty.Text = "" + item.GetQuantity();
        //    }

        //    return convertView;
        //}

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ShoppingListItemHolder vh = holder as ShoppingListItemHolder;

            vh.qty.Text = "" + items[position].GetQuantity();
            vh.productName.Text = items[position].GetName();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.content_shopping_list_item, parent, false);

            // Create a ViewHolder to hold view references inside the CardView:
            ShoppingListItemHolder vh = new ShoppingListItemHolder(itemView);
            return vh;
        }
    }
}
