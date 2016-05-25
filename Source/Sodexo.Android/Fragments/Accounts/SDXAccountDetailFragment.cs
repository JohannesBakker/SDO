
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

namespace Sodexo.Android
{
	public class SDXAccountDetailFragment : SDXBaseFragment
	{
		public AccountModel Account;
		public string LocationId;

		private IList <OutletModel> _outlets;

		View view;
		ListView listView;
		SDXOutletsListAdapter listAdapter;
		LinearLayout accountView;

		#region Fragment Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			SetHeaderTitle ("Account Details", 1);
			HideAllHeaderButtons ();
			AddBackBtn (1);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.AccountDetail, container, false);
			LayoutView (view);

			var editBtn = view.FindViewById (Resource.Id.accountdetail_edit_btn) as ImageButton;
			editBtn.Click += EditBtn_OnClicked;

			listAdapter = new SDXOutletsListAdapter (Activity);
			listAdapter.OfferSelectedEvent = OnOfferRow_Selected;
			listAdapter.DeleteOfferEvent = OnDeleteOffer;
			listAdapter.OutletEditEvent = OutletEditBtn_OnClicked;
			listAdapter.AddOfferEvent = AddOfferBtn_OnClicked;
			listView = view.FindViewById (Resource.Id.accountdetail_listview) as ListView;
			listView.CacheColorHint = Color.Transparent;
			listView.DividerHeight = 0;
			listView.Adapter = listAdapter;

			accountView = view.FindViewById (Resource.Id.accountdetail_account_ll) as LinearLayout;

