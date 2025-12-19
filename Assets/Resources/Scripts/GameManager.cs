using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    bool isPaused = false;
    public bool isMonsterEating = false;
    [SerializeField] private Animator blackScreen;
    [SerializeField] private Animator jumpscareScreen;
    public bool isWin = false;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Animator winScreen;
    [SerializeField] private Sprite ending1;
    [SerializeField] private Sprite ending2;
    [SerializeField] private Sprite ending3;
    [SerializeField] private Sprite endingText1;
    [SerializeField] private Sprite endingText2;
    [SerializeField] private Sprite endingText3;
    LoadingManager loadingManager;
    Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        loadingManager = LoadingManager.Instance;
        player = FindFirstObjectByType<Player>();
        if (!PlayerPrefs.HasKey("bgm") || !PlayerPrefs.HasKey("sfx"))
        {
            PlayerPrefs.SetFloat("bgm", 1f);
            PlayerPrefs.SetFloat("sfx", 1f);
            bgmSlider.value = 1;
            sfxSlider.value = 1;
            mixer.SetFloat("bgm", PlayerPrefs.GetFloat("bgm"));
            mixer.SetFloat("sfx", PlayerPrefs.GetFloat("sfx"));
        }
        else
        {
            bgmSlider.value = PlayerPrefs.GetFloat("bgm");
            sfxSlider.value = PlayerPrefs.GetFloat("sfx");
            mixer.SetFloat("bgm", PlayerPrefs.GetFloat("bgm"));
            mixer.SetFloat("sfx", PlayerPrefs.GetFloat("sfx"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //audio
        if (PlayerPrefs.HasKey("bgm") && PlayerPrefs.HasKey("sfx"))
        {
            PlayerPrefs.SetFloat("bgm", bgmSlider.value);
            PlayerPrefs.SetFloat("sfx", sfxSlider.value);
            mixer.SetFloat("bgm", Mathf.Log10(PlayerPrefs.GetFloat("bgm")) * 20);
            mixer.SetFloat("sfx", Mathf.Log10(PlayerPrefs.GetFloat("sfx")) * 20);
        }
        else
        {
            mixer.SetFloat("bgm", Mathf.Log10(bgmSlider.value) * 20);
            mixer.SetFloat("sfx", Mathf.Log10(sfxSlider.value) * 20);
        }
    }
    IEnumerator MonsterEatsScreen()
    {
        jumpscareScreen.SetBool("in", true);
        yield return new WaitForSeconds(3f);
        jumpscareScreen.SetBool("in", false);
        isMonsterEating = false;
    }
    public void MonsterEats()
    {
        isMonsterEating = true;
        Time.timeScale = 1;
        StartCoroutine(MonsterEatsScreen());
    }
    public void MonsterGetPlayer()
    {
        isMonsterEating = true;
        Time.timeScale = 1;
        StartCoroutine(MonsterGetPlayerScreen());
    }
    IEnumerator MonsterGetPlayerScreen()
    {
        blackScreen.SetBool("in", true);
        yield return new WaitForSeconds(1f);
        blackScreen.SetBool("in", false);
        isMonsterEating = false;
    }
    public void PauseGame()
    {
        if(!isMonsterEating && !isWin)
        {
            isPaused = !isPaused;
            pausePanel.SetActive(isPaused);
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
    public void Win()
    {
        isWin = true;
        pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        //ending 1 - anak full
        if(player.GetRescuedChildrenCount() == 5)
        {
            winScreen.transform.GetChild(0).GetComponent<Image>().sprite = ending1;
            winScreen.transform.GetChild(1).GetComponent<Image>().sprite = endingText1;
            PlayerPrefs.SetInt("ending1", 1);
        }
        //ending 2 - anak sebagian
        else if(player.GetRescuedChildrenCount() < 5 && player.GetRescuedChildrenCount() > 0)
        {
            winScreen.transform.GetChild(0).GetComponent<Image>().sprite = ending2;
            winScreen.transform.GetChild(1).GetComponent<Image>().sprite = endingText2;
            PlayerPrefs.SetInt("ending2", 1);
        }
        //ending 3 - anak none
        else if (player.GetRescuedChildrenCount() <= 0)
        {
            winScreen.transform.GetChild(0).GetComponent<Image>().sprite = ending3;
            winScreen.transform.GetChild(1).GetComponent<Image>().sprite = endingText3;
            PlayerPrefs.SetInt("ending3", 1);
        }
        winScreen.SetBool("in", true);
        Invoke("BackToMenu", 8f);
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("MapTest");
    }
}
