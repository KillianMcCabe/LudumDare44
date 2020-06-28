using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

namespace PaperDungeons
{
    public abstract class Mob : MonoBehaviour
    {
        public const float MoveSpeed = 6f;

        // attached components
        protected PhotonView _photonView;
        protected SpriteRenderer _spriteRenderer;

        private NavNode _currentNodePosition;

        public NavNode NodePosition
        {
            get { return _currentNodePosition; }
            protected set
            {
                if (_currentNodePosition == value)
                    return;
                
                if (_currentNodePosition != null)
                {
                    _currentNodePosition.Mob = null;
                    _currentNodePosition.OnVisibilityChange -= HandleNodeVisibilityChange;
                }

                _currentNodePosition = value;
                _currentNodePosition.Mob = this;

                _currentNodePosition.OnVisibilityChange += HandleNodeVisibilityChange;
                HandleNodeVisibilityChange(_currentNodePosition.Visible);
            }
        }

        private void HandleNodeVisibilityChange(bool isVisible)
        {
            _spriteRenderer.enabled = isVisible;
        }

        public Vector2 WorldPosition
        {
            get { return transform.position; }
            set
            {
                transform.position = value;
                NodePosition = NavigationGrid.Instance.GetNode(transform.position);
            }
        }

        public abstract void ReceiveAttack(int attackPower);

        protected virtual void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void Start()
        {
            // get current position on node grid
            NodePosition = NavigationGrid.Instance.GetNode(transform.position);
        }

        protected virtual void OnDestroy()
        {
            if (_currentNodePosition != null)
            {
                _currentNodePosition.Mob = null;
                _currentNodePosition.OnVisibilityChange -= HandleNodeVisibilityChange;
            }
        }
    }
}
