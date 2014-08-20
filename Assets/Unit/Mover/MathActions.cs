using UnityEngine;
using MQMTech.AI.BT;

namespace MQMTech.AI.Mover.Action
{
    [System.Serializable]
    public class GreaterThan : Behavior
    {
        AIMemoryKey _floatA;
        AIMemoryKey _floatB;

        public GreaterThan(AIMemoryKey floatA, AIMemoryKey floatB)
        {
            _floatA = floatA;
            _floatB = floatB;
        }

        public override Status Update()
        {
            float a;
            _bt.GetMemoryObject(_floatA, out a);

            float b;
            _bt.GetMemoryObject(_floatB, out b);

            return a > b ? Status.SUCCESS : Status.FAILURE;
        }
    }
}
