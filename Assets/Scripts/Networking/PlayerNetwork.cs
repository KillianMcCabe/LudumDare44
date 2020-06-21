using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerNetwork : SingletonMonoBehaviour<PlayerNetwork>
{
    public void SendPlayerMessage(string message)
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("RPC_SendPlayerMessage", RpcTarget.All, message);
    }

    [PunRPC] // defines a method that can be called by other clients:
    private void RPC_SendPlayerMessage(string message)
    {
        Debug.Log($"Received message \"{message}\"");
    }
}
