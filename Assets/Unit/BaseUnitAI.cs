using UnityEngine;
using MQMTech.AI.BT;
using MQMTech.AI.Knowledge;

namespace MQMTech.AI
{
    public class BaseUnitAI : MonoBehaviour, IBehaviorWithTree
    {
        [SerializeField]
        SharedMemory squadMemory;

        [SerializeField]
        SharedMemory matchMemory;

        BaseUnit _unit;

        BehaviorTree _bt;

        void Start()
        {
            _unit = gameObject.GetComponent<BaseUnit>();
            DebugUtils.Assert(_unit!=null, "[BaseUnitAI] _unit!=null");

            FootballerTreeParameters parameters = new FootballerTreeParameters(_unit, _unit.Squad);
            _bt = BehaviorTreeManager.CreateTreeInstance(BehaviortreeNames.Footballer, new Memory(), new Memory(), squadMemory.Memory, matchMemory.Memory, parameters);
            DebugUtils.Assert(_bt!=null, "[BaseUnitAI] bt!=null");
        }

        public BehaviorTree GetBehaviorTree()
        {
            return _bt;
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
