using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace Sodexo.Android
{
	public static class LayoutAdjuster
	{
		public static void FitToScreen(Context context, ViewGroup topLayout, float xRate,
			float yRate, float wRate, float hRate, float density) 
		{
			if (topLayout.GetType () == typeof (ListView))
				return;

			for (int i = 0; i < topLayout.ChildCount; i++) {
				if ((topLayout.GetChildAt(i).GetType ().BaseType == typeof (ViewGroup)) ||
					(topLayout.GetChildAt(i).GetType ().BaseType == typeof (FrameLayout))) {
					if (((ViewGroup) topLayout.GetChildAt(i)).ChildCount > 0) {
						FitToScreen(context, (ViewGroup) topLayout.GetChildAt(i),
							xRate, yRate, wRate, hRate, density);
					}
				}

				View layout = topLayout.GetChildAt(i);

				if (layout.Tag == null || layout.Tag.ToString () == "")
					continue;

				String tag = (String) layout.Tag;

				processItem (layout, tag, xRate, yRate, wRate, hRate, density);
			}
		}

		static void processItem(View view, String tag, float xRate, float yRate, float wRate, 
																		float hRate, float density) 
		{
			IViewParent parentLayout = view.Parent;

			if (parentLayout.GetType () == typeof (LinearLayout))
			{
				var orgParam = (LinearLayout.LayoutParams) view.LayoutParameters;
				int widthParam = GetWidthParam (view, tag, wRate);
				int heightParam = GetHeightParam (view, tag, hRate);
				var param = new LinearLayout.LayoutParams (widthParam, heightParam, orgParam.Weight);
				param.Gravity = GetGravity (tag, orgParam.Gravity);
				SetMargins (orgParam, param, tag, xRate, yRate);
				view.LayoutParameters = param;
			}
			else if (parentLayout.GetType () == typeof (FrameLayout))
			{
				var orgParam = (FrameLayout.LayoutParams) view.LayoutParameters;
				int widthParam = GetWidthParam (view, tag, wRate);
				int heightParam = GetHeightParam (view, tag, hRate);
				var gravity = GetGravity (tag, orgParam.Gravity);
				var param = new FrameLayout.LayoutParams (widthParam, heightParam, gravity);
				SetMargins (orgParam, param, tag, xRate, yRate);
				view.LayoutParameters = param;
			}

			SetPaddings (view, tag, xRate, yRate);
			SetFont (view, tag, hRate, density);
		}

		static int GetWidthParam (View view, String tag, float wRate)
		{
			int widthParam = view.Width;
			if (tag.Contains("fw"))
				widthParam = LinearLayout.LayoutParams.MatchParent;
			else if (tag.Contains("ww"))
				widthParam = LinearLayout.LayoutParams.WrapContent;
			else if (tag.Contains("sw"))
				widthParam = (int) (view.Width * wRate);
			return widthParam;
		}

		static int GetHeightParam (View view, String tag, float hRate)
		{
			int heightParam = view.Height;
			if (tag.Contains("fh"))
				heightParam = LinearLayout.LayoutParams.MatchParent;
			else if (tag.Contains("wh"))
				heightParam = LinearLayout.LayoutParams.WrapContent;
			else if (tag.Contains("sh"))
				heightParam = (int) (view.Height * hRate);
			return heightParam;
		}

		static GravityFlags GetGravity (String tag, GravityFlags org)
		{
			GravityFlags flags;
			if (tag.Contains ("glt"))
				flags = GravityFlags.Left | GravityFlags.Top;
			else if (tag.Contains ("glb"))
				flags = GravityFlags.Left | GravityFlags.Bottom;
			else if (tag.Contains ("glcv"))
				flags = GravityFlags.Left | GravityFlags.CenterVertical;
			else if (tag.Contains ("grt"))
				flags = GravityFlags.Right | GravityFlags.Top;
			else if (tag.Contains ("grb"))
				flags = GravityFlags.Right | GravityFlags.Bottom;
			else if (tag.Contains ("grcv"))
				flags = GravityFlags.Right | GravityFlags.CenterVertical;
			else if (tag.Contains ("gr"))
				flags = GravityFlags.Right;
			else if (tag.Contains ("gbch"))
				flags = GravityFlags.Bottom | GravityFlags.CenterHorizontal;
			else if (tag.Contains ("gb"))
				flags = GravityFlags.Bottom;
			else if (tag.Contains ("gch"))
				flags = GravityFlags.CenterHorizontal;
			else if (tag.Contains ("gcv"))
				flags = GravityFlags.CenterVertical;
			else if (tag.Contains ("gc"))
				flags = GravityFlags.Center;
			else
				flags = org;
			return flags;
		}

		static void SetMargins (ViewGroup.MarginLayoutParams orgParam, ViewGroup.MarginLayoutParams param, 
																		String tag, float xRate, float yRate)
		{
			int marginLeft = orgParam.LeftMargin;
			int marginRight = orgParam.RightMargin;
			int marginTop = orgParam.TopMargin;
			int marginBottom = orgParam.BottomMargin;

			if (tag.Contains("sl"))
				marginLeft = (int) (marginLeft * xRate);
			if (tag.Contains("sr"))
				marginRight = (int) (marginRight * xRate);
			if (tag.Contains("st"))
				marginTop = (int) (marginTop * yRate);
			if (tag.Contains("sb"))
				marginBottom = (int) (marginBottom * yRate);

			param.SetMargins(marginLeft, marginTop, marginRight, marginBottom);
		}

		static void SetPaddings (View view, String tag, float xRate, float yRate)
		{
			int paddingLeft = view.PaddingLeft, paddingRight = view.PaddingRight;
			int paddingTop = view.PaddingTop, paddingBottom = view.PaddingBottom;

			if (tag.Contains("spl"))
				paddingLeft = (int) (paddingLeft * xRate);
			if (tag.Contains("spr"))
				paddingRight = (int) (paddingRight * xRate);
			if (tag.Contains("spt"))
				paddingTop = (int) (paddingTop * yRate);
			if (tag.Contains("spb"))
				paddingBottom = (int) (paddingBottom * yRate);

			view.SetPadding (paddingLeft, paddingTop, paddingRight, paddingBottom);
		}

		private static void SetFont (View view, String tag, float hRate, float density)
		{
			if (tag.Contains ("sfont") && (view.GetType () == typeof (TextView) 
				|| view.GetType () == typeof (ImageButton) || view.GetType () == typeof (Button)
				|| view.GetType () == typeof (EditText))) 
			{
				var textView = (TextView) view;
				textView.SetTextSize (global::Android.Util.ComplexUnitType.Px, (int) (textView.TextSize * hRate));
				if (tag.Contains ("hnlt")) {
					textView.SetTypeface (Fonts.HelveticaNenueLTStd_Lt, TypefaceStyle.Normal);
				} else if (tag.Contains ("kb")) {
					textView.SetTypeface (Fonts.Karla_Bold, TypefaceStyle.Normal);
				} else if (tag.Contains ("kr")) {
					textView.SetTypeface (Fonts.Karla_Regular, TypefaceStyle.Normal);
				} else if (tag.Contains ("ol")) {
					textView.SetTypeface (Fonts.Oswald_Light, TypefaceStyle.Normal);
				} else if (tag.Contains ("or")) {
					textView.SetTypeface (Fonts.Oswald_Regular, TypefaceStyle.Normal);
				}
			}
		}
	}
}

