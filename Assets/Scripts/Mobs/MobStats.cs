using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    [System.Serializable]
    public struct MobStats
    {
        public const int BaseStatPoints = 8;
        public const int StartingAdditionalStatPoints = 4;
        public const int AdditionalStatPointsPerLevel = 10;
        public const int MaxStatPoints = 40;

        private Dictionary<Stat.Type, int> _statDict;

        public void Init()
        {
            _statDict = new Dictionary<Stat.Type, int>();
            foreach (Stat.Type statType in System.Enum.GetValues(typeof(Stat.Type)))
            {
                _statDict.Add(statType, BaseStatPoints);
            }
        }

        public int GetStatValue(Stat.Type statType)
        {
            return _statDict[statType];
        }

        public void SetStatValue(Stat.Type statType, int value)
        {
            value = Mathf.Max(value, BaseStatPoints); // prevent stat value from going below base value
            _statDict[statType] = value;
        }

        public int GetTotalSpentStatPoints()
        {
            int total = 0;
            foreach (Stat.Type statType in _statDict.Keys)
            {
                total += _statDict[statType];
            }
            return total;
        }
    }
}
