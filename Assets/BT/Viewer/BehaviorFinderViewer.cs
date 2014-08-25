using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace MQMTech.AI.BT
{
    public class BehaviorFinderViewer
    {
        public BehaviorTree Tree { get; private set; } 

        IBehaviorWithTree[] _bWithTrees;
        string[] _btreesNames;
        int _btreesIndex;

        int _xPosition;
        int _yPosition;

        public void Render(int xPosition, int yPosition)
        {
            _xPosition = xPosition;
            _yPosition = yPosition;

            bool found = FillBTreeDropDownListFromSelectedGO();
            if(found)
            {
                RenderBehaviorTreeSelector();
                Tree = _bWithTrees[_btreesIndex].GetBehaviorTree();
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
            _btreesIndex = EditorGUILayout.Popup(_btreesIndex, _btreesNames, GUILayout.Width(248));
            GUI.EndGroup();
        }
        
        bool FillBTreeDropDownListFromSelectedGO()
        {
            GameObject activeGO = Selection.activeGameObject;
            if(activeGO != null)
            {
                _bWithTrees = GameObjectUtils.FindRecursiveComponentsByTypeDown<IBehaviorWithTree>(activeGO);
                if(_bWithTrees == null || _bWithTrees.Length <= 0)
                {
                    return false;
                }

                _btreesNames = new string[_bWithTrees.Length];
                for (int i = 0; i < _bWithTrees.Length; ++i)
                {
                    IBehaviorWithTree btree = _bWithTrees[i];
                    if(btree == null)
                    {
                        return false;
                    }
                    BehaviorTree tree = btree.GetBehaviorTree();
                    if(tree == null)
                    {
                        return false;
                    }

                    _btreesNames[i] = tree.Name;
                }
                
                return _bWithTrees.Length > 0;
            }
            
            return false;
        }
    }
}
