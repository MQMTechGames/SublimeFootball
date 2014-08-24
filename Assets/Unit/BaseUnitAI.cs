using UnityEngine;
using MQMTech.AI.BT;
using MQMTech.AI.Knowledge;

namespace MQMTech.AI
{
    public class BaseUnitAI : MonoBehaviour
    {
        [SerializeField]
        DemoUnitBTBuilder btBuilder;

        BehaviorTree _bt;

        void Start()
        {
            _bt = btBuilder.Create();
            DebugUtils.Assert(_bt!=null, "[BaseUnitAI] bt!=null");
        }

        public void SendAIMessage<T>(string messageName, T message)
        {
            AIMemoryKey memoryKey;
            if(!MemoryKeysHashCodeManager.TryMemoryMemoryKeyByNameAndContext(messageName, AIMemoryKey.ContextType.Agent, out memoryKey))
            {
                memoryKey = new AIMemoryKey(messageName);
            }

            _bt.SetMemoryObject<T>(memoryKey, message);
        }

        void Update()
        {
            _bt.Tick();
        }
    }
}
