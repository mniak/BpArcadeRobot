using BpArcadeRobot.Exceptions;
using PInvoke;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace BpArcadeRobot.Infrastructure
{
    public class Hands : IHands
    {
        private readonly InputSimulator inputSimulator;
        private bool pressingRight;
        private bool pressingLeft;

        public Hands()
        {
            this.inputSimulator = new InputSimulator();
        }
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

        private bool ClickOnWindowCenter(Process process)
        {
            if (!User32.GetWindowRect(process.MainWindowHandle, out var winRect))
                return false;

            var x = (int)((winRect.right - winRect.left) / 2.0 + winRect.left);
            var y = (int)((winRect.bottom - winRect.top) / 2.0 + winRect.top);

            this.inputSimulator.Mouse.MoveMouseTo(ushort.MaxValue / 2, ushort.MaxValue / 2);
            this.inputSimulator.Mouse.RightButtonClick();

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
            this.inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            return Task.CompletedTask;
        }

        public Task StartMovingLeft()
        {
            if (!this.pressingLeft)
            {
                this.inputSimulator.Keyboard.KeyDown(VirtualKeyCode.LEFT);
                this.pressingLeft = true;
            }
            return Task.CompletedTask;
        }

        public Task StartMovingRight()
        {
            if (!this.pressingRight)
            {
                this.inputSimulator.Keyboard.KeyDown(VirtualKeyCode.RIGHT);
                this.pressingRight = true;
            }
            return Task.CompletedTask;
        }

        public Task StopMoving()
        {
            if (this.pressingLeft)
            {
                this.inputSimulator.Keyboard.KeyUp(VirtualKeyCode.LEFT);
                this.pressingLeft = false;
            }
            if (this.pressingRight)
            {
                this.inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RIGHT);
                this.pressingRight = false;
            }
            return Task.CompletedTask;
        }
    }
}
