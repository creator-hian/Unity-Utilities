#if UNITY_2023_1_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SceneLoadHelper
{
    /// <summary>
    /// Loads a scene asynchronously by name with the specified load mode using Awaitable.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <param name="mode">The mode to use when loading the scene (Single or Additive).</param>
    /// <param name="tryLoad">If true, the scene will only be loaded if it's not already loading or loaded.</param>
    /// <param name="reload">If true, the scene will be unloaded before loading.</param>
    public static async Awaitable LoadSceneAsyncAwaitable(
        string sceneName,
        LoadSceneMode mode = LoadSceneMode.Single,
        bool tryLoad = false,
        bool reload = false
    )
    {
        if (IsSceneLoaded(sceneName) && !reload)
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

        if (reload)
        {
            await UnloadSceneAsyncAwaitable(sceneName);
        }

        await SceneManager.LoadSceneAsync(sceneName, mode);
        Debug.Log($"Scene '{sceneName}' loaded successfully.");
    }

    /// <summary>
    /// Reloads the currently active scene asynchronously using Awaitable.
    /// </summary>
    public static async Awaitable ReloadActiveSceneAsyncAwaitable()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        await LoadSceneAsyncAwaitable(activeSceneName, LoadSceneMode.Single, false, true);
    }

    /// <summary>
    /// Unloads all scenes except the specified ones asynchronously using Awaitable.
    /// </summary>
    /// <param name="excludeScenes">List of scenes to exclude from unloading.</param>
    public static async Awaitable UnloadAllScenesExceptAwaitable(List<string> excludeScenes)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!excludeScenes.Contains(scene.name))
            {
                await UnloadSceneAsyncAwaitable(scene);
            }
        }
    }

    /// <summary>
    /// Unloads all scenes except the specified one and then loads the specified scene asynchronously using Awaitable.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load after unloading others.</param>
    /// <param name="mode">The mode to use when loading the scene (Single or Additive).</param>
    /// <param name="excludeScenes">Optional list of additional scenes to exclude from unloading.</param>
    public static async Awaitable UnloadAllAndLoadSceneAsyncAwaitable(
        string sceneName,
        LoadSceneMode mode = LoadSceneMode.Single,
        List<string> excludeScenes = null
    )
    {
        excludeScenes = excludeScenes ?? new List<string>();
        excludeScenes.Add(sceneName);
        await UnloadAllScenesExceptAwaitable(excludeScenes);
        await LoadSceneAsyncAwaitable(sceneName, mode);
    }

    /// <summary>
    /// Unloads a scene asynchronously using Awaitable.
    /// </summary>
    /// <param name="scene">The scene to unload.</param>
    public static async Awaitable UnloadSceneAsyncAwaitable(Scene scene)
    {
        if (!scene.IsValid())
        {
            Debug.LogError("Invalid scene provided for unloading.");
            return;
        }

        if (!scene.isLoaded)
        {
            Debug.Log($"Scene '{scene.name}' is not loaded.");
            return;
        }

        await SceneManager.UnloadSceneAsync(scene);
        Debug.Log($"Scene '{scene.name}' unloaded successfully.");
    }

    /// <summary>
    /// Unloads a scene asynchronously using Awaitable.
    /// </summary>
    /// <param name="sceneName">The name of the scene to unload.</param>
    public static async Awaitable UnloadSceneAsyncAwaitable(string sceneName)
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

        await SceneManager.UnloadSceneAsync(sceneName);
        Debug.Log($"Scene '{sceneName}' unloaded successfully.");
    }
}
#endif
