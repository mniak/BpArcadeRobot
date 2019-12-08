using System;
using System.Drawing;
using System.IO;

namespace BpArcadeRobot.Infrastructure
{
    public class Frame : IFrame
    {
        private readonly Bitmap bitmap;

        public Frame(Bitmap bitmap)
        {
            this.bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));

        }

        public int Height => this.bitmap.Height;

        public int Width => this.bitmap.Width;

        public (byte red, byte green, byte blue) GetPixelColor(int x, int y)
        {
            var color = this.bitmap.GetPixel(x, y);
            return (color.R, color.G, color.B);
        }

        public void Dispose()
        {
            this.bitmap.Save(Path.Combine(@"D:\UserProfile\Downloads\temp", $"{DateTime.Now.Ticks}.png"));
            this.bitmap.Dispose();
        }
    }
}
