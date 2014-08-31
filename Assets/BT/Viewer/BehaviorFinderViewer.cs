using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace MQMTech.AI.BT
{
    public class BehaviorFinderViewer
    {
        AvailableTreesModel _availableTrees;

        int _xPosition;
        int _yPosition;

        public BehaviorFinderViewer(AvailableTreesModel availableTrees)
        {
            _availableTrees = availableTrees;
        }

        public void Render(int xPosition, int yPosition)
        {
            _xPosition = xPosition;
            _yPosition = yPosition;

            bool found = FillBTreeDropDownListFromSelectedGO();
            if(found)
            {
                RenderBehaviorTreeSelector();
                _availableTrees.RootTree = _availableTrees.BehaviorsWithTree[_availableTrees.BtreesIndex].GetBehaviorTree();
            }
        }

        void RenderBehaviorTreeSelector()
        {
            GUI.BeginGroup(new Rect(_xPosition, _yPosition, Screen.width, 15));
            Color prevColor = GUI.color;
            GUI.color = Color.green;
            GUI.Label(new Rect(0f, 0f, 180, 15), "Trees in Entity:");
            GUI.color = prevColor;
            _yPosition += 15;
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(_xPosition, _yPosition, Screen.width, 40));
            _availableTrees.BtreesIndex = EditorGUILayout.Popup(_availableTrees.BtreesIndex, _availableTrees.BtreesNames, GUILayout.Width(248));
            GUI.EndGroup();
        }
        
        bool FillBTreeDropDownListFromSelectedGO()
        {
            GameObject activeGO = Selection.activeGameObject;
            if(activeGO != null)
            {
                _availableTrees.BehaviorsWithTree = GameObjectUtils.FindRecursiveComponentsByTypeDown<IBehaviorWithTree>(activeGO);
                if(_availableTrees.BehaviorsWithTree == null || _availableTrees.BehaviorsWithTree.Length <= 0)
                {
                    return false;
                }

                _availableTrees.BtreesNames = new string[_availableTrees.BehaviorsWithTree.Length];
                for (int i = 0; i < _availableTrees.BehaviorsWithTree.Length; ++i)
                {
                    IBehaviorWithTree btree = _availableTrees.BehaviorsWithTree[i];
                    if(btree == null)
                    {
                        return false;
                    }
                    BehaviorTree tree = btree.GetBehaviorTree();
                    if(tree == null)
                    {
                        return false;
                    }

                    _availableTrees.BtreesNames[i] = tree.Name;
                }
                
                return _availableTrees.BehaviorsWithTree.Length > 0;
            }
            
            return false;
        }
    }
}
