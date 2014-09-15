//using UnityEngine;
//using System.Collections.Generic;
//
//public static class MemoryKeysHashCodeManager 
//{
//    static Dictionary<string, AIMemoryKey> _nameMap = new Dictionary<string, AIMemoryKey>();
//    static Dictionary<int, string> _codeMap = new Dictionary<int, string>();
//
//    public static int RegisterMemoryKey(AIMemoryKey key)
//    {
//        string nameContext = CreateNameContextString(key.Name, key.Context);
//        bool containsNameContext = _nameMap.ContainsKey(nameContext);
//        DebugUtils.Assert(!containsNameContext, "the name: " + nameContext + " is already inserted in the same context");
//
//        int hashCode = nameContext.GetHashCode();
//        bool containsHashCode = _codeMap.ContainsKey(hashCode);
//        if(containsHashCode)
//        {
//            Debug.LogWarning("Key HashCode for: " + nameContext + " already exist, trying again with some variation");
//            nameContext = nameContext + "_01";
//            hashCode = nameContext.GetHashCode();
//        }
//        containsHashCode = _codeMap.ContainsKey(hashCode);
//        DebugUtils.Assert(!containsHashCode, "the hash code already exist");
//
//        _nameMap.Add(nameContext, key);
//        _codeMap.Add(hashCode, nameContext);
//
//        return hashCode;
//    }
//
//    public static string GetMemoryNameByHashCode(int hashCode)
//    {
//        return _codeMap[hashCode];
//    }
//
//    public static bool TryMemoryMemoryKeyByNameAndContext(string name, AIMemoryKey.ContextType context, out AIMemoryKey key)
//    {
//        string nameContext = CreateNameContextString(name, context);
//
//        return _nameMap.TryGetValue(nameContext, out key);
//    }
//
//    static string CreateNameContextString(string name, AIMemoryKey.ContextType context)
//    {
//        return name + "_" + context.ToString();
//    }
//
//    public static void Clear()
//    {
//        _nameMap.Clear();
//        _codeMap.Clear();
//    }
//}
