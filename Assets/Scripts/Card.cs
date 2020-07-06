using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.Video;


/// <summary>
/// Unity VideoPlayer Script for Unity 5.6 (currently in beta 0b11 as of March 15, 2017)
/// Blog URL: http://justcode.me/unity2d/how-to-play-videos-on-unity-using-new-videoplayer/
/// YouTube Video Link: https://www.youtube.com/watch?v=nGA3jMBDjHk
/// StackOverflow Disscussion: http://stackoverflow.com/questions/41144054/using-new-unity-videoplayer-and-videoclip-api-to-play-video/
/// Code Contiburation: StackOverflow - Programmer
/// </summary>


public class Card : MonoBehaviour
{

    public GameObject screen;
    public Texture targetScreen;
    public GameObject playButton;
    private VideoPlayer videoPlayer;
    private AudioSource audioSource;

    public GameObject cameraAR;
    public GameObject camera3D;
    public GameObject fullScreenBtn;
    public GameObject backBtn;
    public List<string> urls;
    int i;
    public const string unityRequestUrl = "https://nxstudiosgames.com/videoUrl.php";
    // Use this for initialization
    void Start()
    {
        Application.runInBackground = true;
        i = 0;
        StartCoroutine(LoadUrls());
    }
    IEnumerator LoadUrls()
    {
        bool isLoaded = false;
        string urlString = "";
        using (UnityWebRequest www = UnityWebRequest.Get(unityRequestUrl))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Downloading...");
                urlString = www.downloadHandler.text;
                isLoaded = true;
            }
        }
        yield return new WaitUntil(() => isLoaded == true);
        foreach(string url in urlString.Split(','))
        {
            if(url != "")
            {
                urls.Add(url);
            }
        }
        LoadVideo(urls[i]);
    }
    void LoadVideo(string url)
    {
        playButton.SetActive(false);

        //Add VideoPlayer to the GameObject
        videoPlayer = screen.AddComponent<VideoPlayer>();

        //Add AudioSource
        audioSource = screen.AddComponent<AudioSource>();

        //Disable Play on Awake for both Video and Audio
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;
        audioSource.Pause();
        //We want to play from video clip not from url

        //ideoPlayer.source = VideoSource.VideoClip;

        // Vide clip from Url
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = url;

        //Set Audio Output to AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);
        //Set video To Play then prepare Audio to prevent Buffering
        //videoPlayer.clip = videoToPlay;
        //videoPlayer.Prepare();

        //Wait until video is prepared
        //while (!videoPlayer.isPrepared)
        //{
        //    yield return null;
        //}

        //Debug.Log("Done Preparing Video");

        //Assign the Texture from Video to RawImage to be displayed
        targetScreen = videoPlayer.texture;

        playButton.SetActive(true);

        //Debug.Log("Playing Video");
        //while (videoPlayer.isPlaying)
        //{
        //    Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));
        //    yield return null;
        //}

        //Debug.Log("Done Playing Video");
    }

    // Update is called once per frame
    public void PlayVideo()
    {
        playButton.SetActive(false);
        //Play Video
        videoPlayer.Play();

        //Play Sound
        audioSource.Play();
    }
    public void NextVideo()
    {
        videoPlayer.Stop();
        audioSource.Stop();
        i++;
        if (i >= urls.Count)
        {
            i = 0;
        }
        LoadVideo(urls[i]);
    }
    public void PreviousVideo()
    {
        videoPlayer.Stop();
        audioSource.Stop();
        i--;
        if (i < 0)
        {
            i = urls.Count - 1;
        }
        LoadVideo(urls[i]);
    }
    public void FullScreen()
    {
        fullScreenBtn.SetActive(false);
        backBtn.SetActive(true);
        camera3D.SetActive(true);
        cameraAR.SetActive(false);
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCamera = camera3D.GetComponent<Camera>();
    }
    public void Back()
    {
        fullScreenBtn.SetActive(true);
        backBtn.SetActive(false);
        camera3D.SetActive(false);
        cameraAR.SetActive(true);
        videoPlayer.Stop();
        audioSource.Stop();
        videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
    }
    public void HomeBackButton()
    {
        ProjectManager.instance.Back();
    }
}