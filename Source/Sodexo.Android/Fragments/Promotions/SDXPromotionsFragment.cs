
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
	public class SDXPromotionsFragment : SDXBaseFragment
	{
		public bool IsSelecting = false;
		public FeedbackTypeModel FeedbackType;

		private List<PromotionModel> Promotions;
		private List<PromotionModel> filteredPromotions = new List<PromotionModel> ();
		private List<string> promotionCategoryNames = new List<string> ();
		List <SDXPromotionCategoryView> viewList = new List<SDXPromotionCategoryView> ();
		bool isFilterAppeared = false;

		ListView listView;
		SDXPromotionListAdapter adapter;
		LinearLayout filterView;
		FrameLayout categoriesView;
		EditText keywordEt;
		View view;
		
		#region Fragment Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			HideAllHeaderButtons ();
			if (IsSelecting) {
				SetHeaderTitle ("Select Promotion", 2);
				AddBackBtn (2);
			} else {
				SetHeaderTitle ("Promotions", 2);
				((MainActivity)Activity).FilterBtn.Visibility = ViewStates.Visible;
				((MainActivity)Activity).FilterBtn.Click += FilterBtn_OnClicked;
			}

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.Promotions, container, false);

			LayoutView (view);
			GetInstance ();

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			if (adapter == null) {
				filterView.Visibility = ViewStates.Invisible;
				adapter = new SDXPromotionListAdapter (Activity);
				listView.CacheColorHint = Color.Transparent;
				listView.DividerHeight = 0;
				listView.Adapter = adapter;
				listView.ItemClick += ListViewRow_OnSelected;
			}

			LoadPromotions ();
		}

		public override void OnDestroyView ()
		{
			base.OnDestroyView ();

			((MainActivity)Activity).FilterBtn.Click -= FilterBtn_OnClicked;
		}
		#endregion

		#region Private Functions
		private void GetInstance ()
		{
			listView = view.FindViewById (Resource.Id.promotions_listview) as ListView;
			filterView = view.FindViewById (Resource.Id.promotions_filter_ll) as LinearLayout;
			categoriesView = view.FindViewById (Resource.Id.promotions_filter_categories_fl) as FrameLayout;
			keywordEt = view.FindViewById (Resource.Id.promotions_filter_search_et) as EditText;

			var applyBtn = view.FindViewById (Resource.Id.promotions_filter_apply_btn) as Button;
			applyBtn.Click += ApplyBtn_OnClicked;

			var cancelBtn = view.FindViewById (Resource.Id.promotions_filter_cancel_btn) as Button;
			cancelBtn.Click += CancelBtn_OnClicked;
		}

		private void FillFilterView ()
		{
			if (promotionCategoryNames.Count == 0) {
				return;
			}
			for (int i = 0; i < promotionCategoryNames.Count; i++) {
				SDXPromotionCategoryView categoryV = SDXPromotionCategoryView.Create (Activity, (ViewGroup)view);
				categoryV.view.Tag = i.ToString ("00") + "-" + categoryV.view.Tag;
				categoryV.SelectedEvent = CategoryBtn_OnClicked;
				categoriesView.AddView (categoryV.view);
				var orgParams = (FrameLayout.LayoutParams) categoryV.view.LayoutParameters;
				orgParams.LeftMargin = (int)((i % 4) * (160));
				orgParams.TopMargin = (int)((i / 4) * (160));
				orgParams.Width = (int)((i % 4) == 3 ? 160 : 158);
				orgParams.Height = (int)(158);
				categoryV.view.LayoutParameters = orgParams;
				categoryV.view.Tag += "-sl-st-glt";
				categoryV.Update (promotionCategoryNames [i]);
				viewList.Add (categoryV);
			}
			if (categoriesView.LayoutParameters.Width != 640) {
				(new Handler ()).Post (() => {
					Activity.RunOnUiThread (()=>{
						LayoutAdjuster.FitToScreen (Activity, (ViewGroup)categoriesView, Data.XRate, Data.XRate, Data.XRate, Data.XRate, Data.Density);
					});
				});
			}

			filterView.Animate ().SetDuration (0).Y (-filterView.Height);
			isFilterAppeared = false;
		}

		private async void LoadPromotions ()
		{
			if (Promotions != null)
			{
				Util.StartFadeAnimation (listView, 300);
				adapter.Promotions = filteredPromotions;
				adapter.NotifyDataSetChanged ();

				return;
			}

			SDXPromotionManager manager = TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			Util.ShowLoading (Activity);
			Promotions = await manager.LoadPromotions ();

			if (!manager.IsSuccessed) {
				Util.HideLoading ();
				Util.ShowAlert (manager.ErrorMessage, Activity);
				return;
			}

			List<string> photoUrls = new List<string> ();
			foreach (PromotionModel item in Promotions) {
				if (item.Photo != null)
					photoUrls.Add (item.Photo.ProcessedPhotoBaseUrl);
			}
			int iTotalCount = photoUrls.Count;
			int iCounter = 0;
			foreach (string url in photoUrls) {
				var imageManager = new JHImageManager ();
				imageManager.LoadCompleted += (object sender, byte[] bytes) => {
					if (Activity == null)
						return;
					Activity.RunOnUiThread (async delegate {
						iCounter ++;
						foreach (PromotionModel item in Promotions) {
							if (item.Photo != null) {
								if(item.Photo.ProcessedPhotoBaseUrl + "?autorotate=true&w=600" == (string)sender) {
									if (bytes == null) {
										item.Photo = null;
										item.PhotoId = null;
									} else {
										using (Bitmap bitmap = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
											if (bitmap == null)
												item.PhotoId = 0;
											else
												item.PhotoId = (int)(1000 * bitmap.Height / bitmap.Width);
										}
									}
									break;
								}
							}
						}
						if (iCounter == iTotalCount) 
						{
							Util.StartFadeAnimation (listView, 300);
							adapter.Promotions = Promotions;
							adapter.NotifyDataSetChanged ();

							filteredPromotions.AddRange (Promotions);

							await LoadReferenceData ();
							Util.HideLoading ();

							SDXReferenceDataManager refMgr = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
							if (refMgr.DataModel != null) {
								foreach (PromotionCategoryModel model in refMgr.DataModel.PromotionCategories) {
									promotionCategoryNames.Add (model.Description);
								}
								promotionCategoryNames.Add ("All");
							}

							FillFilterView ();
						}
					});
				};
				imageManager.LoadImageAsync (url, 600, 0);
			}
		}

		private void MoveToPromotionDetailScreen (int row)
		{
			var fragment = new SDXPromotionDetailFragment ();
			fragment.Promotions = Promotions;
			fragment.FilteredPromotions = filteredPromotions;
			fragment.index = row;
			PushFragment (fragment, "SDXPromotionsFragment");
		}

		private void MoveToLeaveFeedbackScreen (int row)
		{
			var fragment = new LeaveFeedbackFragment ();
			fragment.FeedbackType = FeedbackType;
			fragment.Promotion = Promotions [row];
			PushFragment (fragment, "SDXPromotionsFragment");
		}
		#endregion

		#region ListView
		private void ListViewRow_OnSelected (object sender, AdapterView.ItemClickEventArgs e)
		{
			if (!IsSelecting)
				MoveToPromotionDetailScreen (e.Position);
			else
				MoveToLeaveFeedbackScreen (e.Position);
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

		private void ApplyBtn_OnClicked (object sender, EventArgs args)
		{
			filteredPromotions.Clear ();

			if (viewList[viewList.Count-1].IsSelected) {
				filteredPromotions.AddRange (Promotions);
			} else {
				List<string> filteredCategoriesNames = new List<string> ();
				for (int i = 0; i < promotionCategoryNames.Count () - 1; i++) {
					if (viewList[i].IsSelected)
						filteredCategoriesNames.Add (promotionCategoryNames [i]);
				}
				foreach (PromotionModel item in Promotions) {
					foreach (PromotionCategoryModel category in item.PromotionCategories) {
						bool isMatched = false;
						foreach (string name in filteredCategoriesNames) {
							if (name.ToLower () == category.Description.ToLower ()) {
								filteredPromotions.Add (item);
								isMatched = true;
								break;
							}
						}
						if (isMatched) break;
					}
				}
			}

			if (keywordEt.Text != "")
			{
				for (int i = 0; i < filteredPromotions.Count; i++) {
					var item = filteredPromotions [i];
					if (! item.Title.ToLower().Contains (keywordEt.Text.ToLower()))
						filteredPromotions.RemoveAt (i--);
				}
			}

			Util.StartFadeAnimation (listView, 300);
			adapter.Promotions = filteredPromotions;
			adapter.NotifyDataSetChanged ();

			CancelBtn_OnClicked (null, null);

            HideKeyboard(keywordEt);
		}

		private void CancelBtn_OnClicked (object sender, EventArgs args)
		{
			filterView.Animate ().SetDuration (300).Y (-filterView.Height);
			isFilterAppeared = false;
		}

		private void CategoryBtn_OnClicked (object sender, int tag)
		{
			if (tag == promotionCategoryNames.Count - 1) {
				if (viewList [tag].IsSelected) {
					for (int i = 0; i < promotionCategoryNames.Count - 1; i++) {
						viewList [i].Select (true);
					}
				}
			} else {
				if (!viewList [tag].IsSelected) {
					viewList [viewList.Count - 1].Select (false);
				}
			}
		}
		#endregion
	}
}

