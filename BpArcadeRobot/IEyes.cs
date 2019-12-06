using System.Threading.Tasks;

namespace BpArcadeRobot
{
    public interface IEyes
    {
        Task<IFrame> SeeFrame();
    }
}
