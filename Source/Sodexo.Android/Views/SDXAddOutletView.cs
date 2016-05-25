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
using Android.Util;
using Android.Views.InputMethods;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using TinyIoC;

namespace Sodexo.Android
{
	public class SDXAddOutletView
	{
		Activity context;
		public View view;
		public EventHandler DeleteBtnEvent;
		public bool IsLayoutAdjusted = false;

		private int consumerID = -1;
		private int annualSalesID = -1;
		private int localCompetitonID = -1;

		private List <string> consumersOnSiteList, annualSalesList, localCompetitonList;
		private Spinner consumerSp, annualSp, localSp;
		private EditText nameEt;
		private TextView numberTv;
		private FrameLayout deleteView;

		public SDXAddOutletView (Activity context, View view)
		{
			this.context = context;
			this.view = view;

			GetInstance ();
		}

		public static SDXAddOutletView Create (Activity context, ViewGroup parent, bool isLayoutAdjusted)
		{
			View view = context.LayoutInflater.Inflate (Resource.Layout.AddOutlet_View, parent, false);

			(new Handler ()).Post (new Java.Lang.Runnable (() => {
				if (!isLayoutAdjusted) {
					context.RunOnUiThread (() => {
						LayoutAdjuster.FitToScreen (context, (ViewGroup)view, Data.XRate, Data.XRate, Data.XRate, Data.XRate, Data.Density);
					});
				}
			}));

			SDXAddOutletView self = new SDXAddOutletView (context, view);

			return self;
		}

		public void PutList (List <string> consumer, List <string> annual, List<string> local)
		{
			consumersOnSiteList = consumer;
			annualSalesList = annual;
			localCompetitonList = local;

			ArrayAdapter <string> consumerAdp = new ArrayAdapter<string> (context, global::Android.Resource.Layout.SimpleSpinnerItem, consumersOnSiteList.ToArray ());
			consumerAdp.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			consumerSp.Adapter = consumerAdp;
			consumerSp.ItemSelected +=  (object sender, AdapterView.ItemSelectedEventArgs e) => {
				SDXReferenceDataManager mgr = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
				ConsumersOnSiteRangeModel type = mgr.DataModel.ConsumersOnSiteRanges.ElementAt (e.Position);
				consumerID = type.ConsumersOnSiteRangeId;

				HideKeyboard(nameEt);

			};
			consumerSp.Prompt = "Consumers on Site";

			ArrayAdapter <string> annualAdp = new ArrayAdapter<string> (context, global::Android.Resource.Layout.SimpleSpinnerItem, annualSalesList.ToArray ());
			annualAdp.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			annualSp.Adapter = annualAdp;
			annualSp.ItemSelected +=  (object sender, AdapterView.ItemSelectedEventArgs e) => {
				SDXReferenceDataManager mgr = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
				AnnualSalesRangeModel type = mgr.DataModel.AnnualSalesRanges.ElementAt (e.Position);
				annualSalesID = type.AnnualSalesRangeId;

				HideKeyboard(nameEt);
			};
			annualSp.Prompt = "Annual Sales";

			ArrayAdapter <string> localAdp = new ArrayAdapter<string> (context, global::Android.Resource.Layout.SimpleSpinnerItem, localCompetitonList.ToArray ());
			localAdp.SetDropDownViewResource (global::Android.Resource.Layout.SimpleSpinnerDropDownItem);
			localSp.Adapter = localAdp;
			localSp.ItemSelected +=  (object sender, AdapterView.ItemSelectedEventArgs e) => {
				SDXReferenceDataManager mgr = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
				LocalCompetitionTypeModel type = mgr.DataModel.LocalCompetitionTypes.ElementAt (e.Position);
				localCompetitonID = type.LocalCompetitionTypeId;

				HideKeyboard(nameEt);
			};
			localSp.Prompt = "Local Competition";
		}

		protected void HideKeyboard (EditText et)
		{
			context.RunOnUiThread (delegate {
				var inputManager = (InputMethodManager) context.GetSystemService (Activity.InputMethodService);
					inputManager.HideSoftInputFromWindow (et.WindowToken, HideSoftInputFlags.None);
			});

		}

		public void Update (int index)
		{
			numberTv.Text = (index + 1).ToString ("00");
		}

		public void FillContents (OutletModel outlet)
		{
			nameEt.Text = outlet.Name;

			int consumerIdx = consumersOnSiteList.IndexOf (outlet.ConsumersOnSiteRange.Description);
			consumerSp.SetSelection (consumerIdx);
			int annualIdx = annualSalesList.IndexOf (outlet.AnnualSalesRange.Description);
			annualSp.SetSelection (annualIdx);
			int localIdx = localCompetitonList.IndexOf (outlet.LocalCompetitionType.Description);
			localSp.SetSelection (localIdx);

			deleteView.Visibility = ViewStates.Visible;

			consumerID = outlet.ConsumersOnSiteRangeId;
			annualSalesID = outlet.AnnualSalesRangeId;
			localCompetitonID = outlet.LocalCompetitionTypeId;
		}

		public bool IsFilled()
		{
			if (nameEt.Text == "" || consumerID == -1 || annualSalesID == -1 || localCompetitonID == -1)
				return false;

			return true;
		}

		public OutletModel GetOutlet ()
		{
			OutletModel outlet = new OutletModel ();

			outlet.Name = nameEt.Text;
			outlet.ConsumersOnSiteRangeId = consumerID;
			outlet.AnnualSalesRangeId = annualSalesID;
			outlet.LocalCompetitionTypeId = localCompetitonID;

			return outlet;
		}

		private void GetInstance ()
		{
			numberTv = view.FindViewById (Resource.Id.addoutletview_number_tv) as TextView;
			nameEt = view.FindViewById (Resource.Id.addoutletview_name_et) as EditText;
			consumerSp = view.FindViewById (Resource.Id.addoutletview_consumer_sp) as Spinner;
			annualSp = view.FindViewById (Resource.Id.addoutletview_annualsales_sp) as Spinner;
			localSp = view.FindViewById (Resource.Id.addoutletview_localcompetition_sp) as Spinner;
			deleteView = view.FindViewById (Resource.Id.addoutletview_delete_account_fl) as FrameLayout;

			var consumerInfoBtn = view.FindViewById (Resource.Id.addoutletview_consumer_info_imgbtn) as ImageButton;
			consumerInfoBtn.Click += (object sender, EventArgs e) =>  {
				Util.ShowAlert ("This is Consumers on Site Field.", context);
			};

			var annualInfoBtn = view.FindViewById (Resource.Id.addoutletview_annualsales_info_imgbtn) as ImageButton;
			annualInfoBtn.Click += (object sender, EventArgs e) =>  {
				Util.ShowAlert ("This is AnnualSales Field.", context);
			};

			deleteView.Visibility = ViewStates.Invisible;
			var deleteBtn = view.FindViewById (Resource.Id.addoutletview_delete_outlet_btn) as Button;
			deleteBtn.Click += (object sender, EventArgs e) => {
				DeleteBtnEvent (sender, e);
			};
		}
	}
}

