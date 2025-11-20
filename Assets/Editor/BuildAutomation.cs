using UnityEditor;
using UnityEngine;
using System.IO;

public static class BuildAutomation
{
    // Point this directly to the lighting settings asset your project uses.
    private const string TargetLightingSettingsPath = "Assets/New Lighting Settings.lighting";

    public static void DisableAutoBaking()
    {
        Debug.Log($"Starting to ensure '{TargetLightingSettingsPath}' has auto-baking disabled.");

        // Step 1: Load the specific LightingSettings asset the build uses.
        LightingSettings targetSettings = AssetDatabase.LoadAssetAtPath<LightingSettings>(TargetLightingSettingsPath);

        if (targetSettings == null)
        {
            Debug.LogError($"Could not find the lighting settings asset at '{TargetLightingSettingsPath}'. Please ensure the path is correct. Aborting.");
            return;
        }

        // Step 2: Modify the asset directly if needed.
        // Note: We check for 'true' because the obsolete property might still read 'false' incorrectly.
        // The safest action is to just set it to false regardless.
        if (targetSettings.autoGenerate)
        {
            Debug.LogWarning("'autoGenerate' was true. Setting it to false.");
        }
        
        targetSettings.autoGenerate = false;

        // Step 3: Mark the asset as dirty and save it to disk.
        EditorUtility.SetDirty(targetSettings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Finished. '{TargetLightingSettingsPath}' is now configured with auto-baking disabled.");
    }
}