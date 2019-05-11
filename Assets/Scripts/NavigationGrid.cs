using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavigationGrid : MonoBehaviour
{
    const float LightRange = 8;
    private const float zPos = -1;

    [SerializeField]
    Grid _gridBase;

    [SerializeField]
    Tilemap _groundTilemap;

    [SerializeField]
    Tilemap _fogTilemap;

    [SerializeField]
    GameObject _nodePrefab;

    //these are the bounds of where we are searching in the world for tiles, have to use world coords to check for tiles in the tile map
    public int scanStartX = -250, scanStartY = -250, scanFinishX = 250, scanFinishY = 250;

    NavNode[,] nodeGrid;            // sorted 2d array of nodes, may contain null entries if the map is of an odd shape e.g. gaps
    LightNode[,] lightNodeGrid;

    GameObject _nodeGridParent;

    public static NavigationGrid Instance;
    LayerMask _collisionLayerMask;
    LayerMask _navNodeFloorLayerMask;
    LayerMask _navNodeWallLayerMask;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        _collisionLayerMask = LayerMask.GetMask("TileMap");
        _navNodeFloorLayerMask = LayerMask.GetMask("NavNode_Floor");
        _navNodeWallLayerMask = LayerMask.GetMask("NavNode_Wall");
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
                navNode.Init(_groundTilemap, new Vector2Int(x, y));
                if (Physics2D.OverlapCircle(new Vector3(x, y), 0.25f, _collisionLayerMask))
                {
                    navNode.Walkable = false;
                }
                else
                {
                    navNode.Walkable = true;
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
        nodeGrid = new NavNode[navGridSizeX + 1, navGridSizeY + 1];
        foreach (NavNode navNode in unsortedNodes)
        {
            int x = navNode.WorldPosition.x - gridMinWorldX;
            int y = navNode.WorldPosition.y - gridMinWorldY;

            navNode.gameObject.name = $"x:{x} , y:{y}";
            navNode.gridX = x;
            navNode.gridY = y;
            nodeGrid[navNode.gridX, navNode.gridY] = navNode;
        }

        // assign neighbours to each node in 2d grid
        for (int x = 0; x < navGridSizeX; x++)
        {
            for (int y = 0; y < navGridSizeY; y++)
            {
                if (nodeGrid[x, y] != null)
                {
                    NavNode navNode = nodeGrid[x, y];
                    navNode.neighbours = GetNeighbours(navNode);
                }
            }
        }

        InitialiseLightGrid();
    }

    private void InitialiseLightGrid()
    {
        lightGridSizeX = navGridSizeX - 1;
        lightGridSizeY = navGridSizeY - 1;
        lightNodeGrid = new LightNode[lightGridSizeX, lightGridSizeY];

        for (int x = 0; x < lightGridSizeX; x++)
        {
            for (int y = 0; y < lightGridSizeY; y++)
            {
                LightNode newLightNode = new LightNode();
                newLightNode.WorldPosition = new Vector2(gridMinWorldX + x + 0.5f, gridMinWorldY + y + 0.5f);
                newLightNode.navNodes = new List<NavNode>();
                for (int i = 0; i <= 1; i++)
                {
                    for (int j = 0; j <= 1; j++)
                    {
                        if (nodeGrid[x + i, y + j] != null)
                            newLightNode.navNodes.Add(nodeGrid[x + i, y + j]);
                    }
                }
                lightNodeGrid[x, y] = newLightNode;
            }
        }
    }

    public void CalculateLighting()
    {
        // start by setting all nodes to dark
        foreach (NavNode navNode in nodeGrid)
        {
            if (navNode != null)
            {
                navNode.Visible = false;
            }
        }

        // check all light nodes
        foreach (LightNode lightNode in lightNodeGrid)
        {
            Vector2 towardsPlayer = Player.Instance.WorldPosition - lightNode.WorldPosition;
            float dist = towardsPlayer.magnitude;
            // check if within light range
            if (dist < LightRange)
            {
                // check if within light of sight from player
                RaycastHit2D hit = Physics2D.Raycast(lightNode.WorldPosition, (towardsPlayer).normalized, towardsPlayer.magnitude, _navNodeWallLayerMask);
                if (hit.collider == null)
                {
                    // Debug.DrawLine(lightNode.WorldPosition, lightNode.WorldPosition + towardsPlayer, Color.green, 2f);
                    foreach (NavNode n in lightNode.navNodes)
                    {
                        n.Visible = true;
                    }
                }
                else
                {
                    // Debug.DrawLine(lightNode.WorldPosition, hit.point, Color.red, 2f);
                    lightNode.Visible = false;
                }
            }
        }
    }

    // retrieve the node that corresponds to the given position
    public NavNode GetNode(Vector2 pos)
    {
        int x = (int)pos.x - gridMinWorldX;
        int y = (int)pos.y - gridMinWorldY;
        return nodeGrid[x, y];
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
                    neightbours.Add(nodeGrid[checkX, checkY]);
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
            Heap<NavNode> openSet = new Heap<NavNode>(nodeGrid.Length);
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
            // if (depth + 1 < range && neighbour.Walkable && !neighbour.Blocked && neighbour.HasBeenSeen)
            if (depth + 1 < range && neighbour.Walkable && neighbour.HasBeenSeen)
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
}