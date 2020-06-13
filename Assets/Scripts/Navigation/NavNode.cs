using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NavNode : MonoBehaviour , IHeapItem<NavNode>
{
    public static Action<NavNode> OnNodeClicked;

    public int gCost;
    public int hCost;
    public int gridX, gridY;
    public List<NavNode> neighbours;
    public NavNode parent;
    public InteractableObject InteractableObject;
    public Mob Mob;

    private Vector2Int _worldPosition;
    int heapIndex;
    bool _visible = true;
    bool _hasBeenSeen = false;
    bool _walkable = true;
    bool _blocked = false;
    bool _blocksLight = false;

    private Color _highlight = new Color(0, 0, 0, 0);

    SpriteRenderer _spriteRenderer;

    TileBase _fogTile;
    Tilemap _fogTilemap;

    public bool Walkable
    {
        get { return _walkable; }
        set {
            _walkable = value;
            UpdateLayer();
        }
    }

    public bool Blocked
    {
        get { return _blocked; }
        set {
            _blocked = value;
        }
    }

    public bool BlocksLight
    {
        get { return _blocksLight; }
        set {
            _blocksLight = value;
            UpdateLayer();
        }
    }

    public Color Highlight
    {
        get { return _highlight; }
        set {
            _highlight = value;
            UpdateSpriteRenderer();
        }
    }

    public bool HasBeenSeen
    {
        get { return _hasBeenSeen; }
    }

    public bool Visible
    {
        get { return _visible; }
        set {
            _visible = value;
            if (_visible)
            {
                _hasBeenSeen = true;
            }

            UpdateSpriteRenderer();
        }
    }

    public Vector2Int WorldPosition
    {
        get { return _worldPosition; }
    }

    public Vector3 WorldPositionVector3
    {
        get { return new Vector3 (_worldPosition.x, _worldPosition.y, 0 ); }
    }

    public Vector3Int WorldPositionVector3Int
    {
        get { return Vector3Int.RoundToInt(WorldPositionVector3); }
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void Init(Tilemap tilemap, Tilemap fogTilemap, TileBase fogTile, Vector2Int worldPos)
    {
        _fogTilemap = fogTilemap;
        _fogTile = fogTile;

        _worldPosition = worldPos;

        if (fogTile == null)
            Debug.LogError("fogTile is null!");

        // add fog at this position
        _fogTilemap.SetTile(WorldPositionVector3Int, _fogTile);

        _spriteRenderer.enabled = false;

        Visible = false;
    }

    public int CompareTo(NavNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }

    private void UpdateLayer()
    {
        if (!_walkable || _blocksLight)
        {
            gameObject.layer = LayerMask.NameToLayer("NavNode_Wall");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("NavNode_Floor");
        }
    }

    private void UpdateSpriteRenderer()
    {
        // set tile shadow color
        if (!_visible)
        {
            // add fog
            _fogTilemap.SetTile(WorldPositionVector3Int, _fogTile);

            if (_hasBeenSeen)
            {
                // semi-fog
                _fogTilemap.SetColor(WorldPositionVector3Int, new Color( 0.25f, 0.25f, 0.25f, 0.25f));
            }
        }
        else
        {
            // remove fog
            _fogTilemap.SetTile(WorldPositionVector3Int, null);
        }
    }

    void OnMouseDown()
    {
        if (OnNodeClicked != null)
        {
            OnNodeClicked.Invoke(this);
        }
    }

    void OnDrawGizmos()
    {
        if (!_walkable)
            Gizmos.color = Color.red;
        else if (_blocked)
            Gizmos.color = new Color(1f, 0.5f, 0f); // orange
        else
            Gizmos.color = Color.white;

        Gizmos.DrawWireCube(transform.position, new Vector3(0.9f, 0.9f, 0.9f));
    }
}
