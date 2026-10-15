# SnapDesktop: User's Guide



## Downloads & Runtimes

Download the latest release from the [Releases page](https://github.com/RemusRigo/SnapDesktop-VB.NET/releases).

Each release contains one folder per framework and architecture:

|Folder|Target Framework|Architecture|Required Runtime|
|-|-|-|-|
|`net48 x64`|.NET Framework 4.8|64-bit|[.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48) (included in Windows 10/11)|
|`net48 x86`|.NET Framework 4.8|32-bit|[.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48) (included in Windows 10/11)|
|`net8.0 x64`|.NET 8.0|64-bit|[.NET 8.0 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/8.0)|
|`net8.0 x86`|.NET 8.0|32-bit|[.NET 8.0 Desktop Runtime (x86)](https://dotnet.microsoft.com/download/dotnet/8.0)|
|`net10.0 x64`|.NET 10.0|64-bit|[.NET 10.0 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/10.0)|
|`net10.0 x86`|.NET 10.0|32-bit|[.NET 10.0 Desktop Runtime (x86)](https://dotnet.microsoft.com/download/dotnet/10.0)|

> **Which one do I need?** On Windows 10/11, the **net48 x64** build runs without installing anything.
> For the .NET 8.0/10.0 builds, the plain ".NET Runtime" is not enough; install the **Desktop** Runtime.



## Why do I need it?

Windows switches to a separate desktop when a screensaver starts, so a screensaver can't capture your open windows by itself.
SnapDesktop runs in the background and saves a screenshot of your desktop every minute; a screensaver
(e.g. [Caterpillar ScreenSaver](https://github.com/RemusRigo/CaterpillarScreenSaver-VB.NET)) then uses that image.



## Install

1. Download and extract the folder that matches your system (see the table above), e.g. `net48 x64`.
2. Double-click `SnapDesktop.exe`. That's it: there is no window and no tray icon.

On the first run, SnapDesktop:

* copies itself (all the files in its folder) to `%LocalAppData%\Remus RIGO\SnapDesktop` and restarts from there,
  so you can delete the downloaded folder afterwards;
* adds itself to Startup (`HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run`, value `SnapDesktop`),
  so it runs every time you sign in.

Running `SnapDesktop.exe` again closes the running copy and starts a fresh one (e.g. to update it).

> If Windows blocks the file ("Windows protected your PC"), right-click `SnapDesktop.exe` →
> **Properties** → check **Unblock** → **OK**, then try again.



## The screenshot

The screenshot is saved every minute to:

```
%LocalAppData%\Remus RIGO\SnapDesktop\Desktop.png
```

It covers all your monitors and is overwritten each time; no other screenshots are kept.

> **Privacy:** the screenshot shows whatever was on your screen (open documents, e-mails, passwords shown in clear).
> It stays on your PC and is readable only by your user account and administrators.



## Uninstall

1. Delete the value `SnapDesktop` from `HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run`
   (or turn it off in **Task Manager → Startup apps**).
2. End `SnapDesktop.exe` in **Task Manager → Details**.
3. Delete the folder `%LocalAppData%\Remus RIGO\SnapDesktop`.
