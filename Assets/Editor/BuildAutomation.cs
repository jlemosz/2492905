using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

public static class BuildAutomation
{
    // This method can be called from a build script or the command line.
    [MenuItem("Tools/Disable Auto Baking For Build Scenes")]
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

            // In batch mode, we must manually queue an update to ensure the editor
            // processes the scene load and any resulting asset imports.
            EditorApplication.QueuePlayerLoopUpdate();

            if (Lightmapping.TryGetLightingSettings(out LightingSettings lightingSettings))
            {
                lightingSettings.autoGenerate = false;
                Debug.Log($"Set 'autoGenerate' to false for scene: {scene.path}");
                EditorSceneManager.SaveOpenScenes();
            }
            else
            {
                Debug.LogWarning($"Could not retrieve LightingSettings for {scene.path}. The setting may not have been changed.");
            }
        }

        Debug.Log("Finished disabling auto lightmap baking.");
    }
}