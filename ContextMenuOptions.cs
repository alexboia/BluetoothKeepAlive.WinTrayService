using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace BluetoothKeepAlive.WinTrayService
{
	public class ContextMenuOptions
	{

		public event EventHandler OnExitRequested;

		public ContextMenuStrip Create()
		{
			// Add the default menu options.
			ContextMenuStrip menu = new ContextMenuStrip();
			ToolStripMenuItem item;
			ToolStripSeparator sep;

			// About.
			item = new ToolStripMenuItem();
			item.Text = "About";
			item.Click += new EventHandler( About_Click );
			item.Image = Resources.About;
			menu.Items.Add( item );

			// Separator.
			sep = new ToolStripSeparator();
			menu.Items.Add( sep );

			// Exit.
			item = new ToolStripMenuItem();
			item.Text = "Exit";
			item.Click += new System.EventHandler( Exit_Click );
			item.Image = Resources.Exit;
			menu.Items.Add( item );

			return menu;
		}

		protected void About_Click( object sender, EventArgs e )
		{
			ProcessStartInfo sInfo = new ProcessStartInfo()
			{
				FileName = "https://alexboia.net/",
				UseShellExecute = true
			};
			Process.Start( sInfo );
		}

		protected void Exit_Click( object sender, EventArgs e )
		{
			OnExitRequested?.Invoke( this, EventArgs.Empty );
		}
	}
}