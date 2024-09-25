using BluetoothKeepAlive.WinTrayService.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using TimerEx = System.Timers.Timer;

namespace BluetoothKeepAlive.WinTrayService
{
	public class BluetoothKeepAliveWorkerService
	{

		private readonly BluetoothKeepAliveExecutor mExecutor;

		private readonly BluetoothKeepAliveOptions mOptions;

		private readonly ILogger<BluetoothKeepAliveWorkerService> mLogger;

		private TimerEx mTimer;

		public BluetoothKeepAliveWorkerService( BluetoothKeepAliveExecutor executor,
			IOptions<BluetoothKeepAliveOptions> options,
			ILogger<BluetoothKeepAliveWorkerService> logger )
		{
			mExecutor = executor;
			mOptions = options?.Value;
			mLogger = logger;
		}

		public void Start( CancellationToken cancellationToken )
		{
			
			try
			{
				if (cancellationToken.IsCancellationRequested)
					return;

				StartKeepAliveTimer();
			}
			catch (Exception exc)
			{
				mLogger.LogError( exc, "Error running application" );
				Application.Exit();
			}
		}

		private void StartKeepAliveTimer()
		{
			if (mTimer != null)
				return;

			mTimer = new TimerEx();
			mTimer.Interval = mOptions.IntervalSeconds * 1000;
			mTimer.Elapsed += KeepAliveTick;
			mTimer.Start();
		}

		private void KeepAliveTick( object sender, ElapsedEventArgs e )
		{
			if (mTimer == null || !mTimer.Enabled)
				return;

			RunOnce();
		}

		private void RunOnce()
		{
			mExecutor.KeepAlive();
		}

		public void Stop()
		{
			StopKeepAliveTimer();
			Application.Exit();
		}

		private void StopKeepAliveTimer()
		{
			if (mTimer == null)
				return;

			mTimer.Elapsed -= KeepAliveTick;
			mTimer.Stop();
			mTimer = null;
		}
	}
}
