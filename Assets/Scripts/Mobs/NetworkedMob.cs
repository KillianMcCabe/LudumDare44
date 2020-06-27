using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PaperDungeons
{
    public class NetworkedMob : MonoBehaviourPun, IPunObservable
    {
        private const float MaxLagOffset = 2;

        private Vector3 _correctMobPos = Vector3.zero;
        private Mob _mob;

        private void Awake()
        {
            _mob = GetComponent<Mob>();
        }

        private void Update()
        {
            if (photonView.IsMine)
                return;

            Vector3 lagPositionOffset = transform.position - _correctMobPos;

            // teleport the player if lag is too great
            if (lagPositionOffset.magnitude >= MaxLagOffset)
                transform.position = _correctMobPos;

            // lerp towards correct position
            _mob.WorldPosition = Vector3.MoveTowards(transform.position, _correctMobPos, Time.deltaTime * Mob.MoveSpeed);
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
                _correctMobPos = (Vector3)stream.ReceiveNext();
            }
        }

        #endregion
    }
}
