using UnityEngine;
using System.Collections.Generic;

public static class MemoryKeysHashCodeManager 
{
    static Dictionary<string, int> _nameMap = new Dictionary<string, int>();
    static Dictionary<int, string> _codeMap = new Dictionary<int, string>();

    public static int RegisterMemoryKey(AIMemoryKey key)
    {
        string nameContext = key.Name + "_" + key.Context.ToString();
        bool containsNameContext = _nameMap.ContainsKey(nameContext);
        DebugUtils.Assert(!containsNameContext, "the name: " + nameContext + " is already inserted in the same context");

        int hashCode = nameContext.GetHashCode();
        bool containsHashCode = _codeMap.ContainsKey(hashCode);
        if(containsHashCode)
        {
            Debug.LogWarning("Key HashCode for: " + nameContext + " already exist, trying again with some variation");
            nameContext = nameContext + "_01";
            hashCode = nameContext.GetHashCode();
        }
        containsHashCode = _codeMap.ContainsKey(hashCode);
        DebugUtils.Assert(!containsHashCode, "the hash code already exist");

        _codeMap.Add(hashCode, nameContext);
        _nameMap.Add(nameContext, hashCode);

        return hashCode;
    }

    public static string GetMemoryNameByHashCode(int hashCode)
    {
        return _codeMap[hashCode];
    }

    public static void Clear()
    {
        _nameMap.Clear();
        _codeMap.Clear();
    }
}
