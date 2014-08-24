using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MQMTech.AI.BT
{
    public class BehaviorTreeViewer : EditorWindow 
    {
    	const int kYOffsetSeparation = 2;
    	const int kXOffsetSeparation = 40;

    	const int kButtonWidth = 250;
    	const int kButtonHeight = 20;

    	const int kLabelWidth = 250;
    	const int kLabelHeight = 20;

        Vector2 scrollPos = Vector2.zero;

        IBehaviorWithTree[] _bWithTrees;
        string[] _btreesNames;
        int _btreesIndex;

        BehaviorTree _prevBT;
    	BehaviorTree _bt;

        List<BehaviorTree> _children = new List<BehaviorTree>();
        List<string> _childrenNames;
        int _childrenIdx;
        List<BehaviorTree> _behaviorStack = new List<BehaviorTree>();

        Vector2 windowsSize = Vector2.zero;
    	
    	[MenuItem ("Window/BehaviorTreeViewer")]
    	static void Init () 
    	{
    		BehaviorTreeViewer window = (BehaviorTreeViewer)EditorWindow.GetWindow (typeof (BehaviorTreeViewer));
    	}
    	
    	void OnGUI () 
    	{
            scrollPos = GUI.BeginScrollView (
                new Rect (0, 0, Screen.width, Screen.width), 
                scrollPos, 
                new Rect (0, 0, 15000, 15000)
                );

            _prevBT = _bt;
            _bt = null;

            bool found = FillBTreeDropDownListFromSelectedGO();
            if(found)
            {
                RenderBehaviorTreeSelector();
            }

    		if(_bt != null)
    		{
                if(_prevBT != _bt)
                {
                    InitBehaviorStack(_bt);
                }

                RenderStackOptions();

                BehaviorTree currentBt = _behaviorStack[_behaviorStack.Count -1];

                InitSubtreeArrays();

                FindSubtrees(currentBt.RootBehavior);

                AddSelectedChildrenIfNotNullToStack();

                windowsSize = RenderBehaviorTree(currentBt);
    		}

            GUI.EndScrollView ();
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
                    _childrenNames.Add(subtree.GetType().Name);
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

        void InitBehaviorStack(BehaviorTree bt)
        {
            _behaviorStack.Clear();

            _behaviorStack.Add(bt);
        }

        void RenderStackOptions()
        {
            int xOffset = 480;
            int yOffset = 60;

            if(_behaviorStack.Count > 1)
            {
                if(GUI.Button(new Rect(xOffset, yOffset, 100, 50), "Go to parent Tree"))
                {
                    _behaviorStack.RemoveAt(_behaviorStack.Count -1);
                }
            }
            else
            {
                GUI.Button(new Rect(xOffset, yOffset, 100, 50), " - ");
            }
        }

        void AddSelectedChildrenIfNotNullToStack()
        {
            _childrenIdx = EditorGUILayout.Popup(_childrenIdx, _childrenNames.ToArray());
            if(_childrenIdx > 0)
            {
                BehaviorTree bt = _children[_childrenIdx];
                if(bt != null)
                {
                    _behaviorStack.Add(bt);
                }
            }

            _childrenIdx = 0;
        }

        void RenderBehaviorTreeSelector()
        {
            _btreesIndex = EditorGUILayout.Popup(_btreesIndex, _btreesNames);
            _bt = _bWithTrees[_btreesIndex].GetBehaviorTree();
        }

        bool FillBTreeDropDownListFromSelectedGO()
        {
            GameObject activeGO = Selection.activeGameObject;
            if(activeGO != null)
            {
                _bWithTrees = GameObjectUtils.FindRecursiveComponentsByTypeDown<IBehaviorWithTree>(activeGO);

                _btreesNames = new string[_bWithTrees.Length];
                for (int i = 0; i < _bWithTrees.Length; ++i)
                {
                    IBehaviorWithTree btree = _bWithTrees[i];
                    _btreesNames[i] = btree.GetType().Name;
                }

                return _bWithTrees.Length > 0;
            }

            return false;
        }

    	void Update()
    	{
    		Repaint();
    	}

    	Vector2 RenderBehaviorTree(BehaviorTree bt)
    	{
    		Behavior behavior = bt.GetRootBehavior();
    		DebugUtils.Assert(behavior!=null, "behavior!=null");

    		int xOffset = 10;
    		int yOffset = 40;
    		RenderBehavior(behavior, xOffset, ref yOffset, Behavior.Status.SUCCESS);

            if(_bt.MemoryManager.GlobalMemory != null)
            {
                Color prevColor = GUI.color;
                GUI.color = Color.green;
                GUI.Label (new Rect ((float)xOffset, (float)yOffset, kLabelWidth, kButtonHeight), "Global Memory");
                GUI.color = prevColor;
                RenderMemoryVariables(xOffset, ref  yOffset, _bt.MemoryManager.GlobalMemory.Map);
                
                yOffset += 20;
            }

            if(_bt.MemoryManager.SharedMemory != null)
            {
                Color prevColor = GUI.color;
                GUI.color = Color.green;
                GUI.Label (new Rect ((float)xOffset, (float)yOffset, kLabelWidth, kButtonHeight), "Shared Memory");
                GUI.color = prevColor;
                RenderMemoryVariables(xOffset, ref yOffset, _bt.MemoryManager.SharedMemory.Map);
                
                yOffset += 20;
            }

            if(_bt.MemoryManager.AgentMemory != null)
            {
                Color prevColor = GUI.color;
                GUI.color = Color.green;
                GUI.Label (new Rect ((float)xOffset, (float)yOffset, kLabelWidth, kButtonHeight), "Agent Memory");
                GUI.color = prevColor;
                RenderMemoryVariables(xOffset, ref yOffset, _bt.MemoryManager.AgentMemory.Map);

                yOffset += 20;
            }

            if(_bt.MemoryManager.LocalMemory != null)
            {
                Color prevColor = GUI.color;
                GUI.color = Color.green;
                GUI.Label (new Rect ((float)xOffset, (float)yOffset, kLabelWidth, kButtonHeight), "Local Memory");
                GUI.color = prevColor;
                RenderMemoryVariables(xOffset, ref yOffset, _bt.MemoryManager.LocalMemory.Map);
                
                yOffset += 20;
            }
            return new Vector2(xOffset, yOffset);
    	}

    	void RenderBehavior(Behavior behavior, int xOffset, ref int yOffset, Behavior.Status parentStatus)
    	{
    		int myXOffset = xOffset;
    		int myYOffset = yOffset;

    		string name = GetBehaviorName(behavior);

    		Behavior.Status status = parentStatus;
            if(parentStatus != Behavior.Status.FAILURE && parentStatus != Behavior.Status.INVALID)
    		{
    			status = behavior.BehaviorStatus;
    		}
    		RenderBox(name, status, xOffset, yOffset);

    		yOffset += kButtonHeight + kYOffsetSeparation;
    		xOffset += kXOffsetSeparation;

    		List<Behavior> children = behavior.GetChildren();
    		if(children == null)
    		{
    			return;
    		}

    		foreach (Behavior child in children) 
    		{
    			Handles.BeginGUI();
    			Handles.DrawLine( 
    			                 new Vector3(myXOffset, myYOffset + kButtonHeight / 2, 0f),
    			                 new Vector3(myXOffset, yOffset + kButtonHeight / 2,   0f));

    			Handles.DrawLine( 
    			                 new Vector3(myXOffset, yOffset + kButtonHeight / 2, 0f),
    			                 new Vector3(xOffset, yOffset + kButtonHeight / 2,   0f));
    			Handles.EndGUI();

    			RenderBehavior(child, xOffset, ref yOffset, status);
    		}
    	}

    	void RenderMemoryVariables(int xOffset, ref int yOffset, Dictionary<int, object> map)
    	{
    		yOffset += 20;

    		foreach (KeyValuePair<int, System.Object> keyPair in map) 
    		{
                int key = keyPair.Key;
                string keyName = MemoryKeysHashCodeManager.GetMemoryNameByHashCode(key);
                GUI.Label (new Rect ((float)xOffset, (float)yOffset, kLabelWidth, kButtonHeight), keyName);

                object value = keyPair.Value;
                string valueString = value == null ? "null" : value.ToString();
                GUI.Label (new Rect ((float)xOffset + kLabelWidth, (float)yOffset, kLabelWidth, kButtonHeight), valueString);

    			yOffset += 20;
    		}
    	}

    	string GetBehaviorName(Behavior behavior)
    	{
    		string name = behavior.GetType().ToString();
    		if(!string.IsNullOrEmpty(behavior.Name))
    		{
    			name += ": " + behavior.Name;
    		}

    		return name;
    	}

    	void RenderBox(string name, Behavior.Status status, int xOffset, int yOffset)
    	{
    		Color color = GetColorFromStatus(status);
    		GUI.backgroundColor = color;
    		GUI.contentColor = Color.white;

    		GUI.Button (new Rect ((float)xOffset, yOffset, kButtonWidth, kButtonHeight), name);
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
