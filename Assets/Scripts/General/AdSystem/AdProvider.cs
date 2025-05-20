using UnityEngine;
//using Unity.Services.LevelPlay;

// AdProvider
// must be present in each scene to provide ads
// singleton pattern
// created 12/5/25
// modified 12/5/25

public class AdProvider : MonoBehaviour
{
    public static AdProvider instance;
    static string uniqueUserId = "demoUserUnity";

    private void Awake()
    {
        if (instance)
        {
            if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
        else
            instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
