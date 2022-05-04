using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridAuthoring))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var grid = target as GridAuthoring;

        if (GUILayout.Button("Generate map"))
        {
            grid.GenerateGrid();
        }
    }
}