# React Native + SAFE demo

This is a demo application for React Native development in F# using Fable and the SAFE stack. 
SAFE is a technology stack that brings together several technologies into a single, coherent stack for typesafe, flexible end-to-end web-enabled applications that are written entirely in F#.

![SAFE-Stack](images/safe_logo.png "SAFE-Stack")

If you want to try it out then please do the following:

## Requirements

- [Mono](http://www.mono-project.com/) on MacOS/Linux
- [.NET Framework 4.6.2](https://support.microsoft.com/en-us/help/3151800/the--net-framework-4-6-2-offline-installer-for-windows) on Windows
- [node.js](https://nodejs.org/) - JavaScript runtime
- [yarn](https://yarnpkg.com/) - Package manager for npm modules

> On OS X/macOS, make sure you have OpenSSL installed and symlinked correctly, as described here: [https://www.microsoft.com/net/core#macos](https://www.microsoft.com/net/core#macos).

[dotnet SDK 2.0.0](https://www.microsoft.com/net/core) is required but it will be downloaded automatically by the build script if not installed (see below). 
Other tools like [Paket](https://fsprojects.github.io/Paket/) or [FAKE](https://fake.build/) will also be installed by the build script.

### Make sure React Native works

* Install the Ionide [VS Code extension for F#](https://github.com/ionide/ionide-vscode-fsharp)
* Go to https://facebook.github.io/react-native/ and do all steps of the [getting started tutorial](https://facebook.github.io/react-native/docs/getting-started.html#content)
* At this point you should have the default Javascript React Native "Hello World" running

### Running F# code on React Native

* Pull latest master
* Open Android Simulator or connect a device in debug mode via USB
* Run `build.cmd debug` from command prompt
* Go to android device simulator and you should see something like

![Device Running](http://www.navision-blog.de/images/nightwatchapp.gif)

* Go to the F# source in VS Code make a change and save the file
* Fable watch should pick up this change automatically and compile it
* Go to android device simulator - change should be visible

This could look like:

![Hot loading](http://www.navision-blog.de/images/hotloading.gif)


## Error handling

### Unauthorized device

If you run `build.cmd debug` and it compiles everything, but nothing happens... go to the packager
output and search for hints.
One pitfall might be "Skipping device, Device is UNAUTHORIZED". With
```
adb devices
```
you can see the list of devices, probably one of them is marked as UNAUTHORIZED. First,
try
```
adb kill-server
```
and try it again. If it doesn't help, you can push the public key onto the device again:
```
cd ~/.android && adb push adbkey.pub /data/misc/adb/adb_keys
```
After rebooting the device, access should be granted again.

### Licenses not accepted 

If the above does not work, and still nothing happens after it compiles everything, the licenses may need to be accepted on your computer. To accept the licenses run the following commands for your platform

On a Mac run the following 
```
$ANDROID_HOME/tools/bin/sdkmanager --update
```
On Windows run the following 
```
“%ANDROID_HOME%\tools\bin\sdkmanager” --update
```

Re-run the build script and everything should work.

### The app is not updating on saving code changes

If the app does not update whenever you save an F# code change even after Fable compilation succeeds, you may need to enable hot reloading from the in-app developer menu. It is accessed by opening the app and shacking the device or causing the emulator to simulate a shake or by using the command:
```
adb shell input keyevent 82
```

Click on "Enable Hot Reloading". Now when you save a code change and Fable compiles it, you should see the result in the app on your device or emulator automatically.
