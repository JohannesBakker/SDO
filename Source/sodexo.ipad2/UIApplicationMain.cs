using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Sodexo.Ipad2
{
	[Register ("CustomUIApplicationMain")]
	public class CustomUIApplicationMain : UIApplication
	{
		public CustomUIApplicationMain () : base()
		{
		}

		public override UIInterfaceOrientationMask SupportedInterfaceOrientationsForWindow (UIWindow window)
		{
			if (window == null)
				return UIInterfaceOrientationMask.All;

			UIViewController topController = window.RootViewController;

			if (hasPicker(topController))
				return UIInterfaceOrientationMask.All;

			return base.SupportedInterfaceOrientationsForWindow (window);
		}

		private bool hasPicker(UIViewController controller)
		{
			bool hasPickerFlag = false;

			Type c = controller.GetType ();
			if (c.Name == "UIImagePickerController")
				return true;

			foreach (UIViewController child in controller.ChildViewControllers)
			{
				hasPickerFlag = hasPicker(child);

				if (hasPickerFlag)
					return true;
			}

			return false;
		}
	}
}

