using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace MQMTech.AI.BT
{
    public class TreeHierarchyViewer
    {
        const int kYMargin = 10;

        int _xPosition;
        public int XPosition { get { return _xPosition; } private set { _xPosition = value; } }

        int _yPosition;
        public int YPosition { get { return _yPosition; } private set { _yPosition = value; } }

        TreeHierarchyModel _hierarchyModel;

        public TreeHierarchyViewer(TreeHierarchyModel model)
        {
            _hierarchyModel = model;
        }

        public void Init(BehaviorTree tree)
        {
            _hierarchyModel.RootTree = tree;

            _hierarchyModel.BehaviorsStack.Clear();
            
            _hierarchyModel.BehaviorsStack.Add(_hierarchyModel.RootTree);
        }

        public void Render(int xPosition, int yPosition)
        {
            _xPosition = xPosition;
            _yPosition = yPosition;

            RenderStackOptions();

            CleanAndFindChildrenFromTree(_hierarchyModel.TopStackTree);
            AddSelectedChildrenIfNotNullToStack();
        }

        void CleanAndFindChildrenFromTree(BehaviorTree tree)
        {
            InitSubtreeArrays();
            FindSubtrees(tree.GetRootBehavior());
        }

        void InitSubtreeArrays()
        {
            _hierarchyModel.ChildrenNames.Clear();
            _hierarchyModel.ChildrenNames.Add("Select a Subtree");
            
            _hierarchyModel.Children.Clear();
            _hierarchyModel.Children.Add(null);
        }

        void FindSubtrees(Behavior behavior)
        {
            DebugUtils.Assert(behavior != null);
            
            if(behavior is SubtreeBehavior)
            {
                BehaviorTree subtree = ((SubtreeBehavior) behavior).Subtree;
                if(subtree != null)
                {
                    _hierarchyModel.Children.Add(subtree);
                    _hierarchyModel.ChildrenNames.Add(subtree.Name);
                }
            }
            
            List<Behavior> children = behavior.GetChildren();
            if(children == null)
            {
                return;
            }
            
            foreach (Behavior child in children)
            {
                FindSubtrees(child);
            }
        }

        void RenderStackOptions()
        {
            int buttonHeight = 15;
            int xPosition = 0;
            int yPosition = 0;

            GUI.BeginGroup(new Rect(XPosition, YPosition, 480, 15));
            Color prevColor = GUI.color;
            GUI.color = Color.green;
            GUI.Label(new Rect(0f, 0f, 180, 15), "Subtree hierarchy:");
            GUI.color = prevColor;
            yPosition += 5 + kYMargin;
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(XPosition, YPosition, 480, 60));
            if(_hierarchyModel.BehaviorsStack.Count > 1)
            {
                for (int i = 0; i < _hierarchyModel.BehaviorsStack.Count; ++i) 
                {
                    BehaviorTree tree = _hierarchyModel.BehaviorsStack[i];
                    if(GUI.Button(new Rect(xPosition, yPosition, 180, buttonHeight), tree.Name))
                    {
                        RemoveFromStackTill(tree);
                        break;
                    }
                    xPosition += 185;

                    if(i < _hierarchyModel.BehaviorsStack.Count -1)
                    {
                        GUI.Label(new Rect(xPosition, yPosition, 25, buttonHeight), "->");
                        xPosition += 30;
                    }
                }
            }
            else
            {
                GUI.Button(new Rect(xPosition, yPosition, 180, buttonHeight), _hierarchyModel.RootTree.Name);
            }
            
            YPosition += 25 + kYMargin;
            
            GUI.EndGroup();
        }

        void RemoveFromStackTill(BehaviorTree tree)
        {
            int idx = 0;
            for (; idx < _hierarchyModel.BehaviorsStack.Count; ++idx)
            {
                if(_hierarchyModel.BehaviorsStack[idx] == tree)
                {
                    break;
                }
            }

            List<BehaviorTree> treesToRemove = new List<BehaviorTree>();
            for (int i = idx +1; i < _hierarchyModel.BehaviorsStack.Count; i++)
            {
                treesToRemove.Add(_hierarchyModel.BehaviorsStack[i]);
            }

            for (int i = 0; i < treesToRemove.Count; i++)
            {
                _hierarchyModel.BehaviorsStack.Remove(treesToRemove[i]);
            }
        }

        void AddSelectedChildrenIfNotNullToStack()
        {
            GUI.BeginGroup(new Rect(XPosition, YPosition, 480, 20));

            Color prevColor = GUI.color;
            GUI.color = Color.green;
            GUI.Label(new Rect(0f, 0f, 180, 15), "Select a Subtree:");
            GUI.color = prevColor;
            YPosition += 0;
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(XPosition, YPosition, Screen.width, 60));
            _hierarchyModel.ChildrenIdx = EditorGUILayout.Popup(_hierarchyModel.ChildrenIdx, _hierarchyModel.ChildrenNames.ToArray(), GUILayout.Width (248) );
            if(_hierarchyModel.ChildrenIdx > 0)
            {
                BehaviorTree bt = _hierarchyModel.Children[_hierarchyModel.ChildrenIdx];
                _hierarchyModel.PushTree(bt);
            }
            GUI.EndGroup();

            YPosition += 25 + kYMargin;
            
            _hierarchyModel.ChildrenIdx = 0;
        }
    }
}
