using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavigationGrid : SingletonMonoBehaviour<NavigationGrid>
{
    private const float zPos = -1;

    [SerializeField]
    private Grid _gridBase = null;

    [SerializeField]
    private Tilemap _groundTilemap = null;

    [SerializeField]
    private Tilemap _fogTilemap = null;

    [SerializeField]
    private TileBase _fogTile = null;

    [SerializeField]
    private GameObject _nodePrefab = null;

    [SerializeField]
    [Tooltip("These are the world bounds, inside which, we will search for tiles.")]
    private int scanStartX = -250, scanStartY = -250, scanFinishX = 250, scanFinishY = 250;

    // sorted 2d array of nodes, may contain null entries if the map is of an odd shape e.g. gaps
    private NavNode[,] _nodeGrid;
    public NavNode[,] NodeGrid { get { return _nodeGrid; } }

    private Lighting _lighting;
    public Lighting Lighting { get { return _lighting; } }

    private GameObject _nodeGridParent;
    private LayerMask _collisionLayerMask;
    private LayerMask _navNodeFloorLayerMask;

    void Awake()
    {
        _collisionLayerMask = LayerMask.GetMask("TileMap");
        _navNodeFloorLayerMask = LayerMask.GetMask("NavNode_Floor");
        _nodeGridParent = new GameObject("Node Grid");

        GenerateNodes(); // TODO: Calculate this in Editor
    }

    public int navGridSizeX = 0, navGridSizeY = 0;
    public int lightGridSizeX = 0, lightGridSizeY = 0;

    int gridMinWorldX = int.MaxValue;
    int gridMaxWorldX = int.MinValue;

    int gridMinWorldY = int.MaxValue;
    int gridMaxWorldY = int.MinValue;

    /// Generate a grid of nodes, ready to be used by a-star algorigthm
    private void GenerateNodes()
    {
        // scan tiles and create nodes based on where they are
        List<NavNode> unsortedNodes = new List<NavNode>();
        for (int x = scanStartX; x < scanFinishX; x++)
        {
            for (int y = scanStartY; y < scanFinishY; y++)
            {
                GameObject node = Instantiate(_nodePrefab);
                node.transform.position = new Vector3(x + _gridBase.transform.position.x, y + _gridBase.transform.position.y, zPos);
                NavNode navNode = node.GetComponent<NavNode>();
                navNode.transform.SetParent(_nodeGridParent.transform);
                navNode.Init(_groundTilemap, _fogTilemap, _fogTile, new Vector2Int(x, y));
                if (Physics2D.OverlapCircle(new Vector3(x, y), 0.25f, _collisionLayerMask))
                {
                    navNode.Walkable = false;
                    navNode.BlocksLight = true;
                }
                else
                {
                    navNode.Walkable = true;
                    navNode.BlocksLight = false;
                }

                unsortedNodes.Add(navNode);

                // update gridBounds
                if (navNode.WorldPosition.x < gridMinWorldX)
                {
                    gridMinWorldX = navNode.WorldPosition.x;
                }
                if (navNode.WorldPosition.x > gridMaxWorldX)
                {
                    gridMaxWorldX = navNode.WorldPosition.x;
                }
                if (navNode.WorldPosition.y < gridMinWorldY)
                {
                    gridMinWorldY = navNode.WorldPosition.y;
                }
                if (navNode.WorldPosition.y > gridMaxWorldY)
                {
                    gridMaxWorldY = navNode.WorldPosition.y;
                }
            }
        }

        navGridSizeX = gridMaxWorldX - gridMinWorldX;
        navGridSizeY = gridMaxWorldY - gridMinWorldY;

        // go through the unsorted list of nodes and put them into the 2d array in the correct position
        _nodeGrid = new NavNode[navGridSizeX + 1, navGridSizeY + 1];
        foreach (NavNode navNode in unsortedNodes)
        {
            int x = navNode.WorldPosition.x - gridMinWorldX;
            int y = navNode.WorldPosition.y - gridMinWorldY;

            navNode.gameObject.name = $"x:{x} , y:{y}";
            navNode.gridX = x;
            navNode.gridY = y;
            _nodeGrid[navNode.gridX, navNode.gridY] = navNode;
        }

        // assign neighbours to each node in 2d grid
        for (int x = 0; x < navGridSizeX; x++)
        {
            for (int y = 0; y < navGridSizeY; y++)
            {
                if (_nodeGrid[x, y] != null)
                {
                    NavNode navNode = _nodeGrid[x, y];
                    navNode.neighbours = GetNeighbours(navNode);
                }
            }
        }

        _lighting = new Lighting();
        _lighting.InitialiseLightGrid(gridMinWorldX, gridMinWorldY);
    }

    // retrieve the node that corresponds to the given position
    public NavNode GetNode(Vector2 pos)
    {
        int x = (int)pos.x - gridMinWorldX;
        int y = (int)pos.y - gridMinWorldY;
        return _nodeGrid[x, y];
    }

    public List<NavNode> GetNeighbours(NavNode node)
    {
        List<NavNode> neightbours = new List<NavNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < navGridSizeX && checkY >= 0 && checkY < navGridSizeY)
                {
                    neightbours.Add(_nodeGrid[checkX, checkY]);
                }
            }
        }

        return neightbours;
    }

    public List<NavNode> CalculatePath(NavNode startNode, NavNode targetNode)
    {
        List<NavNode> path = new List<NavNode>();

        if (startNode.Walkable && targetNode.Walkable)
        {
            Heap<NavNode> openSet = new Heap<NavNode>(_nodeGrid.Length);
            HashSet<NavNode> closedSet = new HashSet<NavNode>();
            openSet.Add(startNode);
            
            while (openSet.Count > 0)
            {
                NavNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    break;
                }

                foreach (NavNode neighbour in currentNode.neighbours)
                {
                    if (!neighbour.Walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        path = RetracePath(startNode, targetNode);
        if (path != null)
            path.Reverse();

        return path;
    }

    public List<NavNode> GetNodesWithinRange(NavNode startNode, int range)
    {
        List<NavNode> result = new List<NavNode>();

        // Breadth first search
        GetNodesWithinRangeBFS(startNode, range, result, 0);

        return result;
    }

    public void GetNodesWithinRangeBFS(NavNode currentNode, int range, List<NavNode> progress, int depth)
    {
        progress.Add(currentNode);

        foreach (NavNode neighbour in currentNode.neighbours)
        {
            // if (depth + 1 < range && neighbour.Walkable && neighbour.HasBeenSeen)
            if (depth + 1 < range && neighbour.Walkable && !neighbour.Blocked && neighbour.HasBeenSeen)
            {
                if (!progress.Contains(neighbour) || neighbour.gCost > depth+1)
                {
                    neighbour.gCost = depth+1;
                    GetNodesWithinRangeBFS(neighbour, range, progress, depth + 1);
                }
            }
        }
    }

    private int GetDistance(NavNode nodeA, NavNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        else
        {
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }

    private List<NavNode> RetracePath(NavNode startNode, NavNode endNode)
    {
        List<NavNode> path = new List<NavNode>();
        NavNode currentNode = endNode;

        while (currentNode != startNode)
        {
            if (currentNode.parent == null)
            {
                return null;
            }
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        return path;
    }

    void OnDrawGizmos()
    {
        if (_lighting != null && _lighting.LightNodeGrid != null)
        {
            foreach (LightNode lightNode in _lighting.LightNodeGrid)
            {
                if (lightNode.Visible)
                {
                    Gizmos.DrawSphere(lightNode.WorldPosition, 0.5f);
                }
            }
        }
    }
}