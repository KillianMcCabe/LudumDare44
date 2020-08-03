using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    [System.Serializable]
    public struct MobStats
    {
        public const int BaseStatPoints = 10;
        public const int StartingAdditionalStatPoints = 4;
        public const int AdditionalStatPointsPerLevel = 10;
        public const int MaxStatPoints = 40;

        public int strength;
        public int dexterity;
        public int intelligence;
        public int constitution;
        public int wits;
    }
}
