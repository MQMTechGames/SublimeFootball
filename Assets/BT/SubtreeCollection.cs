using System.Collections.Generic;
using MQMTech.AI.BT;

public class SubtreeCollection
{
    static Dictionary<string, IBehaviorTreeBuilder> _btMap = new Dictionary<string, IBehaviorTreeBuilder>();
    static Dictionary<string, TreeLinkConfiguration> _btMapConfig = new Dictionary<string, TreeLinkConfiguration>();
    
    public void Add(string name, IBehaviorTreeBuilder btree, TreeLinkConfiguration linkConfig)
    {
        DebugUtils.Assert(!_btMap.ContainsKey(name));
        DebugUtils.Assert(!_btMapConfig.ContainsKey(name));
        
        _btMap.Add(name, btree);
        _btMapConfig.Add(name, linkConfig);
    }
    
    public bool Remove(string name)
    {
        return _btMap.Remove(name);
        return _btMapConfig.Remove(name);
    }
    
    public BehaviorTree FindTree(string name)
    {
        IBehaviorTreeBuilder btreeBuilder;
        bool isFound = _btMap.TryGetValue(name, out btreeBuilder);
        DebugUtils.Assert(btreeBuilder!=null, "btreeBuilder!=null");
        
        BehaviorTree btree = btreeBuilder.Create();
        
        return btree;
    }

    public bool FindLinkConfiguration(string name, out TreeLinkConfiguration linkConfig)
    {
        bool isFound = _btMapConfig.TryGetValue(name, out linkConfig);
        DebugUtils.Assert(linkConfig!=null, "btreeBuilder!=null");
        
        return isFound;
    }
}