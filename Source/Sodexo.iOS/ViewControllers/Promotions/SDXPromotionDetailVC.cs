
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.EventKit;

using Sodexo.RetailActivation.Portable.Models;
using System.Collections.Generic;
using Sodexo.Core;
using System.Linq;

using TinyIoC;

namespace Sodexo.iOS
{
	public partial class SDXPromotionDetailVC : SDXBaseVC
	{
		public List<PromotionModel> Promotions;
		public List<PromotionModel> FilteredPromotions;
		public int index;
		public int PromotionId = -1;
		public string PromotionTitle = "";

		private PromotionModel promotion;
		private const int PHOTOIV_TAG_BASE = 10;

		public SDXPromotionDetailVC (IntPtr handle) : base (handle)
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

			if (PromotionId < 0) {
				promotion = FilteredPromotions [index];
				NavigationItem.Title = promotion.Title;
				FillContents ();
			} else {
				NavigationItem.Title = PromotionTitle;
				ScrollView.Hidden = true;
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			AddBackButton (2);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (PromotionId > 0 && promotion == null)
				LoadPromotion ();
		}
		#endregion

		#region Private Functions
		async private void LoadPromotion ()
		{
			ShowLoading ("Loading...");
			var manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			promotion = await manager.LoadPromotion (PromotionId);

			if (!manager.IsSuccessed) {
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			if (promotion.Photo != null) {
				JHImageManager imgMgr = new JHImageManager ();
				imgMgr.LoadCompleted += (object sender, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						HideLoading ();
						if (bytes == null) {
							promotion.Photo = null;
							promotion.PhotoId = null;
						} else {
							UIImage img = Util.GetImageFromByteArray (bytes);
							promotion.PhotoId = (int)(1000 * img.Size.Height / img.Size.Width);
						}
						FillContents ();
					});
				};
				imgMgr.LoadImageAsync (promotion.Photo.ProcessedPhotoBaseUrl, 600, 0);
			} else {
				HideLoading ();
				FillContents ();
			}
		}

