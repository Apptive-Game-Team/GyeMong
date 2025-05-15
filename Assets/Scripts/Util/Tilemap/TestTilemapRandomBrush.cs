// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.Tilemaps;
//
// public class TestTilemapRandomBrush : EditorWindow
// {
//     public Tilemap tilemap;
//     public TileBase[] decorativeTiles;
//     public float[] tileProbabilities; // decorativeTiles와 인덱스 맞추기
//     public Vector2Int from = Vector2Int.zero;
//     public Vector2Int to = new Vector2Int(10, 10);
//
//     [MenuItem("Tools/Tilemap Decorator")]
//     public static void ShowWindow()
//     {
//         GetWindow<TestTilemapRandomBrush>("Tilemap Decorator");
//     }
//
//     void OnGUI()
//     {
//         tilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Tilemap), true);
//         SerializedObject so = new SerializedObject(this);
//         SerializedProperty tilesProp = so.FindProperty("decorativeTiles");
//         SerializedProperty probsProp = so.FindProperty("tileProbabilities");
//
//         EditorGUILayout.PropertyField(tilesProp, true);
//         EditorGUILayout.PropertyField(probsProp, true);
//         so.ApplyModifiedProperties();
//
//         from = EditorGUILayout.Vector2IntField("From", from);
//         to = EditorGUILayout.Vector2IntField("To", to);
//
//         if (GUILayout.Button("Decorate!"))
//         {
//             Decorate();
//         }
//     }
//
//     void Decorate()
//     {
//         if (tilemap == null || decorativeTiles == null || decorativeTiles.Length == 0)
//         {
//             Debug.LogError("Tilemap or decorative tiles not set.");
//             return;
//         }
//
//         for (int x = from.x; x <= to.x; x++)
//         {
//             for (int y = from.y; y <= to.y; y++)
//             {
//                 Vector3Int pos = new Vector3Int(x, y, 0);
//                 if (tilemap.GetTile(pos) != null) continue; // 이미 타일 있으면 패스
//
//                 float rand = Random.value;
//                 float cumulative = 0f;
//                 for (int i = 0; i < decorativeTiles.Length; i++)
//                 {
//                     cumulative += tileProbabilities[i];
//                     if (rand <= cumulative)
//                     {
//                         tilemap.SetTile(pos, decorativeTiles[i]);
//                         break;
//                     }
//                 }
//             }
//         }
//     }
// }
