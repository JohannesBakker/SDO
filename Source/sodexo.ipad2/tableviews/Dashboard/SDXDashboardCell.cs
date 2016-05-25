
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using System.Linq;

namespace Sodexo.Ipad2
{

	public partial class SDXDashboardCell : UITableViewCell
	{
		public SDXDashboardCell (IntPtr handle) : base (handle)
		{

		}

		public SDXDashboardCell () : base ()
		{

		}

		public void Update (DashboardItemModel item)
		{
			// Header
			IndicatorIV.Image = UIImage.FromBundle ("indicator_" + item.DasboardItemType.ToString ().ToLower () + ".png");
			SymbolIV.Image = UIImage.FromBundle ("symbol_" + item.DasboardItemType.ToString ().ToLower () + ".png");
			TitleLB.Text = item.Title;
			TitleLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 17);
			TitleLB.TextColor = UIColor.FromRGB (56, 56, 56);

			if (item.CalloutDate != null) {
				DateTime date = (DateTime) item.CalloutDate;
				DateLB.Text = date.Month.ToString ("00") + "." + date.Day.ToString ("00");
			} else {
				DateLB.Text = "";
			}
			DateLB.Font = UIFont.FromName (Constants.OSWALD_LIGHT, 17);
			DateLB.TextColor = UIColor.FromRGB (237, 115, 51);

			PictureIV.Hidden = true;
			ProgressView.Hidden = true;

			float y = 67;
			if (item.Photo != null) {
				PictureIV.Hidden = false;

				PictureIV.Frame = new RectangleF (0, 5, 300, 160);
				var imageManager = new JHImageManager ();
				imageManager.LoadCompleted += (object s, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						PictureIV.Alpha = 0.5f;
						PictureIV.Image = Util.GetImageFromByteArray (bytes);
						UIView.Animate (0.5f, () => {
							PictureIV.Alpha = 1; 
						});
					});
				};
				imageManager.LoadImageAsync (item.Photo.ProcessedPhotoBaseUrl, 600, 0);



			} 
			else 
			{
				PictureIV.Frame = new RectangleF (0, 0, 0, 0);

			}

			if (item.PercentageComplete > 0 && item.PercentageComplete < 100) 
			{
				ProgressView.Hidden = false;

				if (item.Photo == null) {
					ProgressView.Frame = new RectangleF (40, 77, 928, 46);
					FullProgressView.Frame = new RectangleF (0, 0, 928, 18);
					EmptyProgressIV.Frame = new RectangleF (0, 0, 928, 18);
					FullProgressIV.Frame = new RectangleF (0, 0, 928, 18);

					Util.ChangeViewWith (FullProgressView, EmptyProgressIV.Frame.Width * item.PercentageComplete / 100.0f);
					ProgressValueLB.Text = item.PercentageComplete.ToString () + "%";
					ProgressValueLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 9);
					ProgressValueLB.TextColor = UIColor.FromRGB (56, 56, 56);
					Util.MoveViewToX (ProgressValueView, FullProgressView.Frame.X + FullProgressView.Frame.Width - ProgressValueView.Frame.Width / 2);

				} else {
					ProgressView.Frame = new RectangleF (308, 67, 700, 46);
					FullProgressView.Frame = new RectangleF (0, 0, 700, 13);
					EmptyProgressIV.Frame = new RectangleF (0, 0, 700, 13);
					FullProgressIV.Frame = new RectangleF (0, 0, 700, 13);

					Util.ChangeViewWith (FullProgressView, EmptyProgressIV.Frame.Width * item.PercentageComplete / 100.0f);
					ProgressValueLB.Text = item.PercentageComplete.ToString () + "%";
					ProgressValueLB.Font = UIFont.FromName (Constants.OSWALD_REGULAR, 9);
					ProgressValueLB.TextColor = UIColor.FromRGB (56, 56, 56);
					Util.MoveViewToX (ProgressValueView, FullProgressView.Frame.X + FullProgressView.Frame.Width - ProgressValueView.Frame.Width / 2);
				}
				y = 121;


			} 
			else 
			{
				ProgressView.Hidden = true;
				ProgressView.Frame = new RectangleF (0, 0, MainView.Frame.Width, 0);
			}
				
			float textH = Util.GetHeightOfString (item.Text, DescriptionLB.Frame.Width, DescriptionLB.Font) + 10;
			RectangleF frame = DescriptionLB.Frame;
			frame.Y = y;
			frame.Height = textH;
			DescriptionLB.Frame = frame;
			DescriptionLB.Text = item.Text;
			DescriptionLB.Font = UIFont.FromName (Constants.HELVETICA_NEUE_LIGHT, 11);
			//var h = Util.DashboardCellHeight (item);
