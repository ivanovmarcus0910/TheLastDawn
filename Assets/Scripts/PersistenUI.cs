using UnityEngine;
using UnityEngine.SceneManagement; // Cần thư viện này

public class PersistentUI : MonoBehaviour
{
    public static PersistentUI Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}