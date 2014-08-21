using UnityEngine;

namespace MQMTech.AI.Knowledge
{
    public class BallPassedMessage : AIMessage
    {
        public Vector3 WorldPosition{ get; private set; }
        public BaseUnit Unit{ get; private set; }
        public BaseUnit TargetUnit{ get; private set; }
        public Ball Ball{ get; private set; }

        public BallPassedMessage(Vector3 position, BaseUnit unit, BaseUnit targetUnit, Ball ball)
        {
            WorldPosition = position;
            Unit = unit;
            TargetUnit = targetUnit;
            Ball = ball;

            DebugUtils.Assert(Unit!=null, "Unit!=null");
            DebugUtils.Assert(Ball!=null, "Ball!=null");
        }
    }
}
