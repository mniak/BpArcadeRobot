using System.Threading.Tasks;

namespace BpArcadeRobot
{
    public interface IEyes
    {
        Task DetectMainRegion();
        Task<IFrame> SeeFrame();
    }
}
