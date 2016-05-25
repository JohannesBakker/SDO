using System;
using MonoTouch.UIKit;

namespace Sodexo.iOS
{
	[MonoTouch.Foundation.Register ("EmptySegue")]
	public class EmptySegue : UIStoryboardSegue
	{
		public EmptySegue (IntPtr param) : base (param)
		{

		}

		public override void Perform ()
		{

		}
	}
}

