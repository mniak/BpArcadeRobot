using System.Threading.Tasks;

namespace BpArcadeRobot
{
    public interface IHands
    {
        Task StartMovingLeft();
        Task StartMovingRight();
        Task StopMoving();
        Task PressEnter();
        Task InitializeGame();
    }
}
