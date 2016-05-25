
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.EventKit;

using Sodexo.Core;
using TinyIoC;
using System.Collections.Generic;
using System.Linq;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public partial class SDXPromotionsVC : SDXBaseVC
	{
		public bool IsSelecting = false;
		public FeedbackTypeModel FeedbackType;

		private List<PromotionModel> Promotions;
		private List<PromotionModel> filteredPromotions = new List<PromotionModel> ();
		private string[] promotionCategoryNames = {"Cold Beverage", "Simply to Go", "Hot Beverages", "Mutualized", "Mindful", "In My Kitchen", "Pricing", "All"};

		private PromotionModel promotion;
		private const int PHOTOIV_TAG_BASE = 10;

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

			FillFilterView ();
			 
			TableView.Delegate = new SDXPromotionTableDelegate ();
			TableView.DataSource = new SDXPromotionTableDataSource ();
			((SDXPromotionTableDelegate)TableView.Delegate).RowSelectedEvent += OnRow_Selected;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetHeaderBackground (2);
			SetSectionName ("Promotions");

			MessageLB.TextColor = UIColor.FromRGB (80, 80, 80);
			MessageLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 12);

			Util.MoveViewToY (FilterView, - FilterView.Frame.Size.Height);
			MessageView.Hidden = true;

			if (promotion == null) {
				LoadPromotions ();
			}
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);


		}
		#endregion

		#region Private Functions
		private void FillFilterView ()
		{
			var moveX = 28;
			var moveY = 28;

			for (int i = 0; i < promotionCategoryNames.Count (); i++) {
				UIImageView iv = new UIImageView ();
				iv.ContentMode = UIViewContentMode.ScaleAspectFit;
				iv.Image = UIImage.FromBundle ("promotion_" + promotionCategoryNames [i].Replace (" ", "_").ToLower () + ".png");
				//iv.Frame = new RectangleF ((i % 4) * 80 + 20, (i / 4) * 80 + 10, 40, 30);
				iv.Frame = new RectangleF ( i  * 80 + 20 + moveX, 10 + moveY, 40, 30);
				iv.BackgroundColor = UIColor.Clear;
				FilterView.Add (iv);

				UILabel lb = new UILabel ();
				lb.Text = promotionCategoryNames [i].ToUpper ();
				lb.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 11);
				lb.BackgroundColor = UIColor.Clear;
				lb.TextColor = UIColor.DarkGray;
				//lb.Frame = new RectangleF ((i % 4) * 80 + 5, (i / 4) * 80 + 45, 70, 30);
				lb.Frame = new RectangleF ( i * 80 + 5 + moveX, 45 + moveY, 70, 30);
				lb.TextAlignment = UITextAlignment.Center;
				FilterView.Add (lb);

				UIButton btn = new UIButton ();
				btn.BackgroundColor = UIColor.Clear;
				btn.Frame = new RectangleF (i * 80 + moveX, 0 + moveY, 80, 80);
				//btn.Frame = new RectangleF ((i % 4) * 80, (i / 4) * 80, 80, 80);
				btn.TouchUpInside += OnPromotionCategoryBtn_Pressed;
				btn.Tag = i + 1;
				FilterView.Add (btn);
			}

