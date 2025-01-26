using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace runeSystem.RuneTreeSystem
{
    [Serializable]
    public struct Line
    {
        public RectTransform startNode;
        public RectTransform endNode;
    }
    public class LinesDrawer : CurvesDrawer
    {
        public List<Line> nodes = new List<Line>();
        
        private void Start()
        {
            DrawLines();
        }
        
        private void DrawLines()
        {
            foreach (var node in nodes)
            {
                Vector2 position1 = node.startNode.position;
                Vector2 position2 = node.endNode.position;
                AddCurve(new Vector2[]{position1, position1, position2, position2});
            }
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(LinesDrawer))]
    public class LinesDrawerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var targetType = typeof(LinesDrawer);
            var fields = targetType.GetFields(
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly
            );

            foreach (var field in fields)
            {
                var property = serializedObject.FindProperty(field.Name);
                if (property != null)
                {
                    EditorGUILayout.PropertyField(property, true);
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}