using UnityEngine;
using System.Collections.Generic;
using MQMTech.AI.BT;

public class SubtreeBehavior : Behavior
{
    private BehaviorTree _subtree;
    private Behavior _subtreeBehavior;
    private TreeLinkConfiguration _linkConfiguration;

    public BehaviorTree Subtree { get { return _subtree; } }

    SubTreeParameters _parameters = new SubTreeParameters();

    protected string _subtreeName;
    public string SubtreeName { get { return _subtreeName; } }

    public SubtreeBehavior(string subtreeName)
    {
        _subtreeName = subtreeName;
    }

    public void AddInputParameter(AIMemoryKeyPair pair)
    {
        _parameters.AddInputParameter(pair);
    }

    public void AddOutputParameter(AIMemoryKeyPair pair)
    {
        _parameters.AddOutputParameter(pair);
    }

    public override void OnInitialize()
    {
        base.OnInitialize();

        if(_subtreeBehavior == null)
        {
            InitSubtree();
        }

        _subtreeBehavior.OnReset();

        ApplyInputParameters();
    }

    void ApplyInputParameters()
    {
        if(_parameters.Inputs == null)
        {
            return;
        }
        
        for (int i = 0; i < _parameters.Inputs.Count; ++i)
        {
            System.Object obj;
            _bt.GetMemoryObject<System.Object>(_parameters.Inputs[i].Source, out obj);
            
            _subtree.SetMemoryObject(_parameters.Inputs[i].Target, obj);
        }
    }
    
    void ApplyOutputParameters()
    {
        if(_parameters.Outputs == null)
        {
            return;
        }
        
        for (int i = 0; i < _parameters.Inputs.Count; ++i)
        {
            System.Object obj;
            _subtree.GetMemoryObject<System.Object>(_parameters.Outputs[i].Source, out obj);
            
            _bt.SetMemoryObject(_parameters.Outputs[i].Target, obj);
        }
    }

    void InitSubtree()
    {
        _subtree = BehaviorTreeManager.FindSubTree(_subtreeName);
        DebugUtils.Assert(_subtree!=null, "_subtree!=null");

        bool isOk = BehaviorTreeManager.FindSubTreeConfiguration(_subtreeName, out _linkConfiguration);
        DebugUtils.Assert(isOk, "linkConfiguration is not found");

        _linkConfiguration.ConfigureTreeMemory(_subtree, _bt);
        _linkConfiguration.CheckAllSubtreeMemory(_subtree, _bt);

        _subtreeBehavior = _subtree.GetRootBehavior();
        DebugUtils.Assert(_subtreeBehavior!=null, "_subTreeRootBehavior!=null");
    }

    public override Status Update()
    {
        Status status = Status.INVALID;

        if(_subtreeBehavior != null)
        {
            _linkConfiguration.CheckAllSubtreeMemory(_subtree, _bt);

            status = _subtreeBehavior.Tick();
        }

        return status;
    }

    public override void OnTerminate()
    {
        base.OnTerminate();

        if(_subtreeBehavior != null)
        {
            ApplyOutputParameters();
        }
    }
}
