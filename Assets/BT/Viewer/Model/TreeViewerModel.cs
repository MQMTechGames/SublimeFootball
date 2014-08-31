using UnityEngine;
using System.Collections.Generic;

namespace MQMTech.AI.BT
{
    public class AvailableTreesModel
    {
        public BehaviorTree RootTree { get; set; } 
        public IBehaviorWithTree[] BehaviorsWithTree { get; set; }
        public string[] BtreesNames { get; set; }
        public int BtreesIndex { get; set; }
    }

    public class TreeHierarchyModel
    {
        public List<BehaviorTree> Children = new List<BehaviorTree>();
        public List<string> ChildrenNames = new List<string>();
        public int ChildrenIdx;
        public List<BehaviorTree> BehaviorsStack = new List<BehaviorTree>();
        public BehaviorTree RootTree { get; set; } 
        public BehaviorTree TopStackTree 
        { 
            get 
            {
                if(BehaviorsStack.Count > 0)
                {
                    return BehaviorsStack[BehaviorsStack.Count -1];
                }
                
                return null;
            }
        }

        public void PushTree(BehaviorTree bt)
        {
            if(bt != null)
            {
                BehaviorsStack.Add(bt);
            }
        }
    }

    public class TreeViewerModel 
    {
        public AvailableTreesModel AvailableTreesModel = new AvailableTreesModel();
        public TreeHierarchyModel TreeHierarchyModel = new TreeHierarchyModel();
        public BehaviorTree SelectedBehaviorTree 
        { 
            get 
            {
                return TreeHierarchyModel.TopStackTree;
            }
        }
    }
}
