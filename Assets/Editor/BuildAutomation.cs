using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class BuildAutomation
{
    private static Queue<EditorBuildSettingsScene> sceneQueue;
    
    public static void DisableAutoBaking()
    {
        Debug.Log("Queueing build scenes to disable auto lightmap baking.");

        sceneQueue = new Queue<EditorBuildSettingsScene>();

        // Populate the queue with all enabled scenes from Build Settings.
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && File.Exists(scene.path) && !scene.path.Contains("Packages/"))
            {
                sceneQueue.Enqueue(scene);
            }
        }

        // Register the update method to process the queue.
        EditorApplication.update += ProcessSceneQueue;
    }

    private static void ProcessSceneQueue()
    {
        // If the queue is empty, we are done.
        if (sceneQueue.Count == 0)
        {
            Debug.Log("Finished disabling auto lightmap baking for all scenes.");
            // Unregister the update method to stop processing.
            EditorApplication.update -= ProcessSceneQueue;
            return;
        }

        // Dequeue the next scene and open it.
        var sceneToProcess = sceneQueue.Dequeue();
        Debug.Log($"Opening scene: {sceneToProcess.path}");
        EditorSceneManager.OpenScene(sceneToProcess.path);

        // Attempt to get the lighting settings.
        if (Lightmapping.TryGetLightingSettings(out LightingSettings lightingSettings))
        {
            // Disable the auto-generate lighting setting.
            lightingSettings.autoGenerate = false;
            Debug.Log($"Set 'autoGenerate' to false for scene: {sceneToProcess.path}");

            // Save the changes to the scene file.
            EditorSceneManager.SaveOpenScenes();
        }
        else
        {
            // This can happen if the settings asset is not immediately available.
            Debug.LogWarning($"Could not retrieve LightingSettings immediately for {sceneToProcess.path}. The setting may not have been changed.");
        }
    }
}