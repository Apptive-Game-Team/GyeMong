using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Util
{ 
    public static class ScriptableObjectFinder
    {
        public static List<T> FindAllScriptableObjects<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            List<T> results = new List<T>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                {
                    results.Add(asset);
                }
            }

            return results;
        }
    }
}