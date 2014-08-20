using UnityEngine;
using MQMTech.AI.BT;

namespace MQMTech.AI.Mover.Action
{
    [System.Serializable]
    public class SaveListCount<T> : Behavior
    {
        AIMemoryKey _listKey;
        AIMemoryKey _countKey;

        public SaveListCount(AIMemoryKey listKey, AIMemoryKey countKey)
        {
            _listKey = listKey;
            _countKey = countKey;
        }

        public override Status Update()
        {
            SimpleList<T> list;
            _bt.GetMemoryObject(_listKey, out list);
            DebugUtils.Assert(list!=null, "command!=null");

            _bt.SetMemoryObject(_countKey, list.Count());

            return Status.SUCCESS;
        }
    }
}
