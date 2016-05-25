using System;

using MonoTouch.UIKit;

namespace Sodexo.iOS
{
	[MonoTouch.Foundation.Register ("SDXScrollView")]

	public class SDXScrollView : UIScrollView
	{
		public SDXScrollView (IntPtr handle) : base (handle)
		{
		}

		public override void ScrollRectToVisible (System.Drawing.RectangleF rect, bool animated)
		{
			rect.Y -= 30;
			rect.Height += 45 + 30;

			base.ScrollRectToVisible (rect, animated);
		}
	}
}

