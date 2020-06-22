using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkedPlayer : MonoBehaviourPun, IPunObservable
{
    private const float MaxLagOffset = 2;

    private Vector3 _correctPlayerPos = Vector3.zero;
    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (photonView.IsMine)
            return;

        Vector3 lagPositionOffset = transform.position - _correctPlayerPos;

        // teleport the player if lag is too great
        if (lagPositionOffset.magnitude >= MaxLagOffset)
            transform.position = _correctPlayerPos;

        // lerp towards correct position
        transform.position = Vector3.MoveTowards(transform.position, _correctPlayerPos, Time.deltaTime * Mob.MoveSpeed);
    }

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
        }
        else
        {
            // Network player, receive data
            _correctPlayerPos = (Vector3)stream.ReceiveNext();
        }
    }

    #endregion
}
