
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
	public class SDXPricesFragment : SDXBaseFragment
	{
		View view;

		private bool isBasic = false;
		int[] modelIds = new int[4];
		private List<OutletModel> outlets = new List<OutletModel> ();

		List<ProductPriceModel> prices;

		View basicView, advancedView;
		Spinner stateSp, businessSegSp, consumerTpSp, localCompSp;
		EditText keywordEt, basicKeywordEt;
		ScrollView scrollV;
		LinearLayout outletsView, containerView;

		#region Fragment Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			HideAllHeaderButtons ();
			SetHeaderTitle ("Prices", 4);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.Prices, container, false);
			LayoutView (view);

			GetInstance ();

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			((MainActivity)Activity).ChangeBtn.Click += ChangeNavBtn_OnClicked;
			((MainActivity)Activity).ChangeBtn.Visibility = ViewStates.Visible;

			if (basicView.Parent == null || advancedView.Parent == null)
				return;

			scrollV.Visibility = ViewStates.Invisible;

			for (int i = 0; i < 4; i++)
				modelIds [i] = -1;

			FillContents ();
		}

		public override void OnDestroyView ()
		{
			base.OnDestroyView ();

			((MainActivity)Activity).ChangeBtn.Click -= ChangeNavBtn_OnClicked;
		}
		#endregion

		#region Button Actions
		private void ChangeNavBtn_OnClicked (object sender, EventArgs ea)
		{
			if (isBasic) {
				((MainActivity)Activity).ChangeBtn.SetImageResource (Resource.Drawable.icon_basic);
				containerView.RemoveView (basicView);
				containerView.AddView (advancedView);
				containerView.RefreshDrawableState ();
			} else {
				containerView.RemoveView (advancedView);
				containerView.AddView (basicView);
				((MainActivity)Activity).ChangeBtn.SetImageResource (Resource.Drawable.icon_m);
			}
			isBasic = !isBasic;
			Util.StartFadeAnimation (view, 300);
		}

		async void SearchBtn_OnClicked (object sender, EventArgs args)
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXPriceManager> ();
			for (int i = 0; i < modelIds.Count(); i++) {
				if (modelIds [i] < 0) {
					ShowAlert ("Please select all required items.");
					return;
				}
			}
			ShowLoading ();
			prices = await manager.LoadPricesAdvance (modelIds[1], modelIds[0], modelIds[3], modelIds[2], 0, "");
			HideLoading ();

			if (!manager.IsSuccessed) {
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			MoveToPriceDetailScreen ();

            HideKeyboard(keywordEt);
            HideKeyboard(basicKeywordEt);
		}

		async void OutletBtn_OnClicked (object sender, EventArgs arg)
		{
			Button btn = (Button)sender;
			var outletId = outlets [int.Parse ((string)btn.Tag) - 10].OutletId;

            HideKeyboard(keywordEt);
            HideKeyboard(basicKeywordEt);

			var manager = TinyIoCContainer.Current.Resolve <SDXPriceManager> ();
			ShowLoading ();
			prices = await manager.LoadPricesBasic (outletId, "");
			HideLoading ();

			if (!manager.IsSuccessed) {
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			MoveToPriceDetailScreen ();
		}
		#endregion

		#region Private Functions
		private void GetInstance ()
		{
			scrollV = view.FindViewById (Resource.Id.prices_scrollview) as ScrollView;
			containerView = view.FindViewById (Resource.Id.prices_container_ll) as LinearLayout;
			keywordEt = view.FindViewById (Resource.Id.prices_keyword_et) as EditText;
			basicKeywordEt = view.FindViewById (Resource.Id.prices_basic_keyword_et) as EditText;
			basicView = view.FindViewById (Resource.Id.prices_basic_ll) as LinearLayout;
			advancedView = view.FindViewById (Resource.Id.prices_advanced_ll) as LinearLayout;
			stateSp = view.FindViewById (Resource.Id.prices_state_sp) as Spinner;
			businessSegSp = view.FindViewById (Resource.Id.prices_business_segment_sp) as Spinner;
			consumerTpSp = view.FindViewById (Resource.Id.prices_consumer_type_sp) as Spinner;
			localCompSp = view.FindViewById (Resource.Id.prices_local_competition_sp) as Spinner;
			outletsView = view.FindViewById (Resource.Id.prices_outlets_ll) as LinearLayout;

			var searchBtn = view.FindViewById (Resource.Id.prices_search_btn) as Button;
			searchBtn.Click += SearchBtn_OnClicked;
		}

		private void MoveToPriceDetailScreen ()
		{
			var fragment = new SDXPricesResultFragment ();
			fragment.Prices = prices;
			if (isBasic) {
				fragment.query = basicKeywordEt.Text;
			} else
				fragment.query = keywordEt.Text;
			PushFragment (fragment, "SDXPricesFragment");
		}

		private async Task LoadMe ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();

			if (manager.Me == null || manager.Me.Accounts == null) {
				Util.ShowLoading (Activity);
				await manager.LoadMe ();

				if (!manager.IsSuccessed) {
					Util.ShowAlert (manager.ErrorMessage, Activity);
					return;
				}
			}

			var me = manager.Me;

			outlets.Clear ();
			foreach (AccountModel account in me.Accounts) {
				foreach (var outlet in account.Outlets) {
					outlets.Add (outlet);
				}
			}
			return;
		}

		async private void FillContents ()
		{
			await LoadMe ();
			await LoadReferenceData ();
			Util.HideLoading ();

			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel == null)
				return;

			FillSpinners ();
			FillOutlets ();

			scrollV.Visibility = ViewStates.Visible;

			if (outlets.Count == 0) {
				isBasic = false;
				containerView.RemoveView (basicView);
				((MainActivity)Activity).ChangeBtn.Visibility = ViewStates.Gone;
			} else {
				isBasic = true;
				containerView.RemoveView (advancedView);
				((MainActivity)Activity).ChangeBtn.SetImageResource (Resource.Drawable.icon_m);
			}
		}

		private void FillSpinners ()
		{
			SDXReferenceDataManager refMgr = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();

			var states = new List <string> ();
			foreach (StateModel model in refMgr.DataModel.States) {
				if (!model.ShowInPricing)
					continue;
				states.Add (model.Name);
			}
			ArrayAdapter <string> stateAdp = new ArrayAdapter<string> (Activity, global::Android.Resource.Layout.SimpleSpinnerItem, states.ToArray ());
			stateAdp.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			stateSp.Adapter = stateAdp;
			stateSp.ItemSelected +=  (object sender, AdapterView.ItemSelectedEventArgs e) => {
				SDXReferenceDataManager mgr = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
				StateModel type = mgr.DataModel.States.ElementAt (e.Position);
				modelIds [0] = type.PricingBandId;
			};

			var divisions = new List <string> ();
			foreach (DivisionModel model in refMgr.DataModel.Divisions) {
				if (!model.ShowInPricing)
					continue;
				divisions.Add (model.Name);
			}
			ArrayAdapter <string> divisionAdp = new ArrayAdapter<string> (Activity, global::Android.Resource.Layout.SimpleSpinnerItem, divisions.ToArray ());
			divisionAdp.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			businessSegSp.Adapter = divisionAdp;
			businessSegSp.ItemSelected +=  (object sender, AdapterView.ItemSelectedEventArgs e) => {
				SDXReferenceDataManager mgr = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
				DivisionModel type = mgr.DataModel.Divisions.ElementAt (e.Position);
				modelIds [1] = type.DivisionId;
			};

			var consumerTypes = new List <string> ();
			foreach (ConsumerTypeModel model in refMgr.DataModel.ConsumerTypes) {
				consumerTypes.Add (model.Description);
			}
			ArrayAdapter <string> consumerAdp = new ArrayAdapter<string> (Activity, global::Android.Resource.Layout.SimpleSpinnerItem, consumerTypes.ToArray ());
			consumerAdp.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			consumerTpSp.Adapter = consumerAdp;
			consumerTpSp.ItemSelected +=  (object sender, AdapterView.ItemSelectedEventArgs e) => {
				SDXReferenceDataManager mgr = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
				ConsumerTypeModel type = mgr.DataModel.ConsumerTypes.ElementAt (e.Position);
				modelIds [2] = type.ConsumerTypeId;
			};

			var localComps = new List <string> ();
			foreach (LocalCompetitionTypeModel model in refMgr.DataModel.LocalCompetitionTypes) {
				localComps.Add (model.Description);
			}
			ArrayAdapter <string> localCompAdp = new ArrayAdapter<string> (Activity, global::Android.Resource.Layout.SimpleSpinnerItem, localComps.ToArray ());
			localCompAdp.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			localCompSp.Adapter = localCompAdp;
			localCompSp.ItemSelected +=  (object sender, AdapterView.ItemSelectedEventArgs e) => {
				SDXReferenceDataManager mgr = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
				LocalCompetitionTypeModel type = mgr.DataModel.LocalCompetitionTypes.ElementAt (e.Position);
				modelIds [3] = type.LocalCompetitionTypeId;
			};
		}

		private void FillOutlets ()
		{
			for (int i = 0; i < outlets.Count; i++) {
				OutletModel outlet = outlets [i];
				Button btn = new Button (Activity);
				btn.Tag = (i + 10).ToString ();
				btn.SetTextColor (Color.White);
				btn.SetBackgroundColor (Color.Rgb (122, 236, 12));
				btn.SetText ("Search : " + outlet.Name, TextView.BufferType.Normal);
				btn.SetTextSize (ComplexUnitType.Px, (int)(30 * Data.HRate));
				btn.SetTypeface (Fonts.Karla_Regular, TypefaceStyle.Normal);
                btn.SetPadding(0, 0, 0, 0);
				btn.Click += OutletBtn_OnClicked;
				outletsView.AddView (btn);

				LinearLayout.LayoutParams param = (LinearLayout.LayoutParams)btn.LayoutParameters;
				param.Width = (int)(Data.WRate * 600);
				param.Height = (int)(Data.HRate * 60);
				param.TopMargin = (int)(20 * Data.YRate);
				btn.LayoutParameters = param;
			}
		}
		#endregion
	}
}

