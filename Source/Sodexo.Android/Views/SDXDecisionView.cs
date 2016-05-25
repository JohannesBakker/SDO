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

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;

namespace Sodexo.Android
{
	public class SDXDecisionView
	{
		public View view;
		private Activity context;

		public EventHandler <int> RadioViewClickedEvent;

		TextView questionTv;
		LinearLayout answersView;

		List<SDXDecisionRadioView> radioViews = new List<SDXDecisionRadioView> ();

		public static SDXDecisionView Create (Activity context, ViewGroup parent)
		{
			View view = context.LayoutInflater.Inflate (Resource.Layout.DecisionView, parent, false);

			SDXDecisionView self = new SDXDecisionView (context, view);

			return self;
		}

		public SDXDecisionView (Activity context, View view)
		{
			this.view = view;
			this.context = context;

			GetInstance ();
		}

		private void GetInstance ()
		{
			questionTv = view.FindViewById (Resource.Id.decision_question_tv) as TextView;
			answersView = view.FindViewById (Resource.Id.decision_answers_ll) as LinearLayout;
		}

		public void Update (DecisionTreeNodeModel node)
		{
			questionTv.Text = node.Text;

			if (node.ChildDisplayModeId == 1) {
				CreateDecisionRadioView (node.ChildNodes);
			} else {
				CreateDecisionDropListView (node.ChildNodes);
			}
		}

		private void CreateDecisionRadioView (IList<DecisionTreeNodeModel> answerNodes)
		{
			if (answerNodes.Count == 0)
				return;

			for (int i = 0; i < answerNodes.Count; i++) {
				var node = answerNodes [i];

				var nodeV = SDXDecisionRadioView.Create (context, (ViewGroup)view);
				nodeV.Update (node, i);
				nodeV.view.Tag = i.ToString ("00") + "-" + nodeV.view.Tag;
				nodeV.view.Click += RadioView_OnClicked;

				answersView.AddView (nodeV.view);
				radioViews.Add (nodeV);
			}
		}

		private void CreateDecisionDropListView (IList<DecisionTreeNodeModel> answerNodes)
		{
//			UIView view = new UIView ();
//			view.BackgroundColor = UIColor.Clear;
//			view.Frame = new RectangleF (0, 0, 300, 30);
//
//			UITextField tf = new UITextField ();
//			tf.Frame = new RectangleF (5, 0, 290, 30);
//			tf.Text = "";
//			tf.TextColor = UIColor.DarkGray;
//			tf.Font = UIFont.FromName (Constants.OSWALD_LIGHT, 14);
//			tf.BackgroundColor = UIColor.Clear;
//			view.AddSubview (tf);
//			answerTF = tf;
//
//			UIImageView iv = new UIImageView ();
//			iv.Frame = new RectangleF (280, 13, 6, 4);
//			iv.Image = UIImage.FromBundle ("icon_bottom_arrow.png");
//			iv.ContentMode = UIViewContentMode.ScaleAspectFit;
//			iv.BackgroundColor = UIColor.Clear;
//			view.AddSubview (iv);
//
//			UIButton btn = new UIButton (UIButtonType.Custom);
//			btn.Frame = tf.Frame;
//			btn.SetTitle ("", UIControlState.Normal);
//			btn.TouchUpInside += OnDropBtn_Pressed;
//			view.AddSubview (btn);
//
//			List<string> dropAnswerList = new List<string> ();
//			foreach (DecisionTreeNodeModel item in answerNodes) {
//				dropAnswerList.Add (item.Text);
//			}
//			DropTableView.SetDropDescriptionList (dropAnswerList);
//			DropTableView.ReloadData ();
//			DropTableView.ScrollToRow(NSIndexPath.FromRowSection(0,0), UITableViewScrollPosition.Top, false);
//			float fHeight = dropAnswerList.Count >= 4 ? 100.0f : dropAnswerList.Count * 30.0f;
//			Util.ChangeViewHeight (DropTableView, fHeight);
//
//			return view;
		}

		private void RadioView_OnClicked (object sender, EventArgs arg)
		{
			int tag = int.Parse (((string)(((View)sender).Tag)).Substring (0, 2));
			radioViews [tag].Select (true);

			for (int i = 0; i < radioViews.Count; i++) {
				if (i != tag) {
					radioViews [i].Select (false);
				}
			}

			RadioViewClickedEvent (sender, tag);
		}
	}
}

