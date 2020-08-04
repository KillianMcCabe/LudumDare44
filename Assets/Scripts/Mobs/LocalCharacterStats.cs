using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    public static class LocalCharacterStats
    {
        public static int TotalAvailableStatPoints = 50;

        public static int UnspentStatPoints
        {
            get
            {
                return TotalAvailableStatPoints - stats.GetTotalSpentStatPoints();
            }
        }

        public static MobStats stats;
    }
}