using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PaperDungeons
{
    public class Enemy : Mob
    {
        enum State
        {
            Sleeping,
            Wandering,
            Aggravated
        }

        private bool _isMoving;
        public bool IsMoving
        {
            get { return _isMoving; }
        }

        private State _state;
        private List<NavNode> pathing;
        Coroutine pathingCoroutine;

        [SerializeField]
        private MobStatsData _statsData = null;

        protected override void Start()
        {
            base.Start();

            stats = _statsData.stats;

            GameManager.Instance.AddEnemyToList(this);

            _state = State.Sleeping;
            _isMoving = false;
        }

        public void Move()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            switch (_state)
            {
                case State.Sleeping:
                    break;
                case State.Wandering:
                    break;
                case State.Aggravated:

                    _isMoving = true;

                    pathing = Pathing.CalculatePath(NodePosition, GameManager.LocalPlayer.NodePosition);
                    if (pathingCoroutine != null)
                    {
                        StopCoroutine(pathingCoroutine);
                    }
                    pathingCoroutine = StartCoroutine(PathingCoroutine());
                    
                    break;
            }
        }

        public override void ReceiveAttack(int attackPower)
        {
            _state = State.Aggravated;

            int damage = Mathf.Max(attackPower - attributes.armor, 0);
            health -= damage;
            MessageLogController.Instance.AddMessage($"You dealt {damage} damage to the Slime.");
            if (health <= 0)
            {
                MessageLogController.Instance.AddMessage("You killed the Slime.");

                GameManager.Instance.RemoveEnemyFromList(this);
                GameObject.Destroy(gameObject);
            }
        }

        // Update is called once per frame
        private IEnumerator PathingCoroutine()
        {
            while (pathing.Count > 0)
            {
                // pop node
                NavNode nextNode = pathing[0];
                pathing.RemoveAt(0);

                // check if the path is blocked by an object e.g. door
                if (nextNode.InteractableObject != null && nextNode.Blocked)
                {
                    _isMoving = false;
                    yield break;
                }
                // check if the path is blocked by a mob
                else if (nextNode.Mob != null)
                {
                    // is the mob a player?
                    if (nextNode.Mob is Player)
                    {
                        // attack the player
                        nextNode.Mob.ReceiveAttack(stats.strength);
                    }
                    _isMoving = false;
                    yield break;
                }
                else
                {
                    // transform.position = _currentNodePosition.WorldPosition;

                    // _currentNodePosition.Mob = null;
                    // _currentNodePosition = nextNode;
                    // _currentNodePosition.Mob = this;
                }
                yield return new WaitForSeconds(0.25f);
            }

            _isMoving = false;
        }
    }
}
