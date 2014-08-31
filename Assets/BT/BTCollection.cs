using System.Collections.Generic;
using MQMTech.AI.BT;

public class BTCollection
{
    static Dictionary<string, IBehaviorTreeBuilder> _btMap = new Dictionary<string, IBehaviorTreeBuilder>();
    
    public void Add(string name, IBehaviorTreeBuilder btree)
    {
        DebugUtils.Assert(!_btMap.ContainsKey(name));
        _btMap.Add(name, btree);
    }
    
    public bool Remove(string name)
    {
        return _btMap.Remove(name);
    }
    
    public IBehaviorTreeBuilder Find(string name)
    {
        IBehaviorTreeBuilder btreeBuilder;
        bool isFound = _btMap.TryGetValue(name, out btreeBuilder);
        DebugUtils.Assert(btreeBuilder!=null, "btreeBuilder!=null");

        return btreeBuilder;
    }
}