//			if (item.Title == "In My Kitchen Promotion - Trisha Yearwood") {
//				h = 180;
//			}
//			if (h < 160)
//				h = 160;
//			Console.WriteLine ("--------+++++++------" + h + " " + item.Title);		
//			var mainFrame = new RectangleF (10, 10, MainView.Frame.Width, h);
//			MainView.Frame = mainFrame;
//			MainContainer.Frame = mainFrame;
//			MainContainer.BackgroundColor = UIColor.Blue;

		}
	}
}
/**
[
  {
    "modelId": 0,
    "dasboardItemType": 6,
    "feedbackTypeId": null,
    "title": "Thank you for using Retail Ranger!",
    "text": "You are using the non-production Environment.",
    "photoId": null,
    "photo": null,
    "calloutDate": "2014-11-01T00:00:00",
    "percentageComplete": 0
  },
  {
    "modelId": 10001001,
    "dasboardItemType": 1,
    "feedbackTypeId": null,
    "title": "Finish Account Setup",
    "text": "Almost there. Finish selecting planograms for Unit 10001001-ITHACA EGBERT-DINING HALL.  You need to upload photos of your solutions to activate your planograms. Click here to upload.",
    "photoId": null,
    "photo": null,
    "calloutDate": null,
    "percentageComplete": 59
  },
  {
    "modelId": 3,
    "dasboardItemType": 3,
    "feedbackTypeId": null,
    "title": "enjoy Promotion - BBQ Pulled Chicken with Coleslaw",
    "text": "Slow-cooked shredded BBQ chicken with coleslaw, dill pickle chips and Frech-fried onion tanglers on a Kaiser roll.",
    "photoId": 16,
    "photo": {
      "photoId": 16,
      "photoUrl": "https://sodexoitzdevstorage.blob.core.windows.net/promotion/3-Enjoy-BBQPulledChickenWithColeslaw.png",
      "container": "promotion",
      "fileName": "3-Enjoy-BBQPulledChickenWithColeslaw.png",
      "processedPhotoBaseUrl": "https://sodexo-itz-dev.azurewebsites.net/azure/promotion/3-Enjoy-BBQPulledChickenWithColeslaw.png",
      "processedPhotoThumbnailUrl": "https://sodexo-itz-dev.azurewebsites.net/azure/promotion/3-Enjoy-BBQPulledChickenWithColeslaw.png?w=200&h=200&mode=crop&autorotate=true",
      "url": "https://sodexodevapi.azure-api.net/itz-api/photos/16",
      "rowVersion": "AAAAAAAAK/g="
    },
    "calloutDate": "2014-08-19T00:00:00",
    "percentageComplete": 0
  },
  {
    "modelId": 4,
    "dasboardItemType": 3,
    "feedbackTypeId": null,
    "title": "Salsa Rico Promotion - Cinnamon Crisps",
    "text": "This Promotion has started, please Activate the promotion by taking a picture of how you've implemented it in your retail environment.  Click here to Activate!",
    "photoId": 17,
    "photo": {
      "photoId": 17,
      "photoUrl": "https://sodexoitzdevstorage.blob.core.windows.net/promotion/4-SalsaRicoCinnamonCrisps.png",
      "container": "promotion",
      "fileName": "4-SalsaRicoCinnamonCrisps.png",
      "processedPhotoBaseUrl": "https://sodexo-itz-dev.azurewebsites.net/azure/promotion/4-SalsaRicoCinnamonCrisps.png",
      "processedPhotoThumbnailUrl": "https://sodexo-itz-dev.azurewebsites.net/azure/promotion/4-SalsaRicoCinnamonCrisps.png?w=200&h=200&mode=crop&autorotate=true",
      "url": "https://sodexodevapi.azure-api.net/itz-api/photos/17",
      "rowVersion": "AAAAAAAAK/k="
    },
    "calloutDate": "2014-08-19T00:00:00",
    "percentageComplete": 0
  },
  {
    "modelId": 2,
    "dasboardItemType": 3,
    "feedbackTypeId": null,
    "title": "Simply To Go Promotion - Big Flavor On the Go",
    "text": "Simply to Go is always evolving to address consumer preferences. This fall brings three new things which will absolutely delight your consumer.",
    "photoId": 1517,
    "photo": {
      "photoId": 1517,
      "photoUrl": "https://sodexoitzdevstorage.blob.core.windows.net/promotion/2-20141006164015-2-STG_Mindful_SMW_boxy.png",
      "container": "promotion",
      "fileName": "2-20141006164015-2-STG_Mindful_SMW_boxy.png",
      "processedPhotoBaseUrl": "https://sodexo-itz-dev.azurewebsites.net/azure/promotion/2-20141006164015-2-STG_Mindful_SMW_boxy.png",
      "processedPhotoThumbnailUrl": "https://sodexo-itz-dev.azurewebsites.net/azure/promotion/2-20141006164015-2-STG_Mindful_SMW_boxy.png?w=200&h=200&mode=crop&autorotate=true",
      "url": "https://sodexodevapi.azure-api.net/itz-api/photos/1517",
      "rowVersion": "AAAAAAAOD+Y="
    },
    "calloutDate": "2014-08-22T00:00:00",
    "percentageComplete": 0
  }
]
*/
