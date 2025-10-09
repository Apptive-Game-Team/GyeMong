using UnityEngine;

namespace GyeMong.GameSystem.Creature.Direction
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class DirectionVisualizer2 : MonoBehaviour
    {
        private DirectionController _controller;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Mesh _mesh;

        [Header("View Settings")]
        public float radius = 3f;
        [Range(0, 360)]
        public float viewAngle = 90f;
        public int segments = 120;
        public Color frontColor = new Color(1, 1, 0, 0.3f);
        public Color backColor = new Color(0.2f, 0.2f, 0.2f, 0.15f);

        private void Awake()
        {
            _controller = GetComponentInParent<DirectionController>();
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();

            GenerateMesh();

            Material mat = new Material(Shader.Find("Custom/UnlitVertexColor"));
            _meshRenderer.material = mat;

            var sr = GetComponentInParent<SpriteRenderer>();
            if (sr != null)
            {
                _meshRenderer.sortingLayerID = sr.sortingLayerID;
                _meshRenderer.sortingOrder = sr.sortingOrder + 1;
            }
        }

        private void GenerateMesh()
        {
            _mesh = new Mesh();
            _mesh.name = "DirectionCircle";

            Vector3[] vertices = new Vector3[segments + 1];
            Color[] colors = new Color[vertices.Length];
            int[] triangles = new int[segments * 3];

            vertices[0] = Vector3.zero;

            for (int i = 0; i < segments; i++)
            {
                float angle = (360f / segments) * i * Mathf.Deg2Rad;
                vertices[i + 1] = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * radius;

                float normalizedAngle = Mathf.DeltaAngle(0, angle * Mathf.Rad2Deg);
                bool inView = Mathf.Abs(normalizedAngle) <= viewAngle * 0.5f;

                colors[i + 1] = inView ? frontColor : backColor;
            }

            colors[0] = Color.Lerp(frontColor, backColor, 0.5f);

            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = (i + 2) <= segments ? (i + 2) : 1;
            }

            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.colors = colors;
            _mesh.RecalculateNormals();
            _meshFilter.sharedMesh = _mesh;
        }

        private void Update()
        {
            if (_controller == null) return;

            Vector3 dir = _controller.GetDirection();
            if (dir == Vector3.zero) return;

            transform.rotation = Quaternion.FromToRotation(Vector3.up, dir.normalized);
        }
    }
}
