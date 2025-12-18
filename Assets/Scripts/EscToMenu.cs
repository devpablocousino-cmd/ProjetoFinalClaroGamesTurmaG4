using UnityEngine;
using UnityEngine.SceneManagement;

public class EscToMenu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1f; // garante que nada fique pausado
            SceneManager.LoadScene("MenuScene");
        }
    }
}
