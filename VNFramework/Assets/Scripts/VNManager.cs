using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VNManager : MonoBehaviour
{
#region Variables
    public GameObject gamePanel;
    public GameObject dialogueBox;
    public TextMeshProUGUI speakerName;
    public TypewriterEffect typewriterEffect;
    public ScreenShooter screenShooter;

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
    public Button saveButton;
    public Button loadButton;
    public Button historyButton;
    public Button settingsButton;
    public Button homeButton;
    public Button closeButton;

    private readonly string storyPath = Constants.STORY_PATH;
    private readonly string defaultStoryFileName = Constants.DEFAULT_STORY_FILE_NAME;
    private readonly int defaultStartLine = Constants.DEFAULT_START_LINE;
    private readonly string excelFileExtension = Constants.EXCEL_FILE_EXTENSION;

    private string saveFolderPath;
    private byte[] screenshotData;
    private string currentSpeakingContent;

    private List<ExcelReader.ExcelData> storyData;
    private int currentLine = Constants.DEFAULT_START_LINE;
    private float currentTypingSpeed = Constants.DEFAULT_TYPING_SPEED;
    private string currentStoryFileName;

	private bool isAutoPlay = false;
    private bool isSkipping = false;
    private bool isLoaded = false;

    private int maxReachedLineIndex = 0;
    private readonly Dictionary<string, int> globalMaxReachedLineIndices = new Dictionary<string, int>();
    private LinkedList<string> historyRecords = new LinkedList<string>();
    public HashSet<string> unblockedBackgrounds = new HashSet<string>();

    public static VNManager Instance { get; private set; }
#endregion Variables

#region LifeCycle
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

	// Start is called before the first frame update
	void Start()
    {
        InitializeSaveFilePath();
        bottomButtonsAddListener();
    }

	// Update is called once per frame
	void Update()
    {
        if (!MenuManager.Instance.menuPanel.activeSelf &&
            !SaveLoadManager.Instance.saveLoadPanel.activeSelf &&
            !HistoryManager.Instance.historyScrollView.activeSelf &&
            !SettingsManager.Instance.settingsPanel.activeSelf &&
            gamePanel.activeSelf)
        {
            if ( Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {       
                if (!dialogueBox.activeSelf)
                {
                    OpenUI();
                }
                else if (!IsHittingBottomButtons())
                {
                    DisplayNextLine();
                }
                
            } 
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (dialogueBox.activeSelf)
                {
                    CloseUI();
                }
                else
                {
                    OpenUI();
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                CtrlSkip();
            }
        }
    }

#endregion LifeCycle

#region Init
    public void StartGame()
    {
		InitializeAndLoadStory(defaultStoryFileName, defaultStartLine);
    }

    void bottomButtonsAddListener()
    {
		autoButton.onClick.AddListener(OnAutoButtonClick);
		skipButton.onClick.AddListener(OnSkipButtonClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        loadButton.onClick.AddListener(OnLoadButtonClick);
        historyButton.onClick.AddListener(OnHistoryButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        homeButton.onClick.AddListener(OnHomeButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void InitializeAndLoadStory(string filename, int lineNumber)
    {
        Initialize(lineNumber);

		// Could use Path.Combine or ReadOnlySpan and (or) Extension to optimise
		LoadStoryFromFile(filename);
        if (isLoaded)
        {
            RecoverLastBackgroundAndAction();
            isLoaded = false;
        }
		DisplayNextLine();
	}

	private void Initialize(int lineNumber)
	{
        currentLine = lineNumber;

        avatarImage.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
        characterImage1.gameObject.SetActive(false);
        characterImage2.gameObject.SetActive(false);
        backgroundMusic.gameObject.SetActive(false);
        vocalAudio.gameObject.SetActive(false);
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
    private void InitializeSaveFilePath()
    {
        saveFolderPath = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH);
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }
    }
#endregion Init

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
            else if (storyData[currentLine].speaker == Constants.GOTO)
            {
                InitializeAndLoadStory(storyData[currentLine].content, defaultStartLine);
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
		var data = storyData[currentLine];

        string playerName = PlayerData.Instance.playerName;
        string speaker = data.speaker.Replace(Constants.PLAYER_NAME_TAG, playerName);
        speakerName.text = speaker;
        string content = data.content.Replace(Constants.PLAYER_NAME_TAG, playerName);
        currentSpeakingContent = content;

		/*
        speakerName.text = data.speaker;
        currentSpeakingContent = data.content;
		*/

        typewriterEffect.StartTyping(currentSpeakingContent, currentTypingSpeed);

        RecordHistory(speakerName.text, currentSpeakingContent);

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
    
    void RecordHistory(string speaker, string content)
    {
        string historyRecord = speaker + Constants.COLON + content;
        if (historyRecords.Count >= Constants.MAX_LENGTH) 
        {
            historyRecords.RemoveFirst();
        }
        historyRecords.AddLast(historyRecord);

    }

    public void RecoverLastBackgroundAndAction()
    {
        var data = storyData[currentLine];

        if (NotNullNorEmpty(data.lastBackgroundImage))
        {
            UpdateBackgroundImage(data.lastBackgroundImage);
        }
        if (NotNullNorEmpty(data.lastBackgroundMusic))
        {
            PlayBackgroundMusic(data.lastBackgroundMusic);
        }
        if (data.character1Action != Constants.APPEAR_AT && NotNullNorEmpty(data.character1ImageFileName))
        {
            UpdateCharacterImage(Constants.APPEAR_AT , data.character1ImageFileName, characterImage1, data.lastCoordinateX1);
        }
        if (data.character2Action != Constants.APPEAR_AT && NotNullNorEmpty(data.character2ImageFileName))
        {
            UpdateCharacterImage(Constants.APPEAR_AT , data.character2ImageFileName, characterImage2, data.lastCoordinateX2);
        }
    }

    void ShowChoices()
    {
        StopAutoAndSkip();
        typewriterEffect.CompleteLine();
        var Data = storyData[currentLine];
        choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();

        choicePanel.gameObject.SetActive(true);
        choiceButton1.GetComponentInChildren<TextMeshProUGUI>().text = Data.content;
        choiceButton1.onClick.AddListener(()=>InitializeAndLoadStory(Data.avatarImageFileName, defaultStartLine));

		choiceButton2.GetComponentInChildren<TextMeshProUGUI>().text = Data.vocalAudioFileName;
		choiceButton2.onClick.AddListener(() => InitializeAndLoadStory(Data.backgroundImageFileName, defaultStartLine));
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

        if (!unblockedBackgrounds.Contains(imageFileName))
        {
            unblockedBackgrounds.Add(imageFileName);
        }
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
                characterImage.DOFade(1, (isLoaded || action == Constants.APPEAR_AT_INSTANTLY ? 0 : Constants.DURATION_TIME)).From(0);
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
			audioSource.loop = isLoop;
            audioSource.gameObject.SetActive(true);
			
            // todo: Force replay?
            audioSource.Stop();
            audioSource.time = 0;
            audioSource.enabled = true;
            audioSource.Play();
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
            Camera.main
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
    private void OnSaveButtonClick()
    {
        CloseUI();
        Texture2D screenshot = screenShooter.CaptureScreenshot();
        screenshotData = screenshot.EncodeToPNG();
        SaveLoadManager.Instance.ShowSavePanel(SaveGame);
        OpenUI();
    }

    public class SaveData 
    {
        public string savedStoryFileName;
        public int savedLine;
        public string savedSpeakingContent;
        public byte[] screenShotData;
        public LinkedList<string> savedHistoryRecords;
        public string savedPlayerName;
    } 
    void SaveGame(int slotIndex)
    {
        var saveData = new SaveData(){
            savedStoryFileName = currentStoryFileName,
            savedLine = currentLine,
            savedSpeakingContent = currentSpeakingContent,
            screenShotData = screenshotData,
            savedHistoryRecords = historyRecords,
            savedPlayerName = PlayerData.Instance.playerName,
        };
        
        string savePath = Path.Combine(saveFolderPath, slotIndex + Constants.SAVE_FILE_EXTENSION);
        string json = JsonConvert.SerializeObject(saveData, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(savePath, json);
    }

    private void OnLoadButtonClick()
    {
        StopAutoAndSkip();
        ShowLoadPanel(null);
    }

    public void ShowLoadPanel(Action action)
    {
        SaveLoadManager.Instance.ShowLoadPanel(LoadGame, action);
    }

    void LoadGame(int slotIndex)
    {
        string savePath = Path.Combine(saveFolderPath, slotIndex + Constants.SAVE_FILE_EXTENSION);
        if (File.Exists(savePath))
        {
            isLoaded = true;

            string json = File.ReadAllText(savePath);
            var saveData = JsonConvert.DeserializeObject<SaveData>(json);
            historyRecords = saveData.savedHistoryRecords;
            historyRecords.RemoveLast();
            PlayerData.Instance.playerName = saveData.savedPlayerName;

            // Display Next line ++ ed
            var lineNumber = saveData.savedLine - 1;
            InitializeAndLoadStory(saveData.savedStoryFileName, lineNumber);
        }
    }

    private void OnHistoryButtonClick()
    {
        StopAutoAndSkip();
        HistoryManager.Instance.ShowHistory(historyRecords);
    }
    
    private void OnSettingsButtonClick()
    {
        StopAutoAndSkip();
        
        SettingsManager.Instance.ShowSettingsPanel();
    }

    private void OnHomeButtonClick()
    {
        StopAutoAndSkip();
        typewriterEffect.CompleteLine();
        vocalAudio.Stop();
        backgroundMusic.Stop();
        gamePanel.SetActive(false);
        MenuManager.Instance.menuPanel.SetActive(true);
    }

    private void OnCloseButtonClick()
    {
        CloseUI();
    }
    
    void OpenUI()
    {
        dialogueBox.SetActive(true);
        bottomButtons.SetActive(true);
    }
    
    void CloseUI()
    {
        dialogueBox.SetActive(false);
        bottomButtons.SetActive(false);

        StopAutoAndSkip();
    }

    void StopAutoAndSkip()
    {
        isAutoPlay = false;
        UpdateButtonImage(Constants.AUTO_OFF, autoButton);

        EndSkipping();
    }

    bool CanSkip()
    {
        return currentLine < maxReachedLineIndex;
    }

    void StartSkipping()
    {
        isSkipping = true;
        UpdateButtonImage(Constants.SKIP_ON, skipButton);
        currentTypingSpeed = Constants.SKIP_MODE_TYPING_SPEED;

		StartCoroutine(SkipToMaxReachedLine());
	}

    void CtrlSkip()
    {
        currentTypingSpeed = Constants.SKIP_MODE_TYPING_SPEED;
        StartCoroutine(SkipWhilePressingCtrl());
    }

    private IEnumerator SkipWhilePressingCtrl()
    {
        while (Input.GetKey(KeyCode.LeftControl))
        {
            DisplayNextLine();
            yield return new WaitForSeconds(Constants.DEFAULT_SKIP_WAITING_SECONDS);
        }
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
		currentTypingSpeed = Constants.DEFAULT_TYPING_SPEED;
		UpdateButtonImage(Constants.SKIP_OFF, skipButton);
	}

}
