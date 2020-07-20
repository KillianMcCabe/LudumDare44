using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PaperDungeons
{
    [CustomEditor(typeof(MapBounds))]
    public class MapBoundsEditor : Editor
    {
        public void OnSceneGUI()
        {
            var t = (target as MapBounds);

            EditorGUI.BeginChangeCheck();
            Vector3 bottomLeft = Handles.PositionHandle(t.BottomLeft, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move point");
                t.minX = bottomLeft.x;
                t.minY = bottomLeft.y;
            }

            EditorGUI.BeginChangeCheck();
            Vector3 topRight = Handles.PositionHandle(t.TopRight, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Move point");
                t.maxX = topRight.x;
                t.maxY = topRight.y;
            }
        }
    }
}
