using UnityEngine;

namespace MQMTech.AI.Knowledge
{
    public class AIMessage : BaseKnowledge
    {
        public float Timestap { get; private set; }

        public AIMessage()
            :base()
        {
            Timestap = Time.timeSinceLevelLoad;
        }

        public bool CheckValidElapsedTime(float elapsedTime)
        {
            return Time.timeSinceLevelLoad < (Timestap + elapsedTime);
        }
    }
}
