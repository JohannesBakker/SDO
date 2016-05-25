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
	public class SDXOutletsListAdapter : BaseAdapter <string>
	{
		Activity context;
		public IList <OutletModel> Outlets;
		public EventHandler <int> OfferSelectedEvent;
		public EventHandler <int> DeleteOfferEvent;
		public EventHandler <int> OutletEditEvent;
		public EventHandler <int> AddOfferEvent;

		public SDXOutletsListAdapter (Activity context) : base ()
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
				view = context.LayoutInflater.Inflate (Resource.Layout.OutletDetail_Cell, parent, false) as LinearLayout;
				SDXOutletDetailCell cell = new SDXOutletDetailCell (context, view);
				cell.OfferSelectedEvent = OfferSelectedEvent;
				cell.DeleteOfferEvent = DeleteOfferEvent;
				cell.OutletEditEvent = OutletEditEvent;
				cell.AddOfferEvent = AddOfferEvent;
				view.SetTag (Resource.Id.accountdetail_listview, cell);
				(new Handler ()).Post (new Java.Lang.Runnable (() => {
					context.RunOnUiThread (() => {
						LayoutAdjuster.FitToScreen (context, (ViewGroup)view, Data.XRate, Data.XRate, Data.XRate, Data.XRate, Data.Density);
						cell.Update (Outlets [position], position);
					});
				}));
			} else {
				SDXOutletDetailCell cell = view.GetTag (Resource.Id.accountdetail_listview) as SDXOutletDetailCell;
				cell.Update (Outlets [position], position);
			}
			Util.StartFadeAnimation (view, 200);
			return view;
		}
	}
}

