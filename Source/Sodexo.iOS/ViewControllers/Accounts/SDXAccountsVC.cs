
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;

using TinyIoC;
using Newtonsoft.Json;

namespace Sodexo.iOS
{
	public partial class SDXAccountsVC : SDXBaseVC
	{
		public bool IsNeedReload = false;
		private IList<AccountModel> _accounts;

		public SDXAccountsVC (IntPtr handle) : base (handle)
		{

		}

		#region View Lifecycle
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			((AppDelegate)UIApplication.SharedApplication.Delegate).AccountsVC = this;

//			UIBarButtonItem infoMenuItem = new UIBarButtonItem (UIImage.FromBundle("icon_info.png").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), 
//				UIBarButtonItemStyle.Plain, (s, e)=>{ OnInfoMenuBtn_Pressed(s, e); });
//			NavigationItem.RightBarButtonItem = infoMenuItem;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			SetHeaderBackground (1);

			TableView.DataSource = new SDXAccountTableDataSource (this);
			TableView.Delegate = new SDXAccountTableDelegate (this);

			((SDXAccountTableDelegate)TableView.Delegate).Accounts = _accounts;
			((SDXAccountTableDataSource)TableView.DataSource).Accounts = _accounts;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (_accounts == null || _accounts.Count == 0 || IsNeedReload)
				LoadAccounts ();
			else
				TableView.ReloadData ();
		}

		#endregion

		#region Private Functions
		async private void LoadAccounts()
		{
			var authManager = TinyIoCContainer.Current.Resolve <SDXAuthManager> ();
			if (!authManager.IsAuthenticated)
				return;

			var userManager = TinyIoCContainer.Current.Resolve <SDXUserManager> ();
			IsNeedReload = true; //force refresh always
			if (userManager.Me == null || IsNeedReload || userManager.Me.Accounts == null) {
				IsNeedReload = false;

				ShowLoading ("Loading...");
				await userManager.LoadMe ();
				HideLoading ();

				if (!userManager.IsSuccessed) {
					ShowErrorMessage (userManager.ErrorMessage);
					return;
				}
			}

			_accounts  = userManager.Me.Accounts;

			if (_accounts.Count == 0) {
				PerformSegue ("SegueToLookupAccount", this);
			} else {
				((SDXAccountTableDelegate)TableView.Delegate).Accounts = _accounts;
				((SDXAccountTableDataSource)TableView.DataSource).Accounts = _accounts;
				TableView.ReloadData ();
			}
		}
		#endregion

		#region Button Actions
		private void OnInfoMenuBtn_Pressed (object sender, EventArgs ea)
		{
			System.Console.WriteLine ("Accounts: OnInfoMenuBtn_Pressed");
		}

		private int _selectedAccountIndex = -1;
		partial void OnCellNavigateBtn_Pressed (UIButton sender)
		{
			UIButton btn = (UIButton)sender;
			System.Console.WriteLine (btn.Tag.ToString() + " is Pressed");

			if (btn.Tag == Constants.AccountCellNavigateBtnTagBase + _accounts.Count) {
				PerformSegue ("SegueToLookupAccount", this);
			} else {
				_selectedAccountIndex = btn.Tag - Constants.AccountCellNavigateBtnTagBase;
				PerformSegue ("SegueToAccountDetail", this);
			}
		}
		#endregion

		#region Segue
		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			if (segue.Identifier == "SegueToAccountDetail") {
				var vc = segue.DestinationViewController as SDXAccountDetailVC;
				vc.Account = _accounts.ElementAt (_selectedAccountIndex);
			}
		}
		#endregion
	}
}

