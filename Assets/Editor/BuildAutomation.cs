using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

public static class BuildAutomation
{
    // A fallback asset for scenes that don't have any lighting settings assigned.
    private const string CanonicalLightingSettingsPath = "Assets/Editor/BuildAutomationLightingSettings.lighting";

    public static void DisableAllBaking()
    {
        Debug.Log("Starting to disable all GI baking for scenes in the build.");

        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled || !File.Exists(scene.path) || scene.path.Contains("Packages/"))
            {
                continue;
            }

            EditorSceneManager.OpenScene(scene.path);
            Debug.Log($"Opened scene: {scene.path}");

            // Get the LightingSettings object currently used by the active scene.
            LightingSettings currentSettings = Lightmapping.lightingSettings;

            if (currentSettings != null)
            {
                // If the scene has an assigned lighting asset, modify it directly.
                Debug.Log($"Found assigned LightingSettings: '{AssetDatabase.GetAssetPath(currentSettings)}'. Modifying it.");
                
                currentSettings.autoGenerate = false;
                currentSettings.bakedGI = false;
                currentSettings.realtimeGI = false;

                // Mark the modified asset as dirty and save it.
                EditorUtility.SetDirty(currentSettings);
                AssetDatabase.SaveAssets();
            }
            else
            {
                // If the scene has no assigned asset (using internal defaults), assign our canonical one.
                Debug.LogWarning("Scene has no assigned LightingSettings. Assigning canonical asset.");
                
                // Load or create the canonical asset.
                LightingSettings canonicalSettings = AssetDatabase.LoadAssetAtPath<LightingSettings>(CanonicalLightingSettingsPath);
                if (canonicalSettings == null)
                {
                    Debug.Log("Canonical asset not found. Creating it.");
                    canonicalSettings = new LightingSettings { autoGenerate = false, bakedGI = false, realtimeGI = false };
                    Directory.CreateDirectory(Path.GetDirectoryName(CanonicalLightingSettingsPath));
                    AssetDatabase.CreateAsset(canonicalSettings, CanonicalLightingSettingsPath);
                    AssetDatabase.SaveAssets();
                }

                // Assign the canonical settings to the scene and save the scene itself.
                Lightmapping.lightingSettings = canonicalSettings;
                EditorSceneManager.SaveOpenScenes();
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Finished disabling GI baking for all build scenes.");
    }
}