using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractableObject
{
    NavNode currentNodePosition;

    public bool locked = false;

    [SerializeField]
    SpriteRenderer _spriteRenderer = null;

    [SerializeField]
    Sprite _unlockedSprite = null;

    [SerializeField]
    Sprite _lockedSprite = null;

    // Start is called before the first frame update
    void Start()
    {
        IsBlocking = locked;

        if (locked)
        {
            currentNodePosition = NavigationGrid.Instance.GetNode(new Vector2(transform.position.x, transform.position.y));
            Debug.Log("locked door is at " + currentNodePosition.WorldPosition);
            currentNodePosition.InteractableObject = this;
            _spriteRenderer.sprite = _lockedSprite;
        }
        else
        {
            _spriteRenderer.sprite = _unlockedSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        if (locked)
        {
            if (Player.Instance.HasKey)
            {
                currentNodePosition.InteractableObject = null;
                Debug.Log("You unlocked the door");
                locked = false;
                IsBlocking = false;
                _spriteRenderer.sprite = _unlockedSprite;
            }
            else
            {
                Debug.Log("The door is locked");
            }
        }
    }
}
