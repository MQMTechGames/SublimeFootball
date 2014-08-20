using UnityEngine;
using MQMTech.AI.BT;

namespace MQMTech.AI.Knowledge.Action
{
    public class CheckKnowledgeStatus : Behavior
    {
        AIMemoryKey _varName;
        KnowledgeStatus _status;
        
        public CheckKnowledgeStatus(AIMemoryKey memoryVarName, KnowledgeStatus knowledgeStatus)
        {
            _varName = memoryVarName;
            _status = knowledgeStatus;
        }
        
        public override Status Update()
        {
            DebugUtils.Assert(_bt!=null, "_bt!=null");
            
            BaseKnowledge memoryObj;
            bool isOk = _bt.GetMemoryObject<BaseKnowledge>(_varName, out memoryObj);
            if(!isOk)
            {
                return Status.FAILURE;
            }
            
            return memoryObj.Status == _status ? Status.SUCCESS : Status.FAILURE;
        }
    }

    public class SetKnowledgeStatus : Behavior
    {
        AIMemoryKey _varName;
        KnowledgeStatus _status;
        
        public SetKnowledgeStatus(AIMemoryKey memoryVarName, KnowledgeStatus knowledgeStatus)
        {
            _varName = memoryVarName;
            _status = knowledgeStatus;
        }
        
        public override Status Update()
        {
            DebugUtils.Assert(_bt!=null, "_bt!=null");
            
            BaseKnowledge memoryObj;
            bool isOk = _bt.GetMemoryObject<BaseKnowledge>(_varName, out memoryObj);
            if(!isOk)
            {
                return Status.FAILURE;
            }
            
            memoryObj.Status = _status;
            
            return Status.SUCCESS;
        }
    }
}
