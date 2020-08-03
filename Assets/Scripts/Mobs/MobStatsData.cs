using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    [CreateAssetMenu(fileName = "MobStatsData", menuName = "ScriptableObjects/MobStatsData", order = 1)]
    public class MobStatsData : ScriptableObject
    {
        public Dictionary<Stat, int> _stats;

        public void Init()
        {
            _stats = new Dictionary<Stat, int>();
            foreach (Stat.Type statType in System.Enum.GetValues(typeof(Stat.Type)))
            {
                Stat stat = new Stat();
                _stats.Add(stat, 0);
            }
        }
    }
}