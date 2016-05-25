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
	public class SDXPlanogramsListAdapter : BaseAdapter <string>
	{
		Activity context;
		public IList <OutletModel> Outlets;
		public EventHandler <int> OfferSelectedEvent;

		public SDXPlanogramsListAdapter (Activity context) : base ()
		{
			this.context = context;
		}

		public override int Count {
			get {
				int cnt = Outlets == null ? 0 : Outlets.Count;
				return cnt;
			}
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override string this[int index] {
			get {
				return Outlets [index].Name;
			}
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView;
			if (view == null) {
				view = context.LayoutInflater.Inflate (Resource.Layout.PlanogramCell, parent, false) as LinearLayout;
				var cell = new SDXPlanogramCell (context, view);
				cell.OfferSelectedEvent = OfferSelectedEvent;
				view.SetTag (Resource.Id.planograms_listview, cell);
				(new Handler ()).Post (new Java.Lang.Runnable (() => {
					context.RunOnUiThread (()=>{
						LayoutAdjuster.FitToScreen (context, (ViewGroup)view, Data.XRate, Data.XRate, Data.XRate, Data.XRate, Data.Density);
						cell.Update (Outlets [position], position);
					});
				}));
			} else {
				var cell = view.GetTag (Resource.Id.planograms_listview) as SDXPlanogramCell;
				cell.Update (Outlets [position], position);
			}
			return view;
		}
	}
}