		private void FillContents ()
		{
			ScrollView.Hidden = false;

			/* Picture ImageView */

			float y = 10, h =0;
			if (promotion.PhotoId != null) {
				h = (float)promotion.PhotoId * 300 / 1000.0f;
				Util.ChangeViewHeight (PictureIV, h);
				JHImageManager manager = new JHImageManager ();
				manager.LoadCompleted += (object sender, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						PictureIV.Image = Util.GetImageFromByteArray (bytes);
					});
				};
				manager.LoadImageAsync (promotion.Photo.ProcessedPhotoBaseUrl, 600, 0);
				y += h;
			} else {
				PictureIV.Hidden = true;
			}

			/* ContentView */
			h = 0;

			// Number, Title, Date
			NameLB.Text = promotion.Title;
			DateLB.Text = "START:" + promotion.StartDate.ToShortDateString () + "-FINISH:" + promotion.EndDate.ToShortDateString ();
			h += 3 + 60;

			// Description
			DescriptionLB.Text = promotion.Description;
			DescriptionLB.SizeToFit ();
			h += 5 + DescriptionLB.Frame.Height + 5;

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
				y += AcceptView.Frame.Height;
			} else {
				AcceptView.Hidden = true;
				int iCounter = 0, idx = 0;
				if (promotion.Activations != null && promotion.Activations.Count != 0) {
					AddPhotoLB.Hidden = true;
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
			ScrollView.ContentSize = new SizeF (ScrollView.Frame.Width, y);
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
				float contentH = ScrollView.ContentSize.Height + AddPhotoBtn.Frame.Height + 4;
				Util.ChangeViewHeight (PhotosView, PhotosView.Frame.Height + AddPhotoBtn.Frame.Height + 4);
				ScrollView.ContentSize = new SizeF (ScrollView.Frame.Width, contentH);
				ScrollView.ScrollRectToVisible (new RectangleF (0, contentH - 50, ScrollView.Frame.Width, 50), true);
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
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			byte[] bytes = Util.GetByteArrayFromImage (image);
			Console.WriteLine ("Bytes Count = " + bytes.Count ());

			await manager.UploadPhoto (sasRequest, bytes);

			if (!manager.IsSuccessed) {
				RemoveAddedPhotoIV ();
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			PhotoModel photo = await manager.AddPhotoToExistingItem (sasRequest, fileName);

			if (!manager.IsSuccessed) {
				RemoveAddedPhotoIV ();
				HideLoading ();
				ShowErrorMessage (manager.ErrorMessage);
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
				float contentH = ScrollView.ContentSize.Height - AddPhotoBtn.Frame.Height - 4;
				Util.ChangeViewHeight (PhotosView, PhotosView.Frame.Height - AddPhotoBtn.Frame.Height - 4);
				ScrollView.ContentSize = new SizeF (ScrollView.Frame.Width, contentH);
				ScrollView.ScrollRectToVisible (new RectangleF (0, contentH - 50, ScrollView.Frame.Width, 50), true);
			}
		}

		#endregion

		#region Button Actions
		partial void OnAddPhotoBtn_Pressed (UIButton sender)
		{
			JHImagePickHelper helper = new JHImagePickHelper (this);
			helper.PhotoPicked += (object s, UIImage img) => {
				SetPhoto (img);
			};
			helper.PickPhoto();
		}

		async partial void OnAcceptBtn_Pressed (SDXButton sender)
		{
			SDXPromotionManager manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			ShowLoading ("Accepting...");
			await manager.AcceptPromotion (promotion.PromotionId);
			HideLoading ();

			if (!manager.IsSuccessed) {
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			new UIAlertView ("Retail Ranger", "Promotion has been accepted.", null, "OK", null).Show ();

			PhotosView.Hidden = false;
			PhotosView.Alpha = 0;

			SizeF size = ScrollView.ContentSize;
			size.Height = size.Height - AcceptView.Frame.Height + PhotosView.Frame.Height;
			ScrollView.ContentSize = size;

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
			SDXPromotionManager manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXPromotionManager> ();
			ShowLoading ("Declining...");
			await manager.DeclinePromotion (promotion.PromotionId);
			HideLoading ();

			if (!manager.IsSuccessed) {
				ShowErrorMessage (manager.ErrorMessage);
				return;
			}

			UIAlertView alert = new UIAlertView ("Retail Ranger", "Promotion has been declined.", null, "OK", null);
			alert.Clicked += (object s, UIButtonEventArgs e) => {
				if (FilteredPromotions != null) {
					FilteredPromotions.RemoveAt (index);
					Promotions.Remove (promotion);
				}
				NavigationController.PopViewControllerAnimated (true);
			};
			alert.Show ();
		}

		partial void OnReadDocBtn_Pressed (SDXButton sender)
		{
			var storageContent = promotion.StorageContent;

			if (storageContent == null) {
				Util.ShowAlert ("Doc isn't available.");
				return;
			}

			var vc = Storyboard.InstantiateViewController ("SDXDocVC") as SDXDocVC;
			vc.DocUrl = storageContent.StorageContentUrl;
			NavigationController.PushViewController (vc, true);
		}

		async partial void OnSendFeedbackBtn_Pressed (SDXButton sender)
		{
			var isSuccess = await LoadReferenceData ();
			if (!isSuccess)
				return;

			var manager = TinyIoC.TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			FeedbackTypeModel feedbackType = null;
			foreach (var type in manager.DataModel.FeedbackTypes) {
				if (type.FeedbackTypeId == 2) {
					feedbackType = type;
					break;
				}
			}
			if (feedbackType == null)
				return;

			var vc = Storyboard.InstantiateViewController ("SDXLeaveFeedbackVC") as SDXLeaveFeedbackVC;
			vc.FeedbackType = feedbackType;
			vc.Promotion = promotion;
			NavigationController.PushViewController (vc, true);
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
					ShowErrorMessage (manager.ErrorMessage);
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
				ShowErrorMessage (mgr.ErrorMessage);
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
		#endregion
	}
}

