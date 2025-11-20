using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

public static class BuildAutomation
{
    public static void DisableAutoBaking()
    {
        Debug.Log("Starting to disable auto lightmap baking for all build scenes.");

        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled || !File.Exists(scene.path) || scene.path.Contains("Packages/"))
            {
                continue;
            }

            EditorSceneManager.OpenScene(scene.path);
            Debug.Log($"Opened scene: {scene.path}");

            // Attempt to get the existing lighting settings.
            if (Lightmapping.TryGetLightingSettings(out LightingSettings lightingSettings))
            {
                // If settings exist, modify them directly.
                if (lightingSettings.autoGenerate)
                {
                    lightingSettings.autoGenerate = false;
                    Debug.Log($"Set 'autoGenerate' to false for existing LightingSettings in scene: {scene.path}");
                    EditorSceneManager.SaveOpenScenes();
                }
                else
                {
                    Debug.Log($"'autoGenerate' is already false for scene: {scene.path}");
                }
            }
            else
            {
                // If no settings asset exists, the scene uses internal defaults.
                // We must create a new settings object, configure it, and assign it.
                Debug.Log($"No LightingSettings asset found for {scene.path}. Creating and assigning new settings.");
                
                LightingSettings newLightingSettings = new LightingSettings();
                newLightingSettings.autoGenerate = false;

                // Assign the new settings object to the active scene.
                Lightmapping.lightingSettings = newLightingSettings;
                
                Debug.Log($"Assigned new LightingSettings with 'autoGenerate' set to false for scene: {scene.path}");
                EditorSceneManager.SaveOpenScenes();
            }
        }

        Debug.Log("Finished disabling auto lightmap baking.");
    }
}