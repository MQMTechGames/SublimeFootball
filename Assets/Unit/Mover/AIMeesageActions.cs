using UnityEngine;
using MQMTech.AI.BT;
using MQMTech.AI.Knowledge;

namespace MQMTech.AI.Mover.Action
{
    [System.Serializable]
    public class CheckAIMessageIsValidOrRemove : Behavior
    {
        AIMemoryKey _messageKey;
        float _maxElapsedTime;

        public CheckAIMessageIsValidOrRemove(AIMemoryKey messageKey, float maxElapsedTime)
        {
            _messageKey = messageKey;
            _maxElapsedTime = maxElapsedTime;
        }

        public override Status Update()
        {
            AIMessage message;
            _bt.GetMemoryObject(_messageKey, out message);

            if(message == null)
            {
                return Status.FAILURE;
            }

            return message.CheckValidElapsedTime(_maxElapsedTime) ? Status.SUCCESS : Status.FAILURE;
        }
    }

    [System.Serializable]
    public class RemoveKnowledgeIfNotNew : Behavior
    {
        AIMemoryKey _messageKey;
        
        public RemoveKnowledgeIfNotNew(AIMemoryKey messageKey)
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

            if(message.Status == KnowledgeStatus.NEW)
            {
                return Status.FAILURE;
            }

            _bt.RemoveMemoryObject(_messageKey);

            return Status.SUCCESS;
        }
    }
}
