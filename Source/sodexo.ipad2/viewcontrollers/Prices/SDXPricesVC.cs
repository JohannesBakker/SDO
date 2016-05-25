
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;

using Sodexo.RetailActivation.Portable.Models;
using TinyIoC;
using Sodexo.Core;

namespace Sodexo.Ipad2
{
	partial class SDXPricesVC : SDXBaseVC
	{
		private bool isBasic = false;
		private int selectedTag = 0;
		private bool isLoaded;

		List <string> dropDescList = new List<string> ();
		List <int> dropIdsList = new List<int> ();
		int[] modelIds = new int[4];
		private List<OutletModel> outlets = new List<OutletModel> ();

		private int selectedCategoryIdx = -1;
		private int selectedOutletId = -1;

		List <int> offerCategoryIds = new List<int> ();
		List <int> selectedOfferCategoryIds = new List<int> ();

		List<ProductPriceModel> prices = new List<ProductPriceModel> ();
		List <ProductPriceModel> filteredPrices = new List<ProductPriceModel> ();

		public SDXPricesVC (IntPtr handle) : base (handle)
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
			View.Frame = new RectangleF (0, 0, 1024, 594);

			// Set DropTableView Row Select Event
			DropTableView.SelectedEvent += OnDropTableCell_Selected;
		
			for (int i = 0; i < 4; i++)
				modelIds [i] = -1;

			UITableView.Appearance.BackgroundColor = UIColor.White;
			UITableViewCell.Appearance.BackgroundColor = UIColor.White;

			PriceTableView.Delegate = new SDXPricesTableDelegate ();
			PriceTableView.DataSource = new SDXPricesTableDataSource ();
			((SDXPricesTableDataSource)PriceTableView.DataSource).Prices = prices;

			Util.MoveViewToY (FilterView, -FilterView.Frame.Size.Height);
			Util.MoveViewToX (PriceSelectionView, -PriceSelectionView.Frame.Size.Width);
			Util.MoveViewToY (FilterBtn, -FilterBtn.Frame.Size.Height);

			//visual setup
			AdvancedSearchBT.BackgroundColor = UIColor.FromRGB (122 / 255.0f, 236 / 255.0f, 12 / 255.0f);
			AdvancedSearchBT.SetTitleShadowColor (UIColor.LightGray, UIControlState.Normal);
			AdvancedSearchBT.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 12);

