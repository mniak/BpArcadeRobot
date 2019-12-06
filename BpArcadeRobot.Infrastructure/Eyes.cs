using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BpArcadeRobot.Infrastructure
{
    public class Eyes : IEyes
    {
        private const int OFFSET_TOP = 200;
        private const int OFFSET_BOTTOM = 160;
        private const int OFFSET_LEFT = 500;
        private const int OFFSET_RIGHT = 400;
        public Task<IFrame> SeeFrame()
        {
            var screen = Screen.PrimaryScreen;
            var bitmap = new Bitmap(screen.Bounds.Width - OFFSET_LEFT - OFFSET_RIGHT, screen.Bounds.Height - OFFSET_TOP - OFFSET_BOTTOM);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(new Point(screen.Bounds.X + OFFSET_LEFT, screen.Bounds.Y + OFFSET_TOP), new Point(0, 0), bitmap.Size);
            }
            var frame = new Frame(bitmap) as IFrame;
            return Task.FromResult(frame);
        }
    }
}
