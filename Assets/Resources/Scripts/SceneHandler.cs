using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f; // Ensure time scale is reset before loading a new scene
        SceneManager.LoadScene(sceneName);
    }
}