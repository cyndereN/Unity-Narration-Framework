using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroManager : MonoBehaviour
{
    public VideoPlayer VideoPlayer;

    private string videoPath = "video/ph.mp4";

	private void Start()
	{
		string fullpath = System.IO.Path.Combine(Application.streamingAssetsPath, videoPath);
		VideoPlayer.url = fullpath;
		VideoPlayer.loopPointReached += OnVideoEnd;
		VideoPlayer.Play();
	}

	void OnVideoEnd(VideoPlayer videoPlayer)
	{
		SceneManager.LoadScene("SampleScene");
	}
}
