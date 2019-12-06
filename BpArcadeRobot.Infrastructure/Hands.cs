using BpArcadeRobot.Exceptions;
using PInvoke;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BpArcadeRobot.Infrastructure
{
    public class Hands : IHands
    {
        public IntPtr Handle { get; private set; }

        public Task InitializeGame()
        {
            var process = FindGameProcess();
            if (process == null ||
                !AlternateToWindow(process) ||
                !ClickOnWindowCenter(process))
            {
                throw new GameInitializationException("Could not activate the game window.");
            }

            Handle = process.MainWindowHandle;

            return Task.CompletedTask;
        }

        private static bool ClickOnWindowCenter(Process process)
        {
            if (!User32.GetWindowRect(process.MainWindowHandle, out var winRect))
                return false;

            var x = (int)((winRect.right - winRect.left) / 2.0 + winRect.left);
            var y = (int)((winRect.bottom - winRect.top) / 2.0 + winRect.top);

            if (!User32.SetCursorPos(x, y))
                return false;

            User32.mouse_event(User32.mouse_eventFlags.MOUSEEVENTF_LEFTDOWN, x, y, 0, IntPtr.Zero);
            User32.mouse_event(User32.mouse_eventFlags.MOUSEEVENTF_LEFTUP, x, y, 0, IntPtr.Zero);

            return true;
        }

        private static bool AlternateToWindow(Process process)
        {
            if (!User32.SetForegroundWindow(process.MainWindowHandle))
                return false;
            return true;
        }

        private static Process FindGameProcess()
        {
            var process = Process.GetProcesses()
                .FirstOrDefault(x => x.MainWindowTitle.Contains("Galactic Dodge - Arcade Hub"));
            if (process == null)
                return null;
            return process;
        }

        public Task PressEnter()
        {
            User32.PostMessage(Handle, User32.WindowMessage.WM_KEYDOWN, new IntPtr(VK_RETURN), IntPtr.Zero);
            User32.PostMessage(Handle, User32.WindowMessage.WM_KEYUP, new IntPtr(VK_RETURN), IntPtr.Zero);

            return Task.CompletedTask;
        }

        private const int VK_RETURN = 0x0D;
        private const int VK_RIGHT = 0x27;
        private const int VK_LEFT = 0x25;

        public Task StartMovingLeft()
        {
            Console.WriteLine("Press left");
            SendKeys.SendWait("{LEFT}");
            return Task.CompletedTask;
        }

        public Task StartMovingRight()
        {
            Console.WriteLine("Press right");
            SendKeys.SendWait("{RIGHT}");
            return Task.CompletedTask;
        }

        public Task StopMoving()
        {
            Console.WriteLine("stop");
            User32.PostMessage(Handle, User32.WindowMessage.WM_KEYUP, new IntPtr(VK_RIGHT), IntPtr.Zero);
            User32.PostMessage(Handle, User32.WindowMessage.WM_KEYUP, new IntPtr(VK_LEFT), IntPtr.Zero);
            return Task.CompletedTask;
        }
    }
}
