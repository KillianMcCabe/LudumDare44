using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperDungeons
{
    public class MapManager : SingletonMonoBehaviour<MapManager>
    {
        public MapBounds MapBounds => _mapBounds;
        public NavigationGrid NavGrid => _navGrid;

        [SerializeField] private MapBounds _mapBounds = null;
        [SerializeField] private NavigationGrid _navGrid = null;
    }
}