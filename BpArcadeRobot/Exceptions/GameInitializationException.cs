using System;
using System.Runtime.Serialization;

namespace BpArcadeRobot.Exceptions
{
    [Serializable]
    public class GameInitializationException : Exception
    {
        public GameInitializationException()
        {
        }

        public GameInitializationException(string message) : base(message)
        {
        }

        public GameInitializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GameInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}