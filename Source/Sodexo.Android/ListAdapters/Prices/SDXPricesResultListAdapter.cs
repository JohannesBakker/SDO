using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Android.Views.Animations;
using Android.Graphics;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;

namespace Sodexo.Android
{
	public class SDXPricesResultListAdapter : BaseAdapter <string>
	{
		Activity context;
		public List <ProductPriceModel> Prices;

		public SDXPricesResultListAdapter (Activity context) : base ()
		{
			this.context = context;
		}

		public override int Count {
			get {
				return Prices == null ? 0 : Prices.Count;
			}
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override string this[int index] {
			get {
				return Prices [index].ProductName;
			}
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView;
			if (view == null) {
				view = context.LayoutInflater.Inflate (Resource.Layout.PricesResultCell, parent, false);
				var holder = new SDXPricesResultCell (context, view);
				view.SetTag (Resource.Id.pricesresult_listview, holder);
				(new Handler ()).Post (() => {
					context.RunOnUiThread (() => {
						LayoutAdjuster.FitToScreen (context, (ViewGroup)view, Data.XRate, Data.XRate, Data.XRate, Data.XRate, Data.Density);
						holder.Update (Prices [position]);
					});
				});
			} else {
				var holder = view.GetTag (Resource.Id.pricesresult_listview) as SDXPricesResultCell;
				holder.Update (Prices [position]);
			}
			return view;
		}
	}
}

