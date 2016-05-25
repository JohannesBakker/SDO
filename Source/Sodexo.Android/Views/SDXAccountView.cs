using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Android.Views.Animations;
using Android.Graphics;

using Sodexo.RetailActivation.Portable.Models;
using Sodexo.Core;

namespace Sodexo.Android
{
	public class SDXAccountView : Java.Lang.Object
	{
		View view;

		TextView nameTv, addressTv, idTv;
		public EventHandler EditBtnClickedEvent;

		public SDXAccountView (Activity context, View view)
		{
			this.view = view;

			GetInstance ();
		}

		public void Update (LocationModel location)
		{
			nameTv.Text = location.LocationName;
			addressTv.Text = location.LocationCity + " " + location.LocationAddress1;
			idTv.Text = location.LocationId;
		}

		private void GetInstance ()
		{
			nameTv = view.FindViewById (Resource.Id.accountview_name_tv) as TextView;
			addressTv = view.FindViewById (Resource.Id.accountview_address_tv) as TextView;
			idTv = view.FindViewById (Resource.Id.accountview_locationid_tv) as TextView;

			var editBtn = view.FindViewById (Resource.Id.accountview_edit_btn) as ImageButton;
			editBtn.Click += EditBtn_OnClicked;
		}

		private void EditBtn_OnClicked (object sender, EventArgs e)
		{
			EditBtnClickedEvent (sender, e);
		}
	}
}

