using UnityEngine;
using System.Collections.Generic;

public class MonteCarlo : MonoBehaviour
{
    Tilemap _tilemap;
    public Tilemap Map
    {
        get 
        { 
            if (_tilemap == null)
            {
                _tilemap = FindObjectOfType<Tilemap>();
                DebugUtils.Assert(_tilemap!=null, "_tilemap!=null");
            }

            return _tilemap;
        } 
    }

    [SerializeField]
    bool _run = false;

    [SerializeField]
    bool _reset = false;

    [SerializeField]
    private Vector2 _endTile = new Vector2(25, 10);
    public Vector2 EndTile
    {
        get { return _endTile; }
    }

    [SerializeField]
    private Vector2 _startTile = new Vector2(0, 0);
    public Vector2 StartTile
    {
        get { return _startTile; }
    }

    [SerializeField]
    private int _maxIterationsPerSimulation = 500;
    public int MaxIterationsPerSimulation
    {
        get{ return _maxIterationsPerSimulation; }
    }

    [SerializeField]
    private int _maxMontecarloSteps = 1;
    public int MaxMontecarloSteps
    {
        get{ return _maxMontecarloSteps; }
    }

    Node _node;

    void Awake()
    {
        _node = new Node(this, new PathfindingState(_startTile));
    }

    void Update()
    {
        if(_run)
        {
            _run = false;
            CleanColor();
            Run();
        }

        if(_reset)
        {
            _reset = false;
            _node = new Node(this, new PathfindingState(_startTile));
        }
    }

    public void Run()
    {
        for (int i = 0; i < MaxMontecarloSteps; ++i)
        {
            _node.Step();
        }

        List<Node> bestNodes = _node.GetBestNodes();
        Debug.Log("BestNodes Count: " + bestNodes.Count);
        foreach (Node item in bestNodes)
        {
            Vector2 tileId = item.State.CurrentTileId;
            Tile tile = Map.GetTileByTileId(tileId);

            Debug.Log("Best Node: " + tileId + " -> tileIdFromTile; " + tile.Id);
            tile.DebugColor = Color.black;
        }

        Tile startTile = Map.GetTileByTileId(StartTile);
        startTile.DebugColor = Color.green;

        Tile endTile = Map.GetTileByTileId(EndTile);
        endTile.DebugColor = Color.red;
    }

    void CleanColor()
    {
        foreach (Tile tile in Map.Tiles)
        {
            tile.DebugColor = Color.gray;
        }
    }
}

public class Node
{
    public int Numvisits = 0;
    public float Reward = 0f;
    List<Node> _children = new List<Node>();

    PathfindingState _state;
    public PathfindingState State{ get{ return _state; } }
    MonteCarlo _montecarlo;

    public Node(MonteCarlo montecarlo, PathfindingState state)
    {
        _state = state;
        _montecarlo = montecarlo;
    }

    public void Step()
    {
        Debug.Log("Step...");

        List<Node> treeNodes = new List<Node>();

        Node currNode = this;
        treeNodes.Add(currNode);

        // Select node to expand from
        while(!currNode.IsLeaf())
        {
            currNode = currNode.Select();
            treeNodes.Add(currNode);
        }

        // expand
        currNode.Expand();

        // Get some of the new child nodes
        currNode = currNode.Select();
        treeNodes.Add(currNode);

        // Simulate
        float reward = currNode.Simulate(1);

        // Update Reward
        for (int i = 0; i < treeNodes.Count; ++i)
        {
            treeNodes[i].Update(reward);
        }
    }

    public List<Node> GetBestNodes()
    {
        List<Node> nodes = new List<Node>();

        Node currentNode = this;
        nodes.Add(currentNode);
        bool addedChildNodes = false;
        for (int i = 0; !currentNode.IsLeaf(); ++i)
        {
            currentNode = currentNode.GetMaxRewardedNode();
            nodes.Add(currentNode);

            addedChildNodes = true;
        }
        return nodes;
    }

    public bool IsLeaf()
    {
        return _children.Count == 0;
    }

    public Node Select()
    {
        Node selected = _children[0];
        double bestValue = float.MinValue;
        foreach (Node c in _children)
        {
            double uctValue = c.Reward / (c.Numvisits + Mathf.Epsilon) +
                Mathf.Sqrt(Mathf.Log(Numvisits + 1) / (c.Numvisits + Mathf.Epsilon)) +
                    Random.Range(0f, 1f) * Mathf.Epsilon;

            // small random number to break ties randomly in unexpanded nodes
            if (uctValue > bestValue) {
                selected = c;
                bestValue = uctValue;
            }
        }
        return selected;

        //return _children[0];
    }

    public void Expand()
    {
        for (int i = 0; i < 5; ++i)
        {
            _children.Add(new Node(_montecarlo, _state.Clone()));    
        }
    }

    public float Simulate(int numIterations)
    {
        Debug.Log("Simulating...");
        Vector2 prevTileId = _state.CurrentTileId;

        List<Vector2> nonVisitedLinkedTileIds = GetNotVisitedLinkedTileIds(prevTileId);
        if(nonVisitedLinkedTileIds.Count <= 0)
        {
            return Mathf.Epsilon;
        }

        int randomIdx = Random.Range(0, nonVisitedLinkedTileIds.Count);
        _state.CurrentTileId = nonVisitedLinkedTileIds[randomIdx];
        _state.VisitedNodes.Add(_state.CurrentTileId, true);

        DebugUtils.Assert(prevTileId != _state.CurrentTileId);

        Tile tile = _montecarlo.Map.GetTileByTileId(_state.CurrentTileId);
//        tile.DebugColor = Color.cyan;

        // Recursively keep dreaming for next moves
        if(   _state.CurrentTileId != _montecarlo.EndTile
           && numIterations < _montecarlo.MaxIterationsPerSimulation)
        {
            Node nextNode = new Node(_montecarlo, _state.Clone());
            return nextNode.Simulate(numIterations++);
        }
        else
        {
            float distanceToEnd = (_montecarlo.EndTile - _state.CurrentTileId).magnitude;
            
            float reward = 1f;
            if(distanceToEnd > 1e-3)
            {
                reward = 1f / (1f + distanceToEnd + (float) numIterations);
            }

            if(_state.CurrentTileId == _montecarlo.EndTile)
            {
                reward += 1f;
            }

            return reward;
        }
    }

    private List<Vector2> GetNotVisitedLinkedTileIds(Vector2 tileId)
    {
        List<Vector2> linkedTileIds = _montecarlo.Map.GetNeighbors(tileId);
        DebugUtils.Assert(linkedTileIds.Count > 0);

        List<Vector2> notVisitedLinkedTileIds = new List<Vector2>();
        for (int i = 0; i < linkedTileIds.Count; ++i)
        {
            Vector2 currTileId = linkedTileIds[i];
            if(!_state.VisitedNodes.ContainsKey(currTileId))
            {
                notVisitedLinkedTileIds.Add(currTileId);
            }
        }

        return notVisitedLinkedTileIds;
    }

    public void Update(float reward)
    {
        Numvisits += 1;
        Reward += reward;
    }

    public Node GetMaxRewardedNode()
    {
        float maxReward = float.MinValue;
        Node maxNode = null;

        for (int i = 0; i < _children.Count; ++i)
        {
            if(_children[i].Reward > maxReward)
            {
                maxReward = _children[i].Reward;
                maxNode = _children[i];
            }
        }

        return maxNode;
    }
}
