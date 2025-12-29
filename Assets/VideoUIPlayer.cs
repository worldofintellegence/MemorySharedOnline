using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoUIPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;

    private void Start()
    {
        rawImage.texture = videoPlayer.targetTexture;
        videoPlayer.Play();
    }
}
