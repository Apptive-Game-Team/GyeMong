using UnityEngine;

namespace GyeMong.GameSystem.Creature.Direction
{
    public class DirectionVisualizer : MonoBehaviour
    {
        private DirectionController _controller;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Mesh _mesh;

        [Header("Cone Settings")]
        public float length = 3f;
        public float angle = 45f;
        public Color color = new Color(1, 1, 0, 0.3f);

        private void Awake()
        {
            _controller = GetComponentInParent<DirectionController>();

            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();

            _mesh = new Mesh();
            Vector3[] verts = new Vector3[3];
            verts[0] = Vector3.zero;
            verts[1] = Quaternion.Euler(0, 0, -angle) * Vector3.right * length;
            verts[2] = Quaternion.Euler(0, 0, angle) * Vector3.right * length;

            int[] tris = { 0, 1, 2 };

            _mesh.vertices = verts;
            _mesh.triangles = tris;

            _meshFilter.sharedMesh = _mesh;

            Material mat = new Material(Shader.Find("Custom/DirectionCone"));
            mat.SetColor("_Color", color);
            _meshRenderer.material = mat;

            var sr = GetComponentInParent<SpriteRenderer>();
            if (sr != null)
            {
                _meshRenderer.sortingLayerID = sr.sortingLayerID;
                _meshRenderer.sortingOrder = sr.sortingOrder + 1;
            }
        }

        private void Update()
        {
            if (_controller == null) return;

            Vector3 dir = _controller.GetDirection();
            if (dir == Vector3.zero) return;

            Quaternion rot = Quaternion.FromToRotation(Vector3.right, dir.normalized);
            transform.localRotation = rot;
        }
    }
}
