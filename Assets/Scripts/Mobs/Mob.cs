using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PaperDungeons
{
    public abstract class Mob : MonoBehaviour
    {
        public const float MoveSpeed = 6f;

        protected PhotonView _photonView;
        protected NavNode _currentNodePosition;

        public NavNode NodePosition
        {
            get { return _currentNodePosition; }
        }

        public Vector2 WorldPosition
        {
            get { return transform.position; }
            set
            {
                transform.position = value;

                if (_currentNodePosition != null)
                    _currentNodePosition.Mob = null;

                _currentNodePosition = NavigationGrid.Instance.GetNode(transform.position);
                _currentNodePosition.Mob = this;
            }
        }

        public abstract void ReceiveAttack(int attackPower);

        protected virtual void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }

        protected virtual void Start()
        {
            // get current position on node grid
            _currentNodePosition = NavigationGrid.Instance.GetNode(transform.position);
            _currentNodePosition.Mob = this;
        }
    }
}
