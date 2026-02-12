using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DevToolbar.Core.Interfaces;

namespace DevToolbar.Maui.Services;

/// <summary>
/// Real Windows process service using System.Diagnostics.
/// </summary>
public class WindowsProcessService : IProcessService
{
    public async Task<int> StartProcessAsync(string path, string arguments = "")
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = path,
            Arguments = arguments,
            UseShellExecute = true
        };

        var process = Process.Start(startInfo);
        if (process == null)
            throw new InvalidOperationException($"Failed to start process: {path}");

        return await Task.FromResult(process.Id);
    }

    public bool FocusWindowByTitle(string titleRegex)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return false;

        var regex = new Regex(titleRegex, RegexOptions.IgnoreCase);
        var processes = Process.GetProcesses();

        foreach (var process in processes)
        {
            try
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle) && regex.IsMatch(process.MainWindowTitle))
                {
                    // Use P/Invoke to bring window to foreground
                    NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                    return true;
                }
            }
            catch
            {
                // Process may have exited
            }
        }
        return false;
    }

    private static partial class NativeMethods
    {
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetForegroundWindow(IntPtr hWnd);
    }
}
