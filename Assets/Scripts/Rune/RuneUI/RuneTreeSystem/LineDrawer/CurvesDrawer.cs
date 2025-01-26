using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace runeSystem.RuneTreeSystem
{
    public class CurvesDrawer : Image
    {
        [SerializeField] protected Sprite sprite;
        [SerializeField] protected Color tintColor = Color.black;
        [SerializeField] protected float thickness = 5f;
        [SerializeField] protected int meshDetail = 20;
        
        private List<(Vector2 pos, Color32 color, Vector2 uv)> _vertices;
        private List<(int idx, int idx2, int idx3)> _triangles;
        
        private List<Vector2[]> _curves = new List<Vector2[]>();

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            
            vh.Clear();
            if (_vertices == null)
                _vertices = new List<(Vector2 pos, Color32 color, Vector2 uv)>();
            if (_triangles == null)
                _triangles = new List<(int idx, int idx2, int idx3)>();

            (Vector2 left, Vector2 right) start, end = default;
            foreach (Vector2[] points in _curves)
            {
                Vector2 lastPoint = GetCubicBezierPoint(0, points);
                for (int i = 1; i < meshDetail; i++)
                {
                    float t = i / (float) meshDetail;
                    Vector2 point = GetCubicBezierPoint(t, points);
                    start = GetLineSide(lastPoint, point, 0, thickness);
                    end = GetLineSide(lastPoint, point, 1, thickness);
                    lastPoint = point;
                    StackLine(start, end);
                }
            }

            ApplyMesh(vh);
        }
        
        private void StackLine((Vector2 left, Vector2 right) start, (Vector2 left, Vector2 right) end)
        {
            float distance = (Vector2.Distance(start.left, end.left) + Vector2.Distance(start.right, end.right)) * 0.5f;
            _vertices.Add((start.left, color, new Vector2(0f, 0f)));
            _vertices.Add((end.left, color, new Vector2(0f, 1f)));
            _vertices.Add((end.right, color, new Vector2(1f, 1f)));
            _vertices.Add((start.right, color, new Vector2(1f, 0f)));
            int count = _vertices.Count;
            _triangles.Add((count-4, count-3, count-2));
            _triangles.Add((count-4, count-2, count-1));
        }
        
        private void ApplyMesh(VertexHelper vh)
        {
            for (int i = 0, l = _vertices.Count; i < l; ++i)
                vh.AddVert(_vertices[i].pos, _vertices[i].color, _vertices[i].uv);
            for (int i = 0, l = _triangles.Count; i < l; ++i)
                vh.AddTriangle(_triangles[i].idx, _triangles[i].idx2, _triangles[i].idx3);
            
            _vertices.Clear();
            _triangles.Clear();
        }

        public void Clear()
        {
            _vertices = null;
            _triangles = null;
            _curves.Clear();
            SetVerticesDirty();
        }
        
        public void AddCurve(Vector2[] points)
        {
            _curves.Add(points);
            SetVerticesDirty();
        }
        
        public static (Vector2 left, Vector2 right) GetLineSide(Vector2 start, Vector2 end, float t, float thickness)
        {
            Vector3 dir = end - start;
            Quaternion lookRot = Quaternion.LookRotation(dir, -Vector3.forward);
            
            Vector2 center = Vector2.Lerp(start, end, t);
            Vector2 left = center + (Vector2)(lookRot * Quaternion.Euler(0f, -90f, 0f) * Vector3.forward * thickness * 0.5f);
            Vector2 right = center + (Vector2)(lookRot * Quaternion.Euler(0f, 90f, 0f) * Vector3.forward * thickness * 0.5f);

            return (left, right);
        }
        
        public Vector2 GetCubicBezierPoint(float t, Vector2[] points)
        {
            return Mathf.Pow(1-t, 3) * points[0] + 
                   3 * Mathf.Pow(1-t, 2) * t * points[1] + 
                   3 * (1-t)* Mathf.Pow(t, 2) * points[2] +
                   Mathf.Pow(t, 3) * points[3];
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(CurvesDrawer))]
    public class CurvesDrawerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var targetType = typeof(CurvesDrawer);
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