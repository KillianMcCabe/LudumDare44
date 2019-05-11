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

    TileBase _originalTile;
    Tilemap _tilemap;

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
                _tilemap.SetTile(WorldPositionVec3Int, _originalTile);
            }

            UpdateSpriteRenderer();
        }
    }

    public Vector2Int WorldPosition
    {
        get { return _worldPosition; }
    }

    public Vector3 WorldPosition3
    {
        get { return new Vector3 (_worldPosition.x, _worldPosition.y, 0 ); }
    }

    public Vector3Int WorldPositionVec3Int
    {
        get { return Vector3Int.RoundToInt(WorldPosition3); }
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

    public void Init(Tilemap tilemap, Vector2Int worldPos)
    {
        _tilemap = tilemap;

        _worldPosition = worldPos;

        _originalTile = _tilemap.GetTile(WorldPositionVec3Int);
        _tilemap.SetTile(WorldPositionVec3Int, null); // destory

        Visible = true;
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
            if (_hasBeenSeen)
            {
                // _spriteRenderer.color = new Color(0, 0, 0, 0.5f); // darkened
                // visible
                _tilemap.SetColor(WorldPositionVec3Int,  new Color( 0.5f, 0.5f,  0.5f, 0.5f));
            }
            else
            {
                // _spriteRenderer.color = new Color(0, 0, 0, 1); // blacked out
            }
        }
        else
        {
            // visible
            _tilemap.SetColor(WorldPositionVec3Int,  new Color(1, 1, 1, 1));
            if (_highlight != Color.black)
            {
                _spriteRenderer.color = _highlight * new Color(1, 1, 1, 0.25f); // visible
            }
            else
            {
                
                // _spriteRenderer.color = new Color(0, 0, 0, 0); 
            }
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
        Gizmos.color = _walkable ? Color.white : Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.9f, 0.9f, 0.9f));
    }
}
