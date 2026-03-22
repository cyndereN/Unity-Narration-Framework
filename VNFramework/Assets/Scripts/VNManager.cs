using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VNManager : MonoBehaviour
{
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI speakerContent;
    public TypewriterEffect typewriterEffect;
    public Image avatarImage;
    public AudioSource vocalAudio;

    private string storyPath = Constants.STORY_PATH;
    private string defaultStoryName = Constants.DEFAULT_STORY_NAME;
    private List<ExcelReader.ExcelData> storyData;
    private int currentLine = 0;
    // Start is called before the first frame update
    void Start()
    {
		// Could use Path.Combine or ReadOnlySpan and (or) Extension to optimise
		LoadStoryFromFile(storyPath + defaultStoryName);
        DisplayNextLine();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DisplayNextLine();
        }
    }
    void LoadStoryFromFile(string path)
    {
        storyData = ExcelReader.ReadExcel(path);
        if (storyData == null || storyData.Count == 0)
        {
            Debug.LogError(Constants.NO_DATA_FOUND);
        }
    }

	void DisplayNextLine()
	{
		if (currentLine >= storyData.Count)
		{
			Debug.Log(Constants.END_OF_STORY);
            return;
		}

        if (typewriterEffect.IsTyping())
        {
            typewriterEffect.CompleteLine();
        }
        else
        {
            var data = storyData[currentLine];
            speakerName.text = data.speaker;
            speakerContent.text = data.content;
            typewriterEffect.StartTyping(speakerContent.text);
            if (NotNullNorEmpty(data.avatarImageFileName))
            {
                UpdateAvatarImage(data.avatarImageFileName);
            }
            else
            {
                avatarImage.gameObject.SetActive(false);
            }

            if (NotNullNorEmpty(data.vocalAudioFileName))
            {
                PlayVocalAudio(data.vocalAudioFileName);
            }
            currentLine++;
        }
	}

    bool NotNullNorEmpty(string str)
    {
        return !string.IsNullOrEmpty(str);
    }

    void UpdateAvatarImage(string imageFileName)
    {
        string imagePath = Constants.AVATAR_PATH + imageFileName;
        Sprite sprite = Resources.Load<Sprite>(imagePath);

        if (sprite != null)
        {
            avatarImage.sprite = sprite;
            avatarImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError(Constants.IMAGE_LOAD_FAILED + imagePath);
        }
    }

    void PlayVocalAudio(string audioFileName)
    {
        string audioPath = Constants.VOCAL_PATH + audioFileName;
        AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
        if (audioClip != null)
        {
            vocalAudio.clip = audioClip;
            vocalAudio.Play();
        }
        else
        {
            Debug.LogError(Constants.AUDIO_LOAD_FAILED + audioPath);
        }
    }
}
