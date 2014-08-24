using UnityEngine;
using MQMTech.AI.BT;

public class SubtreeBehavior : Behavior
{
    BehaviorTree _subtree;
    Behavior _subTreeRootBehavior;

    protected AIMemoryKeyPair[] _inputs;
    protected AIMemoryKeyPair[] _outputs;

    protected string _subtreeName;

    public override void OnInitialize()
    {
        base.OnInitialize();

        InitSubtree();

        if(_subTreeRootBehavior != null)
        {
            ApplyInputParameters();

            _subTreeRootBehavior.OnReset();
        }
    }

    void InitSubtree()
    {
        _inputs = null;
        _outputs = null;
        
        _subtree = BehaviorTreeManager.FindBehaviorTree(_subtreeName);
        DebugUtils.Assert(_subtree!=null, "subtree: " + _subtreeName + " not found");
        _subTreeRootBehavior = _subtree.GetRootBehavior();
        
//        _subtree.SetAgentMemory(_bt.GetAgentMemory());
//        DebugUtils.Assert(_subtree.GetAgentMemory() != null);

        _subtree.SetSharedMemory(_bt.GetSharedMemory());
        DebugUtils.Assert(_subtree.GetSharedMemory() != null);

        _subtree.SetGlobalMemory(_bt.GetGlobalMemory());
        DebugUtils.Assert(_subtree.GetGlobalMemory() != null);
    }

    public override Status Update()
    {
        Status status = Status.INVALID;

        if(_subTreeRootBehavior != null)
        {
            _subtree.SetAgentMemory(_bt.GetAgentMemory());
            DebugUtils.Assert(_subtree.GetAgentMemory() != null);

            DebugUtils.Assert(_subtree.GetAgentMemory() == _bt.GetAgentMemory(), "Error, agent memory is not equal");
            DebugUtils.Assert(_subtree.GetSharedMemory() == _bt.GetSharedMemory());
            DebugUtils.Assert(_subtree.GetGlobalMemory() == _bt.GetGlobalMemory());

            status = _subTreeRootBehavior.Tick();
        }

        return status;
    }

    public override void OnTerminate()
    {
        base.OnTerminate();

        if(_subTreeRootBehavior != null)
        {
            ApplyOutputParameters();
        }
    }

    void ApplyInputParameters()
    {
        if(_inputs == null)
        {
            return;
        }

        for (int i = 0; i < _inputs.Length; ++i)
        {
            System.Object obj;
            _bt.GetMemoryObject<System.Object>(_inputs[0].Source, out obj);

            _subtree.SetMemoryObject(_inputs[0].Target, obj);
        }
    }
    
    void ApplyOutputParameters()
    {
        if(_outputs == null)
        {
            return;
        }

        for (int i = 0; i < _inputs.Length; ++i)
        {
            System.Object obj;
            _subtree.GetMemoryObject<System.Object>(_outputs[0].Source, out obj);
            
            _bt.SetMemoryObject(_outputs[0].Target, obj);
        }
    }
}
