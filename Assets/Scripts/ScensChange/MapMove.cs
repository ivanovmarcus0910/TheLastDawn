using UnityEngine;
using UnityEngine.SceneManagement;

public class MapMove : MonoBehaviour
{
    public int sceneBuildIndexs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("Trigger Enterd");
        if (collision.tag == "Player")
        {
            print("Switching Scene to " + sceneBuildIndexs);
            SceneManager.LoadScene(sceneBuildIndexs, LoadSceneMode.Single);
        }
    }
}
