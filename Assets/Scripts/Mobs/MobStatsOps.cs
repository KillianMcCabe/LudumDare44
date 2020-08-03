using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    public class MobStatsOps
    {
        /// <summary>
        /// Load stats sheet from a scriptable object
        /// Used for loading stats pre-defined for NPCs or monsters
        /// </summary>
        public static void LoadFromScriptableObject(MobStats stats, MobStatsData data)
        {
            stats = data.stats;
        }

        /// <summary>
        /// Load stats from persistant storage
        /// Used to load stats for players and other entities which grow throughout the game
        /// </summary>
        public static void LoadFromPath()
        {
            
        }
    }
}
