using UnityEngine;
using System.Collections;

namespace MQMTech.AI.Knowledge
{
    public class OnWillPassTheBall : AIMessage
    {
        public Vector3 WorldPosition{ get; private set; }
        public BaseUnit Unit{ get; private set; }

        public OnWillPassTheBall(float validDuration, Vector3 position, BaseUnit unit)
            :base(validDuration)
        {
            WorldPosition = position;
            Unit = unit;

            DebugUtils.Assert(Unit!=null, "Unit!=null");
        }

        public bool IsValid()
        {
            return Time.timeSinceLevelLoad < endValidTime;
        }
    }
}
