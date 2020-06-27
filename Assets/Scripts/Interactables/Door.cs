using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PaperDungeons
{
    public class Door : InteractableObject
    {
        // the nav-node which this object occupies
        NavNode _occupiedNavNode = null;

        public enum State
        {
            Closed,
            Locked,
            Open
        }

        [SerializeField]
        SpriteRenderer _spriteRenderer = null;

        [SerializeField]
        Sprite _openSprite = null;

        [SerializeField]
        Sprite _closedSprite = null;

        [SerializeField]
        Sprite _lockedSprite = null;

        [SerializeField]
        private State _state;

        // Start is called before the first frame update
        void Start()
        {
            _occupiedNavNode = NavigationGrid.Instance.GetNode(new Vector2(transform.position.x, transform.position.y));
            _occupiedNavNode.InteractableObject = this;
            _occupiedNavNode.BlocksLight = true;

            HandleStateChange();
        }

        public void SetState(State state)
        {
            photonView.RPC("RPC_UpdateState", RpcTarget.All, state);
        }

        public override void Interact()
        {
            if (_state == State.Locked)
            {
                if (GameManager.LocalPlayer.HasKey)
                {
                    _occupiedNavNode.InteractableObject = null;
                    MessageLogController.Instance.AddMessage("You unlocked the door using a key.");
                    SetState(State.Closed);
                }
                else
                {
                    MessageLogController.Instance.AddMessage("The door is locked.");
                }
            }
        }

        private void HandleStateChange()
        {
            if (_state == State.Locked)
            {
                _spriteRenderer.sprite = _lockedSprite;
            }
            else if (_state == State.Closed)
            {
                _spriteRenderer.sprite = _closedSprite;
            }
            else
            {
                _spriteRenderer.sprite = _openSprite;
            }

            _occupiedNavNode.Blocked = (_state == State.Locked);
            _occupiedNavNode.BlocksLight = (_state != State.Open);
        }

        /// <summary>
        /// Sent when another object enters a trigger collider attached to this
        /// object (2D physics only).
        /// </summary>
        /// <param name="other">The other Collider2D involved in this collision.</param>
        void OnTriggerEnter2D(Collider2D other)
        {
            SetState(State.Open);
        }

        /// <summary>
        /// Sent when another object leaves a trigger collider attached to
        /// this object (2D physics only).
        /// </summary>
        /// <param name="other">The other Collider2D involved in this collision.</param>
        void OnTriggerExit2D(Collider2D other)
        {
            SetState(State.Closed);
        }

        #region IPunObservable implementation

        [PunRPC]
        private void RPC_UpdateState(State newState)
        {
            _state = newState;
            HandleStateChange();
        }

        #endregion
    }
}
