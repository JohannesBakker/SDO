
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Sodexo.Android
{
	public class SlidingLinearLayout : LinearLayout
	{
		public SlidingLinearLayout (Context context) :
			base (context)
		{
			Initialize ();
		}

		public SlidingLinearLayout (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public SlidingLinearLayout (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}

		public float getXFraction()
		{
			int width = this.Width;
			return (width == 0) ? 0 : GetX () / (float) width;
		}

		public void setXFraction(float xFraction) {
			int width = this.Width;
			SetX ((width > 0) ? (xFraction * width) : 0);
		}
	}
}

