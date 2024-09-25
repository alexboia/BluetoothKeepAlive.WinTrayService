# Bluetooth Speaker Keepalive Utility

The core problem that this small utiltiy Windows Service is meant to solve is to keep a bluetooth speaker from entering auto-standby mode.

My problem was the same as the one described over [here](https://my.marshall.com/forum/question/2961/how-do-i-disable-acton-2%E2%80%99s-auto-standby-mode) (and driving me just as crazy): my ACTON III is entering stand-by mode automatically and, from what I understand, there's nothing that can be officially done about it:

```
European Union Regulation states that electronic devices may only have 0.5w power consumption in Standby Mode (the European Directive on energy related products, or ERP).
This is why the Woburn’s factory default setting is the Powersave Mode, because the Powersave mode saves more energy than the standard mode.
```

Now... that's too bad, because I really like my ACTON III and I hate having to manually connect it everytime. 

So I built this small program that wakes up every couple of [configured] seconds, sees if the devices are active and, if they are, it plays a short sound with a very small volume setting.

## Using the thing

1. Build the thing in release mode using Visual Studio and copy the output to wherever you want.
2. Modify the configuration file per your hidden desires.
3. Double click to start the application - an icon *should* appear in the system tray icon area.
4. Right click that icon to see available options.

If you run in debug mode (either in Visual Studio or standalone), it will only run once as vanilla console application.

## Configuring the thing

Configuration file is `appsettings.json`. Available options are:

- `BluetoothKeepAlive.IntervalSeconds`: how often should it wake up and play the sample. `Integer`. Default = `10` seconds.
- `BluetoothKeepAlive.MatchDeviceNames`: a list of regular expressions that should be matched against device names. `String[]`. Default = `[ "ACTON(\\s*)III" ]`.
- `BluetoothKeepAlive.PlayWhenActiveSessions`: whether or not to play the sample when sessions are active; if set to `true` the volume will not be modified. `Boolean`. Default = `false`.
- `BluetoothKeepAlive.SamplePlayback.Volume`: volume to use when playing the sample, between `0` and `1`. `Float`. Default = `0.01`.
- `BluetoothKeepAlive.SamplePlayback.FileName`: sample file to use; must be placed in the `AudioSamples` sub-folder. `String`. Default = `sound-1-167181.mp3`.

That's it and nothing more.

## Why the approach

I took this approach because:

- the thing had to be non-intrusive: if there's an existing program streaming audio to the speaker , then I don't want anything streamed to it;
- if when the operating system starts the speaker is not connected, I don't want it automatically connected;
- if I manually disconnect the speaker, I don't want it automatically re-connected;
- if, on the other hand, I do connect the speaker sometimes along the way, I do want it kept alive.

So, in short, it attempts to keep the status quo as it finds it when it wakes up.

As for why this is a WinForms application and not a Windows Service... it's because it could not detect the audio sessions currently using the target speaker. It seems enumerating audio sessions is only avaialble for desktop apps ([see here](https://learn.microsoft.com/en-us/windows/win32/api/audiopolicy/nn-audiopolicy-iaudiosessionenumerator)).
So I used [this example](https://www.codeproject.com/Articles/290013/Formless-System-Tray-Application) to create a desktop app container for this service.

## Why the name

Well, initially I attempted to work with it as a bluetooth device using the `32feet.net` library, but:

- I felt I had to jump through a lot of hoops to make it happen;
- I realised I didn't actually need to work at that level, but only feed it something to keep it active.

So I was left with only the audio code, but, since its purpose is what it is, the name stuck. 

Also lazyness.

## Audio samples

Courtesy of [pixabay](https://pixabay.com/sound-effects/search/1%20second/), royalty free. 

## What's next

Nothing: unless I find something's wrong with it or that I want some improvements, I don't plan on developing on a regular basis. 
Feel free to fork and use as you like.