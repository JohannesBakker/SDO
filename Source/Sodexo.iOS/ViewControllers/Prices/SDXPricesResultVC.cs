
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using TinyIoC;
using Sodexo.Core;

namespace Sodexo.iOS
{
	public partial class SDXPricesResultVC : SDXBaseVC
	{
		public List<ProductPriceModel> Prices;

		List <int> offerCategoryIds = new List<int> ();
		List <int> selectedOfferCategoryIds = new List<int> ();
		List <ProductPriceModel> filteredPrices = new List<ProductPriceModel> ();

		public string query;

		public SDXPricesResultVC (IntPtr handle) : base (handle)
		{
		}

		#region View Lifecycle
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			TableView.Delegate = new SDXPricesTableDelegate ();
			TableView.DataSource = new SDXPricesTableDataSource ();


			UIBarButtonItem filterMenuItem = new UIBarButtonItem (UIImage.FromBundle ("icon_filter.png"), 
				UIBarButtonItemStyle.Plain, OnFilterMenuBtn_Pressed);
			NavigationItem.RightBarButtonItem = filterMenuItem;

			FillContents ();

			if (query.Length > 0) {
				SearchTF.Text = query;
				OnApplyBtn_Pressed (null);
			} else {
				((SDXPricesTableDataSource)TableView.DataSource).Prices = Prices;
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			AddBackButton (4);

			NoResultLb.Hidden = Prices.Count > 0 ? true : false;
		}
		#endregion

		#region Private Functions
		private void FillContents ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			int idx = 0;
			for (int i = 0; i < manager.DataModel.OfferCategories.Count; i++) {
				var category = manager.DataModel.OfferCategories [i] as OfferCategoryModel;
				if (!category.ShowInPricing)
					continue;
				var categoryView = SDXOfferCategoryView.Create ();
				categoryView.Update (category);
				categoryView.SelectedEvent += OnOfferCategory_Selected;
				RectangleF frame = new RectangleF ();
				frame.X = 10 + 60 * (idx % 4);
				frame.Y = 40 + 60 * (idx / 4);
				frame.Width = 59;
				frame.Height = 59;
				categoryView.Frame = frame;
				categoryView.Tag = idx + 100;
				FilterView.AddSubview (categoryView);
				offerCategoryIds.Add (category.OfferCategoryId);
				idx++;
			}
			float y = 40 + (offerCategoryIds.Count / 4 + (offerCategoryIds.Count == 0 ? 0 : 1)) * 60;
			Util.MoveViewToY (FilterSearchView, y + 10);
			Util.ChangeViewHeight (FilterView, FilterSearchView.Frame.Y + FilterSearchView.Frame.Height);
			Util.MoveViewToY (FilterView, -FilterView.Frame.Size.Height);
		}
		#endregion

		#region Button Actions
		private void OnFilterMenuBtn_Pressed (object sender, EventArgs ea)
		{
			UIView.Animate (0.3f, () => {
				if (FilterView.Frame.Y < 0)
					Util.MoveViewToY (FilterView, 0);
				else
					Util.MoveViewToY (FilterView, - FilterView.Frame.Height);
			});
		}

		void OnOfferCategory_Selected (object sender, int tag)
		{

		}

		partial void OnApplyBtn_Pressed (SDXButton sender)
		{
			selectedOfferCategoryIds.Clear ();
			for (int i = 0; i < offerCategoryIds.Count; i++) {
				var view = FilterView.ViewWithTag (i + 100) as SDXOfferCategoryView;
				if (view.IsSelected) {
					selectedOfferCategoryIds.Add (offerCategoryIds [i]);
				}
			}
				
			filteredPrices = Prices.Where(item => selectedOfferCategoryIds.IndexOf(item.OfferCategoryId)>=0 && item.ProductName.ToLower().Contains(SearchTF.Text.ToLower()) ).OrderBy(item=>item.ProductName).ToList();

			((SDXPricesTableDataSource)TableView.DataSource).Prices = filteredPrices;
			TableView.ReloadData ();

			NoResultLb.Hidden = filteredPrices.Count > 0 ? true : false;

			MoveFilters();
		}

		partial void OnCancelBtn_Pressed (SDXButton sender)
		{
			MoveFilters();

			//restart to full list
			SearchTF.Text = "";
			for (int i = 0; i < offerCategoryIds.Count; i++) {
				var view = FilterView.ViewWithTag (i + 100) as SDXOfferCategoryView;
				view.IsSelected = true;
				view.Select(true);
			}

			((SDXPricesTableDataSource)TableView.DataSource).Prices = Prices;
			TableView.ReloadData ();

			NoResultLb.Hidden = Prices.Count > 0 ? true : false;

		}

		partial void OnReturnKey_Pressed (UITextField sender)
		{
			SearchTF.ResignFirstResponder ();
		}

		private void MoveFilters()
		{
			if (SearchTF.IsFirstResponder)
				SearchTF.ResignFirstResponder ();

			UIView.Animate (0.3f, () => {
				Util.MoveViewToY (FilterView, -FilterView.Frame.Height);
			});
		}
		#endregion
	}
}

