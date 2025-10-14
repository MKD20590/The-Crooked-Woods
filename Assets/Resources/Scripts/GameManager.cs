using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool isPaused = false;
    public bool isMonsterEating = false;
    [SerializeField] private Animator blackScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator MonsterEats()
    {
        isMonsterEating = true;
        yield return new WaitForSecondsRealtime(7f);
        isMonsterEating = false;
    }
    public void PauseGame()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
