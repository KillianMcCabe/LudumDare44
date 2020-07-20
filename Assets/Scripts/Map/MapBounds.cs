using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PaperDungeons
{
    public class MapBounds : MonoBehaviour
    {
        public float minX = -10;
        public float minY = -10;
        public float maxX = 10;
        public float maxY = 10;

        // public float MinX
        // {
        //     get { return _minX; }
        // }
        // public float MinY
        // {
        //     get { return _minY; }
        // }
        // public float MaxX
        // {
        //     get { return _maxX; }
        // }
        // public float MaxY
        // {
        //     get { return _maxY; }
        // }

        public Vector3 TopLeft
        {
            get { return new Vector3(minX, maxY); }
        }
        public Vector3 TopRight
        {
            get { return new Vector3(maxX, maxY); }
        }
        public Vector3 BottomLeft
        {
            get { return new Vector3(minX, minY); }
        }
        public Vector3 BottomRight
        {
            get { return new Vector3(maxX, minY); }
        }

        private void OnValidate()
        {
            if (minX > maxX)
            {
                float temp = minX;
                minX = maxX;
                maxX = temp;
            }
            if (minY > maxY)
            {
                float temp = minY;
                minY = maxY;
                maxY = temp;
            }
        }

        private void OnDrawGizmos()
        {
            Handles.color = Color.blue;
            Handles.DrawAAPolyLine(width: 5, new Vector3[] { TopLeft, TopRight, BottomRight, BottomLeft, TopLeft });
        }
    }
}
