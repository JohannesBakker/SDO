
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
	public class SDXLookupAccountFragment : SDXBaseFragment
	{
		public AccountModel Account = null;
		public bool IsFromDashboard = false;

		private IList <AccountModel> accountList;
		private int selectedConsumerTypeId = -1;
		private List <string> consumerTypes = new List<string> ();

		private View view;
		private FrameLayout unitContentView, deleteAccountView, lookupView;
		private TextView titleTv, descTv;
		private Button nextBtn;
		private EditText unitNumberEt;
		private Spinner consumerSpinner;

		#region Fragment Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			SetHeaderTitle ("In the Zone Setup", 1);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.LookupAccount, container, false);
			LayoutView (view);

			GetInstance ();

			return view;
		}

		async public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
			if (manager.Me == null || manager.Me.Accounts == null) {
				if (accountList == null)
					accountList = new List <AccountModel> ();
				else
					accountList.Clear ();
			} else
				accountList = manager.Me.Accounts;

			if (Account != null || accountList.Count != 0 || IsFromDashboard)
				AddBackBtn (1);

			var scrollV = view.FindViewById <ScrollView> (Resource.Id.lookupaccount_sv);
			scrollV.Visibility = ViewStates.Invisible;

			if (consumerTypes.Count == 0) {
				SDXReferenceDataManager refMgr = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
				if (refMgr.DataModel == null) {
					await LoadReferenceData ();
				}

				foreach (ConsumerTypeModel model in refMgr.DataModel.ConsumerTypes) {
					consumerTypes.Add (model.Description);
				}
				ArrayAdapter <string> adapter = new ArrayAdapter<string> (Activity, global::Android.Resource.Layout.SimpleSpinnerItem, consumerTypes.ToArray ());
				adapter.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
				consumerSpinner.Adapter = adapter;
				consumerSpinner.ItemSelected +=  (object sender, AdapterView.ItemSelectedEventArgs e) => {
					SDXReferenceDataManager mgr = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
					ConsumerTypeModel type = mgr.DataModel.ConsumerTypes.ElementAt (e.Position);
					selectedConsumerTypeId = type.ConsumerTypeId;
				};
				consumerSpinner.Prompt = "Select Spend Profile";
			}

			if (Account == null) {
				// Add Toolbar on Keyboard
				unitContentView.Visibility = ViewStates.Invisible;
				deleteAccountView.Visibility = ViewStates.Gone;
			} else {
				titleTv.Text = "UPDATE ACCOUNT";
				SetHeaderTitle ("ACCOUNT", 1);
				nextBtn.Text = "DONE";

				unitNumberEt.Enabled = false;
				unitNumberEt.Text = Account.LocationId;

				consumerSpinner.SetSelection (consumerTypes.IndexOf (Account.ConsumerType.Description));

				FillLocationInfoToViews (Account.Location);

				descTv.Visibility = ViewStates.Gone;
				lookupView.Visibility = ViewStates.Gone;
			}

			scrollV.Visibility = ViewStates.Visible;
		}
		#endregion

		#region Private Functions
		private void GetInstance ()
		{
			unitContentView = view.FindViewById (Resource.Id.lookupaccount_unit_content_fl) as FrameLayout;
			titleTv = view.FindViewById (Resource.Id.lookupaccount_title_tv) as TextView;
			descTv = view.FindViewById (Resource.Id.lookupaccount_desc_tv) as TextView;
			unitNumberEt = view.FindViewById (Resource.Id.lookupaccount_unit_number_et) as EditText;
			deleteAccountView = view.FindViewById (Resource.Id.lookupaccount_delete_account_fl) as FrameLayout;
			lookupView = view.FindViewById (Resource.Id.lookupaccount_lookup_fl) as FrameLayout;
			consumerSpinner = view.FindViewById (Resource.Id.lookupaccount_consumer_type_sp) as Spinner;
			nextBtn = view.FindViewById <Button> (Resource.Id.lookupaccount_next_btn);

			var lookupBtn = view.FindViewById <Button> (Resource.Id.lookupaccount_lookup_btn);
			lookupBtn.Click += LookupAccountBtn_OnClicked;
			nextBtn.Click += NextBtn_OnClicked;

			var deleteAccountBtn = view.FindViewById (Resource.Id.lookupaccount_delete_account_btn) as Button;
			deleteAccountBtn.Click += DeleteAccountBtn_OnClicked;
		}

		private void FillLocationInfoToViews(LocationModel location)
		{
			unitContentView.Visibility = ViewStates.Visible;

			var unitNameTv = view.FindViewById (Resource.Id.lookupaccount_unit_name_tv) as TextView;
			var address1Tv = view.FindViewById <TextView> (Resource.Id.lookupaccount_address1_tv);
			var address2Tv = view.FindViewById <TextView> (Resource.Id.lookupaccount_address2_tv);
			var cityTv = view.FindViewById <TextView> (Resource.Id.lookupaccount_city_tv);
			var stateTv = view.FindViewById <TextView> (Resource.Id.lookupaccount_state_tv);
			var zipTv = view.FindViewById <TextView> (Resource.Id.lookupaccount_zip_tv);

			unitNameTv.Text = location.LocationName;
			address1Tv.Text = location.LocationAddress1;
			address2Tv.Text = " ";
			cityTv.Text = location.LocationCity;
			stateTv.Text = location.LocationStateCd;
			zipTv.Text = location.LocationZip;
        }

		async private void LookupLocation ()
		{
			Util.ShowLoading (Activity);
			SDXLocationManager manager = TinyIoCContainer.Current.Resolve <SDXLocationManager> ();
			LocationModel location = await manager.LoadLocation (unitNumberEt.Text);
			Util.HideLoading ();

			if (!manager.IsSuccessed) {
				Util.ShowAlert ("Can't find location. Please try another account", Activity);
				unitNumberEt.Text = "";
				return;
			}

			FillLocationInfoToViews (location);
		}

		async private void AddMeToAccount (AccountModel account)
		{
			Util.ShowLoading (Activity);
			SDXUserManager manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
			await manager.AddMeToAccount (account.LocationId);
			manager.Me.Accounts.Add (account);
			Util.HideLoading ();

			if (!manager.IsSuccessed) {
				Util.ShowAlert ("Can't add this account. Please try again or another.", Activity);
				return;
			}

			// Go To Account Detail
			var fragment = new SDXAccountDetailFragment ();
			fragment.Account = account;
			fragment.PopCount = 2;
			PushFragment (fragment, "SDXLookupAccountFragment");
		}

		async private void UpdateAccount ()
		{
			string locationId = Account.LocationId;
			string consumerTypeId = selectedConsumerTypeId.ToString ();
			string rowVersion = System.Convert.ToBase64String (Account.RowVersion);

			Util.ShowLoading (Activity);
			SDXAccountManager manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			AccountModel account = await manager.UpdateAccount(locationId, consumerTypeId, rowVersion);
			Util.HideLoading ();

			if (!manager.IsSuccessed) {
				Util.ShowAlert ("Can't update account. Please try again.", Activity);
				return;
			} else {
				var builder = new AlertDialog.Builder (Activity);
				builder.SetMessage ("Account has been successfully updated.");
				builder.SetPositiveButton ("OK", (o, e) => {
					var fragment = ((MainActivity)Activity).AccountDetailFragment;
					fragment.Account = account;
					PopFragment ();
				});
				builder.Create ().Show ();
			}
		}

		async private void DeleteAccount ()
		{
			Util.ShowLoading (Activity);
			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			await manager.DeleteAccount (Account.LocationId);
			Util.HideLoading ();

			if (!manager.IsSuccessed) {
				Util.ShowAlert (manager.ErrorMessage, Activity);
				return;
			}

			var builder = new AlertDialog.Builder (Activity);
			builder.SetMessage ("Account has been successfully deleted.");
			builder.SetPositiveButton ("OK", (o, e) => {
				if (accountList.Contains (Account))
					accountList.Remove (Account);
				PopCount = 2;
				PopFragment ();
			});
			builder.Create ().Show ();
		}

		private void MoveToAddOutletScreen ()
		{
			AccountModel account = new AccountModel ();
			account.LocationId = unitNumberEt.Text;
			account.ConsumerTypeId = selectedConsumerTypeId;

			var fragment = new SDXAddOutletFragment ();
			fragment.Account = account;
			PushFragment (fragment, "SDXLookupAccountFragment");
		}
		#endregion

		#region Button Actions
		async void LookupAccountBtn_OnClicked (object sender, EventArgs arg)
		{
			if (Account != null)
				return;
			// Check If Account Number has 8 length
			if (unitNumberEt.Text.Length != 8) {
				Util.ShowAlert ("The length of account number should be 8.", Activity);
				return;
			}

			HideKeyboard (unitNumberEt);

			// Check If Account is Already Associated.
			AccountModel associatedAccount = null;
			int index = 0;
			if (accountList.Count > 0) {
				foreach (AccountModel myaccount in accountList) {
					if (myaccount.LocationId == unitNumberEt.Text) {
						associatedAccount = myaccount;
						break;
					}
					index ++;
				}
			}
			if (associatedAccount != null) {
				var builder = new AlertDialog.Builder (Activity);
				builder.SetTitle ("Account is already associated");
				builder.SetMessage ("Would like to go to details?");
				builder.SetPositiveButton ("Yes", (o, e) => {
					// Go to Account Detail Screen
					var fragment = new SDXAccountDetailFragment ();
					fragment.Account = associatedAccount;
					fragment.PopCount = 2;
					PushFragment (fragment, "SDXLookupAccountFragment");
				});
				builder.SetNegativeButton ("No", (o, el) => {
					unitNumberEt.Text = "";
				});
				builder.Create().Show();
				return;
			}
			Util.ShowLoading (Activity);
			SDXAccountManager accountMgr = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			AccountModel account = await accountMgr.LoadAccount (unitNumberEt.Text, false);
			if (accountMgr.IsSuccessed) {
				Util.HideLoading ();
				var builder = new AlertDialog.Builder (Activity);
				builder.SetTitle ("This Account is already set");
				builder.SetMessage ("Do you want to add this account?");
				builder.SetPositiveButton ("Yes", (o, e) => {
					AddMeToAccount (account);
				});
				builder.SetNegativeButton ("No", (o, el) => {
					unitNumberEt.Text = "";
				});
				builder.Create ().Show ();
				return;
			}
            Util.HideLoading();
			LookupLocation ();
		}

		void NextBtn_OnClicked (object sender, EventArgs args)
		{
			if (Account == null) {
				if (unitNumberEt.Text.Length != 8) {
					Util.ShowAlert ("Please input correct Account Number.", Activity);
					return;
				}

				if (selectedConsumerTypeId == -1) {
					Util.ShowAlert ("Please select consumer type.", Activity);
					return;
				}

				MoveToAddOutletScreen ();
			} else {
				if (Account.ConsumerType.Description == (string)consumerSpinner.SelectedItem) {
					Util.ShowAlert ("No changes, you can change Consumer Type to update.", Activity);
					return;
				} else {
					UpdateAccount ();
				}
			}
		}

		async void DeleteAccountBtn_OnClicked (object sender, EventArgs args)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder (Activity);
			builder.SetMessage ("Are you sure, you want to remove this account?");
			builder.SetPositiveButton ("Yes", (object s, DialogClickEventArgs e) => {
				DeleteAccount ();
			});
			builder.SetNegativeButton ("No", (o, e) => {});
			builder.Create ().Show ();
		}
		#endregion
	}
}

