using DG.Tweening;
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
    public Image characterImage1;
    public Image characterImage2;

    public GameObject choicePanel;
    public Button choiceButton1;
    public Button choiceButton2;
    // todo: what if more buttons?

    public GameObject bottomButtons;
    public Button autoButton;
    public Button skipButton;

    private readonly string storyPath = Constants.STORY_PATH;
    private readonly string defaultStoryFileName = Constants.DEFAULT_STORY_FILE_NAME;
    private readonly string excelFileExtension = Constants.EXCEL_FILE_EXTENSION;

    private List<ExcelReader.ExcelData> storyData;
    private int currentLine = Constants.DEFAULT_START_LINE;
    private string currentStoryFileName;

	private bool isAutoPlay = false;
    private bool isSkipping = false;

    private int maxReachedLineIndex = 0;
    private Dictionary<string, int> globalMaxReachedLineIndices = new Dictionary<string, int>();

	// Start is called before the first frame update
	void Start()
    {
        bottomButtonsAddListener();

		InitializeAndLoadStory(defaultStoryFileName);
    }

    void bottomButtonsAddListener()
    {
		autoButton.onClick.AddListener(OnAutoButtonClick);
		skipButton.onClick.AddListener(OnSkipButtonClick);
	}

    private void InitializeAndLoadStory(string filename)
    {
        Initialize();

		// Could use Path.Combine or ReadOnlySpan and (or) Extension to optimise
		LoadStoryFromFile(filename);
		DisplayNextLine();
	}

	private void Initialize()
	{
        currentLine = Constants.DEFAULT_START_LINE;

        avatarImage.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
        characterImage1.gameObject.SetActive(false);
        characterImage2.gameObject.SetActive(false);
        choicePanel.gameObject.SetActive(false);
	}

	void LoadStoryFromFile(string filename)
	{
        currentStoryFileName = filename;
        var path = storyPath + filename + excelFileExtension;
		storyData = ExcelReader.ReadExcel(path);
		if (storyData == null || storyData.Count == 0)
		{
			Debug.LogError(Constants.NO_DATA_FOUND);
		}

        if (globalMaxReachedLineIndices.ContainsKey(currentStoryFileName))
        {
            maxReachedLineIndex = globalMaxReachedLineIndices[currentStoryFileName];
        }
        else
        {
            maxReachedLineIndex = 0;
            globalMaxReachedLineIndices[currentStoryFileName] = maxReachedLineIndex;
        }
	}

	// Update is called once per frame
	void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsHittingBottomButtons())
            {
				DisplayNextLine();
			}
        }
    }

	void DisplayNextLine()
	{
		if (currentLine > maxReachedLineIndex)
        {
            maxReachedLineIndex = currentLine;
            globalMaxReachedLineIndices[currentStoryFileName] = maxReachedLineIndex;
        }

        if (currentLine >= storyData.Count - 1)
        {
            if (isAutoPlay)
            {
                isAutoPlay = false;
                UpdateButtonImage(Constants.AUTO_OFF, autoButton);
            }

            if (storyData[currentLine].speaker == Constants.END_OF_STORY)
            {
                Debug.Log(Constants.END_OF_STORY);
            }
            else if (storyData[currentLine].speaker == Constants.CHOICE)
            {
                ShowChoices();
            }

			return;
		}

		if (typewriterEffect.IsTyping())
        {
			typewriterEffect.CompleteLine();
        }
        else
        {
            DisplayThisLine();
		}
	}

    void DisplayThisLine()
    {

		if (vocalAudio.isPlaying)
		{
			vocalAudio.Stop();
		}
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
			UpdateCharacterImage(data.character1Action, data.character1ImageFileName, characterImage1, data.coordinateX1);
		}
		if (NotNullNorEmpty(data.character2Action))
		{
			UpdateCharacterImage(data.character2Action, data.character2ImageFileName, characterImage2, data.coordinateX2);
		}

		currentLine++;
	}

    void ShowChoices()
    {
        var Data = storyData[currentLine];
        choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();

        choicePanel.gameObject.SetActive(true);
        choiceButton1.GetComponentInChildren<TextMeshProUGUI>().text = Data.content;
        choiceButton1.onClick.AddListener(()=>InitializeAndLoadStory(Data.avatarImageFileName));

		choiceButton2.GetComponentInChildren<TextMeshProUGUI>().text = Data.vocalAudioFileName;
		choiceButton2.onClick.AddListener(() => InitializeAndLoadStory(Data.backgroundImageFileName));
	}

    bool NotNullNorEmpty(string str)
    {
        return !string.IsNullOrEmpty(str);
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
	
    void UpdateCharacterImage(string action, string imageFileName, Image characterImage, string x)
    {
        if (action.StartsWith(Constants.APPEAR_AT))
        {
            string imagePath = Constants.CHARACTER_PATH + imageFileName;
            if (NotNullNorEmpty(x))
            {
                UpdateImage(imagePath, characterImage);
                var NewPosition = new Vector2(float.Parse(x), characterImage.rectTransform.anchoredPosition.y);
                characterImage.rectTransform.anchoredPosition = NewPosition;
                characterImage.DOFade(1, Constants.DURATION_TIME).From(0);
            }
            else 
            {
                Debug.LogError(Constants.COORDINATE_MISSING);
            }
        }
        else if (action == Constants.DISAPPEAR)
        {
            characterImage.DOFade(0, Constants.DURATION_TIME).OnComplete(()=> characterImage.gameObject.SetActive(false));
        }
        else if (action.StartsWith(Constants.MOVE_TO))
        {
            if (NotNullNorEmpty(x))
            {
                characterImage.rectTransform.DOAnchorPosX(float.Parse(x), Constants.DURATION_TIME);
            }
            else
            {
                Debug.LogError(Constants.DURATION_TIME);
            }
        }
    }

	void UpdateButtonImage(string imageFileName, Button button)
	{
		string imagePath = Constants.BUTTON_PATH + imageFileName;
		UpdateImage(imagePath, button.image);
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


	bool IsHittingBottomButtons()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            bottomButtons.GetComponent<RectTransform>(),
            Input.mousePosition,
            null
        );
    }


	private IEnumerator StartAutoPlay()
	{
		while (isAutoPlay)
		{
			if (!typewriterEffect.IsTyping())
			{
				DisplayNextLine();
			}

			yield return new WaitForSeconds(Constants.DEFAULT_AUTO_WAITING_SECONDS);
		}
	}

	void OnAutoButtonClick()
    {
        isAutoPlay = !isAutoPlay;

        UpdateButtonImage((isAutoPlay ? Constants.AUTO_ON : Constants.AUTO_OFF), autoButton);

        if (isAutoPlay) 
        {
			StartCoroutine(StartAutoPlay());
        }
    }
    
    void OnSkipButtonClick()
    {
        if (!isSkipping && CanSkip())
        {
            StartSkipping();
        }
        else if (isSkipping) 
        {
            StartCoroutine(SkipToMaxReachedLine());
            EndSkipping();
        }
    }

    bool CanSkip()
    {
        return currentLine < maxReachedLineIndex;
    }

    void StartSkipping()
    {
        isSkipping = true;
        UpdateButtonImage(Constants.SKIP_ON, skipButton);
        typewriterEffect.typingSpeed = Constants.SKIP_MODE_TYPING_SPEED;

		StartCoroutine(SkipToMaxReachedLine());
	}

    private IEnumerator SkipToMaxReachedLine()
    {
        while (isSkipping)
        {
            if (CanSkip())
            {
                DisplayThisLine();
            }
            else
            {
				EndSkipping();
            }
            yield return new WaitForSeconds(Constants.DEFAULT_SKIP_WAITING_SECONDS);
        }   
    }

    void EndSkipping()
    {
        isSkipping = false;
		typewriterEffect.typingSpeed = Constants.DEFAULT_TYPING_SPEED;
		UpdateButtonImage(Constants.SKIP_OFF, skipButton);
	}

}
