// using UnityEngine;
// using UnityEditor;
// using UnityEngine.Tilemaps;
// public class PerlinTilemapDecorator : EditorWindow
// {
//     public Tilemap tilemap;
//     public TileBase tile;
//     public Vector2Int from = Vector2Int.zero;
//     public Vector2Int to = new Vector2Int(10, 10);
//     public float noiseScale = 0.1f;
//     public float threshold = 0.6f;
//
//     [MenuItem("Tools/Perlin Decorator")]
//     public static void ShowWindow()
//     {
//         GetWindow<PerlinTilemapDecorator>("Perlin Decorator");
//     }
//
//     private bool picking = false;
//     private int clickCount = 0;
//
//     private void OnEnable()
//     {
//         SceneView.duringSceneGui += OnSceneGUI;
//     }
//
//     private void OnDisable()
//     {
//         SceneView.duringSceneGui -= OnSceneGUI;
//     }
//
//     private void OnSceneGUI(SceneView sceneView)
//     {
//         if (!picking || tilemap == null) return;
//
//         UnityEngine.Event e = UnityEngine.Event.current;
//
//         if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
//         {
//             Vector2 mousePos = e.mousePosition;
//             Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
//             if (Physics.Raycast(ray, out RaycastHit hit)) return; // 무언가를 막 클릭했을 경우 무시
//
//             Vector3 worldPos = HandleUtility.GUIPointToWorldRay(mousePos).origin;
//             Vector3Int cellPos = tilemap.WorldToCell(worldPos);
//
//             if (clickCount == 0)
//             {
//                 from = new Vector2Int(cellPos.x, cellPos.y);
//                 clickCount++;
//             }
//             else
//             {
//                 to = new Vector2Int(cellPos.x, cellPos.y);
//                 clickCount = 0;
//                 picking = false;
//             }
//
//             e.Use();
//             sceneView.Repaint();
//             Repaint();
//         }
//
//         // 박스 시각화
//         if (clickCount == 1)
//         {
//             Handles.color = Color.yellow;
//             Vector3 p1 = tilemap.CellToWorld(new Vector3Int(from.x, from.y, 0));
//             Vector3 p2 = HandleUtility.GUIPointToWorldRay(UnityEngine.Event.current.mousePosition).origin;
//             Vector3Int temp = tilemap.WorldToCell(p2);
//             Vector3 p2Final = tilemap.CellToWorld(temp);
//             Handles.DrawSolidRectangleWithOutline(new Vector3[] {
//                 p1,
//                 new Vector3(p1.x, p2Final.y, 0),
//                 p2Final,
//                 new Vector3(p2Final.x, p1.y, 0)
//             }, new Color(1,1,0,0.2f), Color.yellow);
//         }
//     }
//     
//     void OnGUI()
//     {
//         tilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Tilemap), true);
//         tile = (TileBase)EditorGUILayout.ObjectField("Tile", tile, typeof(TileBase), false);
//         from = EditorGUILayout.Vector2IntField("From", from);
//         to = EditorGUILayout.Vector2IntField("To", to);
//         noiseScale = EditorGUILayout.FloatField("Noise Scale", noiseScale);
//         threshold = EditorGUILayout.Slider("Threshold", threshold, 0f, 1f);
//
//         if (GUILayout.Button("Decorate with Perlin Noise"))
//         {
//             DecorateWithPerlin();
//         }
//
//         if (GUILayout.Button("Clear Tiles in Range"))
//         {
//             ClearTiles();
//         }
//         
//         if (GUILayout.Button("Pick Area In Scene"))
//         {
//             picking = true;
//             clickCount = 0;
//         }
//     }
//
//     void DecorateWithPerlin()
//     {
//         if (tilemap == null || tile == null) return;
//
//         for (int x = from.x; x <= to.x; x++)
//         {
//             for (int y = from.y; y <= to.y; y++)
//             {
//                 float nx = x * noiseScale;
//                 float ny = y * noiseScale;
//                 float noise = Mathf.PerlinNoise(nx, ny);
//
//                 if (noise > threshold)
//                 {
//                     Vector3Int pos = new Vector3Int(x, y, 0);
//                     tilemap.SetTile(pos, tile);
//                 }
//             }
//         }
//     }
//
//     void ClearTiles()
//     {
//         if (tilemap == null) return;
//
//         for (int x = from.x; x <= to.x; x++)
//         {
//             for (int y = from.y; y <= to.y; y++)
//             {
//                 Vector3Int pos = new Vector3Int(x, y, 0);
//                 tilemap.SetTile(pos, null);
//             }
//         }
//     }
// }
