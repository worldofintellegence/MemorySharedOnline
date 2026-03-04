using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeDuration = 0.4f;
public string previousSceneName = "";
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void LoadScene(string sceneName, bool isAdditive = false)
    {

        if (isAdditive)
        {
            NetworkManager  network =   FindAnyObjectByType<NetworkManager>();
            if (network != null)
            {
                Destroy(network.gameObject);
            }
        }
        previousSceneName = SceneManager.GetActiveScene().name;

        StartCoroutine(LoadRoutine(sceneName));



    }

    public void BackToMenu()
    {
        StartCoroutine(LoadRoutine(previousSceneName));
    }
    private IEnumerator LoadRoutine(string scene)
    {
        yield return FadeOut();

        AsyncOperation load = SceneManager.LoadSceneAsync(scene);
        load.allowSceneActivation = false;

        while (load.progress < 0.9f)
            yield return null;

        load.allowSceneActivation = true;

        yield return null;
        yield return FadeIn();
    }

    IEnumerator FadeIn()
    {
        yield return Fade(1f, 0f);
    }

    IEnumerator FadeOut()
    {
        yield return Fade(0f, 1f);
    }

    IEnumerator Fade(float from, float to)
    {
        fadeGroup.blocksRaycasts = true;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }

        fadeGroup.alpha = to;
        fadeGroup.blocksRaycasts = false;
    }
}
