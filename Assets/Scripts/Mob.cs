using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mob : MonoBehaviour
{
    public abstract void ReceiveAttack(int attackPower);
}
