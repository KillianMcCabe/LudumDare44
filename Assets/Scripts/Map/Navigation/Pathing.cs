using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    public class Pathing
    {
        public static List<NavNode> CalculatePath(NavNode startNode, NavNode targetNode)
        {
            List<NavNode> path = new List<NavNode>();

            NavNode closestNodeToTargetNode = startNode;
            closestNodeToTargetNode.hCost = GetDistance(startNode, targetNode);

            if (startNode.Walkable && targetNode.Walkable)
            {
                Heap<NavNode> openSet = new Heap<NavNode>(MapManager.Instance.NavGrid.NodeGrid.Length);
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
                        if (!neighbour.Walkable || !neighbour.HasBeenSeen || neighbour.Blocked || closedSet.Contains(neighbour))
                            continue;

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (neighbour.hCost < closestNodeToTargetNode.hCost)
                                closestNodeToTargetNode = neighbour;

                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                            else
                                openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }

            path = RetracePath(startNode, closestNodeToTargetNode);
            if (path != null)
                path.Reverse();

            return path;
        }

        public static List<NavNode> GetNodesWithinRange(NavNode startNode, int range)
        {
            List<NavNode> result = new List<NavNode>();
            GetNodesWithinRangeBFS(startNode, range, result, 0);
            return result;
        }

        private static void GetNodesWithinRangeBFS(NavNode currentNode, int range, List<NavNode> progress, int depth)
        {
            if (currentNode == null)
            {
                Debug.LogError("currentNode is null");
                return;
            }

            progress.Add(currentNode);

            if (depth + 1 < range)
            {
                foreach (NavNode neighbour in currentNode.neighbours)
                {
                    if (neighbour.Walkable && !neighbour.Blocked && neighbour.HasBeenSeen)
                    {
                        if (!progress.Contains(neighbour) || neighbour.gCost > depth+1)
                        {
                            neighbour.gCost = depth+1;
                            GetNodesWithinRangeBFS(neighbour, range, progress, depth + 1);
                        }
                    }
                }
            }
        }

        private static int GetDistance(NavNode nodeA, NavNode nodeB)
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

        private static List<NavNode> RetracePath(NavNode startNode, NavNode endNode)
        {
            List<NavNode> path = new List<NavNode>();

            NavNode currentNode = endNode;
            while (currentNode != startNode)
            {
                if (currentNode.parent == null)
                    return null;

                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            return path;
        }
    }
}
