using UnityEngine;
using UnityEngine.SceneManagement;

public static class Bootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        var prefab = Resources.Load<GameObject>("GameManager");

        if (!prefab)
        {
            Debug.LogError("GameManager prefab not found.");
            return;
        }

        Object.DontDestroyOnLoad(Object.Instantiate(prefab));
        
    }
}