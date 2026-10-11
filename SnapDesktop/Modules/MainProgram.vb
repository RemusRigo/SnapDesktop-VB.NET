'-------------------------------------------------------------------------------------------------
' SnapDesktop: MainProgram.vb
'    © 2026 Remus Rigo
'       v1.0.20261011
'------------------------------------------------------------------------------------------------

Imports System.IO
Imports Microsoft.Win32

Imports SnapDesktop.API

Module MainProgram

   Private Const GENERIC_ALL As UInteger = &H10000000UI
   Private appPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Remus RIGO\SnapDesktop")
   Private WithEvents snapTimer As New Timer()

   '-----------------------------------------------------------------------------------------------
   ' The main entry point for the application
   Sub Main()
      ' Check if the application is running from the LocalAppData folder
      If IsRunningFromLocalAppData() Then
         ' check if another instance is already running
         If IsAppAlreadyRunning() Then
            CloseOtherInstances("SnapDesktop") ' close other instance if it's running from the same folder
         End If
      Else ' app not running from LocalAppData
         CloseOtherInstances("SnapDesktop")

         ' copy the application files to the LocalAppData folder
         CopyAppToLocalAppData()
         ' Launch the application from the LocalAppData folder and exit
         LaunchFromLocalAppData()
         Return
      End If

      ' Check if the application is set to run at startup, and if not, add it to the registry
      CheckStartup()
      Dim vSrc As Rectangle = SystemInformation.VirtualScreen
      SaveScreenshot(CaptureDesktop(vSrc))

      snapTimer.Interval = 60000 ' 1 minute
      snapTimer.Start()

      System.Windows.Forms.Application.Run()
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' Checks if the application is running from the LocalAppData folder
   Private Function IsRunningFromLocalAppData() As Boolean
      Dim currentFolder As String = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath)
      Return String.Equals(currentFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
         appPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), StringComparison.OrdinalIgnoreCase)
   End Function

   '-----------------------------------------------------------------------------------------------
   ' Copies the application files to the LocalAppData folder if not already present
   Private Sub CopyAppToLocalAppData()
      Dim sourcePath As String = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath)
      Directory.CreateDirectory(appPath)
      For Each sourceFile As String In Directory.GetFiles(sourcePath)
         File.Copy(sourceFile, Path.Combine(appPath, Path.GetFileName(sourceFile)), True)
      Next
   End Sub

   '----------------------------------------------------------------------------------------------
   ' Check if the application is set to run at startup, and if not, add it to the registry
   Private Sub CheckStartup()
      Try
         Using key As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)
            If key.GetValue("SnapDesktop") Is Nothing Then
               key.SetValue("SnapDesktop", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Remus RIGO\SnapDesktop\SnapDesktop.exe"))
            End If
         End Using
      Catch ex As Exception
         MessageBox.Show("Error occurred while checking startup settings.")
      End Try
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' Check if another instance of the application is already running
   Private Function IsAppAlreadyRunning() As Boolean
      Dim currentProcess As Process = Process.GetCurrentProcess()
      Dim processes() As Process = Process.GetProcessesByName(currentProcess.ProcessName)
      For Each process As Process In processes
         If process.Id <> currentProcess.Id Then
            Return True
         End If
      Next
      Return False
   End Function

   '-----------------------------------------------------------------------------------------------
   ' Check if another instance of the application is already running from the same folder
   Private Function IsAppRunningFromFolder(processName As String, processPath As String) As Boolean
      Dim currentId As Integer = Process.GetCurrentProcess().Id

      For Each p As Process In Process.GetProcessesByName(processName)
         If p.Id <> currentId Then
            Try
               Dim exeFolder As String = Path.GetDirectoryName(p.MainModule.FileName)
               'If String.Equals(Path.TrimEndingDirectorySeparator(exeFolder), Path.TrimEndingDirectorySeparator(processPath), StringComparison.OrdinalIgnoreCase) Then
               If String.Equals(exeFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                  processPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                  StringComparison.OrdinalIgnoreCase) Then Return True

            Catch ex As System.ComponentModel.Win32Exception
               ' access denied, so we can't read the path of this process
            Catch ex As InvalidOperationException
               ' the process exited while we were checking it
            End Try
         End If
      Next

      Return False
   End Function

   '-----------------------------------------------------------------------------------------------
   ' Keep current instance running and close any other instances of the application
   Private Sub CloseOtherInstances(processName As String)
      Dim currentId As Integer = Process.GetCurrentProcess().Id

      For Each p As Process In Process.GetProcessesByName(processName)
         If p.Id <> currentId Then
            Try
               p.Kill()
               p.WaitForExit(5000) ' wait up to 5s so the file lock is released
            Catch ex As System.ComponentModel.Win32Exception
               ' access denied
            Catch ex As InvalidOperationException
               ' already exited
            End Try
         End If
      Next
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' Keep current instance running and close any other instances of the application that are running from the same folder
   Private Sub CloseOtherInstanceFromSameFolder(processName As String, processPath As String)
      Dim currentId As Integer = Process.GetCurrentProcess().Id

      For Each p As Process In Process.GetProcessesByName(processName)
         If p.Id <> currentId Then
            Try
               Dim exeFolder As String = Path.GetDirectoryName(p.MainModule.FileName)
               If String.Equals(exeFolder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                  processPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) Then

                  p.Kill()
                  p.WaitForExit(5000) ' wait up to 5s so the file lock is released
               End If
            Catch ex As System.ComponentModel.Win32Exception
               ' access denied
            Catch ex As InvalidOperationException
               ' already exited
            End Try
         End If
      Next
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' Launch the application from the LocalAppData folder
   Private Sub LaunchFromLocalAppData()
      Dim exePath As String = Path.Combine(appPath, "SnapDesktop.exe")
      Dim startInfo As New ProcessStartInfo(exePath) With {
         .UseShellExecute = True,
         .WorkingDirectory = appPath
      }
      Process.Start(startInfo)
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' Capture the entire desktop and return it as a Bitmap
   Private Function CaptureDesktop(bounds As Rectangle) As Bitmap
      Dim original As IntPtr = GetThreadDesktop(GetCurrentThreadId())
      Dim hDefault As IntPtr = OpenDesktop("Default", 0, False, GENERIC_ALL)
      Dim switched As Boolean = False
      If hDefault <> IntPtr.Zero Then switched = SetThreadDesktop(hDefault)
      Try
         Dim bmp As New Bitmap(bounds.Width, bounds.Height, Imaging.PixelFormat.Format32bppPArgb)
         Using g As Graphics = Graphics.FromImage(bmp)
            Try
               g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size)
            Catch ex As System.ComponentModel.Win32Exception
               ' desktop handle was invalid for a moment (e.g. screensaver just exited) - return what we have
            End Try
         End Using
         Return bmp
      Finally
         If switched Then SetThreadDesktop(original)
         If hDefault <> IntPtr.Zero Then CloseDesktop(hDefault)
      End Try
   End Function

   '-----------------------------------------------------------------------------------------------
   ' Save the captured screenshot in the LocalAppData folder
   Private Sub SaveScreenshot(bmp As Bitmap)
      Dim filePath As String = Path.Combine(appPath, "Desktop.png")
      bmp.Save(filePath, Imaging.ImageFormat.Png)
      bmp.Dispose()
   End Sub

   '-----------------------------------------------------------------------------------------------
   ' Timer tick event handler to capture and save the screenshot every minute
   Private Sub snapTimer_Tick(sender As Object, e As EventArgs) Handles snapTimer.Tick
      Try
         SaveScreenshot(CaptureDesktop(SystemInformation.VirtualScreen))
      Catch ex As Exception
         ' never let a single failed capture take down the background process
      End Try
   End Sub

End Module
