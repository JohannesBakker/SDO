
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.Core;
using TinyIoC;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.iOS
{
	public partial class SDXPromotionsVC : SDXBaseVC
	{
		public bool IsSelecting = false;
		public FeedbackTypeModel FeedbackType;

		private List<PromotionModel> Promotions;
		private List<PromotionModel> filteredPromotions = new List<PromotionModel> ();
		private string[] promotionCategoryNames = {"Cold Beverage", "Simply to Go", "Hot Beverages", "Mutualized", "Mindful", "In My Kitchen", "Pricing", "All"};

		public SDXPromotionsVC (IntPtr handle) : base (handle)
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

			if (!IsSelecting) {
				SetHeaderBackground (2);

//				UIBarButtonItem infoMenuItem = new UIBarButtonItem (UIImage.FromBundle ("icon_info.png").ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal), 
//					                               UIBarButtonItemStyle.Plain, (s, e) => {
//					OnInfoMenuBtn_Pressed (s, e);
//				});
				UIBarButtonItem filterMenuItem = new UIBarButtonItem (UIImage.FromBundle ("icon_filter.png"), 
					                                 UIBarButtonItemStyle.Plain, (s, e) => {
					OnFilterMenuBtn_Pressed (s, e);
				});
//				infoMenuItem.ImageInsets = new UIEdgeInsets (0, -15, 0, 0);
//				filterMenuItem.ImageInsets = new UIEdgeInsets (0, 0, 0, -15);
//				UIBarButtonItem[] barButtons = { infoMenuItem, filterMenuItem };
//				NavigationItem.RightBarButtonItems = barButtons;
				NavigationItem.RightBarButtonItem = filterMenuItem;

				FillFilterView ();
			} else {
				NavigationItem.Title = "Select Promotion";
			}

			TableView.Delegate = new SDXPromotionTableDelegate ();
			TableView.DataSource = new SDXPromotionTableDataSource ();
			((SDXPromotionTableDelegate)TableView.Delegate).RowSelectedEvent += OnRow_Selected;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (IsSelecting)
				AddBackButton (2);

			Util.MoveViewToY (FilterView, - FilterView.Frame.Size.Height);

			LoadPromotions ();
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);


		}
		#endregion

		#region Private Functions
		private void FillFilterView ()
		{
			for (int i = 0; i < promotionCategoryNames.Count (); i++) {
				UIImageView iv = new UIImageView ();
				iv.ContentMode = UIViewContentMode.ScaleAspectFit;
				iv.Image = UIImage.FromBundle ("promotion_" + promotionCategoryNames [i].Replace (" ", "_").ToLower () + ".png");
				iv.Frame = new RectangleF ((i % 4) * 80 + 20, (i / 4) * 80 + 10, 40, 30);
				iv.BackgroundColor = UIColor.Clear;
				FilterView.Add (iv);

				UILabel lb = new UILabel ();
				lb.Text = promotionCategoryNames [i].ToUpper ();
				lb.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 11);
				lb.BackgroundColor = UIColor.Clear;
				lb.TextColor = UIColor.DarkGray;
				lb.Frame = new RectangleF ((i % 4) * 80 + 5, (i / 4) * 80 + 45, 70, 30);
				lb.TextAlignment = UITextAlignment.Center;
				FilterView.Add (lb);

				UIButton btn = new UIButton ();
				btn.BackgroundColor = UIColor.Clear;
				btn.Frame = new RectangleF ((i % 4) * 80, (i / 4) * 80, 80, 80);
				btn.TouchUpInside += OnPromotionCategoryBtn_Pressed;
				btn.Tag = i + 1;
				FilterView.Add (btn);
			}

			Util.ChangeViewHeight (FilterView, (promotionCategoryNames.Count () / 4) * 80 + FilterSubView.Frame.Height);
			Util.MoveViewToY (FilterSubView, (promotionCategoryNames.Count () / 4) * 80);
		}

		private async void LoadPromotions ()
		{
			if (Promotions != null)
			{
				((SDXPromotionTableDelegate)TableView.Delegate).Promotions = filteredPromotions;
				((SDXPromotionTableDataSource)TableView.DataSource).Promotions = filteredPromotions;

				TableView.ReloadData ();

				return;
			}

			SDXPromotionManager manager = TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			ShowLoading ("Loading...");
			Promotions = await manager.LoadPromotions ();

			if (!manager.IsSuccessed) {
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
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
					InvokeOnMainThread (delegate {
						iCounter ++;
						foreach (PromotionModel item in Promotions) {
							if (item.Photo != null) {
								if(item.Photo.ProcessedPhotoBaseUrl + "?autorotate=true&w=600" == (string)sender) {
									if (bytes == null) {
										item.Photo = null;
										item.PhotoId = null;
									} else {
										UIImage img = Util.GetImageFromByteArray (bytes);
										item.PhotoId = (int)(1000 * img.Size.Height / img.Size.Width);
									}

									break;
								}
							}
						}
						if (iCounter == iTotalCount) 
						{
							HideLoading ();

							((SDXPromotionTableDelegate)TableView.Delegate).Promotions = Promotions;
							((SDXPromotionTableDataSource)TableView.DataSource).Promotions = Promotions;

							TableView.ReloadData ();

							filteredPromotions.AddRange (Promotions);
						}
					});
				};
				imageManager.LoadImageAsync (url, 600, 0);
			}
		}
		#endregion

		#region Button Actions
		private void OnInfoMenuBtn_Pressed (object sender, EventArgs ea)
		{
			System.Console.WriteLine ("Accounts: OnInfoMenuBtn_Pressed");
		}

		private void OnFilterMenuBtn_Pressed (object sender, EventArgs ea)
		{
			UIView.Animate (0.3f, () => {
				if (FilterView.Frame.Y < 0)
					Util.MoveViewToY (FilterView, 0);
				else
					Util.MoveViewToY (FilterView, - FilterView.Frame.Height);
			});
		}

		partial void OnApplyBtn_Pressed (SDXButton sender)
		{
			var allBtn = FilterView.ViewWithTag (promotionCategoryNames.Count ()) as UIButton;
			filteredPromotions.Clear ();

			if (allBtn.BackgroundColor != UIColor.Clear) {
				filteredPromotions.AddRange (Promotions);
			} else {
				List<string> filteredCategoriesNames = new List<string> ();
				for (int i = 0; i < promotionCategoryNames.Count () - 1; i++) {
					var btn = FilterView.ViewWithTag (i + 1) as UIButton;
					if (btn.BackgroundColor != UIColor.Clear)
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

			if (KeywordTF.Text != "")
			{
				for (int i = 0; i < filteredPromotions.Count; i++) {
					var item = filteredPromotions [i];
					if (! item.Title.Contains (KeywordTF.Text))
						filteredPromotions.RemoveAt (i--);
				}
			}

			((SDXPromotionTableDelegate)TableView.Delegate).Promotions = filteredPromotions;
			((SDXPromotionTableDataSource)TableView.DataSource).Promotions = filteredPromotions;

			TableView.ReloadData ();

			OnCancelBtn_Pressed (null);
		}

		partial void OnCancelBtn_Pressed (SDXButton sender)
		{
			if (KeywordTF.IsFirstResponder)
				KeywordTF.ResignFirstResponder ();

			UIView.Animate (0.3f, () => {
				Util.MoveViewToY (FilterView, - FilterView.Frame.Height);
			});
		}

		partial void OnReturnKey_Pressed (UITextField sender)
		{
			UITextField tf = sender as UITextField;
			tf.ResignFirstResponder ();
		}

		void OnPromotionCategoryBtn_Pressed (object sender, System.EventArgs args)
		{
			UIButton btn = sender as UIButton;
			if (btn.Tag == promotionCategoryNames.Count ()) {
				if (btn.BackgroundColor == UIColor.Clear) {
					for (int i = 1; i <= promotionCategoryNames.Count (); i++) {
						var b = FilterView.ViewWithTag (i) as UIButton;
						b.BackgroundColor = UIColor.FromWhiteAlpha (0, 0.3f);
					}
				}
			} else {
				if (btn.BackgroundColor == UIColor.Clear)
					btn.BackgroundColor = UIColor.FromWhiteAlpha (0, 0.3f);
				else {
					btn.BackgroundColor = UIColor.Clear;
					var allBtn = FilterView.ViewWithTag (promotionCategoryNames.Count ()) as UIButton;
					if (allBtn.BackgroundColor != UIColor.Clear)
						allBtn.BackgroundColor = UIColor.Clear;
				}
			}
		}
		#endregion

		#region Event Handler
		private void OnRow_Selected(object sender, int row)
		{
			if (!IsSelecting)
				PerformSegue ("SegueToPromotionDetail", this);
			else
				PerformSegue ("SegueToLeaveFeedback", this);
		}
		#endregion

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToPromotionDetail") {
				var detailVC = segue.DestinationViewController as SDXPromotionDetailVC;
				detailVC.Promotions = Promotions;
				detailVC.FilteredPromotions = filteredPromotions;
				detailVC.index = TableView.IndexPathForSelectedRow.Row;
			} else if (segue.Identifier == "SegueToLeaveFeedback") {
				var vc = segue.DestinationViewController as SDXLeaveFeedbackVC;
				vc.FeedbackType = FeedbackType;
				vc.Promotion = Promotions [TableView.IndexPathForSelectedRow.Row];
			}
		}
		#endregion
	}
}