			return view;
		}

		async public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			((MainActivity)Activity).AccountDetailFragment = null;

			var addBtn = ((MainActivity)Activity).AddBtn;
			addBtn.Visibility = ViewStates.Visible;
			addBtn.SetPadding (0, 0, 0, 0);
			addBtn.Click += AddMenuBtn_OnClicked;

			accountView.Visibility = ViewStates.Invisible;

			if (Account != null) {
				bool isSuccess = await LoadReferenceData ();
				if (isSuccess)
					FillContents ();
			} else {
				LoadAccount ();
			}
		}

		public override void OnDestroyView ()
		{
			base.OnDestroyView ();

			((MainActivity)Activity).AddBtn.Click -= AddMenuBtn_OnClicked;
		}
		#endregion

		#region Private Functions
		private void FillContents ()
		{
			FillAccountView ();

			_outlets = Account.Outlets;

			Util.StartFadeAnimation (listView, 300);
			listAdapter.Outlets = _outlets;
			listAdapter.NotifyDataSetChanged ();
		}

		private void FillAccountView ()
		{
			accountView.Visibility = ViewStates.Visible;

			var nameTv = view.FindViewById (Resource.Id.accountdetail_name_tv) as TextView;
			var addressTv = view.FindViewById (Resource.Id.accountdetail_address_tv) as TextView;
			var idTv = view.FindViewById (Resource.Id.accountdetail_locationid_tv) as TextView;

			var location = Account.Location;
			nameTv.Text = location.LocationName;
			addressTv.Text = location.LocationCity + " " + location.LocationAddress1;
			idTv.Text = location.LocationId;
		}

		async private void LoadAccount ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			Util.ShowLoading (Activity);
			Account = await manager.LoadAccount (LocationId, false);

			if (!manager.IsSuccessed) {
				Util.HideLoading ();
				Util.ShowAlert (manager.ErrorMessage, Activity);
				return;
			}
	
			bool isSuccess = await LoadReferenceData ();
			Util.HideLoading ();
			if (isSuccess)
				FillContents ();
		}

		async private void DeleteOffer (int outletIdx, int offerIdx)
		{
			OutletModel outlet = _outlets [outletIdx];
			var offer = outlet.Offers [offerIdx];

			string outletId = outlet.OutletId.ToString ();
			string offerId = offer.OfferId.ToString ();
			string locationId = Account.LocationId;

			ShowLoading ();
			SDXAccountManager manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			await manager.DeleteOffer (locationId, outletId, offerId);
			HideLoading ();

			if (manager.IsSuccessed) {
				outlet.Offers.Remove (offer);
				listAdapter.NotifyDataSetChanged ();
			} else {
				ShowErrorMessage (manager.ErrorMessage);
			}
		}

		private void MoveToAddOutletScreen (OutletModel outlet)
		{
			var fragment = new SDXAddOutletFragment ();
			fragment.Outlet = outlet;
			fragment.LocationId = Account.LocationId;
			fragment.Account = Account;
			Console.WriteLine (this.Class.SimpleName);
			PushFragment (fragment, this.Class.SimpleName);
		}

		private void MoveToOfferDetailScreen (int outletIdx, int offerIdx)
		{
			Console.WriteLine ("MoveToOfferDetail : " + outletIdx.ToString () + " " + offerIdx.ToString ());
			var fragment = new SDXOfferDetailFragment ();
			fragment.Account = Account;
			fragment.OutletIndex = outletIdx;
			fragment.OfferIndex = offerIdx;
			Console.WriteLine (this.Class.SimpleName);
			PushFragment (fragment, this.Class.SimpleName);
		}

		private void MoveToSelectPlanogramScreen (int outletIdx, int offerIdx)
		{
			var fragment = new SDXSelectPlanogramFragment ();
			fragment.Account = Account;
			fragment.OutletIndex = outletIdx;
			fragment.OfferIndex = offerIdx;
			PushFragment (fragment, this.Class.SimpleName);
		}
		#endregion

		#region Button Actions
		private void AddMenuBtn_OnClicked (object sender, EventArgs ea)
		{
			MoveToAddOutletScreen (null);
		}

		private void EditBtn_OnClicked (object sender, EventArgs e)
		{
			((MainActivity)Activity).AccountDetailFragment = this;
			var fragment = new SDXLookupAccountFragment ();
			fragment.Account = Account;
			PushFragment (fragment, "SDXAccountDetailFragment");
		}
		#endregion

		#region Event Handler
		private void OutletEditBtn_OnClicked (object sender, int row)
		{
			MoveToAddOutletScreen (_outlets [row]);
		}

		private async void AddOfferBtn_OnClicked (object sender, int offerCategoryIdx)
		{
			Button btn = sender as Button;
			int tag = int.Parse (((string)btn.Tag).Substring (0, 2));
			OutletModel outlet = _outlets[tag];

			string outletId = outlet.OutletId.ToString ();

			var dataManager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			OfferCategoryModel offerCategory = dataManager.DataModel.OfferCategories [offerCategoryIdx];

			ShowLoading ();
			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			OfferModel newOffer = await manager.AddOffer (Account.LocationId, outletId, offerCategory.Description, offerCategory.OfferCategoryId.ToString());
			HideLoading ();

			if (!manager.IsSuccessed) {
				ShowErrorMessage (manager.ErrorMessage);
			} else {
				AlertDialog.Builder builder = new AlertDialog.Builder (Activity);
				builder.SetMessage ("Offer has been successfully added.");
				builder.SetPositiveButton ("OK", (object s, DialogClickEventArgs e) => {
					outlet.Offers.Add (newOffer);
					listAdapter.NotifyDataSetChanged ();
				});
				builder.Create ().Show ();
			}
		}

		private void OnOfferRow_Selected (object sender, int row)
		{
			OutletModel outlet = _outlets [row];
			var btn = (Button) sender;
			int tag = int.Parse ((((string)btn.Tag).Substring (0, 2)));
			var offer = outlet.Offers [tag];

			if (offer.Responses.Count != 0) {
				MoveToOfferDetailScreen (row, tag);
			} else {
				MoveToSelectPlanogramScreen (row, tag);
			}
		}

		private void OnDeleteOffer (object sender, int row)
		{
			var btn = (Button) sender;
			int tag = int.Parse ((((string)btn.Tag).Substring (0, 2)));
			AlertDialog.Builder builder = new AlertDialog.Builder (Activity);
			builder.SetMessage ("Are you sure, you want to remove this offer?");
			builder.SetPositiveButton ("Yes", (object s, DialogClickEventArgs e) => {
				DeleteOffer (row, tag);
			});
			builder.SetNegativeButton ("No", (o, e) => {});
			builder.Create ().Show ();
		}
		#endregion
	}
}

