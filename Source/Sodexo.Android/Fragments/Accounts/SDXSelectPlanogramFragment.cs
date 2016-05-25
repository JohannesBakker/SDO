
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
	public class SDXSelectPlanogramFragment : SDXBaseFragment
	{
		public AccountModel Account;
		public int OutletIndex;
		public int OfferIndex;
		public bool IsFromOfferDetail = false;

		private int offerCategoryId;

		private SDXDecisionView currentView;
		private DecisionTreeNodeModel currentNode, selectedNode;

		private List<SDXDecisionView> decisionViews = new List<SDXDecisionView> ();
		private List<DecisionTreeNodeModel> parentNodes = new List<DecisionTreeNodeModel> ();
		private List<DecisionTreeNodeModel> selectedNodes = new List<DecisionTreeNodeModel> ();

		View view;
		LinearLayout planogramView, answersContainerView, containerView;
		FrameLayout actionsView;
		Button backBtn, nextBtn;

		#region Fragment Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			HideAllHeaderButtons ();
			SetHeaderTitle ("Select Planogram", 1);
			AddBackBtn (1);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.SelectPlanogram, container, false);
			LayoutView (view);

			GetInstance ();

			containerView.RemoveView (planogramView);

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);

			FillContents ();

			if (planogramView.Parent == null) {
				LoadDecisionTree ();
			}
		}
		#endregion

		#region Private Functions
		private void GetInstance ()
		{
			containerView = view.FindViewById (Resource.Id.selectplanogram_container_ll) as LinearLayout;
			planogramView = view.FindViewById (Resource.Id.selectplanogram_planogram_ll) as LinearLayout;
			actionsView = view.FindViewById (Resource.Id.selectplanogram_actions_fl) as FrameLayout;
			answersContainerView = view.FindViewById (Resource.Id.selectplanogram_answers_container_ll) as LinearLayout;
			backBtn = view.FindViewById (Resource.Id.selectplanogram_back_btn) as Button;
			nextBtn = view.FindViewById (Resource.Id.selectplanogram_next_btn) as Button;

			backBtn.Click += BackBtn_OnClicked;
			nextBtn.Click += NextBtn_OnClicked;

			var plBackBtn = view.FindViewById (Resource.Id.selectplanogram_planogram_back_btn) as Button;
			var plSetBtn = view.FindViewById (Resource.Id.selectplanogram_planogram_next_btn) as Button;

			plBackBtn.Click += BackToDecisionTreeBtn_OnClicked;
			plSetBtn.Click += SetAsPlanogramBtn_OnClicked;
		}

		private void FillContents ()
		{
			var accountNameTv = view.FindViewById (Resource.Id.selectplanogram_account_name_tv) as TextView;
			var accountAddrTv = view.FindViewById (Resource.Id.selectplanogram_account_address_tv) as TextView;
			var locationIdTv = view.FindViewById (Resource.Id.selectplanogram_account_locationid_tv) as TextView;
			accountNameTv.Text = Account.Location.LocationName;
			accountAddrTv.Text = Account.Location.LocationCity + " " + Account.Location.LocationAddress1;
			locationIdTv.Text = Account.Location.LocationId;

			var outletNameTv = view.FindViewById (Resource.Id.selectplanogram_outlet_name_tv) as TextView;
			var outletPictureIv = view.FindViewById (Resource.Id.selectplanogram_outlet_picture_iv) as ImageView;

			OutletModel outlet = Account.Outlets [OutletIndex];
			outletNameTv.Text = outlet.Name;
			if (outlet.Photo != null) {
				var imgManager = new JHImageManager ();
				imgManager.LoadCompleted += (object s, byte[] bytes) => {
					Activity.RunOnUiThread (delegate {
						if (bytes == null || outletPictureIv == null)
							return;
						using (Bitmap bmp = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
							if (bmp != null)
								outletPictureIv.SetImageBitmap (bmp);
						}
					});
				};
				imgManager.LoadImageAsync (outlet.Photo.ProcessedPhotoBaseUrl, 148 * 2, 148 * 2);
			}

			var offerNameTv = view.FindViewById (Resource.Id.selectplanogram_offer_name_tv) as TextView;
			var offerPictureTv = view.FindViewById (Resource.Id.selectplanogram_offer_picture_iv) as ImageView;
			var offerStatusTv = view.FindViewById (Resource.Id.selectplanogram_offer_status_tv) as TextView;

			OfferModel offer = outlet.Offers [OfferIndex];
			offerNameTv.Text = offer.Name;
			var imgName = "img_offer_categories_" + offer.OfferCategory.OfferCategoryId.ToString ();
			offerPictureTv.SetImageResource (Activity.Resources.GetIdentifier (imgName, "drawable", Activity.PackageName));

			if (offer.Responses.Count == 0) {
				offerStatusTv.Text = "NOT SELECTED";
			} else {
				var offerResponse = offer.Responses [0];
				var planogram = offerResponse.AnswerNode.Planogram;
				if (!offerResponse.PlanogramActivated) {
					offerStatusTv.Text = planogram.Name +  "/NOT ACTIVE";
					offerStatusTv.SetTextColor (Color.Rgb (233, 70, 122));
				} else {
					offerStatusTv.Text = planogram.Name + "/ACTIVE";
					offerStatusTv.SetTextColor (Color.Rgb (125, 244, 146));
				}
			}

			offerCategoryId = offer.OfferCategoryId;
			actionsView.Visibility = ViewStates.Invisible;
		}

		async private void LoadDecisionTree ()
		{
			DecisionTreeModel decisionTree;
			SDXReferenceDataManager manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DecisionTrees.ContainsKey (offerCategoryId)) {
				decisionTree = manager.DecisionTrees [offerCategoryId];
			} else {
				ShowLoading ();
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
			answersContainerView.AddView (currentView.view);

			if (answersContainerView.LayoutParameters.Width != 600) {
				(new Handler ()).Post (new Java.Lang.Runnable (() => {
					Activity.RunOnUiThread (() => {
						LayoutAdjuster.FitToScreen (Activity, (ViewGroup)answersContainerView, Data.XRate, Data.YRate, Data.XRate, Data.XRate, Data.Density);
					});
				}));
			}

			actionsView.Visibility = ViewStates.Visible;
		}

		private SDXDecisionView CreateDecisionView (DecisionTreeNodeModel node)
		{
			SDXDecisionView decisionV = SDXDecisionView.Create (Activity, (ViewGroup)view);

			decisionV.RadioViewClickedEvent = RadioView_OnClicked;
			decisionV.Update (node);

			return decisionV;
		}

		private void FillPlanogramView (PlanogramModel planogram, byte[] bytes)
		{
			var nameTv = view.FindViewById (Resource.Id.selectplanogram_planogram_name_tv) as TextView;
			var titleTv = view.FindViewById (Resource.Id.selectplanogram_planogram_title_tv) as TextView;
			var avatarIv = view.FindViewById (Resource.Id.selectplanogram_planogram_thumb_iv) as ImageView;
			var pictureIv = view.FindViewById (Resource.Id.selectplanogram_planogram_picture_iv) as ImageView;
			var detailsTv = view.FindViewById (Resource.Id.selectplanogram_planogram_details_tv) as TextView;

			if (pictureIv.LayoutParameters.Width == 600) {
				(new Handler ()).Post (new Java.Lang.Runnable (() => {
					Activity.RunOnUiThread (() => {
						LayoutAdjuster.FitToScreen (Activity, (ViewGroup)planogramView, Data.XRate, Data.YRate, Data.XRate, Data.XRate, Data.Density);
					});
				}));
			}

			nameTv.Text = planogram.Name;
			titleTv.Text = planogram.Description;

			var pvm = planogram.Versions [0] as PlanogramVersionModel;
			if (pvm.IconPhoto != null) {
				var imgMgr = new JHImageManager ();
				imgMgr.LoadCompleted += (object s, byte[] bytesP) => {
					if (bytesP == null)
						return;
					using (Bitmap bmp = BitmapFactory.DecodeByteArray (bytesP, 0, bytesP.Length)) {
						if (bmp != null)
							avatarIv.SetImageBitmap (bmp);
					}
				};
				imgMgr.LoadImageAsync (pvm.IconPhoto.ProcessedPhotoBaseUrl, 80, 80);
			}

			using (Bitmap bmp = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length)) {
				if (bmp != null) {
					float h = pictureIv.LayoutParameters.Width * bmp.Height / bmp.Width;
					Util.ChangeViewHeight (pictureIv, (int)h);
					pictureIv.SetImageBitmap (bmp);
				}
			}

			detailsTv.Text = planogram.Details;
		}
		#endregion

		#region Button Actions
		void RadioView_OnClicked (object sender, int tag)
		{
			selectedNode = currentNode.ChildNodes [tag];
		}

		void BackBtn_OnClicked (object sender, EventArgs arg)
		{
			if (parentNodes.Count == 0) {
				PopFragment ();
			} else {
				var prevView = decisionViews.Last ();

				actionsView.Visibility = ViewStates.Invisible;

				Util.StartFadeAnimation (currentView.view, 300, 1.0f, 0.0f);
				(new Handler ()).PostDelayed (() => {
					Activity.RunOnUiThread (() => {
						answersContainerView.RemoveView (currentView.view);
						answersContainerView.AddView (prevView.view);
						currentView = prevView;
						decisionViews.Remove (prevView);

						currentNode = parentNodes.Last ();
						parentNodes.Remove (currentNode);
						selectedNode = selectedNodes.Last ();
						selectedNodes.Remove (selectedNode);

						actionsView.Visibility = ViewStates.Visible;

						nextBtn.Enabled = true;
					});
				}, 280);
			}
		}

		void NextBtn_OnClicked (object sender, EventArgs arg)
		{
			if (selectedNode == null)
				return;

			if (selectedNode.Planogram != null) {
				ShowLoading ();
				var versionModel = selectedNode.Planogram.Versions [0];
				var imgManager = new JHImageManager ();
				imgManager.LoadCompleted += (object s, byte[] bytes) => {
					Activity.RunOnUiThread (() => {
						HideLoading ();
						if (bytes == null) {
							ShowAlert ("Something went wrong. Please try again.");
						} else {
							containerView.RemoveView (actionsView);
							containerView.AddView (planogramView);
							FillPlanogramView (selectedNode.Planogram, bytes);
							answersContainerView.RemoveView (currentView.view);
						}
					});
				};
				imgManager.LoadImageAsync (versionModel.Photo.ProcessedPhotoBaseUrl, 600, 0);
				return;
			}

			SDXDecisionView nextView = CreateDecisionView (selectedNode.ChildNodes[0]);

			actionsView.Visibility = ViewStates.Invisible;

			Util.StartFadeAnimation (currentView.view, 300, 1.0f, 0.0f);
			(new Handler ()).PostDelayed (() => {
				Activity.RunOnUiThread (() => {
					answersContainerView.RemoveView (currentView.view);
					answersContainerView.AddView (nextView.view);
					if (answersContainerView.LayoutParameters.Width != 600) {
						(new Handler ()).Post (new Java.Lang.Runnable (() => {
							Activity.RunOnUiThread (() => {
								LayoutAdjuster.FitToScreen (Activity, (ViewGroup)answersContainerView, Data.XRate, Data.YRate, Data.XRate, Data.XRate, Data.Density);
							});
						}));
					}

					decisionViews.Add (currentView);
					parentNodes.Add (currentNode);
					selectedNodes.Add (selectedNode);

					currentView = nextView;
					currentNode = selectedNode.ChildNodes [0];
					selectedNode = null;

					actionsView.Visibility = ViewStates.Visible;
					backBtn.Enabled = true;
				});
			}, 280);
		}

		void BackToDecisionTreeBtn_OnClicked (object sender, EventArgs arg)
		{
			containerView.RemoveView (planogramView);
			answersContainerView.AddView (currentView.view);
			containerView.AddView (actionsView);
		}

		async void SetAsPlanogramBtn_OnClicked (object sender, EventArgs arg)
		{
			string locationId = Account.LocationId;
			int outletId = Account.Outlets [OutletIndex].OutletId;
			int offerId = Account.Outlets [OutletIndex].Offers [OfferIndex].OfferId;
			int decisionTreeNodeId = selectedNode.DecisionTreeNodeId;

			var manager = TinyIoCContainer.Current.Resolve <SDXAccountManager> ();
			ShowLoading ();
			var response = await manager.SetPlanogram (locationId, outletId, offerId, decisionTreeNodeId);
			HideLoading ();

			if (!manager.IsSuccessed) {
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			Account.Outlets [OutletIndex].Offers [OfferIndex].Responses.Insert (0, response);

			if (IsFromOfferDetail) {
				PopFragment ();
			} else {
				var fragment = new SDXOfferDetailFragment ();
				fragment.Account = Account;
				fragment.OutletIndex = OutletIndex;
				fragment.OfferIndex = OfferIndex;
				fragment.PopCount = 2;
				PushFragment (fragment, this.Class.SimpleName);
			}
		}
		#endregion
	}
}

