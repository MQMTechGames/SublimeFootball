using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace MQMTech.AI.BT
{
    public class TreeViewer
    {
        BehaviorTree _bt;

        int _xPosition;
        public int XPosition { get { return _xPosition; } }

        int _yPosition;
        public int YPosition { get { return _yPosition; } }

        const int kYOffsetSeparation = 2;
        const int kXOffsetSeparation = 40;
        
        const int kButtonWidth = 250;
        const int kButtonHeight = 20;
        
        const int kLabelWidth = 250;
        const int kLabelHeight = 20;

        TreeHierarchyModel _treeHierarchyModel;
        public TreeViewer(TreeHierarchyModel model)
        {
            _treeHierarchyModel = model;
        }

        public void Render(BehaviorTree bt, int xPosition, int yPosition)
        {
            _bt = bt;
            _xPosition = xPosition;
            _yPosition = yPosition;

            Behavior behavior = bt.GetRootBehavior();
            DebugUtils.Assert(behavior!=null, "behavior!=null");

            RenderTitle();
            RenderBehavior(behavior, _xPosition, ref _yPosition, Behavior.Status.SUCCESS);
        }
        void RenderTitle()
        {
            GUI.BeginGroup(new Rect(_xPosition, _yPosition, 480, 20));
            Color prevColor = GUI.color;
            GUI.color = Color.green;
            GUI.Label(new Rect(0f, 0f, 180, 15), "Behavior Tree Viewer:");
            GUI.color = prevColor;
            _yPosition += 25;
            GUI.EndGroup();
        }
        
        void RenderBehavior(Behavior behavior, int xPosition, ref int yPosition, Behavior.Status parentStatus)
        {
            int behaviorXposition = xPosition;
            int behaviorYPosition = yPosition;
            
            string name = BehaviorViewerUtils.GetBehaviorName(behavior);
            
            Behavior.Status status = parentStatus;
            if(parentStatus != Behavior.Status.FAILURE && parentStatus != Behavior.Status.INVALID)
            {
                status = behavior.BehaviorStatus;
            }
            Vector2 size = RenderNode(behavior, name, status, xPosition, yPosition);
            
            yPosition += ((int)size.y) + kYOffsetSeparation;
            xPosition += kXOffsetSeparation;
            
            List<Behavior> children = behavior.GetChildren();
            if(children == null)
            {
                return;
            }

            foreach (Behavior child in children) 
            {
                Handles.BeginGUI();
                Handles.DrawLine( 
                                 new Vector3(behaviorXposition, behaviorYPosition + kButtonHeight / 2, 0f),
                                 new Vector3(behaviorXposition, yPosition + kButtonHeight / 2, 0f));
                
                Handles.DrawLine( 
                                 new Vector3(behaviorXposition, yPosition + kButtonHeight / 2, 0f),
                                 new Vector3(xPosition, yPosition + kButtonHeight / 2, 0f));  
                Handles.EndGUI();
                
                RenderBehavior(child, xPosition, ref yPosition, status);
            }
        }

        Vector2 RenderNode(Behavior behavior, string name, Behavior.Status status, int xOffset, int yOffset)
        {
            Color color = GetColorFromStatus(status);
            GUI.backgroundColor = color;
            GUI.contentColor = Color.white;
         
            int maxWidth = kButtonWidth;
            int maxHeight = kButtonHeight;

            bool isSubtree = false;

            if(behavior is SubtreeBehavior)
            {
                int extraWidth = 6;
                int extraHeight = 6;

                int BoxXOffset = xOffset - extraWidth;
                int BoxYOffset = yOffset;

                int BoxWidth = kButtonWidth + 2*extraWidth;
                int BoxHeight = kButtonHeight + 2*extraHeight;

                maxWidth = BoxWidth;
                maxHeight = BoxHeight;

                GUI.backgroundColor = Color.white;
                if( GUI.Button(new Rect ((float) BoxXOffset, (float) BoxYOffset, BoxWidth, BoxHeight), ""))
                {
                    _treeHierarchyModel.PushTree(((SubtreeBehavior)behavior).Subtree);
                }

                GUI.backgroundColor = color;

                yOffset += extraHeight;

                name = ((SubtreeBehavior) behavior).SubtreeName;
                isSubtree = true;
            }

            if ( GUI.Button (new Rect ((float) xOffset, (float) yOffset, kButtonWidth, kButtonHeight), name) )
            {
                if(isSubtree)
                {
                    _treeHierarchyModel.PushTree(((SubtreeBehavior)behavior).Subtree);
                }
            }

            return new Vector2(maxWidth, maxHeight);
        }
        
        Color GetColorFromStatus(Behavior.Status status)
        {
            switch(status)
            {
                case Behavior.Status.INVALID:
                    return Color.gray;
                    break;
                    
                case Behavior.Status.FAILURE:
                    return Color.red;
                    break;
                    
                case Behavior.Status.SUCCESS:
                    return Color.green;
                    break;
                    
                case Behavior.Status.RUNNING:
                    return Color.cyan;
                    break;
            }
            
            return Color.magenta;
        }
    }
}
