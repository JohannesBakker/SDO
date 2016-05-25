
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
	public class SDXAccountsFragment : SDXBaseFragment
	{
		SDXAccountsListAdapter listAdapter;
		ListView listView;
		IList<AccountModel> accounts;
		View view;

		public bool isNeededBack = false;

		#region View Lifecycle
		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			SetHeaderTitle ("Planograms", 1);
			HideAllHeaderButtons ();

			if (isNeededBack)
				AddBackBtn (1);

			if (view != null)
				return view;

			view = inflater.Inflate (Resource.Layout.Accounts, container, false);

			LayoutView (view);

			listAdapter = new SDXAccountsListAdapter (Activity);
			listAdapter.NavgiateBtn_OnClicked = NavigationBtn_OnClicked;

			listView = view.FindViewById (Resource.Id.accounts_listview) as ListView;
			listView.CacheColorHint = Color.Transparent;
			listView.DividerHeight = 0;
			listView.Adapter = listAdapter;

			return view;
		}

		public override void OnViewCreated (View view, Bundle savedInstanceState)
		{
			base.OnViewCreated (view, savedInstanceState);
            if (SDXBaseFragment.bPopFlag == false)
			    LoadAccounts ();
		}
		#endregion

		#region Button Actions
		private void NavigationBtn_OnClicked (object sender, EventArgs ea)
		{
			Button btn = sender as Button;
			int tag = int.Parse(((string)btn.Tag).Substring (0, 2));
			Console.WriteLine (tag.ToString ());

			if (tag == accounts.Count) {
				MoveToLookupAccountScreen ();
			} else {
				var fragment = new SDXAccountDetailFragment ();
				fragment.Account = accounts [tag];
				PushFragment (fragment, "SDXAccountsFragment");
			}
		}
		#endregion

		#region Private Functions
		private async void LoadAccounts ()
		{
			SDXUserManager manager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
			UserModel me = manager.Me;
//			if (me == null || me.Accounts == null) {
				Util.ShowLoading (Activity);
				me = await manager.LoadMe ();
				Util.HideLoading ();

				if (!manager.IsSuccessed) {
					Util.ShowAlert (manager.ErrorMessage, Activity);
					return;
				}
//			}
			accounts = me.Accounts;
			if (accounts.Count == 0) {
				MoveToLookupAccountScreen ();
			} else {
				listAdapter.Accounts = accounts;
				listAdapter.NotifyDataSetChanged ();
				Util.StartFadeAnimation (listView, 500);
			}
		}

		private void MoveToLookupAccountScreen ()
		{
			var fragment = new SDXLookupAccountFragment ();
			PushFragment (fragment, "SDXAccountsFragment");
		}
		#endregion
	}
}

