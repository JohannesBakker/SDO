
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Ipad2
{
	public partial class SDXAccountItemView : UIView
	{
		public static readonly UINib Nib = UINib.FromName ("SDXAccountItemView", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("Account Item");

		public EventHandler <int> AccountBtnPressedEvent;
		public EventHandler <int> AccountEditBtnPressedEvent;

		public SDXAccountItemView (IntPtr handle) : base (handle)
		{
		}

		public static SDXAccountItemView Create ()
		{
			return (SDXAccountItemView)Nib.Instantiate (null, null) [0];
		}

		public void Update (AccountModel node, int index)
		{
			lblNumber.Text = (index + 1).ToString ("00");
			lblLocation.Text = node.Location.LocationName;
			lblAddress.Text = node.Location.LocationCity + " " + node.Location.LocationAddress1;
			lblAccouontNumber.Text = node.LocationId.ToString ();

			BtnEdit.Tag = index;
			BtnSelect.Tag = index;
			//BtnEdit.Enabled = false;
			//BtnSelect.Enabled = false;
		}

		public void SetFocus(bool bFocus)
		{
			if (bFocus) {
				//BackgroundColor = UIColor.DarkGray;
				viewRoot.BackgroundColor = UIColor.DarkGray;
				lblNumber.TextColor = UIColor.White;
				lblAddress.TextColor = UIColor.White;
				lblAccouontNumber.TextColor = UIColor.White;
				lblLocation.TextColor = UIColor.White;
				BtnEditImage.Image = Util.GetColoredImage ("btn_edit.png", UIColor.White);
			} else {
				//BackgroundColor = UIColor.White;
				viewRoot.BackgroundColor = UIColor.White;
				lblNumber.TextColor = UIColor.Black;
				lblAddress.TextColor = UIColor.Black;
				lblAccouontNumber.TextColor = UIColor.Black;
				lblLocation.TextColor = UIColor.Black;
				BtnEditImage.Image = UIImage.FromBundle ("btn_edit.png");

			}
		}

		partial void OnAccountBtn_Pressed (UIButton sender)
		{
			AccountBtnPressedEvent (sender, ((UIButton)sender).Tag);
		}

		partial void OnAccountEditBtnPressed (UIButton sender)
		{
			AccountEditBtnPressedEvent (sender, ((UIButton)sender).Tag);
		}
	}
}

