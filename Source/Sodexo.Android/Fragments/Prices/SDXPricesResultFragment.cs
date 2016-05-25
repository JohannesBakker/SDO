
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
	public class SDXPricesResultFragment : SDXBaseFragment
	{
		View view;
		ListView listView;

		public List <ProductPriceModel> Prices;

		List <int> offerCategoryIds = new List<int> ();
		List <int> selectedOfferCategoryIds = new List<int> ();
		List <ProductPriceModel> filteredPrices = new List<ProductPriceModel> ();

		public string query;

		SDXPricesResultListAdapter adapter = null;

		LinearLayout filterView;
		TextView noResultTv;
		bool isFilterAppeared;
		FrameLayout categoriesView;
		EditText searchEt;

		List <SDXOfferCategoryView> viewList = new List<SDXOfferCategoryView> ();

		#region Fragment Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			HideAllHeaderButtons ();
			SetHeaderTitle ("Prices", 4);
			AddBackBtn (4);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.PricesResult, container, false);
			LayoutView (view);

			GetInstance ();

			FillContents ();

			((MainActivity)Activity).FilterBtn.Visibility = ViewStates.Visible;
			((MainActivity)Activity).FilterBtn.Click += FilterBtn_OnClicked;

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			if (adapter == null) {
				filterView.Visibility = ViewStates.Invisible;
				adapter = new SDXPricesResultListAdapter (Activity);
				adapter.Prices = Prices;

				listView = view.FindViewById (Resource.Id.pricesresult_listview) as ListView;
				listView.DividerHeight = 0;
				listView.CacheColorHint = Color.Transparent;
				listView.Adapter = adapter;
			}

			ShowNoResult (Prices.Count > 0);

			if (query.Length > 0) {
				searchEt.Text = query;
				ApplyBtn_OnClicked (null, null);
			}
		}
		#endregion

		#region Private Functions
		private void GetInstance ()
		{
			filterView = view.FindViewById (Resource.Id.pricesresult_filter_ll) as LinearLayout;
			noResultTv = view.FindViewById (Resource.Id.pricesresult_nothing_tv) as TextView;
			categoriesView = view.FindViewById (Resource.Id.pricesresult_filter_categories_fl) as FrameLayout;
			searchEt = view.FindViewById (Resource.Id.pricesresult_filter_search_et) as EditText;

			var applyBtn = view.FindViewById (Resource.Id.pricesresult_filter_apply_btn) as Button;
			var clearBtn = view.FindViewById (Resource.Id.pricesresult_filter_cancel_btn) as Button;
			applyBtn.Click += ApplyBtn_OnClicked;
			clearBtn.Click += ClearBtn_OnClicked;
		}

		private void ShowNoResult (bool bHide)
		{
			if (bHide)
				noResultTv.Visibility = ViewStates.Invisible;
			else
				noResultTv.Visibility = ViewStates.Visible;
		}

		private void FillContents ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();

			if (manager.DataModel.OfferCategories.Count == 0) {
				return;
			}
			int idx = 0;
			for (int i = 0; i < manager.DataModel.OfferCategories.Count; i++) {
				var category = manager.DataModel.OfferCategories [i] as OfferCategoryModel;
				if (!category.ShowInPricing)
					continue;

				SDXOfferCategoryView categoryV = SDXOfferCategoryView.Create (Activity, (ViewGroup)view);
				categoryV.view.Tag = idx.ToString ("00") + "-" + categoryV.view.Tag;
				categoryV.SelectedEvent = CategoryBtn_OnClicked;
				categoriesView.AddView (categoryV.view);
				var orgParams = (FrameLayout.LayoutParams) categoryV.view.LayoutParameters;
				orgParams.LeftMargin = 20 + (int)((idx % 5) * (120));
				orgParams.TopMargin = (int)((idx / 5) * (120));
				orgParams.Width = (int)((idx % 5) == 4 ? 120 : 118);
				orgParams.Height = (int)(118);
				categoryV.view.LayoutParameters = orgParams;
				categoryV.view.Tag += "-sl-st-glt";
				categoryV.Update (category);
				offerCategoryIds.Add (category.OfferCategoryId);
				categoryV.Select (true);
				viewList.Add (categoryV);
				idx++;
			}
			if (categoriesView.LayoutParameters.Width != 640) {
				(new Handler ()).Post (() => {
					Activity.RunOnUiThread (()=>{
						LayoutAdjuster.FitToScreen (Activity, (ViewGroup)categoriesView, Data.XRate, Data.XRate, Data.XRate, Data.XRate, Data.Density);
					});
				});
			}

			HideFilters ();
		}

		private void HideFilters()
		{
			filterView.Animate ().SetDuration (300).Y (-filterView.Height);
			isFilterAppeared = false;
		}
		#endregion

		#region Button Actions
		private void FilterBtn_OnClicked (object sender, EventArgs args)
		{
			filterView.Visibility = ViewStates.Visible;
			if (!isFilterAppeared)
				filterView.Animate ().SetDuration (300).Y (0);
			else
				filterView.Animate ().SetDuration (300).Y (-filterView.Height);
			isFilterAppeared = !isFilterAppeared;
		}

		private void CategoryBtn_OnClicked (object sender, int tag)
		{

		}

		void ApplyBtn_OnClicked (object sender, EventArgs arg)
		{
			selectedOfferCategoryIds.Clear ();
			for (int i = 0; i < offerCategoryIds.Count; i++) {
				if (viewList [i].IsSelected) {
					selectedOfferCategoryIds.Add (offerCategoryIds [i]);
				}
			}

			filteredPrices = Prices.Where(item => selectedOfferCategoryIds.IndexOf(item.OfferCategoryId)>=0 && item.ProductName.ToLower().Contains(searchEt.Text.ToLower()) ).OrderBy(item=>item.ProductName).ToList();

			adapter.Prices = filteredPrices;
			adapter.NotifyDataSetChanged ();

			ShowNoResult (filteredPrices.Count > 0);

			HideFilters();
		}

		void ClearBtn_OnClicked (object sender, EventArgs arg)
		{
			HideFilters();

			//restart to full list
			searchEt.Text = "";
			for (int i = 0; i < viewList.Count; i++) {
				viewList [i].Select (true);
			}

			adapter.Prices = Prices;
			adapter.NotifyDataSetChanged ();

			ShowNoResult (Prices.Count > 0);
		}
		#endregion
	}
}

