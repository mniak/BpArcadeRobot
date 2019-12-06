namespace BpArcadeRobot
{
    public interface IBrain
    {
        bool DetectGameIsPaused(IFrame frame);
        Move CalculateMove(IFrame frame);
    }
}
