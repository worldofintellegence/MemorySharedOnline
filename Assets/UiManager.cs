using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    

    public VideoUIPlayer playerVideo;

    public GameObject introScreen;

    public void PlayVideo()
    {
        introScreen.SetActive(true  );
        playerVideo.videoPlayer.Play();
    }
    public void CloseIntroScreen()
    {
        playerVideo.videoPlayer.Stop();
        introScreen.SetActive(false  );

        
    }
    public void LoadScene()
    {
        SceneLoader.Instance.LoadScene("The Farmer's memory");
    }
}
