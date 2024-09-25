using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace BluetoothKeepAlive.WinTrayService
{
	class ProcessIcon : IDisposable
	{
		private NotifyIcon mNotificationIcon;

		public event EventHandler OnExitRequested;

		public ProcessIcon()
		{
			mNotificationIcon = new NotifyIcon();
		}

		public void Display()
		{
			mNotificationIcon.MouseClick += new MouseEventHandler(NotificationIcon_MouseClick);
			mNotificationIcon.Icon = Resources.BTTray;
			mNotificationIcon.Text = "BT Keep Alive Utility";
			mNotificationIcon.Visible = true;

			ContextMenuOptions contextMenu = new ContextMenuOptions();
			contextMenu.OnExitRequested += ContextMenu_OnExitRequested;
			mNotificationIcon.ContextMenuStrip = contextMenu.Create();
		}

		private void ContextMenu_OnExitRequested( object sender, EventArgs e )
		{
			OnExitRequested?.Invoke(this, EventArgs.Empty);
		}

		private void NotificationIcon_MouseClick(object sender, MouseEventArgs e)
		{
			return;
		}

		public void Dispose()
		{
			mNotificationIcon.Dispose();
		}
	}
}