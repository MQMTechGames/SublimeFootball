using UnityEngine;
using System.Collections;

namespace MQMTech.AI.Knowledge
{
    public class AIMessage : BaseKnowledge
    {
        // When the message will stop being valid
        public float endValidTime { get; set; }

        public AIMessage(float validDuration)
            :base()
        {
            endValidTime = Time.timeSinceLevelLoad + validDuration;
        }

        public bool IsValid()
        {
            return Time.timeSinceLevelLoad < endValidTime;
        }
    }
}
