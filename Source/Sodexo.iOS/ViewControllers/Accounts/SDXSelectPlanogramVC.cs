
using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using TinyIoC;

namespace Sodexo.iOS
{
	public partial class SDXSelectPlanogramVC : SDXBaseVC
	{
		public AccountModel Account;
		public int OutletIndex;
		public int OfferIndex;

		private int offerCategoryId;

		private UIView currentView;
		private DecisionTreeNodeModel currentNode, selectedNode;
		private UITextField answerTF;

		private List<UIView> decisionViews = new List<UIView> ();
		private List<DecisionTreeNodeModel> parentNodes = new List<DecisionTreeNodeModel> ();
		private List<DecisionTreeNodeModel> selectedNodes = new List<DecisionTreeNodeModel> ();

		public SDXSelectPlanogramVC (IntPtr handle) : base (handle)
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

			DropTableView.SelectedEvent += OnDropTableCell_Selected;

			FillContents ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			AddBackButton (1);

			if (PlanogramView.Hidden == true) {
				LoadDecisionTree ();
			}
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			if (ActionsView.Hidden == true)
				return;

			ResetContentSize (false);
		}
		#endregion

		#region Private Functions
		private void FillContents ()
		{
			NameLB.Text = Account.Location.LocationName;
			AddressLB.Text = Account.Location.LocationCity + " " + Account.Location.LocationAddress1;
			LocationIdLB.Text = Account.Location.LocationId;

			OutletModel outlet = Account.Outlets [OutletIndex];
			OutletNameLB.Text = outlet.Name;
			if (outlet.Photo != null) {
				var imgManager = new JHImageManager ();
				imgManager.LoadCompleted += (object s, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						if (bytes == null || OutletPictureIV == null)
							return;
						var img = Util.GetImageFromByteArray (bytes);
						OutletPictureIV.Image = img;
					});
				};
				imgManager.LoadImageAsync (outlet.Photo.ProcessedPhotoBaseUrl, 148 * 2, 148 * 2);
			}

			OfferModel offer = outlet.Offers [OfferIndex];
			OfferNameLB.Text = offer.Name;
			var imgName = "img_offer_categories_" + offer.OfferCategory.OfferCategoryId.ToString () + ".png";
			OfferPictureIV.Image = UIImage.FromBundle (imgName);

			if (offer.Responses.Count == 0) {
				OfferStatusLB.Text = "NOT SELECTED";
			} else {
				var offerResponse = offer.Responses [0];
				var planogram = offerResponse.AnswerNode.Planogram;
				if (!offerResponse.PlanogramActivated) {
					OfferStatusLB.Text = planogram.Name +  "/NOT ACTIVE";
					OfferStatusLB.TextColor = UIColor.FromRGB (233 / 255.0f, 70 / 255.0f, 122 / 255.0f);
				} else {
					OfferStatusLB.Text = planogram.Name + "/ACTIVE";
					OfferStatusLB.TextColor = UIColor.FromRGB (125 / 255.0f, 244 / 255.0f, 146 / 255.0f);
				}
			}

			offerCategoryId = offer.OfferCategoryId;
			ActionsView.Hidden = true;
		}

		async private void LoadDecisionTree ()
		{
			DecisionTreeModel decisionTree;
			SDXReferenceDataManager manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DecisionTrees.ContainsKey (offerCategoryId)) {
				decisionTree = manager.DecisionTrees [offerCategoryId];
			} else {
				ShowLoading ("Loading...");
				decisionTree = await manager.LoadDecisionTree (offerCategoryId);
				HideLoading ();

				if (!manager.IsSuccessed) {
					ShowErrorMessage (manager.ErrorMessage);
					return;
				}
			}

			var treeNodes = (List<DecisionTreeNodeModel>) decisionTree.Versions [0].Nodes;

			if (treeNodes == null | treeNodes.Count == 0)
				return;

			currentNode = treeNodes [0];
			currentView = CreateDecisionView (currentNode);
			ScrollView.AddSubview (currentView);

			Util.MoveViewToY (ActionsView, currentView.Frame.Y + currentView.Frame.Height);
			ActionsView.Hidden = false;

			ResetContentSize (false);
		}

		private void ResetContentSize (bool isPlanogramViewAppeared)
		{
			float contentH = 0;
			if (isPlanogramViewAppeared)
				contentH = PlanogramView.Frame.Y + PlanogramView.Frame.Height + 10;
			else
				contentH = ActionsView.Frame.Y + ActionsView.Frame.Height + 10;
			contentH = contentH < ScrollView.Frame.Size.Height ? ScrollView.Frame.Size.Height + 0.5f : contentH;
			ScrollView.ContentSize = new SizeF (ScrollView.Frame.Width, contentH);
		}

		private UIView CreateDecisionView (DecisionTreeNodeModel node)
		{
			UIView view = new UIView ();
			view.BackgroundColor = UIColor.FromRGB (249 / 255.0f, 249 / 255.0f, 249 / 255.0f);

			UILabel questionLb = new UILabel ();
			questionLb.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 15);
			questionLb.Lines = 0;
			questionLb.TextColor = UIColor.DarkGray;
			questionLb.TextAlignment = UITextAlignment.Left;
			questionLb.BackgroundColor = UIColor.Clear;
			questionLb.Frame = new RectangleF (10, 10, 280, 10);
			view.AddSubview (questionLb);
			questionLb.Text = node.Text;
			questionLb.SizeToFit ();

			UIView answersView;
			if (node.ChildDisplayModeId == 1) {
				answersView = CreateDecisionRadioView (node.ChildNodes);
			} else {
				answersView = CreateDecisionDropListView (node.ChildNodes);
			}
			Util.MoveViewToY (answersView, questionLb.Frame.Y + questionLb.Frame.Height + 10); 
			view.AddSubview (answersView);

			view.Frame = new RectangleF (0, 207, 300, answersView.Frame.Y + answersView.Frame.Height + 10);

			return view;
		}

		private UIView CreateDecisionRadioView (IList<DecisionTreeNodeModel> answerNodes)
		{
			if (answerNodes.Count == 0)
				return null;

			UIView view = new UIView ();
			view.BackgroundColor = UIColor.Clear;

			UILabel firstSeparatorLine = new UILabel ();
			firstSeparatorLine.Frame = new RectangleF (5, 0, 290, 1);
			firstSeparatorLine.Text = "";
			firstSeparatorLine.BackgroundColor = UIColor.FromRGB (229 / 255.0f, 229 / 255.0f, 231 / 255.0f);
			view.AddSubview (firstSeparatorLine);

			float y = 0;
			for (int i = 0; i < answerNodes.Count; i++) {
				var node = answerNodes [i];

				var nodeV = SDXDecisionTreeRadioView.Create ();
				nodeV.Update (node, i);
				nodeV.RadioBtnPressedEvent = OnRadioBtn_Pressed;
				nodeV.Tag = 100 + i;

				view.AddSubview (nodeV);
				Util.MoveViewToY (nodeV, y);

				y += nodeV.Frame.Height;
			}

			view.Frame = new RectangleF (0, 0, 300, y);

			return view;
		}

		private UIView CreateDecisionDropListView (IList<DecisionTreeNodeModel> answerNodes)
		{
			UIView view = new UIView ();
			view.BackgroundColor = UIColor.Clear;
			view.Frame = new RectangleF (0, 0, 300, 30);

			UITextField tf = new UITextField ();
			tf.Frame = new RectangleF (5, 0, 290, 30);
			tf.Text = "";
			tf.TextColor = UIColor.DarkGray;
			tf.Font = UIFont.FromName (Constants.OSWALD_LIGHT, 14);
			tf.BackgroundColor = UIColor.Clear;
			view.AddSubview (tf);
			answerTF = tf;

			UIImageView iv = new UIImageView ();
			iv.Frame = new RectangleF (280, 13, 6, 4);
			iv.Image = UIImage.FromBundle ("icon_bottom_arrow.png");
			iv.ContentMode = UIViewContentMode.ScaleAspectFit;
			iv.BackgroundColor = UIColor.Clear;
			view.AddSubview (iv);

			UIButton btn = new UIButton (UIButtonType.Custom);
			btn.Frame = tf.Frame;
			btn.SetTitle ("", UIControlState.Normal);
			btn.TouchUpInside += OnDropBtn_Pressed;
			view.AddSubview (btn);

			List<string> dropAnswerList = new List<string> ();
			foreach (DecisionTreeNodeModel item in answerNodes) {
				dropAnswerList.Add (item.Text);
			}
			DropTableView.SetDropDescriptionList (dropAnswerList);
			DropTableView.ReloadData ();
			DropTableView.ScrollToRow(NSIndexPath.FromRowSection(0,0), UITableViewScrollPosition.Top, false);
			float fHeight = dropAnswerList.Count >= 4 ? 100.0f : dropAnswerList.Count * 30.0f;
			Util.ChangeViewHeight (DropTableView, fHeight);

			return view;
		}

		private void FillPlanogramView (PlanogramModel planogram, byte[] bytes)
		{
			PlanogramNameLB.Text = planogram.Name;
			PlanogramTitleLB.Text = planogram.Description;

			var pvm = planogram.Versions [0] as PlanogramVersionModel;
			if (pvm.IconPhoto != null) {
				var imgMgr = new JHImageManager ();
				imgMgr.LoadCompleted += (object s, byte[] bytesP) => {
					if (bytesP == null)
						return;
					PlanogramAvatarIV.Image = Util.GetImageFromByteArray (bytesP);
				};
				imgMgr.LoadImageAsync (pvm.IconPhoto.ProcessedPhotoBaseUrl, 80, 80);
			}

			UIImage img = Util.GetImageFromByteArray (bytes);
			float h = PlanogramPictureIV.Frame.Width * img.Size.Height / img.Size.Width;
			Util.ChangeViewHeight (PlanogramPictureIV, h);
			PlanogramPictureIV.Image = img;

			bool isTapGestureAdded = false;
			if (PlanogramPictureIV.GestureRecognizers != null) {
				foreach (UIGestureRecognizer gesture in PlanogramPictureIV.GestureRecognizers) {
					if (gesture.GetType () == typeof(UITapGestureRecognizer)) {
						isTapGestureAdded = true;
						break;
					}
				}
			}
			if (!isTapGestureAdded) {
				UITapGestureRecognizer tapGesture = new UITapGestureRecognizer ();
				tapGesture.AddTarget (() => {
					PerformSegue ("SegueToFullImage", this);
				});
				PlanogramPictureIV.AddGestureRecognizer (tapGesture);
			}

			Util.MoveViewToY (PlanogramDetailsLB, PlanogramPictureIV.Frame.Y + h + 10);
			PlanogramDetailsLB.Text = planogram.Details;
			PlanogramDetailsLB.SizeToFit ();
			Util.ChangeViewHeight (PlanogramDetailsLB, PlanogramDetailsLB.Frame.Height + 10);

			Util.MoveViewToY (SetAsPlanogramBtn, PlanogramDetailsLB.Frame.Y + PlanogramDetailsLB.Frame.Height + 10);
			Util.MoveViewToY (BackToDecisionTreeBtn, PlanogramDetailsLB.Frame.Y + PlanogramDetailsLB.Frame.Height + 10);

			Util.ChangeViewHeight (PlanogramView, SetAsPlanogramBtn.Frame.Y + SetAsPlanogramBtn.Frame.Height + 10);
		}
		#endregion

		#region Button Actions
		void OnRadioBtn_Pressed (object sender, int tag)
		{
			var selectedBtn = sender as UIButton;
			UIView answersView = selectedBtn.Superview.Superview;
			for (int i = 0;; i++) {
				var view = answersView.ViewWithTag (i + 100) as UIView;
				if (view == null)
					break;
				((UIImageView)(view.ViewWithTag (1000))).Highlighted = false;
			}
			UIView answerView = selectedBtn.Superview;
			((UIImageView)(answerView.ViewWithTag (1000))).Highlighted = true;
			selectedNode = currentNode.ChildNodes [selectedBtn.Tag - 1];
		}

		partial void OnBackBtn_Pressed (SDXButton sender)
		{
			if (parentNodes.Count == 0){
				NavigationController.PopViewControllerAnimated(true);
			} else {

			var prevView = decisionViews.Last ();

				ActionsView.Hidden = true;
				UIView.Animate (0.3f, () => {
					Util.MoveViewToX (currentView, 300);
					Util.MoveViewToX (prevView, 0);
				}, () => {
					currentView.RemoveFromSuperview ();
					currentView = prevView;
					decisionViews.Remove (prevView);

					currentNode = parentNodes.Last ();
					parentNodes.Remove (currentNode);
					selectedNode = selectedNodes.Last ();
					selectedNodes.Remove (selectedNode);

					Util.MoveViewToY (ActionsView, currentView.Frame.Y + currentView.Frame.Height);
					ActionsView.Hidden = false;
					ResetContentSize (false);
					NextBtn.Enabled = true;

				});
			}
		}

		partial void OnNextBtn_Pressed (SDXButton sender)
		{
			if (selectedNode == null)
				return;

			if (selectedNode.Planogram != null) {
				ShowLoading ("Loading...");
				var versionModel = selectedNode.Planogram.Versions [0];
				var imgManager = new JHImageManager ();
				imgManager.LoadCompleted += (object s, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						HideLoading ();
						if (bytes == null) {
							Util.ShowAlert ("Something went wrong. Please try again.");
						} else {
							ActionsView.Hidden = true;
							PlanogramView.Hidden = false;
							FillPlanogramView (selectedNode.Planogram, bytes);
							Util.MoveViewToX (PlanogramView, 300);
							UIView.Animate (0.3f, () => {
								Util.MoveViewToX (currentView, -300);
								Util.MoveViewToX (PlanogramView, 0);
							}, () => {
								ResetContentSize (true);
							});
						}
					});
				};
				imgManager.LoadImageAsync (versionModel.Photo.ProcessedPhotoBaseUrl, 600, 0);
				return;
			}

			UIView nextView = CreateDecisionView (selectedNode.ChildNodes[0]);
			Util.MoveViewToX (nextView, 300);
			ScrollView.AddSubview (nextView);

			ActionsView.Hidden = true;
			UIView.Animate (0.3f, () => {
				Util.MoveViewToX (currentView, -300);
				Util.MoveViewToX (nextView, 0);
			}, () => {
				decisionViews.Add (currentView);
				parentNodes.Add (currentNode);
				selectedNodes.Add (selectedNode);

				currentView = nextView;
				currentNode = selectedNode.ChildNodes [0];
				selectedNode = null;

				Util.MoveViewToY (ActionsView, currentView.Frame.Y + currentView.Frame.Height);
				ActionsView.Hidden = false;
				ResetContentSize (false);
				BackBtn.Enabled = true;
			});
		}

		void OnDropBtn_Pressed (object sender, EventArgs args)
		{
			if (DropView.Hidden == false)
				OnHideDropBtn_Pressed (null);

			float fH = DropTableView.Frame.Height;
			RectangleF frame = DropTableView.Frame;
			frame.Y = ActionsView.Frame.Y - 10;
			frame.Height = 1.0f;
			DropTableView.Frame = frame;

			DropView.Hidden = false;
			UIView.Animate (0.3f, () => {
				Util.ChangeViewHeight (DropTableView, fH);
			});
		}

		partial void OnHideDropBtn_Pressed (UIButton sender)
		{
			RectangleF frame = DropTableView.Frame;
			UIView.Animate (0.3f, () => {
				Util.ChangeViewHeight (DropTableView, 0);
			}, () => {
				DropView.Hidden = true;
			});
		}

		partial void OnBackToDecisionTreeBtn_Pressed (SDXButton sender)
		{
			UIView.Animate (0.3f, () => {
				Util.MoveViewToX (PlanogramView, 300);
				Util.MoveViewToX (currentView, 0);
			}, () => {
				ActionsView.Hidden = false;
				ResetContentSize (false);
			});
		}

		async partial void OnSetAsPlanogramBtn_Pressed (SDXButton sender)
		{
			string locationId = Account.LocationId;
			int outletId = Account.Outlets [OutletIndex].OutletId;
			int offerId = Account.Outlets [OutletIndex].Offers [OfferIndex].OfferId;
			int decisionTreeNodeId = selectedNode.DecisionTreeNodeId;

			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			ShowLoading ("Accepting...");
			var response = await manager.SetPlanogram (locationId, outletId, offerId, decisionTreeNodeId);
			HideLoading ();

			if (!manager.IsSuccessed) {
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			Account.Outlets [OutletIndex].Offers [OfferIndex].Responses.Insert (0, response);

			PerformSegue ("SegueToOfferDetail", this);
		}
		#endregion

		#region Event Handler
		private void OnDropTableCell_Selected(object sender, int row)
		{
			selectedNode = currentNode.ChildNodes [row];
			answerTF.Text = selectedNode.Text;
			OnHideDropBtn_Pressed (null);
		}
		#endregion

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToOfferDetail") {
				var vc = segue.DestinationViewController as SDXOfferDetailVC;
				vc.Account = Account;
				vc.OutletIndex = OutletIndex;
				vc.OfferIndex = OfferIndex;
				vc.PopVC = ((AppDelegate)UIApplication.SharedApplication.Delegate).AccountDetailVC;
			} else if (segue.Identifier == "SegueToFullImage") {
				var vc = segue.DestinationViewController as SDXFullImageVC;
				vc.Picture = PlanogramPictureIV.Image;
			}
		}
		#endregion
	}
}

