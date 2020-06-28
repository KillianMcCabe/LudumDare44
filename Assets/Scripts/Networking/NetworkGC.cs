using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    /// <summary>
    /// This class is used to delete networked gameobjects
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class NetworkGC : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
    {
        public static NetworkGC Instance;

        private static List<PhotonView> photonViewsToDestroy = new List<PhotonView>();

        private PhotonView _photonView;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Instance already exists");
                GameObject.Destroy(gameObject);
                return;
            }

            _photonView = GetComponent<PhotonView>();
        }

        public void DestroyPhotonView(PhotonView photonView)
        {
            if (PhotonNetwork.IsConnected)
            {
                if (photonView.isRuntimeInstantiated) // instantiated at runtime
                {
                    if (photonView.IsMine)
                    {
                        PhotonNetwork.Destroy(photonView);
                    }
                    else
                    {
                        photonView.RequestOwnership();
                        photonViewsToDestroy.Add(photonView);
                    }
                }
                else // scene view loaded in the scene
                {
                    _photonView.RPC("LocalDestroy", RpcTarget.AllBuffered, photonView.ViewID); // another option
                }
            }
            else
            {
                GameObject.Destroy(photonView.gameObject);
            }
        }

        public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
        {
        }

        public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
        {
            if (photonViewsToDestroy.Remove(targetView))
            {
                PhotonNetwork.Destroy(targetView);
            }
        }

        // destroy from a single PhotonView available on all clients
        [PunRPC]
        private void LocalDestroy(int viewId)
        {
            GameObject.Destroy(PhotonView.Find(viewId).gameObject);
        }
    }
}
