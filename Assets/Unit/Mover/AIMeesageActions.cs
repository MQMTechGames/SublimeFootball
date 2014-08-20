using UnityEngine;
using MQMTech.AI.BT;
using MQMTech.AI.Knowledge;

namespace MQMTech.AI.Mover.Action
{
    [System.Serializable]
    public class CheckAIMessageIsValidOrRemove : Behavior
    {
        AIMemoryKey _messageKey;

        public CheckAIMessageIsValidOrRemove(AIMemoryKey messageKey)
        {
            _messageKey = messageKey;
        }

        public override Status Update()
        {
            AIMessage message;
            _bt.GetMemoryObject(_messageKey, out message);

            if(message == null)
            {
                return Status.FAILURE;
            }

            return message.IsValid() ? Status.SUCCESS : Status.FAILURE;
        }
    }
}
