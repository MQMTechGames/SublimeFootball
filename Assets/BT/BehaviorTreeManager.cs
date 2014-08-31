using UnityEngine;
using MQMTech.AI.BT;

public static class BehaviorTreeManager
{
    static BTCollection _bTreeCollection = new BTCollection();
    static SubtreeCollection _btSubtreeCollection = new SubtreeCollection();

    public static void AddTree(string name, IBehaviorTreeBuilder tree)
    {
        _bTreeCollection.Add(name, tree);
    }

    public static BehaviorTree CreateTreeInstance(string name, Memory localMemory, Memory agentMemory, Memory sharedMemory, Memory globalMemory, TreeParameters parameters)
    {
        IBehaviorTreeBuilder treeBuilder = _bTreeCollection.Find(name);
        DebugUtils.Assert(treeBuilder != null);

        BehaviorTree tree = treeBuilder.Create();

        tree.SetLocalMemory(localMemory);
        tree.SetAgentMemory(agentMemory);
        tree.SetSharedMemory(sharedMemory);
        tree.SetGlobalMemory(globalMemory);

        for (int i = 0; i < parameters.Inputs.Count; ++i)
        {
            tree.SetMemoryObject(parameters.Inputs[i].Key, parameters.Inputs[i].Value);
        }

        return tree;
    }

    public static void AddSubtree(string name, IBehaviorTreeBuilder tree, TreeLinkConfiguration linkConfig)
    {
        _btSubtreeCollection.Add(name, tree, linkConfig);
    }

    public static BehaviorTree FindSubTree(string name)
    {
        return _btSubtreeCollection.FindTree(name);
    }

    public static bool FindSubTreeConfiguration(string name, out TreeLinkConfiguration linkConfiguration)
    {
        return _btSubtreeCollection.FindLinkConfiguration(name, out linkConfiguration);
    }
}
