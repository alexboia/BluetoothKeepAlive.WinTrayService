using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace BluetoothKeepAlive.WinTrayService.Options
{
	public class BluetoothKeepAliveOptionsConfiguration : IConfigureOptions<BluetoothKeepAliveOptions>
	{
		private const string BaseKeyName = "BluetoothKeepAlive";

		private readonly IConfiguration mConfiguration;

		public BluetoothKeepAliveOptionsConfiguration( IConfiguration configuration )
		{
			mConfiguration = configuration 
				?? throw new ArgumentNullException( nameof( configuration ) );
		}

		public void Configure( BluetoothKeepAliveOptions options )
		{
			options.IntervalSeconds = mConfiguration
				.GetValue<int>( $"{BaseKeyName}:IntervalSeconds" );
			options.MatchDeviceNames = mConfiguration
				.GetSection( $"{BaseKeyName}:MatchDeviceNames" )
				.Get<List<string>>();
			options.PlayWhenActiveSessions = mConfiguration
				.GetValue<bool>( $"{BaseKeyName}:PlayWhenActiveSessions" );

			options.SamplePlayback = new SamplePlaybackOptions();
			options.SamplePlayback.Volume = mConfiguration
				.GetValue<float>( $"{BaseKeyName}:SamplePlayback:Volume" );
			options.SamplePlayback.FileName = mConfiguration
				.GetValue<string>( $"{BaseKeyName}:SamplePlayback:FileName" );
		}
	}
}
