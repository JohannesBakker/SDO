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
	public class SDXAccountsListAdapter : BaseAdapter <string>
	{
		Activity context;
		public IList <AccountModel> Accounts;
		public EventHandler NavgiateBtn_OnClicked;

		public SDXAccountsListAdapter (Activity context) : base ()
		{
			this.context = context;
		}

		public override int Count {
			get {
				int cnt = Accounts != null ? Accounts.Count + 1 : 1;
				return cnt;
			}
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View view = convertView;
			if (view == null) {
				view = context.LayoutInflater.Inflate (Resource.Layout.Accounts_Cell, parent, false) as LinearLayout;
				view.SetTag (Resource.Id.accounts_listview, new InstanceHolder ());
				(new Handler ()).Post (new Java.Lang.Runnable (() => {
					context.RunOnUiThread (()=>{
						LayoutAdjuster.FitToScreen (context, (ViewGroup)view, Data.XRate, Data.YRate, Data.XRate, Data.XRate, Data.Density);
						Update (position, view);
					});
				}));
			} else {
				Update (position, view);
			}
			return view;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override string this[int position] {  
			get { 
				return Accounts [position].AccountId.ToString ();
			}
		}

		private InstanceHolder GetInstanceHolder (View view, out bool isExist)
		{
			InstanceHolder holder = view.GetTag (Resource.Id.accounts_listview) as InstanceHolder;

			if (holder.nameTv == null) {
				isExist = false;
				holder.contentsLL = view.FindViewById (Resource.Id.accounts_cell_contents_ll) as LinearLayout;
				holder.numberTv = view.FindViewById (Resource.Id.accounts_cell_number_tv) as TextView;
				holder.navigateBtn = view.FindViewById (Resource.Id.accounts_cell_navigate_btn) as Button;
				holder.navigateBtn.Click += NavgiateBtn_OnClicked;
				holder.separateIv = view.FindViewById (Resource.Id.accounts_cell_separate_line_iv) as ImageView;
				holder.addressTv = view.FindViewById (Resource.Id.accounts_cell_address_tv) as TextView;
				holder.locationIdTv = view.FindViewById (Resource.Id.accounts_cell_locationid_tv) as TextView;
				holder.plusIv = view.FindViewById (Resource.Id.accounts_cell_plus_iv) as ImageView;
				holder.arrowIv = view.FindViewById (Resource.Id.accounts_cell_arrow_iv) as ImageView;
				holder.nameTv = view.FindViewById (Resource.Id.accounts_cell_name_tv) as TextView;
			} else
				isExist = true;

			return holder;
		}

		private void Update (int num, View view)
		{
			bool isExist = false;
			InstanceHolder holder = GetInstanceHolder (view, out isExist);

			for (int i = 0;; i++) {
				View outletV = holder.contentsLL.FindViewWithTag ("outlet-" + i);
				if (outletV != null)
					holder.contentsLL.RemoveView (outletV);
				else
					break;
			}

			AccountModel account = null;
			if (Accounts != null && num < Accounts.Count)
				account = Accounts [num];

			holder.numberTv.Text = (num + 1).ToString ("00");
			if (!isExist)
				holder.navigateBtn.Tag = num.ToString ("00") + "-" + holder.navigateBtn.Tag;

			if (account == null) {
				holder.separateIv.Visibility = ViewStates.Gone;
				holder.numberTv.Enabled = false;
				holder.addressTv.Visibility = ViewStates.Gone;
				holder.locationIdTv.Visibility = ViewStates.Gone;
				holder.arrowIv.Visibility = ViewStates.Gone;
				holder.plusIv.Visibility = ViewStates.Visible;
				holder.nameTv.Text = "ADD NEW";
				return;
			}

			holder.separateIv.Visibility = ViewStates.Visible;

			holder.numberTv.Enabled = true;
			holder.addressTv.Visibility = ViewStates.Visible;
			holder.locationIdTv.Visibility = ViewStates.Visible;
			holder.arrowIv.Visibility = ViewStates.Visible;
			holder.plusIv.Visibility = ViewStates.Gone;

			holder.nameTv.Text = account.Location.LocationName;
			holder.addressTv.Text = account.Location.LocationCity + " " + account.Location.LocationAddress1;
			holder.locationIdTv.Text = account.LocationId.ToString ();

			IEnumerable<OutletModel> outlets = account.Outlets;
			for (int i = 0; i < outlets.Count(); i++) 
			{
				OutletModel outlet = outlets.ElementAt (i);

				var outletV = SDXOutletView.Create (context, (ViewGroup)view);
				outletV.Update (outlet);

				outletV.view.Tag = "outlet-" + i;
				holder.contentsLL.AddView (outletV.view);
			}
		}

		class InstanceHolder : Java.Lang.Object
		{
			public LinearLayout contentsLL;
			public TextView numberTv, nameTv, addressTv, locationIdTv;
			public Button navigateBtn;
			public ImageView separateIv, plusIv, arrowIv;
		}
	}
}

