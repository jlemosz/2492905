using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using UnityEditor;

public static class BuildAutomation
{
    private const string BuildLightingSettingsPath = "Assets/Editor/BuildAutomationLightingSettings.lighting";

    public static void DisableAutoBaking()
    {
        Debug.Log("Starting to disable auto lightmap baking for all build scenes.");

        // Step 1: Load or create a canonical LightingSettings asset with autoGenerate=false.
        LightingSettings buildLightingSettings = AssetDatabase.LoadAssetAtPath<LightingSettings>(BuildLightingSettingsPath);
        if (buildLightingSettings == null)
        {
            Debug.LogWarning($"Canonical lighting settings not found at '{BuildLightingSettingsPath}'. Creating it now.");
            LightingSettings newSettings = new LightingSettings();
            newSettings.autoGenerate = false;
            
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

        // Step 2: Unconditionally apply the canonical settings to every scene.
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled || !File.Exists(scene.path) || scene.path.Contains("Packages/"))
            {
                continue;
            }

            EditorSceneManager.OpenScene(scene.path);
            Debug.Log($"Opened scene: {scene.path}");

            // Since the 'autoGenerate' property is obsolete and unreliable, we will not check it.
            // Instead, we unconditionally assign our known-good settings to every scene
            // to guarantee the correct state for the build.
            Lightmapping.lightingSettings = buildLightingSettings;
            
            Debug.Log($"Unconditionally applied canonical lighting settings to scene: {scene.path}");
            EditorSceneManager.SaveOpenScenes();
        }

        Debug.Log("Finished disabling auto lightmap baking.");
    }
}