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

        [System.Serializable]
        public struct StatElement
        {
            [HideInInspector]
            public string name;     // This is displayed as the name of the element in the inspector
            [HideInInspector]
            public Stat.Type type;
            public int value;

            public StatElement(Stat.Type type, int value)
            {
                this.type = type;
                this.name = type.ToString();
                this.value = value;
            }
        }

        public List<StatElement> _statList;

        public void Init()
        {
            _statList = new List<StatElement>();
            foreach (Stat.Type statType in System.Enum.GetValues(typeof(Stat.Type)))
            {
                _statList.Add(new StatElement(statType, BaseStatPoints));
            }
        }

        public void OnValidate()
        {
            // ensure stat list is not empty
            if (_statList == null)
                _statList = new List<StatElement>();

            Stat.Type[] statTypes = (Stat.Type[]) System.Enum.GetValues(typeof(Stat.Type));

            // ensure stat list is correct size (same count as stat type enum)
            if (_statList.Count != statTypes.Length)
            {
                ListExtensions.Resize(_statList, statTypes.Length);
            }

            // ensure all types are in unique
            for (int i = 0; i < statTypes.Length; i++)
            {
                StatElement el = _statList[i];
                el.type = statTypes[i];
                el.name = statTypes[i].ToString();
                _statList[i] = el;
            }
        }

        public int GetStatValue(Stat.Type statType)
        {
            for (int i = 0; i < _statList.Count; i++)
            {
                if (_statList[i].type == statType)
                    return _statList[i].value;
            }

            Debug.LogError("Could not find stat of type " + statType);
            return -1;
        }

        public void SetStatValue(Stat.Type statType, int value)
        {
            value = Mathf.Max(value, BaseStatPoints); // prevent stat value from going below base value

            for (int i = 0; i < _statList.Count; i++)
            {
                if (_statList[i].type == statType)
                {
                    StatElement el = _statList[i];
                    el.value = value;
                    _statList[i] = el;
                    return;
                }
            }

            Debug.LogError("Could not find stat of type " + statType);
        }

        public int GetTotalSpentStatPoints()
        {
            int total = 0;
            foreach (StatElement stat in _statList)
            {
                total += stat.value;
            }
            return total;
        }
    }
}
