# SnapDesktop
SnapDesktop: a screensaver helper that takes screenshots of the desktop



## Author

* [@remusrigo](https://github.com/RemusRigo)
* [@remusrigo](https://sourceforge.net/u/remusrigo/profile)



## Features

* Takes a screenshot of the whole desktop (all monitors) every minute, in the background
* Creates only one screenshot (`Desktop.png`) and overwrites it
* Copies itself to `%LocalAppData%\Remus RIGO\SnapDesktop` and runs from there
* Adds itself to Startup, so it runs when you sign in
* No window, no tray icon
* Used by [Caterpillar ScreenSaver](https://github.com/RemusRigo/CaterpillarScreenSaver-VB.NET) to show your real desktop



## User's Guide

See the [User's Guide](UsersGuide.md) for downloads, runtimes, installation and uninstall.



## Statistics

![VB.NET](https://img.shields.io/badge/Language-VB.NET-blue)
![API](https://img.shields.io/badge/API-User32-lightblue)
![.NET4.8](https://img.shields.io/badge/Runtime-.NET%204.8-lightblue)
![.NET8.0](https://img.shields.io/badge/Runtime-.NET%208.0-lightblue)
![.NET10.0](https://img.shields.io/badge/Runtime-.NET%2010.0-lightblue)
![VS](https://img.shields.io/badge/IDE-Visual%20Studio-purple)
<br>
![status](https://img.shields.io/badge/status-completed-brightgreen) 
![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/RemusRigo/SnapDesktop-VB.NET/total)



## Roadmap

* 2026-10-11: Fix: replace some function to be compatible with .NET 4.8
* 2026-10-11: Fix: App crashes if workstation is locked or screensaver is running
* 2026-10-10: Add: Take screenshot every minute
* 2026-10-10: Add: Check if it runs at user login, if not set it up
* 2026-10-10: Add: Check if it's running from "%LocalAppData%\Remus RIGO\SnapDesktop", if not then it copies itself to that location
* 2026-10-10: Project started
