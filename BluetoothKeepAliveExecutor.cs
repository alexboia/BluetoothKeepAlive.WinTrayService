using BluetoothKeepAlive.WinTrayService.Helpers;
using BluetoothKeepAlive.WinTrayService.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace BluetoothKeepAlive
{
	public class BluetoothKeepAliveExecutor
	{
		private readonly ILogger<BluetoothKeepAliveExecutor> mLogger;

		private readonly BluetoothKeepAliveOptions mOptions;

		public BluetoothKeepAliveExecutor( IOptions<BluetoothKeepAliveOptions> options,
			ILogger<BluetoothKeepAliveExecutor> logger )
		{
			mOptions = options?.Value ?? throw new ArgumentNullException( nameof( options ) );
			mLogger = logger ?? throw new ArgumentNullException( nameof( logger ) );
		}

		public void KeepAlive()
		{
			try
			{
				DoKeepAlive();
			}
			catch (Exception exc)
			{
				mLogger.LogError( exc, "Error executing keep alive routine." );
			}
		}

		private void DoKeepAlive()
		{
			using (MMDeviceEnumerator enumerator = new MMDeviceEnumerator())
			{
				foreach (MMDevice wasapi in enumerator.EnumerateKeepAliveEligibleAudioEndPoints())
				{
					if (ShouldKeepAlive( wasapi ))
					{
						mLogger.LogDebug( $"Processing keep alive for {wasapi.FriendlyName} {wasapi.DeviceFriendlyName} {wasapi.State}." );
						bool hasActiveSession = HasActiveSession( wasapi );
						if (!hasActiveSession || mOptions.PlayWhenActiveSessions)
							PlayKeepaliveStream( wasapi, hasActiveSession );
						else
							mLogger.LogDebug( "Device has active sessions. Will not play keep alive stream." );
					}
				}
			}
		}

		private bool ShouldKeepAlive( MMDevice wasapi )
		{
			if (mOptions.MatchDeviceNames != null
				&& mOptions.MatchDeviceNames.Count > 0)
			{
				foreach (string deviceNameRegexSrc in mOptions.MatchDeviceNames)
				{
					Regex regex = new Regex( deviceNameRegexSrc,
						RegexOptions.IgnoreCase );

					if (regex.IsMatch( wasapi.DeviceFriendlyName ))
						return true;
				}
			}

			return wasapi.DeviceFriendlyName.Contains( "ACTON",
				StringComparison.InvariantCultureIgnoreCase );
		}

		private bool HasActiveSession( MMDevice wasapi )
		{
			wasapi.AudioSessionManager.RefreshSessions();
			SessionCollection sessions = wasapi.AudioSessionManager.Sessions;
			mLogger.LogDebug( $"Found {sessions.Count} sessions." );

			if (sessions.Count == 0)
				return false;

			for (int i = 0; i < sessions.Count; i++)
			{
				AudioSessionControl sess = sessions [ i ];
				mLogger.LogDebug( $"Session: {sess.DisplayName} is {sess.State} ." );

				if (sess.State == AudioSessionState.AudioSessionStateActive)
					return true;
			}

			return false;
		}

		private void PlayKeepaliveStream( MMDevice wasapi, bool hasActiveSession )
		{
			string filePath = PlaybackSampleFile();
			float volume = PlaybackVolume();

			float originalVolume = wasapi.AudioEndpointVolume
				.MasterVolumeLevelScalar;

			mLogger.LogDebug( $"Will play keep alive stream: {filePath}, volume = {volume}." );

			using (AudioFileReader audioFile = new AudioFileReader( PlaybackSampleFile() ))
			using (WasapiOut outputDevice = new WasapiOut( wasapi, AudioClientShareMode.Shared, true, 100 ))
			using (ManualResetEvent manualResetEvent = new ManualResetEvent( false ))
			{
				outputDevice.PlaybackStopped += ( sender, args ) => manualResetEvent.Set();
				outputDevice.Init( audioFile );
				if (!hasActiveSession)
					outputDevice.Volume = volume;
				outputDevice.Play();

				manualResetEvent.WaitOne();
			}

			if (!hasActiveSession)
				wasapi.AudioEndpointVolume.MasterVolumeLevelScalar =
					originalVolume;

			mLogger.LogDebug( $"Volume restored to: {originalVolume}." );
		}

		private string PlaybackSampleFile()
		{
			string fileName = mOptions.SamplePlayback.FileName;
			return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "AudioSamples", fileName );
		}

		private float PlaybackVolume()
		{
			return mOptions.SamplePlayback.Volume > 0
				? mOptions.SamplePlayback.Volume
				: 0.01f;
		}
	}
}
