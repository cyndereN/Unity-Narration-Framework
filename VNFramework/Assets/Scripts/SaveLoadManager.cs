using Newtonsoft.Json;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
	public GameObject saveLoadPanel;
	public TextMeshProUGUI panelTitle;
	public Button[] saveLoadButtons;
	public Button prevPageButton;
	public Button nextPageButton;
	public Button backButton;

	private bool isSave;
	private int currentPage = Constants.DEFAULT_START_INDEX;
	private readonly int slotsPerPage = Constants.SLOTS_PER_PAGE;
	private readonly int totalSlots = Constants.TOTAL_SLOTS;

	private System.Action<int> currentAction;

	public static SaveLoadManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		prevPageButton.onClick.AddListener(PrevPage);
		nextPageButton.onClick.AddListener(NextPage);
		backButton.onClick.AddListener(GoBack);
		saveLoadPanel.SetActive(false);
	}

	public void ShowSavePanel(System.Action<int> action)
	{
		isSave = true;
		panelTitle.text = Constants.SAVE_GAME;
		currentAction = action;
		UpdateUI();
		saveLoadPanel.SetActive(true);
	}

	public void ShowLoadPanel(System.Action<int> action)
	{
		isSave = false;
		panelTitle.text = Constants.LOAD_GAME;
		currentAction = action;
		UpdateUI();
		saveLoadPanel.SetActive(true);
	}
	
	private void UpdateUI()
	{
		for (int i = 0; i < slotsPerPage; i++)
		{
			int slotIndex = currentPage * slotsPerPage + i;
			if (slotIndex < totalSlots)
			{
				UpdateSaveLoadButtons(saveLoadButtons[i], slotIndex);
				LoadStorylineAndScreenshots(saveLoadButtons[i], slotIndex);
			}
			else
			{
				saveLoadButtons[i].gameObject.SetActive(false);
			}
		}
	}

	private void UpdateSaveLoadButtons(Button button, int slotIndex)
	{
		button.gameObject.SetActive(true);
		button.interactable = true;

		var savePath = GenerateDataPath(slotIndex);
		var fileExists = File.Exists(savePath);
		
		if (!isSave && !fileExists)
		{
			button.interactable = false;
		}

		var textComponents = button.GetComponentsInChildren<TextMeshProUGUI>();
		textComponents[0].text = null;
		textComponents[1].text = (slotIndex+1)+Constants.COLON+Constants.EMPTY_SLOT;
		button.GetComponentInChildren<RawImage>().texture = null;

		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(() => OnButtonClick(button, slotIndex));

	}

	private void OnButtonClick(Button button, int slotIndex)
	{
		currentAction?.Invoke(slotIndex);
		if (isSave)
		{
			LoadStorylineAndScreenshots(button, slotIndex);
		}
		else
		{
			
		}
	}

	private void PrevPage()
	{
		if (currentPage > 0)
		{
			currentPage--;
			UpdateUI();
		}
	}

	private void NextPage()
	{
		if ((currentPage + 1) * slotsPerPage < totalSlots)
		{
			currentPage++;
			UpdateUI();
		}
	}

	private void GoBack()
	{
		saveLoadPanel.SetActive(false);
	}

	private void LoadStorylineAndScreenshots(Button button, int slotIndex)
	{
		var savePath = GenerateDataPath(slotIndex);
		if(File.Exists(savePath))
		{
			var json = File.ReadAllText(savePath);
			var saveData = JsonConvert.DeserializeObject<VNManager.SaveData>(json);

			if(saveData.screenShotData != null)
			{
				Texture2D screenShot = new Texture2D(2,2);
				screenShot.LoadImage(saveData.screenShotData);
				button.GetComponentInChildren<RawImage>().texture = screenShot;
			}

			if (saveData.currentSpeakingContent != null)
			{
				var textComponents = button.GetComponentsInChildren<TextMeshProUGUI>();
				textComponents[0].text = saveData.currentSpeakingContent;
				textComponents[1].text = File.GetLastWriteTime(savePath).ToString("G");
			}
		}
	}

	private string GenerateDataPath(int slotIndex)
	{
		return Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH, slotIndex + Constants.SAVE_FILE_EXTENSION);
	}
}
