using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    LoadingManager loadingManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1;
        loadingManager = LoadingManager.Instance;
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
    public void Startgame()
    {
        SceneManager.LoadScene("MapWira");
    }
    public void OpenOption()
    {
        optionPanel.SetActive(!optionPanel.activeSelf);
    }
    public void Exitgame()
    {
        Application.Quit();
    }
}
