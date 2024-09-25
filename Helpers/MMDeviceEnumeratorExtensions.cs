using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothKeepAlive.WinTrayService.Helpers
{
    public static class MMDeviceEnumeratorExtensions
    {
        public static MMDeviceCollection EnumerateKeepAliveEligibleAudioEndPoints(this MMDeviceEnumerator enumerator)
        {
            return enumerator.EnumerateAudioEndPoints(DataFlow.Render,
                DeviceState.Active);
        }
    }
}
