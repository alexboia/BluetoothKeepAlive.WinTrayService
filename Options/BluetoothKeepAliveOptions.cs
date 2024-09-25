using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothKeepAlive.WinTrayService.Options
{
	public class BluetoothKeepAliveOptions
	{
		public List<string> MatchDeviceNames
		{
			get; set;
		}

		public bool PlayWhenActiveSessions
		{
			get; set;
		}

		public int IntervalSeconds
		{
			get; set;
		}

		public SamplePlaybackOptions SamplePlayback
		{
			get; set;
		}
	}
}
