using System;
using System.Threading.Tasks;

namespace BpArcadeRobot
{
    public class Player
    {
        private readonly IEyes eyes;
        private readonly IBrain brain;
        private readonly IHands hands;

        public Player(
            IEyes eyes,
            IBrain brain,
            IHands hands)
        {
            this.eyes = eyes ?? throw new ArgumentNullException(nameof(eyes));
            this.brain = brain ?? throw new ArgumentNullException(nameof(brain));
            this.hands = hands ?? throw new ArgumentNullException(nameof(hands));
        }

        public bool Playing { get; set; }

        public async Task StartPlaying()
        {
            Playing = true;
            await this.hands.InitializeGame();
            await this.eyes.DetectMainRegion();
            while (Playing)
            {
                await WaitALittle();

                using (var frame = await this.eyes.SeeFrame())
                {
                    if (this.brain.DetectGameIsPaused(frame))
                    {
                        await this.hands.PressEnter();
                        continue;
                    }

                    var movement = this.brain.CalculateMove(frame);
                    switch (movement)
                    {
                        case Move.Stay:
                            await this.hands.StopMoving();
                            break;
                        case Move.Left:
                            await this.hands.StartMovingLeft();
                            break;
                        case Move.Right:
                            await this.hands.StartMovingRight();
                            break;
                    }
                }
            }
        }

        private Task WaitALittle()
        {
            return Task.Delay(100);
        }
    }
}
