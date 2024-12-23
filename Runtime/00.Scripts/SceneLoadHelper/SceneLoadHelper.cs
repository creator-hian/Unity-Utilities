using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SceneLoadHelper
{
    private static Dictionary<string, AsyncOperation> loadingScenes =
        new Dictionary<string, AsyncOperation>();
    private static Dictionary<string, List<Action>> pendingCallbacks =
        new Dictionary<string, List<Action>>();

    /// <summary>
    /// Loads a scene synchronously by name with the specified load mode.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <param name="mode">The mode to use when loading the scene (Single or Additive).</param>
    public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (IsSceneLoaded(sceneName))
        {
            Debug.Log($"Scene '{sceneName}' is already loaded.");
            return;
        }

        if (loadingScenes.ContainsKey(sceneName))
        {
            Debug.Log($"Scene '{sceneName}' is already loading.");
            return;
        }

        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' is not in build settings.");
            return;
        }

        SceneManager.LoadScene(sceneName, mode);
        Debug.Log($"Scene '{sceneName}' loaded successfully.");
    }

    /// <summary>
    /// Unloads a scene synchronously by name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to unload.</param>
    public static void UnloadScene(string sceneName)
    {
        if (!IsSceneLoaded(sceneName))
        {
            Debug.Log($"Scene '{sceneName}' is not loaded.");
            return;
        }

        if (loadingScenes.ContainsKey(sceneName))
        {
            Debug.Log($"Scene '{sceneName}' is currently loading. Cannot unload.");
            return;
        }

        SceneManager.UnloadSceneAsync(sceneName);
        Debug.Log($"Scene '{sceneName}' unloaded successfully.");
    }

    public static bool IsSceneLoaded(string sceneName)
    {
        return SceneManager.GetSceneByName(sceneName).isLoaded;
    }

    public static bool IsSceneInBuildSettings(string sceneName)
    {
        return Application.CanStreamedLevelBeLoaded(sceneName);
    }
}
