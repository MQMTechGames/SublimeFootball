using UnityEngine;

namespace MQMTech.AI.Knowledge
{
    public class SmashUnitMessage : AIMessage
    {
        public BaseUnit Smasher{ get; private set; }
        public Vector3 SmashForce{ get; private set; }

        public SmashUnitMessage(BaseUnit smasher, Vector3 smashForce)
        {
            Smasher = smasher;
            SmashForce = smashForce;

            DebugUtils.Assert(Smasher!=null, "Smasher!=null");
        }
    }
}
