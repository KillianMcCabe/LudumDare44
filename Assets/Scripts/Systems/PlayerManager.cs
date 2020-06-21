using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static PlayerManager LocalPlayerInstance = null;

    [Tooltip("The ally player instance.")]
    public static PlayerManager AllyPlayerInstance = null;

    public System.Action<int> OnHealthChange;

    private int _health = 8;
    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;

            if (OnHealthChange != null)
                OnHealthChange.Invoke(_health);
        }
    }

    private bool IsFiring; // True, when the user is firing

    #region MonoBehaviour CallBacks

    void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            Debug.Log("Local player: " + photonView.Owner.NickName);
            PlayerManager.LocalPlayerInstance = this;
        }
        else
        {
            Debug.Log("Ally player: " + photonView.Owner.NickName);
            PlayerManager.AllyPlayerInstance = this;
        }

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // we only process Inputs if we are the local player
        if (photonView.IsMine)
        {
            ProcessInputs ();
        }
    }

    #endregion

    /// <summary>
    /// Processes the inputs. Maintain a flag representing when the user is pressing Fire.
    /// </summary>
    private void ProcessInputs()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!IsFiring)
            {
                IsFiring = true;
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (IsFiring)
            {
                IsFiring = false;
            }
        }
    }

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(IsFiring);
            stream.SendNext(Health);
        }
        else
        {
            // Network player, receive data
            this.IsFiring = (bool)stream.ReceiveNext();
            this.Health = (int)stream.ReceiveNext();
        }
    }

    #endregion
}