			messageLb.TextColor = UIColor.FromRGB (80, 80, 80);
			messageLb.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 12);

			DoPriceViewSetup (PriceSelectionView);
			DoPriceViewSetup (AdvancedView);

			//table indent fix. issue is only on ios 8 thats why the respondtoselector check
			if (PriceTableView.RespondsToSelector (new Selector ("setSeparatorInset:")))
				PriceTableView.SeparatorInset = UIEdgeInsets.Zero;

			if (PriceTableView.RespondsToSelector (new Selector ("setLayoutMargins:")))
				PriceTableView.LayoutMargins = UIEdgeInsets.Zero;

			ShowLoading ("Loading...");
			FillContents ();

			Console.WriteLine ("View Did Load");
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetHeaderBackground (4);
			SetSectionName ("Prices");

			Console.WriteLine ("View Will Appear");
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			Console.WriteLine ("View Did Appear");

		}
		#endregion

		#region Button Actions

		partial void FilterMenuBtn_Pressed (UIButton sender)
		{
			UIView.Animate (0.3f, () => {
				if (FilterView.Frame.Y < 0)
				{
					Util.MoveViewToY (FilterView, 0);
					Util.MoveViewToY (FilterBtn, FilterView.Frame.Height-5);
				}	
				else
				{
					Util.MoveViewToY (FilterView, - FilterView.Frame.Height);
					Util.MoveViewToY (FilterBtn, 0);
				}
			});
		}


		partial void OnAdvancedBtn_Pressed (UIButton sender)
		{
			UIView currentView, nextView;
			if (isBasic) 
			{
				currentView = BasicViewContainer;
				nextView = AdvancedView;

				advancedButton.Selected = true;
				basicButton.Selected = false;

				UIView.Animate (0.3f, () => {
					currentView.Alpha = 0.0f;
					nextView.Alpha = 1.0f;
				});
			} 
			messageLb.Text = "Fill in the form to search for prices ";	
			isBasic = false;
		}

		partial void OnBasicBtn_Pressed (UIButton sender)
		{
			UIView currentView, nextView;
			if (!isBasic) 
			{
				currentView = AdvancedView;
				nextView = BasicViewContainer;

				advancedButton.Selected = false;
				basicButton.Selected = true;

				UIView.Animate (0.3f, () => {
					currentView.Alpha = 0.0f;
					nextView.Alpha = 1.0f;
				});
			} 

			messageLb.Text = "Select Outlet to see prices";
			isBasic = true;
		}



		async partial void OnSearchBtn_Pressed (UIButton sender)
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXPriceManager> ();

			for (int i = 0; i < modelIds.Count(); i++) {
				if (modelIds [i] < 0) {
					Util.ShowAlert ("Please select all required items.");
					return;
				}
			}
			pricesView.Hidden = true;
			ShowLoading ("Loading...");

			prices = await manager.LoadPricesAdvance (modelIds[1], modelIds[0], modelIds[3], modelIds[2], 0, "");



			if (BasicSearchField.Text.Length > 0) {
				SearchTF.Text = BasicSearchField.Text;
				filteredPrices = prices.Where(item =>item.ProductName.ToLower().Contains(SearchTF.Text.ToLower()) ).OrderBy(item=>item.ProductName).ToList();
				SearchPerformed(filteredPrices);
			} else {
				SearchPerformed(prices);
			}

			HideLoading ();

			if (!manager.IsSuccessed) {
				Util.ShowAlert (manager.ErrorMessage);
				return;
			}


		}



		partial void OnDropBtn_Pressed (UIButton sender)
		{
			if (DropView.Hidden == false)
				OnHideDropBtn_Pressed (null);

			SDXReferenceDataManager manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel == null)
				return;

			var btn = sender as UIButton;
			dropDescList.Clear ();
			dropIdsList.Clear ();
			if (btn.Tag == 11) {
				if (!isBasic)
					foreach (StateModel state in manager.DataModel.States) {
						if (!state.ShowInPricing) {continue;}
						dropDescList.Add (state.Name);
						dropIdsList.Add (state.PricingBandId);
					}
				else {
					foreach (OutletModel outlet in outlets) {
						dropDescList.Add (outlet.Name);
						dropIdsList.Add (outlet.OutletId);
					}
				}
			}
			else if (btn.Tag == 12)
				foreach (DivisionModel division in manager.DataModel.Divisions) {
					if (!division.ShowInPricing) continue;
					dropDescList.Add (division.Name);
					dropIdsList.Add (division.DivisionId);
				}
			else if (btn.Tag == 13)
				foreach (ConsumerTypeModel consumer in manager.DataModel.ConsumerTypes) {
					dropDescList.Add (consumer.Description);
					dropIdsList.Add (consumer.ConsumerTypeId);
				}
			else if (btn.Tag == 14)
				foreach (LocalCompetitionTypeModel competition in manager.DataModel.LocalCompetitionTypes) {
					dropDescList.Add (competition.Description);
					dropIdsList.Add (competition.LocalCompetitionTypeId);
				}


			if (dropDescList.Count == 0)
				return;

			selectedTag = btn.Tag;

			DropTableView.SetDropDescriptionList (dropDescList);
			DropTableView.ReloadData ();
			DropTableView.ScrollToRow (NSIndexPath.FromRowSection(0,0), UITableViewScrollPosition.Top, false);

			RectangleF frame = DropTableView.Frame;
			frame.Y = btn.Frame.Y + btn.Frame.Height;// + BasicView.Frame.Y;
			frame.Height = 1.0f;

			float fHeight = dropDescList.Count >= 4 ? 100.0f : dropDescList.Count * 30.0f;
			DropTableView.Frame = frame;
			View.BringSubviewToFront (DropView);
			DropView.Hidden = false;

			UIView.Animate (0.3f, () => {
				frame.Height = fHeight;
				DropTableView.Frame = frame;
			});
		}

		partial void OnHideDropBtn_Pressed (UIButton sender)
		{
			RectangleF frame = DropTableView.Frame;
			UIView.Animate (0.3f, () => {
				DropTableView.Frame = new RectangleF (frame.X, frame.Y, frame.Width, 1.0f);
			}, () => {
				DropView.Hidden = true;
			});
		}

		private async void OnOutletBtn_Pressed (object sender, EventArgs args)
		{
			UIButton Button = (UIButton)sender;
			var outletId = outlets [Button.Tag - 10].OutletId;

			var manager = TinyIoCContainer.Current.Resolve <SDXPriceManager> ();
			ShowLoading ("Loading...");
			prices = await manager.LoadPricesBasic (outletId, "");

			SearchPerformed(prices);

			HideLoading ();

			if (!manager.IsSuccessed) {
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			for (int i = 0; i < outlets.Count; i++) {
				UIButton btn = (UIButton)View.ViewWithTag (i + 10);
				btn.SetImage (null, UIControlState.Normal);

			}

			Button.SetImage (UIImage.FromBundle ("selected_checkmark.png"), UIControlState.Normal);
			Button.ImageEdgeInsets = new UIEdgeInsets (3, 230, 0, 0);

			if (BasicSearchField.Text.Length > 0) {
				SearchTF.Text = BasicSearchField.Text;
				filteredPrices = prices.Where(item =>item.ProductName.ToLower().Contains(SearchTF.Text.ToLower()) ).OrderBy(item=>item.ProductName).ToList();
				SearchPerformed(filteredPrices);
			}
		}
		 
		#endregion

		#region Event Handler
		//event for dropdown in advanced form
		void OnDropTableCell_Selected(object sender, int row)
		{
			UITextField tf = null;
			if (isBasic) {
				tf = BasicView.ViewWithTag (1) as UITextField;
				selectedOutletId = dropIdsList [row];
			} else {
				tf = AdvancedView.ViewWithTag (selectedTag - 10) as UITextField;
			}
			if (tf == null)
				return;

			tf.Text = dropDescList [row];
			modelIds [selectedTag - 11] = dropIdsList [row];

			OnHideDropBtn_Pressed (null);
		}

		//event for price category selection in filter view
		void OnOfferCategory_Selected (object sender, int tag)
		{
			for (int i = 0;; i++) {
				var view = BasicView.ViewWithTag (i + 100) as SDXOfferCategoryView;
				if (view == null)
					break;
				if (view.Tag != tag)
					view.Select (false);
				else {
					view.Select (true);
					selectedCategoryIdx = tag - 100;
				}
			}
		}

//		void OnOutlet_Selected(object sender,int row)
//		{
//
//		}
		#endregion

		#region Initial Setup
		private void DoPriceViewSetup(UIView pack)
		{
			foreach (UIView sub in pack) {
				if (sub is UILabel) {
					((UILabel)sub).TextColor = UIColor.FromRGB (56, 56, 56);
					((UILabel)sub).Font = UIFont.FromName (Constants.OSWALD_LIGHT, 12);
				} else if (sub is UITextField) {
					((UITextField)sub).Font = UIFont.FromName (Constants.HELVETICA_NEUE_LIGHT, 12);
					((UITextField)sub).TextColor = UIColor.FromRGB(56, 56, 56);
					((UITextField)sub).Layer.BorderColor = UIColor.FromRGB(199, 199, 199).CGColor;
					((UITextField)sub).Layer.CornerRadius = 5f;
					((UITextField)sub).Layer.BorderWidth = 1f;
				}
			}
		}

		async private void FillContents ()
		{
			pricesView.Hidden = true;
			messageView.Hidden = true;


			await LoadMe ();
			await LoadReferenceData ();
			await LoadFilter ();


			if (outlets.Count == 0) {
				isBasic = false;
				AdvancedView.Alpha = 1.0f;
				BasicViewContainer.Alpha = 0f;

				advancedButton.Selected = true;
				basicButton.Selected = false;
				basicButton.Enabled = false;

				messageLb.Text = "Select price search criteria ";



			} else {
				isBasic = true;
				BasicViewContainer.Alpha = 1.0f;
				AdvancedView.Alpha = 0f;

				advancedButton.Selected = false;
				basicButton.Selected = true;
				messageLb.Text = "Select Outlet to see prices";

				//draw outlet buttons
				for (int i = 0; i < outlets.Count; i++) {
					OutletModel outlet = outlets [i];
					SDXButton btn = new SDXButton ();
					float y = 5+(40 + 10) * i;

					btn.Tag = i + 10;
					btn.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 12);
					btn.SetTitleColor (UIColor.White, UIControlState.Normal);
					string title = "Search : " + outlet.Name;
					title = title.ToUpper ();
					btn.SetTitle (title, UIControlState.Normal);
					btn.BackgroundColor = UIColor.FromRGB (122 / 255.0f, 236 / 255.0f, 12 / 255.0f);
					btn.SetTitleShadowColor (UIColor.LightGray, UIControlState.Normal);
					btn.TitleShadowOffset = new SizeF (1, 1);
					btn.TouchUpInside += OnOutletBtn_Pressed;
					BasicView.AddSubview (btn);
					btn.Frame = new RectangleF (20, y, 260, 40);
				}

				BasicView.Frame = new RectangleF(BasicView.Frame.X,BasicView.Frame.Y,300, 10 + (40 + 10) * outlets.Count+40);
				BasicScrollView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
				BasicScrollView.ContentSize = new SizeF (BasicView.Frame.Width, BasicView.Frame.Height);
				BasicScrollView.CanCancelContentTouches = true;
				BasicScrollView.DelaysContentTouches = true;
			}

			messageView.Hidden = false;

			HideLoading ();

			UIView.Animate (0.8f, () => {
				Util.MoveViewToX (PriceSelectionView, 0);
				Util.MoveViewToY(FilterBtn,0);
			});
		}

		private async Task LoadMe ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();

			if (manager.Me == null || manager.Me.Accounts == null) {
				await manager.LoadMe ();

				if (!manager.IsSuccessed) {
					new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
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

		private async Task LoadFilter()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel == null)
				return;
				
			int idx = 0;
			for (int i = 0; i < manager.DataModel.OfferCategories.Count; i++) {
				var category = manager.DataModel.OfferCategories [i] as OfferCategoryModel;
				if (!category.ShowInPricing)
					continue;
				var categoryView = SDXOfferCategoryView.Create ();
				categoryView.Update (category);
				categoryView.SelectedEvent += OnOfferCategory_Selected;
				RectangleF frame = new RectangleF ();
				frame.X = 30 + 60 * (idx % 4);
				frame.Y = 40;
				frame.Width = 59;
				frame.Height = 59;
				categoryView.Frame = frame;
				categoryView.Tag = idx + 100;
				FilterView.AddSubview (categoryView);
				offerCategoryIds.Add (category.OfferCategoryId);
				idx++;
			}

		}
		#endregion

		#region Search Engine
		//called from outlet button event, or search button on advanced form
		private void SearchPerformed(List<ProductPriceModel> currentList)
		{
			if(currentList.Count>0)
			{
				((SDXPricesTableDataSource)PriceTableView.DataSource).Prices = currentList;
				PriceTableView.ReloadData();
				messageView.Hidden = true;
				pricesView.Hidden = false;
			}
			else
			{
				pricesView.Hidden = true;
				messageLb.Text = "No prices found, please change search criteria";
				messageView.Hidden = false;
			}
		}
		//do the search 
		partial void OnApplyBtn_Pressed (SDXButton sender)
		{
			selectedOfferCategoryIds.Clear ();
			for (int i = 0; i < offerCategoryIds.Count; i++) {
				var view = FilterView.ViewWithTag (i + 100) as SDXOfferCategoryView;
				if (view.IsSelected) {
					selectedOfferCategoryIds.Add (offerCategoryIds [i]);
				}
			}
			if(prices.Count==0){
				var alert = new UIAlertView ("Prices missing!", "Please select the outlet first, or fill the advanced form prior to filtering", null, "OK", null);
				alert.Show ();
			}else {
				filteredPrices = prices.Where(item => selectedOfferCategoryIds.IndexOf(item.OfferCategoryId)>=0 && item.ProductName.ToLower().Contains(SearchTF.Text.ToLower()) ).OrderBy(item=>item.ProductName).ToList();
				SearchPerformed(filteredPrices);
			}
			FilterMenuBtn_Pressed(null);
		}

		//clear and reset to default list of prices for conditions
		partial void OnCancelBtn_Pressed (SDXButton sender)
		{
			//restart to full list
			SearchTF.Text = "";
			for (int i = 0; i < offerCategoryIds.Count; i++) {
				var view = FilterView.ViewWithTag (i + 100) as SDXOfferCategoryView;
				view.IsSelected = true;
				view.Select(true);
			}

			SearchPerformed(prices);
			FilterMenuBtn_Pressed(null);
		}


		partial void OnReturnKey_Pressed (UITextField sender)
		{
			SearchTF.ResignFirstResponder ();
		}
		#endregion



	}
}
