using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PaperDungeons
{
    public class Lighting
    {
        private const float LightRange = 8;

        LightNode[,] _lightNodeGrid;
        public LightNode[,] LightNodeGrid { get { return _lightNodeGrid; } }

        private int _lightGridSizeX = 0;
        private int _lightGridSizeY = 0;

        LayerMask _navNodeWallLayerMask = LayerMask.GetMask("NavNode_Wall");

        public void InitialiseLightGrid(int gridMinWorldX, int gridMinWorldY)
        {
            _lightGridSizeX = MapManager.Instance.NavGrid.navGridSizeX - 1;
            _lightGridSizeY = MapManager.Instance.NavGrid.navGridSizeY - 1;
            _lightNodeGrid = new LightNode[_lightGridSizeX, _lightGridSizeY];

            // create and initialise all blocks in the light grid
            for (int x = 0; x < _lightGridSizeX; x++)
            {
                for (int y = 0; y < _lightGridSizeY; y++)
                {
                    LightNode newLightNode = new LightNode();
                    newLightNode.Init(
                        worldPos: new Vector2(gridMinWorldX + x + 0.5f, gridMinWorldY + y + 0.5f)
                    );
                    newLightNode.navNodes = new List<NavNode>();
                    for (int i = 0; i <= 1; i++)
                    {
                        for (int j = 0; j <= 1; j++)
                        {
                            if (MapManager.Instance.NavGrid.NodeGrid[x + i, y + j] != null)
                                newLightNode.navNodes.Add(MapManager.Instance.NavGrid.NodeGrid[x + i, y + j]);
                        }
                    }
                    _lightNodeGrid[x, y] = newLightNode;
                }
            }

            Debug.Log("LightGrid ready");
        }

        /// <summary>
        /// Recalculates lighting from players viewpoint
        /// </summary>
        public void Recalculate(Player player)
        {
            Debug.Log("recaulculating lighting...");

            int navNodeCount = 0;
            int navNodeBlocksLightCount = 0;

            // start by resetting all nodes to not visible
            foreach (NavNode navNode in MapManager.Instance.NavGrid.NodeGrid)
            {
                if (navNode != null)
                {
                    navNode.Visible = false;
                    navNodeCount ++;
                    if (navNode.BlocksLight)
                        navNodeBlocksLightCount ++;
                }
            }

            Debug.Log($"{navNodeCount} nodes, {navNodeBlocksLightCount} of which block light");

            // check all light nodes for visibility
            foreach (LightNode lightNode in _lightNodeGrid)
            {
                Vector2 towardsPlayer = player.WorldPosition - lightNode.WorldPosition;

                // check if within light range
                if (towardsPlayer.magnitude < LightRange)
                {
                    // check if within light of sight from player
                    RaycastHit2D hit = Physics2D.Raycast(lightNode.WorldPosition, (towardsPlayer).normalized, towardsPlayer.magnitude, _navNodeWallLayerMask);
                    if (hit.collider == null)
                    {
                        // Debug.DrawLine(lightNode.WorldPosition, lightNode.WorldPosition + towardsPlayer, Color.green, 2f);
                        lightNode.Visible = true;

                        // mark all nodes connected to this light node as visible
                        foreach (NavNode n in lightNode.navNodes)
                            n.Visible = true;
                    }
                    else
                    {
                        // Debug.DrawLine(lightNode.WorldPosition, hit.point, Color.red, 2f);
                        lightNode.Visible = false;
                    }
                }
            }
        }
    }
}