//			Util.ChangeViewHeight (FilterView, (promotionCategoryNames.Count () / 4) * 80 + FilterSubView.Frame.Height);
//			Util.MoveViewToY (FilterSubView, (promotionCategoryNames.Count () / 4) * 80);
		}

		private async void LoadPromotions ()
		{
			if (Promotions != null)
			{
				((SDXPromotionTableDelegate)TableView.Delegate).Promotions = filteredPromotions;
				((SDXPromotionTableDataSource)TableView.DataSource).Promotions = filteredPromotions;

				TableView.ReloadData ();
				PromotionView.Hidden = true;
				MessageView.Hidden = false;

				return;
			}

			SDXPromotionManager manager = TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			ShowLoading ("Loading...");
			Promotions = await manager.LoadPromotions ();

			if (!manager.IsSuccessed) {
				HideLoading ();
				Util.ShowAlert (manager.ErrorMessage);
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

			PromotionView.Hidden = true;
			MessageView.Hidden = false;

			//If there is a default promotion set up on dashboard
			if (dashboardItem!=null && dashboardItem.ModelId > 0) {
				var promotion = filteredPromotions.First (x => x.PromotionId == dashboardItem.ModelId);
				var index = filteredPromotions.IndexOf (promotion);
				var path = NSIndexPath.FromRowSection (index, 0);
		
				TableView.SelectRow (path,true,UITableViewScrollPosition.Middle);
				OnRow_Selected (null, 0);
			}
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

			PromotionView.Hidden = true;

			if(filteredPromotions.Count==0)
			{
				Util.MoveViewToX(MessageView,View.Frame.Width/2-MessageView.Frame.Width/2);
				MessageLB.Text = "No promotions found";
				MessageView.Hidden = false;

			}else{
				Util.MoveViewToX(MessageView,View.Frame.Width/2-MessageView.Frame.Width/2+160);
				MessageLB.Text = "Please select promotion to see details";
				MessageView.Hidden = false;
			}
		}

		partial void OnCancelBtn_Pressed (SDXButton sender)
		{
			if (KeywordTF.IsFirstResponder)
				KeywordTF.ResignFirstResponder ();

			UIView.Animate (0.3f, () => {
				Util.MoveViewToY (FilterView, - FilterView.Frame.Height);
				Util.MoveViewToY (FilterBtn, 0);
			});
		}

//		partial void OnReturnKey_Pressed (UITextField sender)
//		{
//			UITextField tf = sender as UITextField;
//			tf.ResignFirstResponder ();
//		}

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
			PromotionView.Hidden = true;
			MessageView.Hidden = true;
			LoadPromotion ();
		}
		#endregion

		#region Promotion Detail

		async private void LoadPromotion ()
		{
			ShowLoading ("Loading...");
			promotion = Promotions [TableView.IndexPathForSelectedRow.Row];


			/* Picture ImageView */

			float y = 10, h =0;
			if (promotion.Photo != null) {
				h = (float)promotion.PhotoId * 663 / 1000.0f;
				Util.ChangeViewHeight (PictureIV, h);
				JHImageManager manager = new JHImageManager ();
				manager.LoadCompleted += (object sender, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						PictureIV.Image = Util.GetImageFromByteArray (bytes);
					});
				};
				manager.LoadImageAsync (promotion.Photo.ProcessedPhotoBaseUrl, 1326, 0);
				y += h;
			} else {
				PictureIV.Hidden = true;
			}

			/* ContentView */
			h = 0;

			// Number, Title, Date
			NameLB.Text = promotion.Title.ToUpper();
			NameLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 17);

			DateLB.Text = ("START:" + promotion.StartDate.ToShortDateString () + "-FINISH:" + promotion.EndDate.ToShortDateString ()).ToUpper();
			DateLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 9);

			h += 3 + 60;

			// Description
			DescriptionLB.Text = promotion.Description;
			DescriptionLB.Font = UIFont.FromName (Constants.HELVETICA_NEUE_LIGHT, 9);

			float textH = Util.GetHeightOfString (promotion.Description, DescriptionLB.Frame.Width, DescriptionLB.Font) + 10;
			Util.ChangeViewHeight (DescriptionLB, textH);
			h += 5 + textH + 5;

			// Actions View
			Util.MoveViewToY (ActionsView, h);
			h += ActionsView.Frame.Height + 5;

			// Status View
			if (promotion.PromotionActivated) {
				StatusLB.TextColor = UIColor.FromRGB (0, 191 / 255.0f, 77 / 255.0f);
				StatusLB.Text = "STATUS: ACTIVE";
				StatusIV.Image = UIImage.FromBundle ("img_active.png");
			} else {
				StatusLB.TextColor = UIColor.FromRGB (255 / 255.0f, 0, 114 / 255.0f);
				StatusLB.Text = "STATUS: NOT ACTIVE";
				StatusIV.Image = UIImage.FromBundle ("img_inactive.png");
			}
			Util.MoveViewToY (StatusView, h);
			h += StatusView.Frame.Height;

			if (promotion.PromotionCategories.Count > 0) {
				var categoryName = promotion.PromotionCategories [0].Description;
				var fileName = "promotion_" + categoryName.Replace (" ", "_").ToLower () + ".png";
				CategoryIV.Image = UIImage.FromBundle (fileName);
			}

			RectangleF frame = ContentView.Frame;
			frame.Y = y; frame.Height = h;
			ContentView.Frame = frame;

			y += h + 5;

			Util.MoveViewToY (PhotosView, y);
			Util.MoveViewToY (AcceptView, y);

			/* Photos View Or Accept View */
			if (!promotion.PromotionAccepted) {
				PhotosView.Hidden = true;
				AcceptView.Hidden = false;
				y += AcceptView.Frame.Height;

				AcceptNoBtn.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 12);
				AcceptNoBtn.SetTitleShadowColor (UIColor.FromRGB (203, 203, 203), UIControlState.Normal);
				AcceptNoBtn.TitleShadowOffset = new SizeF (1, 1);

				AcceptYesBtn.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 12);
				AcceptYesBtn.SetTitleShadowColor (UIColor.FromRGB (203, 203, 203), UIControlState.Normal);
				AcceptYesBtn.TitleShadowOffset = new SizeF (1, 1);

				AcceptLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 14);

			} else {
				AcceptView.Hidden = true;
				PhotosView.Hidden = false;
				int iCounter = 0, idx = 0;
				if (promotion.Activations != null && promotion.Activations.Count != 0) {
					AddPhotoLB.Hidden = true;
				}

				foreach (UIView sub in PhotosView.Subviews) {
					if (sub != AddPhotoBtn && sub!=AddPhotoLB) {
						sub.RemoveFromSuperview ();
					}
				}

				foreach (PhotoModel photo in promotion.Activations) 
				{
					RectangleF frm = AddPhotoBtn.Frame;
					frm.X = (idx % 2) * (AddPhotoBtn.Frame.Width + 4);
					frm.Y = (idx / 2) * (AddPhotoBtn.Frame.Height + 4);

					UIImageView iv = new UIImageView (frm);
					iv.ContentMode = UIViewContentMode.ScaleAspectFill;
					iv.ClipsToBounds = true;
					iv.Tag = idx + PHOTOIV_TAG_BASE;
					PhotosView.Add (iv);

					JHImageManager imgManager = new JHImageManager ();
					imgManager.LoadCompleted += (object s, byte[] bytes) => {
						InvokeOnMainThread (delegate {
							if (PhotosView == null) return;
							var imgV = (UIImageView) PhotosView.ViewWithTag (iCounter + PHOTOIV_TAG_BASE);
							if (imgV != null)
								imgV.Image = Util.GetImageFromByteArray (bytes);
							iCounter ++;
						});
					};
					imgManager.LoadImageAsync (photo.ProcessedPhotoBaseUrl, (int) AddPhotoBtn.Frame.Width * 2, 0);
					idx++;
				}

				RectangleF frmPhoto = AddPhotoBtn.Frame;
				frmPhoto.X = (promotion.Activations.Count % 2) * (AddPhotoBtn.Frame.Width + 4);
				frmPhoto.Y = (promotion.Activations.Count / 2) * (AddPhotoBtn.Frame.Height + 4);
				AddPhotoBtn.Frame = frmPhoto;

				Util.ChangeViewHeight (PhotosView, ((int)((promotion.Activations.Count + 1) / 2.0f + 0.5f)) * (AddPhotoBtn.Frame.Height + 4) - 4);

				y += PhotosView.Frame.Height;
			}

			y += 10;
			/* Scroll View */
			PromotionView.ContentSize = new SizeF (PromotionView.Frame.Width, y);

			MessageView.Hidden = true;
			PromotionView.Hidden = false;
			HideLoading ();


		}

		private void SetPhoto (UIImage image)
		{
			if (image == null)
				return;

			Console.WriteLine ("Picked Image Size : " + image.Size.ToString());

			UIImageView iv = new UIImageView (AddPhotoBtn.Frame);
			iv.ContentMode = UIViewContentMode.ScaleAspectFill;
			iv.ClipsToBounds = true;
			iv.Image = image;
			iv.Tag = promotion.Activations.Count + PHOTOIV_TAG_BASE;
			PhotosView.Add (iv);

			RectangleF frame = AddPhotoBtn.Frame;
			frame.X = ((promotion.Activations.Count + 1) % 2) * (AddPhotoBtn.Frame.Width + 4);
			frame.Y = ((promotion.Activations.Count + 1) / 2) * (AddPhotoBtn.Frame.Height + 4);
			AddPhotoBtn.Frame = frame;

			if ((promotion.Activations.Count + 1) % 2 == 0) {
				float contentH = PromotionView.ContentSize.Height + AddPhotoBtn.Frame.Height + 4;
				Util.ChangeViewHeight (PhotosView, PhotosView.Frame.Height + AddPhotoBtn.Frame.Height + 4);
				PromotionView.ContentSize = new SizeF (PromotionView.Frame.Width, contentH);
				PromotionView.ScrollRectToVisible (new RectangleF (0, contentH - 50, PromotionView.Frame.Width, 50), true);
			}

			PerformSelector (new MonoTouch.ObjCRuntime.Selector ("UploadPhoto:"), image, 0.5f);
		}

		[Export ("UploadPhoto:")]
		private async void UploadPhoto (UIImage image)
		{
			ShowLoading ("Uploading...");

			SDXImageManager manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXImageManager> ();

			string fileName = promotion.Title.Replace(" ", "").ToLower () + ".png";
			Console.WriteLine ("FileName = " + fileName);

			PhotoPostingSasRequest sasRequest = await manager.GetPhotoPostingToken (fileName, "Promotion", promotion.PromotionId.ToString ());

			if (!manager.IsSuccessed) {
				RemoveAddedPhotoIV ();
				HideLoading ();
				new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
				return;
			}

			byte[] bytes = Util.GetByteArrayFromImage (image);
			Console.WriteLine ("Bytes Count = " + bytes.Count ());

			await manager.UploadPhoto (sasRequest, bytes);

			if (!manager.IsSuccessed) {
				RemoveAddedPhotoIV ();
				HideLoading ();
				new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
				return;
			}

			PhotoModel photo = await manager.AddPhotoToExistingItem (sasRequest, fileName);

			if (!manager.IsSuccessed) {
				RemoveAddedPhotoIV ();
				HideLoading ();
				new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
				return;
			}

			if (promotion.Activations.Count == 0) {
				promotion.PromotionActivated = true;
				StatusLB.TextColor = UIColor.FromRGB (0, 191 / 255.0f, 77 / 255.0f);
				StatusLB.Text = "STATUS: PROMOTION IS ACTIVE";
				StatusIV.Image = UIImage.FromBundle ("img_active.png");
			}

			promotion.Activations.Add (photo);

			if (AddPhotoLB.Hidden == false)
				AddPhotoLB.Hidden = true;

			SizeF scaledSize = new SizeF (AddPhotoBtn.Frame.Width * 2, AddPhotoBtn.Frame.Height * 2);
			UIImage scaledImg = Util.GetScaledImage (image, scaledSize);
			JHImageManager imgManager = new JHImageManager ();
			imgManager.WriteImageToFile (sasRequest.FileName, Util.GetByteArrayFromImage(scaledImg), (int)scaledSize.Width, 0);
			HideLoading ();
			new UIAlertView ("Retail Ranger", "Successfully uploaded.", null, "OK", null).Show ();
		}

		private void RemoveAddedPhotoIV ()
		{
			var iv = PhotosView.ViewWithTag (promotion.Activations.Count + PHOTOIV_TAG_BASE) as UIImageView;
			AddPhotoBtn.Frame = iv.Frame;
			iv.RemoveFromSuperview ();

			if ((promotion.Activations.Count) % 2 == 1) {
				float contentH = PromotionView.ContentSize.Height - AddPhotoBtn.Frame.Height - 4;
				Util.ChangeViewHeight (PhotosView, PhotosView.Frame.Height - AddPhotoBtn.Frame.Height - 4);
				PromotionView.ContentSize = new SizeF (PromotionView.Frame.Width, contentH);
				PromotionView.ScrollRectToVisible (new RectangleF (0, contentH - 50, PromotionView.Frame.Width, 50), true);
			}
		}

		#endregion

		partial void OnReadDocBtn_Pressed (SDXButton sender)
		{
			var storageContent = promotion.StorageContent;

			if (storageContent == null) {
				Util.ShowAlert ("Doc isn't available.");
				return;
			}

			var vc = Storyboard.InstantiateViewController ("SDXPromotionDocVC") as SDXPromotionDocVC;
			vc.DocUrl = storageContent.StorageContentUrl;
			NavigationController.PushViewController (vc, true);
			//PresentViewController(vc,true,null);
		}

		async partial void OnSendFeedbackBtn_Pressed (SDXButton sender)
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel == null) {
				await LoadReferenceData ();
				return;
			}

			if (manager.DataModel.FeedbackTypes.Count < 1)
				return;

			var vc = Storyboard.InstantiateViewController ("SDXPromotionsFeedbackVC") as SDXPromotionsFeedbackVC;
			var item = new DashboardItemModel();
			item.ModelId = promotion.PromotionId;

			FeedbackTypeModel selectedFeedbackType = manager.DataModel.FeedbackTypes [1];
			vc.dashboardItem = item;
			vc.FeedbackType = selectedFeedbackType;
			NavigationController.PushViewController (vc, true);

