using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    [CreateAssetMenu(fileName = "DoorData", menuName = "ScriptableObjects/DoorData", order = 1)]
    public class DoorData : ScriptableObject
    {
        [SerializeField]
        private Sprite _openSprite = null;

        [SerializeField]
        private Sprite _closedSprite = null;

        [SerializeField]
        private Sprite _lockedSprite = null;

        public Sprite openSprite => _openSprite;
        public Sprite closedSprite => _closedSprite;
        public Sprite lockedSprite => _lockedSprite;
    }
}
