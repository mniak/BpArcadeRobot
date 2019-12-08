using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BpArcadeRobot.Infrastructure
{
    public class Brain : IBrain
    {
        private double currentSpeed = 0;
        private DateTime? lastMoveTime = null;
        private int lastShipPositionX = 0;
        private Move lastMove = Move.Stay;
        public Move CalculateMove(IFrame frame)
        {
            var now = DateTime.Now;
            var milliseconds = this.lastMoveTime.HasValue
                 ? (now - this.lastMoveTime.Value).TotalMilliseconds
                 : this.cycleTimeMs;


            if (this.lastShipPositionX == 0)
                this.lastShipPositionX = frame.Width / 2;

            var ship = FindShip(frame);

            if (this.lastMove != Move.Stay)
            {
                var currentX = ship.X + (ship.Width / 2);
                this.currentSpeed = (currentX - this.lastShipPositionX) / milliseconds;
                Console.WriteLine($"Current speed: {this.currentSpeed} pixels/ms");
            }

            //var blocks = FindBlocks(frame);
            var move = DateTime.Now.Second % 2 == 0
                ? Move.Right
                : Move.Left;
            this.lastMove = move;
            this.lastMoveTime = DateTime.Now;
            return move;
        }

        //private HashSet<Rectangle> FindBlocks(IFrame frame)
        //{
        //    throw new NotImplementedException();
        //}

        private Rectangle FindShip(IFrame frame)
        {
            const int widestLine = 17;
            const int tipLine = 57;

            var waistLine = Enumerable.Range(0, frame.Width)
                .Select(x => new
                {
                    X = x,
                    Color = frame.GetPixelColor(x, frame.Height - widestLine),
                })
                .SkipWhile(x => x.Color.red + x.Color.green + x.Color.blue == 0)
                .TakeWhile(x => x.Color.red + x.Color.green + x.Color.blue > 0);

            var startingColumn = waistLine.FirstOrDefault()?.X ?? 0;
            var width = waistLine.Count();

            return new Rectangle(
                new Point(startingColumn, frame.Height - tipLine),
                new Size(width, tipLine));
        }

        public bool DetectGameIsPaused(IFrame frame)
        {
            const int offset_top = 200;
            const int offset_down = 300;
            const int offset_left = 300;
            const int offset_right = 300;
            const int threshold = 250;

            var square_height = frame.Height - offset_top - offset_down;
            var square_width = frame.Width - offset_left - offset_right;

            for (var y = offset_top; y < offset_top + square_height; y++)
            {
                for (var x = offset_left; x < offset_left + square_width; x++)
                {
                    var rgb = frame.GetPixelColor(x, y);
                    if (rgb.red > threshold &&
                        rgb.green < threshold &&
                        rgb.blue < threshold)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private readonly int cycleTimeMs = 1000;
        private readonly DateTime? lastDelay = null;
        public Task WaitSomeTime()
        {
            var delayMs = this.lastDelay.HasValue
                ? (int)(DateTime.Now - this.lastDelay.Value).TotalMilliseconds - this.cycleTimeMs
                : this.cycleTimeMs;
            return Task.Delay(delayMs);
        }
    }
}