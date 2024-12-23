using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SceneLoadHelper
{
    /// <summary>
    /// Asynchronously loads a scene by name with the specified load mode.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <param name="mode">The mode to use when loading the scene (Single or Additive).</param>
    /// <param name="onCompleted">An optional callback to invoke upon completion.</param>
    /// <param name="tryLoad">If true, the scene will only be loaded if it's not already loading or loaded.</param>
    /// <param name="reload">If true, the scene will be unloaded before loading.</param>
    public static async Task LoadSceneAsync(
        string sceneName,
        LoadSceneMode mode = LoadSceneMode.Single,
        Action onCompleted = null,
        bool tryLoad = false,
        bool reload = false
    )
    {
        if (IsSceneLoaded(sceneName) && !reload)
        {
            Debug.Log($"Scene '{sceneName}' is already loaded.");
            onCompleted?.Invoke();
            return;
        }

        if (loadingScenes.ContainsKey(sceneName))
        {
            Debug.Log($"Scene '{sceneName}' is already loading.");
            if (!tryLoad && onCompleted != null)
            {
                // Add the callback to the pending list if not null and not a tryLoad
                if (!pendingCallbacks.ContainsKey(sceneName))
                {
                    pendingCallbacks[sceneName] = new List<Action>();
                }
                pendingCallbacks[sceneName].Add(onCompleted);
            }
            return;
        }

        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' is not in build settings.");
            return;
        }

        if (reload)
        {
            await UnloadSceneAsync(sceneName);
        }

        // Initialize the pending callbacks list
        if (onCompleted != null)
        {
            pendingCallbacks[sceneName] = new List<Action> { onCompleted };
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
        loadingScenes.Add(sceneName, asyncLoad);

        try
        {
            await asyncLoad;
            Debug.Log($"Scene '{sceneName}' loaded successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load scene '{sceneName}': {ex.Message}");
        }
        finally
        {
            loadingScenes.Remove(sceneName);

            // Invoke all pending callbacks
            if (pendingCallbacks.ContainsKey(sceneName))
            {
                foreach (var callback in pendingCallbacks[sceneName])
                {
                    callback?.Invoke();
                }
                pendingCallbacks.Remove(sceneName);
            }
        }
    }

    /// <summary>
    /// Asynchronously reloads the currently active scene.
    /// </summary>
    /// <param name="onCompleted">An optional callback to invoke upon completion.</param>
    public static async Task ReloadActiveSceneAsync(Action onCompleted = null)
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        await LoadSceneAsync(activeSceneName, LoadSceneMode.Single, onCompleted, false, true);
    }

    /// <summary>
    /// Asynchronously unloads all scenes except the specified ones.
    /// </summary>
    /// <param name="excludeScenes">List of scenes to exclude from unloading.</param>
    public static async Task UnloadAllScenesExcept(List<string> excludeScenes)
    {
        List<Task> unloadTasks = new List<Task>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!excludeScenes.Contains(scene.name))
            {
                unloadTasks.Add(UnloadSceneAsync(scene));
            }
        }
        await Task.WhenAll(unloadTasks);
    }

    /// <summary>
    /// Asynchronously unloads all scenes except the specified one and then loads the specified scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load after unloading others.</param>
    /// <param name="mode">The mode to use when loading the scene (Single or Additive).</param>
    /// <param name="onCompleted">An optional callback to invoke upon completion.</param>
    /// <param name="excludeScenes">Optional list of additional scenes to exclude from unloading.</param>
    public static async Task UnloadAllAndLoadSceneAsync(
        string sceneName,
        LoadSceneMode mode = LoadSceneMode.Single,
        Action onCompleted = null,
        List<string> excludeScenes = null
    )
    {
        excludeScenes = excludeScenes ?? new List<string>();
        excludeScenes.Add(sceneName);
        await UnloadAllScenesExcept(excludeScenes);
        await LoadSceneAsync(sceneName, mode, onCompleted);
    }

    private static async Task UnloadSceneAsync(Scene scene)
    {
        await SceneManager.UnloadSceneAsync(scene);
    }

    private static async Task UnloadSceneAsync(string sceneName)
    {
        await SceneManager.UnloadSceneAsync(sceneName);
    }
}
