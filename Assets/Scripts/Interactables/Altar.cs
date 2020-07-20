using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    public class Altar : InteractableObject
    {
        public bool boonGranted = false;
        public GameObject choiceOfGreed;

        // Start is called before the first frame update
        void Start()
        {
            NavNode node1 = MapManager.Instance.NavGrid.GetNode(new Vector2(transform.position.x,     transform.position.y));
            node1.InteractableObject = this;
            node1.Blocked = true;
            NavNode node2 = MapManager.Instance.NavGrid.GetNode(new Vector2(transform.position.x + 1, transform.position.y));
            node2.InteractableObject = this;
            node2.Blocked = true;
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
}