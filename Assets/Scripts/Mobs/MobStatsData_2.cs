using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PaperDungeons
{
    [CreateAssetMenu(fileName = "MobStatsData_2", menuName = "ScriptableObjects/MobStatsData_2", order = 1)]
    public class MobStatsData_2 : ScriptableObject
    {
        [System.Serializable]
        public struct DataElement
        {
            [HideInInspector]
            public string name;     // This is displayed as the name of the element in the inspector
            [HideInInspector]
            public Stat.Type type;
            public int value;

            public DataElement(Stat.Type type, int value)
            {
                this.type = type;
                this.name = type.ToString();
                this.value = value;
            }
        }

        public List<DataElement> _statValues = new List<DataElement>();

        public int armor = 0;
        public int initiative = 0;
        public int speed = 0;

        // skills (maybe only needed for players)
        public int acrobatics = 0;

        public void Init()
        {
            _statValues = new List<DataElement>();
            foreach (Stat.Type statType in System.Enum.GetValues(typeof(Stat.Type)))
            {
                _statValues.Add(new DataElement(statType, 0));
            }
        }

        private void OnValidate()
        {
            if (_statValues == null)
                _statValues = new List<DataElement>();

            Stat.Type[] statTypes = (Stat.Type[]) System.Enum.GetValues(typeof(Stat.Type));

            // resize
            if (_statValues.Count != statTypes.Length)
            {
                ListExtra.Resize(_statValues, statTypes.Length);
            }

            // ensure all types are in unique
            for (int i = 0; i < statTypes.Length; i++)
            {
                DataElement el = _statValues[i];
                el.type = statTypes[i];
                el.name = statTypes[i].ToString();
                _statValues[i] = el;
            }
        }
    }

    public static class ListExtra
    {
        public static void Resize<T>(this List<T> list, int sz, T c)
        {
            int cur = list.Count;
            if(sz < cur)
                list.RemoveRange(sz, cur - sz);
            else if(sz > cur)
            {
                if(sz > list.Capacity)//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                list.Capacity = sz;
                list.AddRange(Enumerable.Repeat(c, sz - cur));
            }
        }

        // public static void Resize<T>(this List<T> list, int size, T element = default(T))
        // {
        //     int count = list.Count;

        //     if (size < count)
        //     {
        //         list.RemoveRange(size, count - size);
        //     }
        //     else if (size > count)
        //     {
        //         if (size > list.Capacity)   // Optimization
        //             list.Capacity = size;

        //         list.AddRange(Enumerable.Repeat(element, size - count));
        //     }
        // }

        public static void Resize<T>(this List<T> list, int sz) where T : new()
        {
            Resize(list, sz, new T());
        }
    }
}