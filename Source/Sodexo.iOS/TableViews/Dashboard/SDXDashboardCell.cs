
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;

namespace Sodexo.iOS
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
			if (item.CalloutDate != null) {
				DateTime date = (DateTime) item.CalloutDate;
				DateLB.Text = date.Month.ToString ("00") + "." + date.Day.ToString ("00");
			} else {
				DateLB.Text = "";
			}

			PictureIV.Hidden = true;
			ProgressView.Hidden = true;

			float y = 0;
			if (item.Photo != null) {
				PictureIV.Hidden = false;
				PictureIV.Frame = new RectangleF (0, 0, MainView.Frame.Width, ((float)item.PhotoId * 300 / 1000.0f));
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

				y = PictureIV.Frame.Size.Height;
			} else if (item.PercentageComplete > 0 && item.PercentageComplete < 100) {
				ProgressView.Hidden = false;
				ProgressView.Frame = new RectangleF (0, 0, MainView.Frame.Width, ProgressView.Frame.Height);
				Util.ChangeViewWith (FullProgressView, EmptyProgressIV.Frame.Width * item.PercentageComplete / 100.0f);
				ProgressValueLB.Text = item.PercentageComplete.ToString () + "%";
				Util.MoveViewToX (ProgressValueView, FullProgressView.Frame.X + FullProgressView.Frame.Width - ProgressValueView.Frame.Width / 2);

				y = ProgressView.Frame.Height;
			} else {
				y = 0;
			}

			y += 5;
			RectangleF frame = DescriptionLB.Frame;
			frame.Y = y;
			Util.MoveViewToY (DescriptionLB, y);
			DescriptionLB.Text = item.Text;
			DescriptionLB.SizeToFit ();
		}

		public float GetHeightOfCell (DashboardItemModel item)
		{
			float h = 60;
			if (item.Photo != null) {
				h += item.PhotoId != null ? (float)(item.PhotoId * 300 / 1000.0f) : 0;
			} else if (item.PercentageComplete > 0 && item.PercentageComplete < 100) {
				h += 60;
			}

			h += 5;
			DescriptionLB.Text = item.Text;
			DescriptionLB.SizeToFit ();
			h += DescriptionLB.Frame.Height + 5;

			return h + 10;
		}
	}
}



/*
 * [
  {
    "modelId": 1,
    "dasboardItemType": 2,
    "title": "Finish Account Setup",
    "text": "You're almost done setting up Account 10001008-ITHACA COLLEGE.",
    "photoId": null,
    "photo": null,
    "calloutDate": null,
    "percentageComplete": 65
  },
  {
    "modelId": 4,
    "dasboardItemType": 2,
    "title": "Finish User Profile Setup",
    "text": "Please upload a photo of yourself to your profile.",
    "photoId": null,
    "photo": null,
    "calloutDate": null,
    "percentageComplete": 70
  },
  {
    "modelId": 0,
    "dasboardItemType": 3,
    "title": "In My Kitchen Promotion",
    "text": "One of the highest-selling female artists in country music history, a New York Times best-selling cookbook author and now a Food Network Star, Trisha Yearwood has a habit of surpassing expectations",
    "photoId": 13,
    "photo": {
      "photoId": 13,
      "photoUrl": "https://sodexoitzstorage.blob.core.windows.net/promotion/1-inmykitchen-trishayearwood.png",
      "container": "promotion",
      "fileName": "1-inmykitchen-trishayearwood.png",
      "processedPhotoBaseUrl": "https://sodexo-itz.azurewebsites.net/azure/promotion/1-inmykitchen-trishayearwood.png",
      "processedPhotoThumbnailUrl": "https://sodexo-itz.azurewebsites.net/azure/promotion/1-inmykitchen-trishayearwood.png?w=200&h=200&mode=crop&autorotate=true",
      "url": "https://sodexoapi.azure-api.net/itz-api/photos/13",
      "rowVersion": "AAAAAAAA6YU="
    },
    "calloutDate": null,
    "percentageComplete": 0
  },
  {
    "modelId": 0,
    "dasboardItemType": 4,
    "title": "Planogram Update",
    "text": "Cold Beverage Planogram A1 – Two Single Door Coolers (Pepsi & Coca-Cola) has been updated.",
    "photoId": 14,
    "photo": {
      "photoId": 14,
      "photoUrl": "https://sodexoitzstorage.blob.core.windows.net/planogram/1-A1 – Two Single Door Coolers (Pepsi & Coca-Cola).png",
      "container": "planogram",
      "fileName": "1-A1 – Two Single Door Coolers (Pepsi & Coca-Cola).png",
      "processedPhotoBaseUrl": "https://sodexo-itz.azurewebsites.net/azure/planogram/1-A1 – Two Single Door Coolers (Pepsi & Coca-Cola).png",
      "processedPhotoThumbnailUrl": "https://sodexo-itz.azurewebsites.net/azure/planogram/1-A1 – Two Single Door Coolers (Pepsi & Coca-Cola).png?w=200&h=200&mode=crop&autorotate=true",
      "url": "https://sodexoapi.azure-api.net/itz-api/photos/14",
      "rowVersion": "AAAAAAAA6YY="
    },
    "calloutDate": null,
    "percentageComplete": 0
  },
  {
    "modelId": 0,
    "dasboardItemType": 5,
    "title": "Feedback Requested",
    "text": "Please provide feedback about this In The Zone - Retail Ranger App.",
    "photoId": null,
    "photo": null,
    "calloutDate": "2014-08-16T00:00:00",
    "percentageComplete": 0
  },
  {
    "modelId": 0,
    "dasboardItemType": 6,
    "title": "General Info with Calloutdate",
    "text": "This is a sample general information entry that also has a Callout Date.",
    "photoId": null,
    "photo": null,
    "calloutDate": "2014-08-16T00:00:00",
    "percentageComplete": 0
  },
]
 */