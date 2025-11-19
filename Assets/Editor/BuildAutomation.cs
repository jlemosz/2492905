using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

public static class BuildAutomation
{
    public static void DisableAutoBaking()
    {
        Debug.Log("Starting to disable auto lightmap baking for all build scenes.");

        // Iterate through all scenes listed in the Build Settings.
        foreach (var scene in EditorBuildSettings.scenes)
        {
            // Skip scenes that are disabled, don't exist, or are in packages.
            if (!scene.enabled || !File.Exists(scene.path) || scene.path.Contains("Packages/"))
            {
                continue;
            }

            // Open the scene to modify its settings.
            EditorSceneManager.OpenScene(scene.path);
            Debug.Log($"Opened scene: {scene.path}");

            LightingSettings lightingSettings = new LightingSettings();
            Lightmapping.TryGetLightingSettings(out lightingSettings);

            if (lightingSettings != null)
            {
                // Disable the auto-generate lighting setting.
                lightingSettings.autoGenerate = false;
                Debug.Log("Set 'autoGenerate' to false.");
            }
            else
            {
                Debug.LogWarning("LightingSettings not found. Skipping.");
            }

            // Save the changes to the scene file.
            EditorSceneManager.SaveOpenScenes();
        }

        Debug.Log("Finished disabling auto lightmap baking.");
    }
}