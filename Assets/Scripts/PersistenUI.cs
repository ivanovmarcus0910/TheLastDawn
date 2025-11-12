using UnityEngine;
using UnityEngine.SceneManagement; // Cần thư viện này

public class PersistentUI : MonoBehaviour
{
    public static PersistentUI Instance;
    //public Canvas myCanvas; // Kéo Canvas vào đây

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Đăng ký sự kiện: Mỗi khi load scene xong thì gọi hàm OnSceneLoaded
        }
        else
        {
            Destroy(gameObject);
        }
    }

   
}