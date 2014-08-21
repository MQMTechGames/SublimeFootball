using UnityEngine;

namespace MQMTech.AI.Knowledge
{
    public class BallShotMessage : AIMessage
    {
        public Vector3 WorldPosition{ get; private set; }
        public BaseUnit Unit{ get; private set; }
        public Ball Ball{ get; private set; }

        public BallShotMessage(Vector3 position, BaseUnit unit, Ball ball)
        {
            WorldPosition = position;
            Unit = unit;
            Ball = ball;

            DebugUtils.Assert(Unit!=null, "Unit!=null");
            DebugUtils.Assert(Ball!=null, "Ball!=null");
        }
    }
}