//			var isSuccess = await LoadReferenceData ();
//			if (!isSuccess)
//				return;
//
//			var manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
//			FeedbackTypeModel feedbackType = null;
//			foreach (var type in manager.DataModel.FeedbackTypes) {
//				if (type.FeedbackTypeId == 2) {
//					feedbackType = type;
//					break;
//				}
//			}
//			if (feedbackType == null)
//				return;
//
//			var vc = Storyboard.InstantiateViewController ("SDXLeaveFeedbackVC") as SDXLeaveFeedbackVC;
//			vc.FeedbackType = feedbackType;
//			vc.Promotion = promotion;
//			NavigationController.PushViewController (vc, true);
		}

		async partial void OnSendToMyMailBtn_Pressed (SDXButton sender)
		{
			string email = "";
			// Get Email Address
			var manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
			if (manager.Me == null) {
				ShowLoading ("Loading...");
				UserModel me = await manager.LoadMe (false);
				if (!manager.IsSuccessed) {
					HideLoading ();
					new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
					return;
				}
				email = me.Email;
			} else
				email = manager.Me.Email;

			if (email == null || email == "") {
				HideLoading ();
				Util.ShowAlert ("Email is Empty");
				return;
			}

			int promotionId = promotion.PromotionId;

			ShowLoading ("Sending...");
			var mgr = TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			await mgr.SendPromotionViaEmail (promotionId, email);
			HideLoading ();

			if (!mgr.IsSuccessed) {
				Util.ShowAlert (mgr.ErrorMessage);
				return;
			}
			Util.ShowAlert ("Email has been successfully sent.");
		}

		partial void OnAddToCalendarBtn_Pressed (SDXButton sender)
		{
			Console.WriteLine ("Start Date = " + promotion.StartDate);
			Console.WriteLine ("End Date = " + promotion.EndDate);

			EKEventStore store = new EKEventStore();

			store.RequestAccess (EKEntityType.Event, (granted, err) => {
				if ((bool)granted) {
					EKCalendar calendar = store.DefaultCalendarForNewEvents;

					// Query the event
					if (calendar != null)
					{
						// Searches for every event in the next year
						NSPredicate predicate = store.PredicateForEvents (DateTime.Now.AddDays (-360), DateTime.Now.AddDays (360), new EKCalendar[] {calendar});

						var isStopped = false;
						store.EnumerateEvents (predicate, delegate(EKEvent currentEvent, ref bool stop)
							{
								if (currentEvent.Title == promotion.Title) {
									isStopped = true;
									stop = true;
								}
							});
						if (isStopped) {
							InvokeOnMainThread (() => {
								Util.ShowAlert ("Already added to calendar.");
							});
							return;
						}

						//Fetch Calendar notes if available
						var eventNotes= string.Empty;						
						var settingManager = TinyIoCContainer.Current.Resolve <SDXSettingManager> ();
						if (settingManager.IsAuthorizedLoaded) {
							eventNotes = settingManager.CalendarEventText;
						}

						// Add a new event
						EKEvent newEvent = EKEvent.FromStore (store);
						newEvent.Title = promotion.Title;
						newEvent.Calendar = calendar;
						newEvent.StartDate = promotion.StartDate.Date.AddHours(9);//9am seems reasonable as a start time
						newEvent.EndDate = promotion.StartDate.Date.AddHours(9).AddMinutes(30);  //give em 30 minutes on their calendar to activate :)
						newEvent.Notes = eventNotes;
						newEvent.Availability = EKEventAvailability.Free;
						NSError error = null;
						store.SaveEvent (newEvent, EKSpan.ThisEvent, out error);
						InvokeOnMainThread (() => {
							if (error != null) {
								Console.WriteLine ("Error = " + error.Description);
								Util.ShowAlert ("Failed in Adding Event.");
							} else {
								Util.ShowAlert ("Success in Adding Event.");
							}
						});
					}
				} else {
					InvokeOnMainThread (() => {
						Util.ShowAlert ("You need permission to access calendar.");
					});
				}
			});
		}

		partial void OnAddPhotoBtn_Pressed (UIButton sender)
		{
			JHImagePickHelper helper = new JHImagePickHelper (this);
			helper.PhotoPicked += (object s, UIImage img) => {
				SetPhoto (img);
			};
			helper.PickPhoto();
		}

		#region Add or Reject Promotion
		async partial void OnAcceptBtn_Pressed (SDXButton sender)
		{
			promotion = Promotions [TableView.IndexPathForSelectedRow.Row];

			SDXPromotionManager manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			ShowLoading ("Accepting...");
			await manager.AcceptPromotion (promotion.PromotionId);
			HideLoading ();

			if (!manager.IsSuccessed) {
				new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
				return;
			}

			new UIAlertView ("Retail Ranger", "Promotion has been accepted.", null, "OK", null).Show ();

			PhotosView.Hidden = false;
			PhotosView.Alpha = 0;

			SizeF size = PromotionView.ContentSize;
			size.Height = size.Height - AcceptView.Frame.Height + PhotosView.Frame.Height;
			PromotionView.ContentSize = size;

			promotion.PromotionAccepted = true;

			UIView.Animate (0.3f, () => {
				AcceptView.Alpha = 0.0f;
				PhotosView.Alpha = 1.0f;
			}, () => {
				AcceptView.Hidden = true;
			});
		}

		async partial void OnDeclineBtn_Pressed (SDXButton sender)
		{
			promotion = Promotions [TableView.IndexPathForSelectedRow.Row];

			SDXPromotionManager manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			ShowLoading ("Declining...");
			await manager.DeclinePromotion (promotion.PromotionId);
			HideLoading ();

			if (!manager.IsSuccessed) {
				new UIAlertView ("Retail Ranger", manager.ErrorMessage, null, "OK", null).Show ();
				return;
			}

			UIAlertView alert = new UIAlertView ("Retail Ranger", "Promotion has been declined.", null, "OK", null);
			alert.Clicked += (object s, UIButtonEventArgs e) => {
				Promotions.Remove (promotion);
				((SDXPromotionTableDataSource)TableView.DataSource).Promotions = Promotions;
				TableView.ReloadData();
			};
			alert.Show ();
		}
		#endregion

	}
}

