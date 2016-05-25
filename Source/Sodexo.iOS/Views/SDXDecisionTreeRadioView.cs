
using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;
using TinyIoC;
using Newtonsoft.Json;

namespace Sodexo.iOS
{
	public partial class SDXDecisionTreeRadioView : UIView
	{
		public static readonly UINib Nib = UINib.FromName ("SDXDecisionTreeRadioView", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("SDXDecisionTreeRadioView");

		public EventHandler <int> RadioBtnPressedEvent;

		public SDXDecisionTreeRadioView (IntPtr handle) : base (handle)
		{
		}

		public static SDXDecisionTreeRadioView Create ()
		{
			return (SDXDecisionTreeRadioView)Nib.Instantiate (null, null) [0];
		}

		public void Update (DecisionTreeNodeModel node, int index)
		{
			float fX = node.Photo == null ? 40 : 75;
			float fW = node.Photo == null ? 250 : 220;
			DescLB.Frame = new RectangleF (fX, DescLB.Frame.Y, fW, DescLB.Frame.Height);
			DescLB.Text = node.Text;
			DescLB.SizeToFit ();

			float fH = DescLB.Frame.Height;
			fH = fH < 30 ? 30 : fH + 5;

			Util.ChangeViewHeight (DescLB, fH);

			fH += 10;
			Util.ChangeViewHeight (this, fH);

			if (node.Photo != null) {
				JHImageManager imgMgr = new JHImageManager ();
				imgMgr.LoadCompleted += (object s, byte[] bytes) => {
					InvokeOnMainThread (delegate {
						if (bytes == null)
							return;
						PictureIV.Image = Util.GetImageFromByteArray (bytes);
					});
				};
				imgMgr.LoadImageAsync (node.Photo.ProcessedPhotoBaseUrl, (int)(2 * PictureIV.Frame.Width), (int)(2 * PictureIV.Frame.Height));
			} else
				PictureIV.Hidden = true;

			RadioIV.Tag = 1000;

			Button.Tag = index + 1;
		}

		partial void OnRadioBtn_Pressed (MonoTouch.Foundation.NSObject sender)
		{
			RadioBtnPressedEvent (sender, ((UIButton)sender).Tag);
		}
	}
}

