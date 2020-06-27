using UnityEngine;
using Photon.Pun;

namespace PaperDungeons
{
    public abstract class InteractableObject : MonoBehaviourPunCallbacks, IInteractable
    {
        public abstract void Interact();

        // TODO: right-click = inspect
        // public abstract void Inspect();
    }
}
