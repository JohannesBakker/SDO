
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
using Android.Graphics;
using Android.Views.Animations;

using TinyIoC;
using Sodexo.Core;
using Sodexo.RetailActivation.Portable.Models;

namespace Sodexo.Android
{
	public class SDXFeedbackFragment : SDXBaseFragment
	{

		FeedbackTypeModel selectedFeedbackType;

		View view;

		#region View Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			HideAllHeaderButtons ();
			SetHeaderTitle ("Feedback", 3);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.Feedback, container, false);
			LayoutView (view);
			GetInstance (view);

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
		}
		#endregion

		#region Private Functions
		private void GetInstance (View view)
		{
			for (int i = 1; i <= 6; i++) {
				var tag = i.ToString ("00") + "-sw-sh-sl-st";
				var iv = view.FindViewWithTag (tag);
				iv.Click += SubjectBtn_OnClicked;
			}
		}

		private async void SubjectBtn_OnClicked (object sender, EventArgs e)
		{
			var manager = TinyIoCContainer.Current.Resolve <SDXReferenceDataManager> ();
			if (manager.DataModel == null) {
				await LoadReferenceData ();
			}

			var iv = sender as ImageView;
			var tag = int.Parse (((string)iv.Tag).Substring (0, 2));

			if (manager.DataModel.FeedbackTypes.Count < tag)
				return;

			selectedFeedbackType = manager.DataModel.FeedbackTypes [tag - 1];

			if (tag == 1)
				MoveToPlanogramScreen ();
			else if (tag == 2)
				MoveToPromotionScreen ();
			else {
				MoveToLeaveFeedbackScreen ();
			}
		}

		private void MoveToPlanogramScreen ()
		{
			var fragment = new SDXPlanogramsFragment ();
			fragment.FeedbackType = selectedFeedbackType;
			PushFragment (fragment, "SDXFeedbackFragment");
		}

		private void MoveToPromotionScreen ()
		{
			var fragment = new SDXPromotionsFragment ();
			fragment.FeedbackType = selectedFeedbackType;
			fragment.IsSelecting = true;
			PushFragment (fragment, "SDXFeedbackFragment");
		}

		private void MoveToLeaveFeedbackScreen ()
		{
			var fragment = new LeaveFeedbackFragment ();
			fragment.FeedbackType = selectedFeedbackType;
			PushFragment (fragment, "SDXFeedbackFragment");
		}
		#endregion
	}
}

