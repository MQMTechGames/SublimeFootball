using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MQMTech.AI.BT
{
    public class BehaviorTreeViewer : EditorWindow 
    {
        const int kYMargin = 20;
        const int kRightMargin = 20;

        TreeViewerModel _treeViewerModel = new TreeViewerModel();

        BehaviorFinderViewer _behaviorFinderViewer;
        TreeHierarchyViewer _treeHierarchyViewer;
        TreeViewer _viewer;
        MemoryViewer _memoryViewer;

        Vector2 _treeViewerScroll;
        Vector2 _memoryViewerScroll;

        int _xPosition;
        int _yPosition;

        int _treeHierarchyXSize;
        int _treeHierarchyYSize;

        int _treeViewerXSize;
        int _treeViewerYSize;

        int _memoryViewerYSize;
        int _memoryViewerXSize;

        public BehaviorTreeViewer()
        {
            _behaviorFinderViewer = new BehaviorFinderViewer(_treeViewerModel.AvailableTreesModel);
            _treeHierarchyViewer = new TreeHierarchyViewer(_treeViewerModel.TreeHierarchyModel);
            _viewer = new TreeViewer(_treeViewerModel.TreeHierarchyModel);
            _memoryViewer = new MemoryViewer();
        }

    	[MenuItem ("Window/BehaviorTreeViewer")]
    	static void Init () 
    	{
    		BehaviorTreeViewer window = (BehaviorTreeViewer)EditorWindow.GetWindow (typeof (BehaviorTreeViewer));
    	}
    	
    	void OnGUI () 
    	{
            _xPosition = 10;
            _yPosition = 10;

            _treeHierarchyXSize = 480;
            _treeHierarchyYSize = 65;

            _treeViewerYSize = Screen.height / 2;
            _treeViewerXSize = Screen.width;

            _memoryViewerYSize = (Screen.height / 2) - (2 * kYMargin);
            _memoryViewerXSize = Screen.width;

            _behaviorFinderViewer.Render(_xPosition, _yPosition);
            _yPosition += 40;

            if(_treeViewerModel.AvailableTreesModel.RootTree != null)
    		{
                if(_treeViewerModel.TreeHierarchyModel.RootTree != _treeViewerModel.AvailableTreesModel.RootTree)
                {
                    _treeHierarchyViewer.Init(_treeViewerModel.AvailableTreesModel.RootTree);
                }

                _treeHierarchyViewer.Render(_xPosition, _yPosition);
                _yPosition += _treeHierarchyYSize + kYMargin;

                GUI.BeginGroup(new Rect(_xPosition, _yPosition, _treeViewerXSize, _treeViewerYSize));
                _treeViewerScroll = GUI.BeginScrollView (
                    new Rect (0, 0, _treeViewerXSize - kRightMargin, _treeViewerYSize), 
                    _treeViewerScroll, 
                    new Rect (0, 0, 15000, 15000)
                    );

                _viewer.Render(_treeViewerModel.SelectedBehaviorTree, 0, 0);
                _yPosition += _treeViewerYSize + kYMargin;

                GUI.EndScrollView ();
                GUI.EndGroup ();

                GUI.BeginGroup(new Rect(_xPosition, _yPosition, _memoryViewerXSize, _memoryViewerYSize));
                _memoryViewerScroll = GUI.BeginScrollView (
                    new Rect (0, 0, _memoryViewerXSize - kRightMargin, _memoryViewerYSize), 
                    _memoryViewerScroll, 
                    new Rect (0, 0, 15000, 15000)
                    );
                _memoryViewer.Render(_treeViewerModel.SelectedBehaviorTree, 0, 0);
                GUI.EndScrollView ();
                GUI.EndGroup ();
    		}
    	}

    	void Update()
    	{
    		Repaint();
    	}
    }
}
