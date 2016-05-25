using System;
using System.Drawing;
using MonoTouch.UIKit;
using Sodexo.Core;

namespace Sodexo.Ipad2
{
	public class SDXLoadingView : UIView
	{
		private UIActivityIndicatorView loadingAIV;
		private UILabel loadingLB;

		public string Text
		{
			get { return loadingLB.ToString(); }
			set { loadingLB.Text = value; }
		}

		public SDXLoadingView( RectangleF frame) : base(frame)
		{
			this.Frame = frame;
			this.BackgroundColor = UIColor.FromWhiteAlpha(0.0f, 0.3f);

			loadingAIV = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
			loadingAIV.Center = this.Center;
			this.Add(loadingAIV);
			loadingAIV.StartAnimating();

			loadingLB = new UILabel();
			loadingLB.Frame = new RectangleF(0, loadingAIV.Frame.Y + loadingAIV.Frame.Size.Height + 10, this.Frame.Size.Width, 25.0f);
			loadingLB.TextAlignment = UITextAlignment.Center;
			loadingLB.Font = UIFont.FromName(Constants.KARLA_REGULAR, 20);
			loadingLB.TextColor = UIColor.White;
			loadingLB.BackgroundColor = UIColor.Clear;
			this.Add(loadingLB);
		}
	}
}