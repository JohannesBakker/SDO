
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Views.Animations;

using TinyIoC;
using Sodexo.Core;
using Sodexo.RetailActivation.Portable.Models;
using System.Threading.Tasks;

namespace Sodexo.Android
{
	public class SDXPlanogramsFragment : SDXBaseFragment
	{
		public FeedbackTypeModel FeedbackType;

		private List<OutletModel> filteredOutlets = new List<OutletModel> ();
		ListView listView;
		SDXPlanogramsListAdapter listAdp;

		View view;

		#region Fragment Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			HideAllHeaderButtons ();
			SetHeaderTitle ("Select Planogram", 3);
			AddBackBtn (3);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.Planograms, container, false);
			LayoutView (view);

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			if (listAdp == null) {
				listAdp = new SDXPlanogramsListAdapter (Activity);
				listAdp.OfferSelectedEvent = OnOfferSelected;

				listView = view.FindViewById (Resource.Id.planograms_listview) as ListView;
				listView.DividerHeight = 0;
				listView.CacheColorHint = Color.Transparent;
				listView.Adapter = listAdp;
			}

			LoadMe ();
		}
		#endregion

		#region Event Handler
		private void OnOfferSelected (object sender, int row)
		{
			var btn = (Button) sender;
			int tag = int.Parse ((((string)btn.Tag).Substring (0, 2)));
			var selectedOffer = filteredOutlets [row].Offers [tag];
			var fragment = new LeaveFeedbackFragment ();
			fragment.FeedbackType = FeedbackType;
			fragment.Offer = selectedOffer;
			PushFragment (fragment, "SDXPlanogramsFragment");
		}
		#endregion

		#region Private Functions
		private async void LoadMe ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();

			if (manager.Me == null || manager.Me.Accounts == null) {
				ShowLoading ();
				await manager.LoadMe ();
				HideLoading ();

				if (!manager.IsSuccessed) {
					ShowErrorMessage (manager.ErrorMessage);
					return;
				}
			}

			var me = manager.Me;

			filteredOutlets.Clear ();

			foreach (var account in me.Accounts) {
				foreach (var outlet in account.Outlets) {
					for (int i = 0; i < outlet.Offers.Count; i++) {
						if (outlet.Offers [i].Responses.Count == 0) {
							outlet.Offers.RemoveAt (i);
							i--;
						}
					}
					if (outlet.Offers.Count > 0)
						filteredOutlets.Add (outlet);
				}
			}

			Util.StartFadeAnimation (listView, 300);
			listAdp.Outlets = filteredOutlets;
			listAdp.NotifyDataSetChanged ();

			if (filteredOutlets.Count == 0) {
				AlertDialog.Builder builder = new AlertDialog.Builder (Activity);
				builder.SetMessage ("You did not set up any planograms yet, Click OK to go to planograms section and create one, or Cancel to select different feedback subject");
				builder.SetPositiveButton ("OK", (object sender, DialogClickEventArgs e) => {
					var fragment = new SDXAccountsFragment ();
					PushFragment (fragment, "SDXPlanogramsFragment");
				});
				builder.SetNegativeButton ("Cancel", (object sender, DialogClickEventArgs e) => {
					PopFragment ();
				});
				builder.Create ().Show ();
			}
		}
		#endregion
	}
}