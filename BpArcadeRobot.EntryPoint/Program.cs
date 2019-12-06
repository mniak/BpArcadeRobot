using BpArcadeRobot.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace BpArcadeRobot
{
    internal class Program
    {
        public static object Eyes { get; private set; }

        private static async Task Main(string[] args)
        {
            var svc = new ServiceCollection()
                .AddTransient<IEyes, Eyes>()
                .AddTransient<IBrain, Brain>()
                .AddTransient<IHands, Hands>()
                .AddTransient<Player>()
                .BuildServiceProvider();

            var player = svc.GetRequiredService<Player>();
            await player.StartPlaying();
        }
    }
}
