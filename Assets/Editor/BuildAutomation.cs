using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

public static class BuildAutomation
{
    private const string BuildLightingSettingsPath = "Assets/Editor/BuildAutomationLightingSettings.lighting";

    [MenuItem("Tools/Disable Auto Baking For Build Scenes")]
    public static void DisableAutoBaking()
    {
        Debug.Log("Starting to disable auto lightmap baking for all build scenes.");

        // Step 1: Load or create a canonical LightingSettings asset.
        LightingSettings buildLightingSettings = AssetDatabase.LoadAssetAtPath<LightingSettings>(BuildLightingSettingsPath);
        if (buildLightingSettings == null)
        {
            Debug.LogWarning($"Canonical lighting settings not found at '{BuildLightingSettingsPath}'. Creating it now.");
            LightingSettings newSettings = new LightingSettings();
            newSettings.autoGenerate = false;
            
            // Ensure the 'Assets/Editor' directory exists.
            Directory.CreateDirectory(Path.GetDirectoryName(BuildLightingSettingsPath));
            
            AssetDatabase.CreateAsset(newSettings, BuildLightingSettingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            buildLightingSettings = AssetDatabase.LoadAssetAtPath<LightingSettings>(BuildLightingSettingsPath);
        }

        if (buildLightingSettings == null)
        {
            Debug.LogError($"Failed to create or load the canonical lighting settings asset at '{BuildLightingSettingsPath}'. Aborting.");
            return;
        }
        
        Debug.Log("Successfully loaded canonical lighting settings asset.");

        // Step 2: Loop through scenes and apply the settings where needed.
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled || !File.Exists(scene.path) || scene.path.Contains("Packages/"))
            {
                continue;
            }

            EditorSceneManager.OpenScene(scene.path);
            Debug.Log($"Opened scene: {scene.path}");

            bool settingsApplied = false;
            if (Lightmapping.TryGetLightingSettings(out LightingSettings currentSettings) && currentSettings.autoGenerate)
            {
                // Scene has settings, but auto-baking is enabled.
                Debug.Log("Scene has existing settings with autoGenerate=true. Overwriting with canonical settings.");
                Lightmapping.lightingSettings = buildLightingSettings;
                settingsApplied = true;
            }
            else if (currentSettings == null)
            {
                // Scene is using default settings, so we must assign ours.
                Debug.Log("Scene is using default lighting settings. Assigning canonical settings.");
                Lightmapping.lightingSettings = buildLightingSettings;
                settingsApplied = true;
            }
            else
            {
                Debug.Log("Scene already has correct lighting settings. No changes needed.");
            }

            if (settingsApplied)
            {
                Debug.Log($"Applied canonical lighting settings to scene: {scene.path}");
                EditorSceneManager.SaveOpenScenes();
            }
        }

        Debug.Log("Finished disabling auto lightmap baking.");
    }
}