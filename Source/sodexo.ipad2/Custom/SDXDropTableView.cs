
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Sodexo.RetailActivation.Portable.Models;
using TinyIoC;
using Sodexo.Core;

namespace Sodexo.Ipad2
{
	[MonoTouch.Foundation.Register ("SDXDropTableView")]
	public class SDXDropTableView : UITableView
	{
		public EventHandler<int> SelectedEvent;

		public SDXDropTableView (IntPtr handle) : base (handle)
		{
			this.Delegate = new SDXDropTableViewDelegate ();
			this.DataSource = new SDXDropTableViewDataSource ();

			((SDXDropTableViewDelegate)this.Delegate).SelectedEvent += (s,e)=>{
				if (SelectedEvent != null)
					SelectedEvent (s, e);
			};
		}

		public void SetDropDescriptionList (List<string> list)
		{
			((SDXDropTableViewDataSource)this.DataSource).DropDescList = list;
		}

		public void SetSelectedEvent (EventHandler<int> selectedEvent)
		{

		}
	}

	public class SDXDropTableViewDelegate : UITableViewDelegate
	{
		public EventHandler<int> SelectedEvent = null;

		public SDXDropTableViewDelegate () : base ()
		{
		}

		public override float GetHeightForRow (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			return 30.0f;
		}

		public override void RowSelected (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			SelectedEvent (tableView, indexPath.Row);
		}
	}

	public class SDXDropTableViewDataSource : UITableViewDataSource
	{
		public List<string> DropDescList = null;

		public SDXDropTableViewDataSource () : base ()
		{

		}

		public override int RowsInSection (UITableView tableView, int section)
		{
			return DropDescList != null ? DropDescList.Count : 0;
		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell ("SDXDropTableViewCell");

			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Default, "SDXDropTableViewCell");
				//cell.TextLabel.Font = UIFont.FromName (Constants.OSWALD_LIGHT, 14);
				cell.TextLabel.Font = UIFont.FromName (Constants.HELVETICA_NEUE_LIGHT, 12);
			}

			cell.TextLabel.Text = DropDescList.ElementAt (indexPath.Row);

			return cell;
		}
	}


}

