using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    public class MobStats
    {
        private Dictionary<Stat, int> _stats;

        public void Init()
        {
            _stats = new Dictionary<Stat, int>();
            foreach (Stat.Type statType in System.Enum.GetValues(typeof(Stat.Type)))
            {
                Stat stat = new Stat();
                _stats.Add(stat, 0);
            }
        }

        /// <summary>
        /// Load stats sheet from a scriptable object
        /// Used for loading stats pre-defined for NPCs or monsters
        /// </summary>
        public void LoadFromScriptableObject()
        {
            Init();
        }

        /// <summary>
        /// Load stats from persistant storage
        /// Used to load stats for players and other entities which grow throughout the game
        /// </summary>
        public void LoadFromPath()
        {
            Init();
        }
    }
}
