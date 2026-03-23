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
    public Image backgroundImage;
    public AudioSource backgroundMusic;
    public Image CharacterImage1;
    public Image CharacterImage2;

    private string storyPath = Constants.STORY_PATH;
    private string defaultStoryName = Constants.DEFAULT_STORY_NAME;
    private List<ExcelReader.ExcelData> storyData;
    private int currentLine = Constants.DEFAULT_START_LINE;
    // Start is called before the first frame update
    void Start()
    {
		// Could use Path.Combine or ReadOnlySpan and (or) Extension to optimise
		LoadStoryFromFile(storyPath + defaultStoryName);
        //DisplayNextLine();
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
		if (vocalAudio.isPlaying)
		{
			vocalAudio.Stop();
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

            if (NotNullNorEmpty(data.backgroundImageFileName)) 
            {
                UpdateBackgroundImage(data.backgroundImageFileName);
            }
            if (NotNullNorEmpty(data.backgroundMusicFileName)) 
            {
				PlayBackgroundMusic(data.backgroundMusicFileName);
            }

            if (NotNullNorEmpty(data.character1Action))
            {
                UpdateCharacterImage(data.character1Action, data.character1ImageFileName, CharacterImage1);
            }
			if (NotNullNorEmpty(data.character2Action))
			{
				UpdateCharacterImage(data.character2Action, data.character2ImageFileName, CharacterImage2);
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
		UpdateImage(imagePath, avatarImage);
	}

    

	void UpdateBackgroundImage(string imageFileName)
	{
		string imagePath = Constants.BACKGROUND_PATH + imageFileName;
		UpdateImage(imagePath, backgroundImage);
	}
	
    void UpdateCharacterImage(string action, string imageFileName, Image characterImage)
    {
        // todo: enable appear and move to in one frame
        // todo: cache same image when disappear/appear
        if (action.StartsWith(Constants.characterActionAppearAt))
        {
            string imagePath = Constants.CHARACTER_PATH + imageFileName;
            UpdateImage(imagePath, characterImage);
        }
        else if (action == Constants.characterActionDisappear)
        {
            characterImage.gameObject.SetActive(false);
        }
        else if (action.StartsWith(Constants.characterActionMoveTo))
        {
            // todo
        }
    }

	void UpdateImage(string imagePath, Image image)
	{
		Sprite sprite = Resources.Load<Sprite>(imagePath);

		if (sprite != null)
		{
			image.sprite = sprite;
			image.gameObject.SetActive(true);
		}
		else
		{
			Debug.LogError(Constants.IMAGE_LOAD_FAILED + imagePath);
		}
	}

	void PlayVocalAudio(string audioFileName)
	{
		string audioPath = Constants.VOCAL_PATH + audioFileName;
        PlayAudio(audioPath, vocalAudio, false);
	}
	void PlayBackgroundMusic(string audioFileName)
	{
		string musicPath = Constants.MUSIC_PATH + audioFileName;
        PlayAudio(musicPath, backgroundMusic, true);
	}

    void PlayAudio(string audioPath, AudioSource audioSource, bool isLoop)
    {
		AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
		if (audioClip != null)
		{
			audioSource.clip = audioClip;
			audioSource.Play();
			audioSource.loop = isLoop;
		}
		else
		{
			Debug.LogError(Constants.AUDIO_LOAD_FAILED + audioPath);
		}
	}
}
