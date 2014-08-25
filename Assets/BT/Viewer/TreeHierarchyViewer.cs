using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace MQMTech.AI.BT
{
    public class TreeHierarchyViewer
    {
        const int kYMargin = 10;

        public BehaviorTree RootTree { get; set; }

        int _xPosition;
        public int XPosition { get { return _xPosition; } private set { _xPosition = value; } }

        int _yPosition;
        public int YPosition { get { return _yPosition; } private set { _yPosition = value; } }

        List<BehaviorTree> _children = new List<BehaviorTree>();
        List<string> _childrenNames = new List<string>();
        int _childrenIdx;
        List<BehaviorTree> _behaviorStack = new List<BehaviorTree>();

        public BehaviorTree TopStackTree 
        { 
            get 
            {
                if(_behaviorStack.Count > 0)
                {
                    return _behaviorStack[_behaviorStack.Count -1];
                }

                return null;
            }
        }

        public void Init(BehaviorTree tree)
        {
            RootTree = tree;

            _behaviorStack.Clear();
            
            _behaviorStack.Add(RootTree);
        }

        public void Render(int xPosition, int yPosition)
        {
            _xPosition = xPosition;
            _yPosition = yPosition;

            RenderStackOptions();

            CleanAndFindChildrenFromTree(TopStackTree);
            AddSelectedChildrenIfNotNullToStack();
        }

        void CleanAndFindChildrenFromTree(BehaviorTree tree)
        {
            InitSubtreeArrays();
            FindSubtrees(tree.GetRootBehavior());
        }

        void InitSubtreeArrays()
        {
            _childrenNames.Clear();
            _childrenNames.Add("Select a Subtree");
            
            _children.Clear();
            _children.Add(null);
        }

        void FindSubtrees(Behavior behavior)
        {
            DebugUtils.Assert(behavior != null);
            
            if(behavior is SubtreeBehavior)
            {
                BehaviorTree subtree = ((SubtreeBehavior) behavior).Subtree;
                if(subtree != null)
                {
                    _children.Add(subtree);
                    _childrenNames.Add(subtree.Name);
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
            if(_behaviorStack.Count > 1)
            {
                for (int i = 0; i < _behaviorStack.Count; ++i) 
                {
                    BehaviorTree tree = _behaviorStack[i];
                    if(GUI.Button(new Rect(xPosition, yPosition, 180, buttonHeight), tree.Name))
                    {
                        RemoveFromStackTill(tree);
                        break;
                    }
                    xPosition += 185;

                    if(i < _behaviorStack.Count -1)
                    {
                        GUI.Label(new Rect(xPosition, yPosition, 25, buttonHeight), "->");
                        xPosition += 30;
                    }
                }
            }
            else
            {
                GUI.Button(new Rect(xPosition, yPosition, 180, buttonHeight), RootTree.Name);
            }
            
            YPosition += 25 + kYMargin;
            
            GUI.EndGroup();
        }

        void RemoveFromStackTill(BehaviorTree tree)
        {
            int idx = 0;
            for (; idx < _behaviorStack.Count; ++idx)
            {
                if(_behaviorStack[idx] == tree)
                {
                    break;
                }
            }

            List<BehaviorTree> treesToRemove = new List<BehaviorTree>();
            for (int i = idx +1; i < _behaviorStack.Count; i++)
            {
                treesToRemove.Add(_behaviorStack[i]);
            }

            for (int i = 0; i < treesToRemove.Count; i++)
            {
                _behaviorStack.Remove(treesToRemove[i]);
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
            _childrenIdx = EditorGUILayout.Popup(_childrenIdx, _childrenNames.ToArray(), GUILayout.Width (248) );
            if(_childrenIdx > 0)
            {
                BehaviorTree bt = _children[_childrenIdx];
                if(bt != null)
                {
                    _behaviorStack.Add(bt);
                }
            }
            GUI.EndGroup();

            YPosition += 25 + kYMargin;
            
            _childrenIdx = 0;
        }
    }
}
