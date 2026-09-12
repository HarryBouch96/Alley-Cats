using UnityEngine;

public class SystemPersistence : MonoBehaviour
{
    private void Awake()
    {
        // Check if we already have a System GameObject, if so destroy this one
        if (FindObjectsByType<SystemPersistence>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Otherwise make this one persist across scenes
        DontDestroyOnLoad(gameObject);
    }
}
