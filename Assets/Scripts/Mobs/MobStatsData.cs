using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PaperDungeons
{
    [CreateAssetMenu(fileName = "MobStatsData", menuName = "ScriptableObjects/MobStatsData", order = 1)]
    public class MobStatsData : ScriptableObject
    {
        public MobStats stats;
        public MobAttributes attributes;

        private void OnValidate()
        {
            stats.OnValidate();
        }

        // TODO: list of attacks & spells
    }
}