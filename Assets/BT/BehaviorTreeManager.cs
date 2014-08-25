using System.Collections.Generic;
using MQMTech.AI.BT;

public static class BehaviorTreeManager
{
    static Dictionary<string, BehaviorTree> _map = new Dictionary<string, BehaviorTree>();
    static Dictionary<string, IBehaviorTreeBuilder> _builderMap = new Dictionary<string, IBehaviorTreeBuilder>();

    public static void Add(string name, BehaviorTree btree)
    {
        DebugUtils.Assert(!_map.ContainsKey(name));
        _map.Add(name, btree);
    }

    public static bool Remove(string name)
    {
        return _map.Remove(name);
    }

    public static BehaviorTree FindBehaviorTree(string name)
    {
        BehaviorTree btree;
        bool isFound = _map.TryGetValue(name, out btree);

        return isFound ? btree : null;
    }
    
    public static void AddBuilder(string name, IBehaviorTreeBuilder btree)
    {
        DebugUtils.Assert(!_builderMap.ContainsKey(name));
        _builderMap.Add(name, btree);
    }
    
    public static bool RemoveBuilder(string name)
    {
        return _builderMap.Remove(name);
    }
    
    public static IBehaviorTreeBuilder FindBuilder(string name)
    {
        IBehaviorTreeBuilder btree;
        bool isFound = _builderMap.TryGetValue(name, out btree);
        
        return isFound ? btree : null;
    }
}
