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
	public class SDXPromotionListAdapter : BaseAdapter <string>
	{
		public List <PromotionModel> Promotions;

		Activity context;

		public SDXPromotionListAdapter (Activity context) : base ()
		{
			this.context = context;
		}

		public override int Count {
			get {
				return Promotions != null ? Promotions.Count : 0;
			}
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override string this[int index] {
			get {
				return Promotions [index].Description;
			}
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView;
			if (view == null) {
				view = context.LayoutInflater.Inflate (Resource.Layout.PromotionCell, parent, false);
				SDXPromotionCell cell = new SDXPromotionCell (context, view);
				view.SetTag (Resource.Id.promotions_listview, cell);
				(new Handler ()).Post (() => {
					context.RunOnUiThread (()=>{
						LayoutAdjuster.FitToScreen (context, (ViewGroup)view, Data.XRate, Data.XRate, Data.XRate, Data.XRate, Data.Density);
						cell.Update (Promotions [position]);
					});
				});
			} else {
				SDXPromotionCell cell = view.GetTag (Resource.Id.promotions_listview) as SDXPromotionCell;
				cell.Update (Promotions [position]);
			}

			return view;
		}
	}
}

