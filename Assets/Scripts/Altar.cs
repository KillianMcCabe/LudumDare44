using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : InteractableObject
{
    public bool boonGranted = false;
    public GameObject choiceOfGreed;

    // Start is called before the first frame update
    void Start()
    {
        IsBlocking = true;

        NavigationGrid.Instance.GetNode(new Vector2(transform.position.x,     transform.position.y)).InteractableObject = this;
        NavigationGrid.Instance.GetNode(new Vector2(transform.position.x + 1, transform.position.y)).InteractableObject = this;;
    }

    public override void Interact()
    {
        if (!boonGranted)
        {
            MessageLogController.Instance.AddMessage("You sense a greedy presense...");
            choiceOfGreed.SetActive(true);
            boonGranted = true;
        }
        else
        {
            MessageLogController.Instance.AddMessage("There's nothing else to do here.");
        }
    }
}
