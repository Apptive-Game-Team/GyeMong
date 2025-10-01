using UnityEngine;

namespace GyeMong.GameSystem.Creature.Direction
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class DirectionVisualizer : MonoBehaviour
    {
        private DirectionController _controller;
        private MeshFilter _meshFilter;
        private Mesh _mesh;

        [Header("Vision Settings")]
        public float viewAngle = 45f;
        public float viewDistance = 5f;
        public int segments = 10;
        public Color coneColor = new Color(1f, 0f, 0f, 0.3f);

        private void Awake()
        {
            _controller = GetComponentInParent<DirectionController>();
            _meshFilter = GetComponent<MeshFilter>();

            _mesh = new Mesh();
            _mesh.name = "VisionConeMesh";
            _meshFilter.mesh = _mesh;

            var renderer = GetComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("Unlit/Color"))
            {
                color = coneColor
            };
        }

        private void LateUpdate()
        {
            DrawVisionCone();
        }

        private void DrawVisionCone()
        {
            Vector3 forward = _controller.GetDirection().normalized;
            Vector3 origin = Vector3.zero;

            Vector3[] vertices = new Vector3[segments + 2];
            vertices[0] = origin;

            float halfAngle = viewAngle * 0.5f;

            for (int i = 0; i <= segments; i++)
            {
                float angle = -halfAngle + (viewAngle / segments) * i;
                float rad = Mathf.Deg2Rad * angle;

                Vector3 dir = new Vector3(
                    forward.x * Mathf.Cos(rad) - forward.y * Mathf.Sin(rad),
                    forward.x * Mathf.Sin(rad) + forward.y * Mathf.Cos(rad),
                    0);

                vertices[i + 1] = dir * viewDistance;
            }

            int[] triangles = new int[segments * 3];
            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }
    }
}
