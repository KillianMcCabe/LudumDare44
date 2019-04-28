using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavigationGrid : MonoBehaviour
{
    public Grid gridBase;
    public Tilemap floor;

    //these are the bounds of where we are searching in the world for tiles, have to use world coords to check for tiles in the tile map
    public int scanStartX = -250, scanStartY = -250, scanFinishX = 250, scanFinishY = 250;

    NavNode[,] nodeGrid;            // sorted 2d array of nodes, may contain null entries if the map is of an odd shape e.g. gaps

    GameObject _nodeGridParent;

    public static NavigationGrid Instance;
    LayerMask _collisionLayerMask;

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
        _nodeGridParent = new GameObject("Node Grid");
        GenerateNodes(); // TODO: Calculate this in Editor
    }

    public int gridBoundX = 0, gridBoundY = 0;
    int gridMinWorldX = int.MaxValue;
    int gridMaxWorldX = int.MinValue;

    int gridMinWorldY = int.MaxValue;
    int gridMaxWorldY = int.MinValue;

    /// Generate a grid of nodes, ready to be used by a-star algorigthm
    void GenerateNodes()
    {
        // scan tiles and create nodes based on where they are
        List<NavNode> unsortedNodes = new List<NavNode>();
        for (int x = scanStartX; x < scanFinishX; x++)
        {
            for (int y = scanStartY; y < scanFinishY; y++)
            {
                // check if we have a floor tile at that world coords
                TileBase floorTileBase = floor.GetTile(new Vector3Int(x, y, 0));
                if (floorTileBase != null)
                {
                    GameObject node = new GameObject("Nav Node");
                    node.transform.position = new Vector3(x + gridBase.transform.position.x, y + gridBase.transform.position.y, 0);
                    NavNode navNode = node.AddComponent<NavNode>();
                    navNode.transform.SetParent(_nodeGridParent.transform);
                    navNode.worldPosX = x;
                    navNode.worldPosY = y;
                    if (Physics2D.OverlapCircle(new Vector3(x, y), 0.25f, _collisionLayerMask))
                    {
                        navNode.walkable = false;
                    }

                    unsortedNodes.Add(navNode);

                    // update gridBounds
                    if (navNode.worldPosX < gridMinWorldX)
                    {
                        gridMinWorldX = navNode.worldPosX;
                    }
                    if (navNode.worldPosX > gridMaxWorldX)
                    {
                        gridMaxWorldX = navNode.worldPosX;
                    }
                    if (navNode.worldPosY < gridMinWorldY)
                    {
                        gridMinWorldY = navNode.worldPosY;
                    }
                    if (navNode.worldPosY > gridMaxWorldY)
                    {
                        gridMaxWorldY = navNode.worldPosY;
                    }
                }
            }
        }

        gridBoundX = gridMaxWorldX - gridMinWorldX;
        gridBoundY = gridMaxWorldY - gridMinWorldY;

        // go through the unsorted list of nodes and put them into the 2d array in the correct position
        nodeGrid = new NavNode[gridBoundX + 1, gridBoundY + 1];
        foreach (NavNode navNode in unsortedNodes)
        {
            int x = navNode.worldPosX - gridMinWorldX;
            int y = navNode.worldPosY - gridMinWorldY;

            navNode.gameObject.name = $"x:{x} , y:{y}";
            navNode.gridX = x;
            navNode.gridY = y;
            nodeGrid[navNode.gridX, navNode.gridY] = navNode;
        }

        // assign neighbours to each node in 2d grid
        for (int x = 0; x < gridBoundX; x++)
        {
            for (int y = 0; y < gridBoundY; y++)
            {
                if (nodeGrid[x, y] != null)
                {
                    NavNode navNode = nodeGrid[x, y];
                    navNode.neighbours = GetNeighbours(x, y, gridBoundX, gridBoundY);
                }
            }
        }
    }

    // retrieve the node that corresponds to the given position
    public NavNode GetNode(Vector2 pos)
    {
        // NavNode closestNode = null;
        // float distToClosestNode = Mathf.Infinity;
        // foreach (NavNode n in nodeGrid)
        // {
        //     if (n != null)
        //     {
        //         Debug.Log(pos);
        //         float dist = Vector2.Distance(pos, n.Position);
        //         if (dist < distToClosestNode)
        //         {
        //             closestNode = n;
        //             distToClosestNode = dist;
        //         }
        //     }
        // }

        int x = (int)pos.x - gridMinWorldX;
        int y = (int)pos.y - gridMinWorldY;

        return nodeGrid[x, y];
    }

    // gets neighbours of a tile at x/y in a specific tilemap, can also have a border
    public int unwalkableNodeBorder = 1;
    public List<TileBase> GetNeighbouringTiles(int x, int y, Tilemap t)
    {
        List<TileBase> retVal = new List<TileBase>();

        for (int i = x - unwalkableNodeBorder; i < x + unwalkableNodeBorder; i++)
        {
            for (int j = y - unwalkableNodeBorder; j < y + unwalkableNodeBorder; j++)
            {
                TileBase tile = t.GetTile(new Vector3Int(i, j, 0));
                if (tile == null)
                {

                }
                else
                {
                    retVal.Add(tile);
                }
            }
        }

        return retVal;
    }

    // gets the neighbours of the coords passed in
    public List<NavNode> GetNeighbours(int x, int y, int width, int height)
    {
        List<NavNode> myNeighbours = new List<NavNode>();

        //needs the width & height to work out if a tile is not on the edge, also needs to check if the nodes is null due to the accounting for odd shapes

        if (x > 0 && x < width - 1)
        {
            //can get tiles on both left and right of the tile

            if (y > 0 && y < height - 1)
            {
                //top and bottom
                if (nodeGrid[x + 1, y] == null)
                {

                }
                else
                {

                    NavNode wt1 = nodeGrid[x + 1, y].GetComponent<NavNode>();
                    if (wt1 == null)
                    {
                    }
                    else
                    {
                        myNeighbours.Add(wt1);
                    }
                }

                if (nodeGrid[x - 1, y] == null)
                {

                }
                else
                {
                    NavNode wt2 = nodeGrid[x - 1, y].GetComponent<NavNode>();

                    if (wt2 == null)
                    {

                    }
                    else
                    {
                        myNeighbours.Add(wt2);
                    }
                }

                if (nodeGrid[x, y + 1] == null)
                {

                }
                else
                {
                    NavNode wt3 = nodeGrid[x, y + 1].GetComponent<NavNode>();
                    if (wt3 == null)
                    {

                    }
                    else
                    {
                        myNeighbours.Add(wt3);

                    }
                }

                if (nodeGrid[x, y - 1] == null)
                {

                }
                else
                {
                    NavNode wt4 = nodeGrid[x, y - 1].GetComponent<NavNode>();
                    if (wt4 == null)
                    {

                    }
                    else
                    {
                        myNeighbours.Add(wt4);
                    }
                }
            }
            else if (y == 0)
            {
                // just top
                if (nodeGrid[x + 1, y] == null)
                {

                }
                else
                {

                    NavNode wt1 = nodeGrid[x + 1, y].GetComponent<NavNode>();
                    if (wt1 == null)
                    {
                    }
                    else
                    {
                        myNeighbours.Add(wt1);
                    }
                }

                if (nodeGrid[x - 1, y] == null)
                {

                }
                else
                {
                    NavNode wt2 = nodeGrid[x - 1, y].GetComponent<NavNode>();

                    if (wt2 == null)
                    {

                    }
                    else
                    {
                        myNeighbours.Add(wt2);

                    }
                }
                if (nodeGrid[x, y + 1] == null)
                {

                }
                else
                {
                    NavNode wt3 = nodeGrid[x, y + 1].GetComponent<NavNode>();
                    if (wt3 == null)
                    {

                    }
                    else
                    {
                        myNeighbours.Add(wt3);

                    }
                }
            }
            else if (y == height - 1)
            {
                // just bottom
                if (nodeGrid[x, y - 1] == null)
                {

                }
                else
                {
                    NavNode wt4 = nodeGrid[x, y - 1].GetComponent<NavNode>();
                    if (wt4 == null)
                    {

                    }
                    else
                    {
                        myNeighbours.Add(wt4);
                    }
                }
                if (nodeGrid[x + 1, y] == null)
                {

                }
                else
                {
                    NavNode wt1 = nodeGrid[x + 1, y].GetComponent<NavNode>();
                    if (wt1 == null)
                    {
                    }
                    else
                    {
                        myNeighbours.Add(wt1);
                    }
                }

                if (nodeGrid[x - 1, y] == null)
                {

                }
                else
                {
                    NavNode wt2 = nodeGrid[x - 1, y].GetComponent<NavNode>();

                    if (wt2 == null)
                    {

                    }
                    else
                    {
                        myNeighbours.Add(wt2);

                    }
                }
            }
        }
        else if (x == 0)
        {
            // can't get tile on left
            if (y > 0 && y < height - 1)
            {
                // top and bottom
                if (nodeGrid[x + 1, y] == null)
                {

                }
                else
                {
                    NavNode wt1 = nodeGrid[x + 1, y].GetComponent<NavNode>();
                    if (wt1 == null)
                    {
                    }
                    else
                    {
                        myNeighbours.Add(wt1);
                    }
                }

                if (nodeGrid[x, y - 1] == null)
                {

                }
                else
                {
                    NavNode wt4 = nodeGrid[x, y - 1].GetComponent<NavNode>();
                    if (wt4 == null)
                    {

                    }
                    else
                    {
                        myNeighbours.Add(wt4);
                    }
                }
                if (nodeGrid[x, y + 1] == null)
                {

                }
                else
                {
                    NavNode wt3 = nodeGrid[x, y + 1].GetComponent<NavNode>();
                    if (wt3 == null)
                    {

                    }
                    else
                    {
                        myNeighbours.Add(wt3);

                    }
                }
            }
            else if (y == 0)
            {
                //just top
                if (nodeGrid[x + 1, y] == null)
                {

                }
                else
                {
                    NavNode wt1 = nodeGrid[x + 1, y].GetComponent<NavNode>();
                    if (wt1 == null)
                    {
                    }
                    else
                    {
                        myNeighbours.Add(wt1);
                    }
                }

                if (nodeGrid[x, y + 1] == null)
                {

                }
                else
                {
                    NavNode wt3 = nodeGrid[x, y + 1].GetComponent<NavNode>();
                    if (wt3 == null)
                    {

                    }
                    else
                    {
                        myNeighbours.Add(wt3);

                    }
                }
            }
            else if (y == height - 1)
            {
                //just bottom
                if (nodeGrid[x + 1, y] == null)
                {

                }
                else
                {

                    NavNode wt1 = nodeGrid[x + 1, y].GetComponent<NavNode>();
                    if (wt1 == null)
                    {
                    }
                    else
                    {
                        myNeighbours.Add(wt1);
                    }
                }
                if (nodeGrid[x, y - 1] == null)
                {

                }
                else
                {

                    NavNode wt4 = nodeGrid[x, y - 1].GetComponent<NavNode>();
                    if (wt4 == null)
                    {

                    }
                    else
                    {
                        myNeighbours.Add(wt4);
                    }
                }
            }
        }
        else if (x == width - 1)
        {
            //can't get tile on right
            if (y > 0 && y < height - 1)
            {
                //top and bottom
                if (nodeGrid[x - 1, y] == null)
                {

                }
                else
                {
                    NavNode wt2 = nodeGrid[x - 1, y].GetComponent<NavNode>();
                    if (wt2 != null)
                    {
                        myNeighbours.Add(wt2);
                    }
                }

                if (nodeGrid[x, y + 1] == null)
                {

                }
                else
                {
                    NavNode wt3 = nodeGrid[x, y + 1].GetComponent<NavNode>();
                    if (wt3 != null)
                    {
                        myNeighbours.Add(wt3);
                    }
                }
                if (nodeGrid[x, y - 1] == null)
                {

                }
                else
                {
                    NavNode wt4 = nodeGrid[x, y - 1].GetComponent<NavNode>();
                    if (wt4 != null)
                    {
                        myNeighbours.Add(wt4);
                    }
                }
            }
            else if (y == 0)
            {
                //just top
                if (nodeGrid[x - 1, y] == null)
                {

                }
                else
                {
                    NavNode wt2 = nodeGrid[x - 1, y].GetComponent<NavNode>();
                    if (wt2 != null)
                    {
                        myNeighbours.Add(wt2);
                    }
                }
                if (nodeGrid[x, y + 1] == null)
                {

                }
                else
                {
                    NavNode wt3 = nodeGrid[x, y + 1].GetComponent<NavNode>();
                    if (wt3 != null)
                    {
                        myNeighbours.Add(wt3);
                    }
                }
            }
            else if (y == height - 1)
            {
                //just bottom
                if (nodeGrid[x - 1, y] == null)
                {

                }
                else
                {
                    NavNode wt2 = nodeGrid[x - 1, y].GetComponent<NavNode>();

                    if (wt2 != null)
                    {
                        myNeighbours.Add(wt2);
                    }
                }
                if (nodeGrid[x, y - 1] == null)
                {

                }
                else
                {
                    NavNode wt4 = nodeGrid[x, y - 1].GetComponent<NavNode>();
                    if (wt4 != null)
                    {
                        myNeighbours.Add(wt4);
                    }
                }
            }
        }

        return myNeighbours;
    }

    public List<NavNode> CalculatePath(NavNode startNode, NavNode targetNode)
    {
        List<NavNode> path = new List<NavNode>();

        if (startNode.walkable && targetNode.walkable)
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
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
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
        path.Reverse();

        return path;
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
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        return path;
    }
}