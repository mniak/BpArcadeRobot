using System;
using System.Drawing;

namespace BpArcadeRobot.Infrastructure
{
    public class Frame : IFrame
    {
        private readonly Bitmap bitmap;

        public Frame(Bitmap bitmap)
        {
            this.bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
        }

        public int Height => bitmap.Height;

        public int Width => bitmap.Width;

        public (byte red, byte green, byte blue) GetPixelColor(int x, int y)
        {
            var color = bitmap.GetPixel(x, y);
            return (color.R, color.G, color.B);
        }

        public void Dispose()
        {
            bitmap.Dispose();
        }
    }
}
