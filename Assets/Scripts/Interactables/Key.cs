using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PaperDungeons
{
    public class Key : InteractableObject
    {
        NavNode currentNodePosition;

        // Start is called before the first frame update
        void Start()
        {
            currentNodePosition = MapManager.Instance.NavGrid.GetNode(new Vector2(transform.position.x, transform.position.y));
            currentNodePosition.InteractableObject = this;
            Debug.Log("key is at " + currentNodePosition.WorldPosition);
        }

        public override void Interact()
        {
            GameManager.LocalPlayer.HasKey = true;
            currentNodePosition.InteractableObject = null;
            MessageLogController.Instance.AddMessage("You found a key.");

            NetworkGC.Instance.DestroyPhotonView(photonView);
        }
    }
}
