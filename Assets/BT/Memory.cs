using UnityEngine;
using System.Collections.Generic;

public class Memory
{
    Dictionary<int, System.Object> _map = new Dictionary<int, object>();
    public Dictionary<int, System.Object> Map { get{ return _map; } }

    public bool GetMemoryObject<T>(int name, out T oVar)
    {
        oVar = default(T);
        System.Object obj;
        bool isOk = _map.TryGetValue(name, out obj);
        isOk &= obj is T;
        
        if(isOk)
        {
            oVar = (T) obj;
        }
        
        return isOk;
    }
    
    public void SetMemoryObject<T>(int name, T oVar)
    {
        if (ContainsMemoryObject(name)) 
        {
            RemoveMemoryObject(name);
        }
        
        _map.Add(name, oVar);
    }
    
    public bool RemoveMemoryObject(int name)
    {
        if (ContainsMemoryObject(name)) 
        {
            _map.Remove(name);
            
            return true;
        }
        
        return false;
    }
    
    public bool ContainsMemoryObject(int name)
    {
        return _map.ContainsKey(name);
    }
}
