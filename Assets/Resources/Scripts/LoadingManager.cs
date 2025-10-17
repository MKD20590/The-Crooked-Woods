using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;
    string sceneToLoad;
    Animator anim;
    bool isLoading;
    [SerializeField] private float listenerVolume = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
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

    // Update is called once per frame
    void Update()
    {
        AudioListener.volume = listenerVolume;
    }
    public void LoadScene()
    {
        StartCoroutine(Loading());
    }
    IEnumerator Loading()
    {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadSceneAsync(sceneToLoad);
        anim.SetBool("out", true);
    }
    public void DoneLoad()
    {
        isLoading = false;
    }
    public void StartLoad(string sceneName)
    {
        isLoading = true;
        anim.SetBool("out", false);
        sceneToLoad = sceneName;
        anim.Play("load");
    }
}
