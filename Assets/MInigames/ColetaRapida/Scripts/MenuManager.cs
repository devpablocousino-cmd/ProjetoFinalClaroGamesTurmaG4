using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void LoadCollectClaro()
    {
        SceneManager.LoadScene("CollectClaro");
    }

    public void ReturnToWorld()
    {
        SceneManager.LoadScene("SampleScene"); // nome exato do mundo aberto
    }
}
