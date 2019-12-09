namespace System.Drawing
{
    internal static class RectangleExtensions
    {
        public static Point Midpoint(this Rectangle rectangle)
        {
            var point = new Point(rectangle.X, rectangle.Y);
            point.Offset(rectangle.Width / 2, rectangle.Height / 2);
            return point;
        }

        public static bool IsInCollisionRouteWith(this Rectangle r1, Rectangle r2)
        {
            //var diag1 = r1.X + r1.Width - r2.X;

            //return  && r1.X <= r2.X + r2.Width;
            throw new NotImplementedException();
        }
    }
}
