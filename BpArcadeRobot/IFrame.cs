using System;

namespace BpArcadeRobot
{
    public interface IFrame : IDisposable
    {
        int Height { get; }
        int Width { get; }

        (byte red, byte green, byte blue) GetPixelColor(int x, int y);
    }
}