using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using Util;
using Object = UnityEngine.Object;

public static class SceneRuntimeMigration
{
    private static readonly string[] Prefabs =
    {
        "Assets/Resources/Prefabs/GameManager.prefab",
        "Assets/Resources/Prefabs/UI/OptionController.prefab",
        "Assets/Resources/Prefabs/InGameManager.prefab",
        "Assets/Resources/Prefabs/UI/ChatCanvas.prefab"
    };

    public static void Audit() { Run(false); }
    public static void Migrate() { Run(true); }

    private static void Run(bool save)
    {
        if (!Application.isBatchMode) throw new InvalidOperationException("Run in batch mode with closed scenes.");
        // Preflight the complete set before saving any scene.
        var paths = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).Distinct().ToArray();
        foreach (var path in paths) Inspect(path, false);
        if (save) foreach (var path in paths) Inspect(path, true);
        Debug.Log($"SceneRuntimeMigration PASS: {paths.Length} scenes; saved={save}");
    }

    private static void Inspect(string path, bool save)
    {
        var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        var installed = Object.FindObjectsOfType<SceneRuntimeInstaller>();
        if (installed.Length > 0)
        {
            if (installed.Length != 1) throw new InvalidOperationException($"Duplicate installers: {path}");
            using (var settings = new SerializedObject(installed[0]))
                foreach (var field in new[] { "gameManagerPrefab", "optionPrefab" })
                    if (settings.FindProperty(field).objectReferenceValue == null)
                        throw new InvalidOperationException($"Missing runtime prefab: {path}/{field}");
            if (scene.GetRootGameObjects().Any(root => Prefabs.Contains(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root))))
                throw new InvalidOperationException($"Legacy services remain: {path}");
            Debug.Log($"SceneRuntimeMigration installed configuration PASS: {path}");
            return;
        }
        var roots = scene.GetRootGameObjects();
        var services = roots.Where(root => Prefabs.Contains(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root))).ToList();
        // LoadingScene intentionally has no game services or scene UI.
        if (services.Count == 0) return;
        var removedRoots = new List<GameObject>(services);
        var removed = new HashSet<Object>();
        foreach (var root in removedRoots)
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                removed.Add(transform.gameObject);
                foreach (var component in transform.GetComponents<Component>()) if (component != null) removed.Add(component);
            }
        foreach (var root in roots)
            foreach (var component in root.GetComponentsInChildren<Component>(true))
            {
                if (component == null)
                {
                    Debug.LogWarning($"Existing missing script: {path}/{root.name}");
                    continue;
                }
                if (removed.Contains(component)) continue;
                using (var serialized = new SerializedObject(component))
                {
                    var property = serialized.GetIterator();
                    while (property.Next(true))
                        if (property.propertyType == SerializedPropertyType.ObjectReference && removed.Contains(property.objectReferenceValue))
                            throw new InvalidOperationException($"External service reference: {path}/{component.name}.{property.propertyPath}");
                }
            }
        foreach (var service in services)
        {
            bool stageSelectionHud = scene.name == "StageSelectPage" && PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(service) == Prefabs[2];
            if (stageSelectionHud) continue; // Preserved as a prefab variant below, including removed objects/components.
            if (PrefabUtility.GetAddedComponents(service).Count > 0 || PrefabUtility.GetRemovedComponents(service).Count > 0 ||
                PrefabUtility.GetAddedGameObjects(service).Count > 0 ||
                PrefabUtility.GetRemovedGameObjects(service).Any(item => item.assetGameObject.name != "ChatWindow"))
                throw new InvalidOperationException($"Structural override: {path}/{service.name}");
            foreach (var modification in PrefabUtility.GetPropertyModifications(service) ?? Array.Empty<PropertyModification>())
            {
                if (modification.target == null) continue;
                Debug.Log($"SceneRuntimeOverride {path}/{service.name}: {modification.target.name}.{modification.propertyPath} = {modification.value} [{modification.objectReference}]");
                // Layout overrides are reviewed in the audit log. Never discard gameplay configuration.
                string field = modification.propertyPath.Split('.')[0];
                if (new[] { "m_Name", "m_IsActive", "m_LocalPosition", "m_LocalRotation", "m_LocalScale", "m_LocalEulerAnglesHint",
                    "m_AnchoredPosition", "m_AnchorMin", "m_AnchorMax", "m_Pivot", "m_SizeDelta", "m_fontAsset", "m_sharedMaterial",
                    "m_hasFontAssetChanged", "m_Size", "m_Value", "m_SortingOrder" }.Contains(field)) continue;
                using (var source = new SerializedObject(modification.target))
                {
                    var property = source.FindProperty(modification.propertyPath);
                    if (property == null) continue;
                    bool same = property.propertyType == SerializedPropertyType.ObjectReference
                        ? property.objectReferenceValue == modification.objectReference
                        : property.propertyType == SerializedPropertyType.ArraySize && property.intValue.ToString() == modification.value;
                    if (!same) throw new InvalidOperationException($"Configuration override requires migration: {path}/{service.name}.{modification.propertyPath}");
                }
            }
        }
        Debug.Log($"SceneRuntimeMigration audited {path}: {string.Join(", ", removedRoots.Select(root => root.name))}");
        if (!save) return;
        var installer = new GameObject("SceneRuntime").AddComponent<SceneRuntimeInstaller>();
        using (var settings = new SerializedObject(installer))
        {
            settings.FindProperty("gameManagerPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs[0]);
            settings.FindProperty("optionPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs[1]);
            var hud = services.FirstOrDefault(root => PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root) == Prefabs[2]);
            settings.FindProperty("hudPrefab").objectReferenceValue = hud == null ? null : scene.name == "StageSelectPage"
                ? PrefabUtility.SaveAsPrefabAsset(hud, "Assets/Resources/Prefabs/StageSelectRuntime.prefab")
                : AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs[2]);
            settings.FindProperty("chatPrefab").objectReferenceValue = services.Any(root => PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root) == Prefabs[3]) ? AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs[3]) : null;
            settings.ApplyModifiedPropertiesWithoutUndo();
        }
        foreach (var root in removedRoots) Object.DestroyImmediate(root);
        EditorSceneManager.SaveScene(scene);
    }
}
