using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace BpArcadeRobot
{
    public class Brain : IBrain
    {
        private int iteration = 0;

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

            var currentX = ship.Midpoint().X;
            if (this.lastMove != Move.Stay)
            {
                var speed = Math.Abs((currentX - this.lastShipPositionX) / milliseconds);
                if (this.currentSpeed < 1 || speed > this.currentSpeed * 0.8)
                    this.currentSpeed = speed;
                Console.WriteLine($"Current speed: {this.currentSpeed:0.####} px/ms. Instantaneous: {speed:0.####} px/ms");
            }

            var blocks = FindBlocks(frame);
            var move = MakeADecision(ship, blocks, this.currentSpeed);

            this.lastMove = move;
            this.lastMoveTime = DateTime.Now;
            this.lastShipPositionX = currentX;
            return move;
        }

        private Move MakeADecision(Rectangle ship, IEnumerable<Rectangle> blocks, double currentSpeed)
        {
            if (!blocks.Any())
                return Move.Stay;

            blocks = blocks
                .OrderByDescending(b => b.Y)
                .ThenBy(b => Math.Abs(ship.Midpoint().X - b.X));

            var first = blocks.First();


            return this.iteration++ % 2 == 0
                ? Move.Right
                : Move.Left;
        }

        private IEnumerable<Rectangle> FindBlocks(IFrame frame)
        {
            var points = Enumerable.Range(0, frame.Height)
                .Zip(Enumerable.Range(0, frame.Width), (x, y) => (x, y))
                    .Select(p => (p.x, p.y, color: frame.GetPixelColor(p.x, p.y)))
                    .Select(p => (p.x, p.y, isblock: p.color.blue > p.color.red && p.color.blue > p.color.green))
                    .Where(p => p.isblock)
                    .Select(p => (p.x, p.y));

            var segments = points
                .GroupBy(p => p.y)
                .Select(v => v.GlueSequence(p => p.x))
                .SelectMany(g => g)
                .Select(v => (v.Item1.x, v.Item1.y, length: v.Item2));

            var blocks = segments
                .GroupBy(p => (p.x, p.length))
                .Select(v => v.GlueSequence(p => p.y))
                .SelectMany(g => g)
                .Select(v => new Rectangle(
                    new Point(v.Item1.x, v.Item1.y),
                    new Size(v.Item1.length, v.Item2)));

            return blocks;
        }

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

        private readonly int cycleTimeMs = 100;
        private readonly DateTime? lastDelay = null;
        public Task WaitSomeTime()
        {
            var delayMs = this.lastDelay.HasValue
                ? (int)(DateTime.Now - this.lastDelay.Value).TotalMilliseconds - this.cycleTimeMs
                : this.cycleTimeMs;
            Console.WriteLine($"Waiting {this.cycleTimeMs} ms");
            return Task.Delay(delayMs);
        }
    }
}