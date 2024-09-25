using System;
using System.Diagnostics;
using System.IO;
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

			item = new ToolStripMenuItem();
			item.Text = "About";
			item.Click += new EventHandler( About_Click );
			item.Image = Resources.About;
			menu.Items.Add( item );

			item = new ToolStripMenuItem();
			item.Text = "View logs";
			item.Click += new EventHandler( ViewLogs_Click );
			item.Image = Resources.Explorer;
			menu.Items.Add( item );

			sep = new ToolStripSeparator();
			menu.Items.Add( sep );

			item = new ToolStripMenuItem();
			item.Text = "Exit";
			item.Click += new System.EventHandler( Exit_Click );
			item.Image = Resources.Exit;
			menu.Items.Add( item );

			return menu;
		}

		protected void ViewLogs_Click( object sender, EventArgs e )
		{
			string logsDir = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Logs" );
			Process.Start( "explorer.exe", logsDir );
		}

		protected void About_Click( object sender, EventArgs e )
		{
			ProcessStartInfo sInfo = new ProcessStartInfo()
			{
				FileName = "https://github.com/alexboia/BluetoothKeepAlive.WinTrayService",
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