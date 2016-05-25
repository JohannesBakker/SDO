
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
	public partial class SDXPricesVC : SDXBaseVC
	{
		private bool isBasic = false;
		private int selectedTag = 0;
		List <string> dropDescList = new List<string> ();
		List <int> dropIdsList = new List<int> ();
		int[] modelIds = new int[4];
		private List<OutletModel> outlets = new List<OutletModel> ();

		List<ProductPriceModel> prices;

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

			ScrollView.Hidden = true;

			// Set DropTableView Row Select Event
			DropTableView.SelectedEvent += OnDropTableCell_Selected;

			for (int i = 0; i < 4; i++)
				modelIds [i] = -1;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetHeaderBackground (4);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (AdvancedView.Alpha == 0.0f && BasicView.Alpha == 0.0f)
				FillContents ();
		}
		#endregion

		#region Button Actions
		private void OnChangeMenuBtn_Pressed (object sender, EventArgs ea)
		{
			DropView.Hidden = true;

			UIView currentView, nextView;
			if (isBasic) {
				currentView = BasicView;
				nextView = AdvancedView;
				((UIBarButtonItem)sender).Image = UIImage.FromBundle ("icon_basic.png");

				ScrollView.ContentSize = new SizeF (ScrollView.Frame.Width, AdvancedView.Frame.Height);
				DropView.Frame = AdvancedView.Frame;
			} else {
				currentView = AdvancedView;
				nextView = BasicView;
				((UIBarButtonItem)sender).Image = UIImage.FromBundle ("icon_m.png");

				ScrollView.ContentSize = new SizeF (ScrollView.Frame.Width, BasicView.Frame.Height);
			}
			UIView.Animate (0.3f, () => {
				currentView.Alpha = 0.0f;
				nextView.Alpha = 1.0f;
			});
			isBasic = !isBasic;
		}

		partial void OnDropBtn_Pressed (UIButton sender)
		{
			Console.WriteLine("presssed");

			if (DropView.Hidden == false)
				OnHideDropBtn_Pressed (null);

			SDXReferenceDataManager manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel == null)
				return;

			var btn = sender as UIButton;
			dropDescList.Clear ();
			dropIdsList.Clear ();
			if (btn.Tag == 11) {
				foreach (StateModel state in manager.DataModel.States) {
					if (!state.ShowInPricing) {continue;}
					dropDescList.Add (state.Name);
					dropIdsList.Add (state.PricingBandId);
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
			else if (btn.Tag == 15)
				foreach (OfferCategoryModel category in manager.DataModel.OfferCategories) {
					if (!category.ShowInPricing) {continue;}
					dropDescList.Add (category.Description);
					dropIdsList.Add (category.OfferCategoryId);
				}

			if (dropDescList.Count == 0)
				return;

			selectedTag = btn.Tag;

			DropTableView.SetDropDescriptionList (dropDescList);
			DropTableView.ReloadData ();
			DropTableView.ScrollToRow (NSIndexPath.FromRowSection(0,0), UITableViewScrollPosition.Top, false);

			RectangleF frame = DropTableView.Frame;
			frame.Y = btn.Frame.Y + btn.Frame.Height + BasicView.Frame.Y;
			frame.Height = 1.0f;

			float fHeight = dropDescList.Count >= 4 ? 100.0f : dropDescList.Count * 30.0f;
			DropTableView.Frame = frame;
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

		async partial void OnSearchBtn_Pressed (SDXButton sender)
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXPriceManager> ();
			for (int i = 0; i < modelIds.Count(); i++) {
				if (modelIds [i] < 0) {
					Util.ShowAlert ("Please select all required items.");
					return;
				}
			}
			ShowLoading ("Loading...");
			prices = await manager.LoadPricesAdvance (modelIds[1], modelIds[0], modelIds[3], modelIds[2], 0, "");
			HideLoading ();

			if (!manager.IsSuccessed) {
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			PerformSegue ("SegueToPricesResult", this);
		}

		partial void OnReturnKey_Pressed (SDXTextField sender)
		{
			((UITextField)sender).ResignFirstResponder ();
		}

		private async void OnOutletBtn_Pressed (object sender, EventArgs args)
		{
			UIButton btn = (UIButton)sender;
			var outletId = outlets [btn.Tag - 10].OutletId;

			var manager = TinyIoCContainer.Current.Resolve <SDXPriceManager> ();
			ShowLoading ("Loading...");
			prices = await manager.LoadPricesBasic (outletId, "");
			HideLoading ();

			if (!manager.IsSuccessed) {
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			PerformSegue ("SegueToPricesResult", this);
		}
		#endregion

		#region Private Functions
		private async Task LoadMe ()
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();

			if (manager.Me == null || manager.Me.Accounts == null) {
				ShowLoading ("Loading...");
				await manager.LoadMe ();

				if (!manager.IsSuccessed) {
					ShowErrorMessage (manager.ErrorMessage);
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
			HideLoading ();

			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel == null)
				return;

			for (int i = 0; i < outlets.Count; i++) {
				OutletModel outlet = outlets [i];
				SDXButton btn = new SDXButton ();
				float y = 130 + (30 + 5) * i;
				btn.Frame = new RectangleF (20, y, 280, 30);
				btn.Tag = i + 10;
				btn.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 15);
				btn.SetTitleColor (UIColor.White, UIControlState.Normal);
				btn.SetTitle ("Search : "+outlet.Name, UIControlState.Normal);
				btn.BackgroundColor = UIColor.FromRGB (122 / 255.0f, 236 / 255.0f, 12 / 255.0f);
				btn.TouchUpInside += OnOutletBtn_Pressed;
				BasicView.AddSubview (btn);
			}
			Util.ChangeViewHeight (BasicView, 130 + outlets.Count * (30 + 5) + 10);

			ScrollView.Hidden = false;
			if (outlets.Count == 0) {
				isBasic = false;
				AdvancedView.Alpha = 1.0f;

				ScrollView.ContentSize = new SizeF (ScrollView.Frame.Size.Width, AdvancedView.Frame.Height);
				DropView.Frame = AdvancedView.Frame;
			} else {
				isBasic = true;
				BasicView.Alpha = 1.0f;
				BasicView.Hidden = false;

				ScrollView.ContentSize = new SizeF (ScrollView.Frame.Size.Width, BasicView.Frame.Height);

				UIBarButtonItem changeMenuItem = new UIBarButtonItem (UIImage.FromBundle("icon_m.png"), 
					UIBarButtonItemStyle.Plain, (s, e)=>{ OnChangeMenuBtn_Pressed(s, e); });
				NavigationItem.RightBarButtonItem = changeMenuItem;
			}
		}
		#endregion

		#region Event Handler
		void OnDropTableCell_Selected(object sender, int row)
		{
			UITextField tf = null;
			tf = AdvancedView.ViewWithTag (selectedTag - 10) as UITextField;

			if (tf == null)
				return;

			tf.Text = dropDescList [row];
			modelIds [selectedTag - 11] = dropIdsList [row];

			OnHideDropBtn_Pressed (null);
		}
		#endregion

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToPricesResult") {
				var vc = segue.DestinationViewController as SDXPricesResultVC;
				vc.Prices =  prices.OrderBy (item => item.ProductName).ToList();

				if (isBasic) {
					var tf = BasicView.ViewWithTag (1) as UITextField;
					vc.query = tf.Text;
				} else {
					vc.query = KeywordTF.Text;
				}
				
			}
		}
		#endregion
	}
}

