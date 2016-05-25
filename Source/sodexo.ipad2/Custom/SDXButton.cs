using System;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Sodexo.Ipad2
{
	[MonoTouch.Foundation.Register ("SDXButton")]

	public class SDXButton : UIButton
	{
		public SDXButton (IntPtr handle) : base (handle)
		{

		}

		public SDXButton () : base ()
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			if (Font.Name == Constants.HELVETICA_REGULAR) 
			{
				Font = UIFont.FromName (Constants.KARLA_REGULAR, Font.PointSize);
			}
			else if (Font.Name == Constants.HELVETICA_BOLD) 
			{
				Font = UIFont.FromName (Constants.KARLA_BOLD, Font.PointSize);
			} 
			else if (Font.Name == Constants.HELVETICA_NEUE_CONDENSED_BLACK) 
			{
				Font = UIFont.FromName (Constants.OSWALD_REGULAR, Font.PointSize);
			} 
			else if (Font.Name == Constants.HELVETICA_NEUE_CONDENSED_BOLD) 
			{
				Font = UIFont.FromName (Constants.OSWALD_LIGHT, Font.PointSize);
			}
		}
	}
}

