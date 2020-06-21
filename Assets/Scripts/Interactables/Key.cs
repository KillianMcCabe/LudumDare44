using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : InteractableObject
{
    NavNode currentNodePosition;

    // Start is called before the first frame update
    void Start()
    {
        currentNodePosition = NavigationGrid.Instance.GetNode(new Vector2(transform.position.x, transform.position.y));
        currentNodePosition.InteractableObject = this;
        Debug.Log("key is at " + currentNodePosition.WorldPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        GameSetupController.LocalPlayer.HasKey = true;
        currentNodePosition.InteractableObject = null;
        MessageLogController.Instance.AddMessage("You found a key.");
        GameObject.Destroy(gameObject);
    }
}
