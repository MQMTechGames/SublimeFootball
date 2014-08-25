using UnityEngine;
using System.Collections;

namespace MQMTech.AI.BT
{
    public static class BehaviorViewerUtils
    {
        public static string GetBehaviorName(Behavior behavior)
        {
            string name = behavior.GetType().ToString();
            if(!string.IsNullOrEmpty(behavior.Name))
            {
                name += ": " + behavior.Name;
            }
            
            return name;
        }
    }
}
