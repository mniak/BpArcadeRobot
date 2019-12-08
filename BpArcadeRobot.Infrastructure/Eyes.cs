using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BpArcadeRobot.Infrastructure
{
    public class Eyes : IEyes
    {
        private Rectangle mainRegion;
        private readonly Screen screen;

        public Eyes()
        {
            this.screen = Screen.PrimaryScreen;
            this.mainRegion = this.screen.Bounds;
        }

        public Task DetectMainRegion()
        {
            (int start, int length) largestVerticalBlackSegment;
            (int start, int length) largestHorizontalBlackSegment;

            using (var bitmap = new Bitmap(this.screen.Bounds.Width, this.screen.Bounds.Height))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(new Point(this.screen.Bounds.X, this.screen.Bounds.Y), new Point(0, 0), bitmap.Size);
                }

                largestVerticalBlackSegment = FindLargestContiguousSegment(
                    this.screen.Bounds.Width,
                    this.screen.Bounds.Height,
                    (x, y) => IsBlackPixel(bitmap, x, y));
                largestHorizontalBlackSegment = FindLargestContiguousSegment(
                    this.screen.Bounds.Height,
                    this.screen.Bounds.Width,
                    (y, x) => IsBlackPixel(bitmap, x, y));
            }

            this.mainRegion = new Rectangle(
                new Point(largestHorizontalBlackSegment.start, largestVerticalBlackSegment.start),
                new Size(largestHorizontalBlackSegment.length, largestVerticalBlackSegment.length));

            return Task.CompletedTask;
        }

        private bool IsBlackPixel(Bitmap bitmap, int x, int y)
        {
            var color = bitmap.GetPixel(x, y);
            return color.GetBrightness() == 0;
        }

        private static (int, int) FindLargestContiguousSegment(int d1length, int d2length, Func<int, int, bool> isActivePoint)
        {
            var largestSegment = (index: 0, start: 0, length: 0);
            for (var d1 = 0; d1 < d1length; d1++)
            {
                var start = 0;
                var isOnActiveSegment = false;

                for (var d2 = 0; d2 < d2length; d2++)
                {
                    var pixelIsBlack = isActivePoint.Invoke(d1, d2);

                    if (pixelIsBlack && !isOnActiveSegment)
                    {
                        start = d2;
                        isOnActiveSegment = true;
                    }
                    else if (!pixelIsBlack && isOnActiveSegment)
                    {
                        var length = d2 - start;
                        if (length > largestSegment.length)
                            largestSegment = (d1, start, d2 - start);

                        isOnActiveSegment = false;
                    }
                }
            }
            return (largestSegment.start, largestSegment.length);
        }

        public Task<IFrame> SeeFrame()
        {
            var bitmap = new Bitmap(this.mainRegion.Width, this.mainRegion.Height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(
                    this.mainRegion.Location,
                    new Point(0, 0),
                    this.mainRegion.Size);
            }
            var frame = new Frame(bitmap) as IFrame;
            return Task.FromResult(frame);
        }
    }
}
