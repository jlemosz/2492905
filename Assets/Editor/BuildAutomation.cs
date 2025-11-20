using UnityEditor;
using UnityEngine;
using System.IO;

public static class BuildAutomation
{
    // Point this directly to the lighting settings asset your project uses.
    private const string TargetLightingSettingsPath = "Assets/New Lighting Settings.lighting";

    public static void DisableAutoBaking()
    {
        Debug.Log($"Starting to ensure '{TargetLightingSettingsPath}' has all GI baking disabled.");

        // Step 1: Load the specific LightingSettings asset the build uses.
        LightingSettings targetSettings = AssetDatabase.LoadAssetAtPath<LightingSettings>(TargetLightingSettingsPath);

        if (targetSettings == null)
        {
            Debug.LogError($"Could not find the lighting settings asset at '{TargetLightingSettingsPath}'. Please ensure the path is correct. Aborting.");
            return;
        }

        // Step 2: Disable all settings that can trigger a bake.
        targetSettings.autoGenerate = false; // Disable auto-generation.
        targetSettings.bakedGI = false;      // CRITICAL: Disable Baked Global Illumination.
        targetSettings.realtimeGI = false;   // Also disable Realtime Global Illumination.

        Debug.Log("Set autoGenerate, bakedGI, and realtimeGI to false.");

        // Step 3: Mark the asset as dirty and save it to disk.
        EditorUtility.SetDirty(targetSettings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Finished. '{TargetLightingSettingsPath}' is now configured with all baking disabled.");
    }
}