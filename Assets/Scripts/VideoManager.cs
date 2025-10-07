using UnityEngine;
using UnityEngine.Video;
using System.Linq;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips;

    public void VideoToPlay(string videoName)
    {
        VideoClip clip = videoClips.FirstOrDefault(v => v.name == videoName);
        if (clip != null)
        {
            videoPlayer.clip = clip;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("Video not found: " + videoName);
        }
    }
}
