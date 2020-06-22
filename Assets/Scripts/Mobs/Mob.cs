using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Mob : MonoBehaviourPunCallbacks
{
    public const float MoveSpeed = 6f;

    public abstract void ReceiveAttack(int attackPower);
}
