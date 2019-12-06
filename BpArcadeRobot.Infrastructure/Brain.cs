namespace BpArcadeRobot.Infrastructure
{
    public class Brain : IBrain
    {
        public Move CalculateMove(IFrame frame)
        {
            return Move.Right;
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
    }
}