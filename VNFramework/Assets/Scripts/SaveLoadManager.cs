using Newtonsoft.Json;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
	public GameObject saveLoadPanel;
	public TextMeshProUGUI panelTitle;
	public SaveSlot[] slots;
	public Button prevPageButton;
	public Button nextPageButton;
	public Button backButton;

	public GameObject confirmPanel;
	public TextMeshProUGUI confirmText;
	public Button confirmButton;
	public Button cancelButton;

	private bool isSave;
	private int currentPage = Constants.DEFAULT_START_INDEX;
	private readonly int slotsPerPage = Constants.SLOTS_PER_PAGE;
	private readonly int totalSlots = Constants.TOTAL_SLOTS;

	private System.Action<int> currentAction;
	private System.Action menuAction;

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

		confirmPanel.SetActive(false);
		saveLoadPanel.SetActive(false);
		
		RefreshPage();
	}

	private void RefreshPage()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			int slotIndex = currentPage * slotsPerPage + i;
			if (slotIndex >= totalSlots) 
			{
				slots[i].gameObject.SetActive(false);
				continue;
			}
			slots[i].gameObject.SetActive(true);
			slots[i].Init(this, slotIndex);
			slots[i].Refresh();
		}
	}

	public void HandleEmptySlot(int slotIndex, SaveSlot slot)
	{
		SaveToSlot(slotIndex, slot);
	}

	public void HandleExistingSlot(int slotIndex, SaveSlot slot)
	{
		if (!isSave)
		{
			VNManager.Instance.LoadGame(slotIndex);
			menuAction?.Invoke();
		}
		else
		{
			ShowConfirm( Constants.CONFIRM_OVERWRITE, ()=> {SaveToSlot(slotIndex, slot);} );
		}
	}

	public void RequestDelete(int slotIndex, SaveSlot slot)
	{
		ShowConfirm( Constants.CONFIRM_DELETE, ()=> {DeleteSlot(slotIndex, slot);} );
	}

	private void SaveToSlot(int slotIndex, SaveSlot slot)
	{
		VNManager.Instance.SaveGame(slotIndex);
		slot.Refresh();
	}

	private void DeleteSlot(int slotIndex, SaveSlot slot)
	{
		File.Delete(GenerateDataPath(slotIndex));
		slot.Refresh();
	}

	private void ShowConfirm(string msg, System.Action OnYes){
		confirmText.text = msg;
		confirmPanel.SetActive(true);
		cancelButton.onClick.RemoveAllListeners();
		cancelButton.onClick.AddListener(() => 
		{	
			confirmPanel.SetActive(false);
		});
		confirmButton.onClick.RemoveAllListeners();
		confirmButton.onClick.AddListener(() => 
		{	
			confirmPanel.SetActive(false);
			OnYes?.Invoke();
		});
	}

	public void ShowSavePanel(System.Action<int> action)
	{
		isSave = true;
		panelTitle.text = Constants.SAVE_GAME;
		currentAction = action;
		RefreshPage();
		saveLoadPanel.SetActive(true);
	}

	public void ShowLoadPanel(System.Action<int> action, System.Action menuAction)
	{
		isSave = false;
		panelTitle.text = Constants.LOAD_GAME;
		currentAction = action;
		this.menuAction = menuAction;
		RefreshPage();	
		saveLoadPanel.SetActive(true);
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
		menuAction?.Invoke();
		currentAction?.Invoke(slotIndex);
		if (isSave)
		{
			LoadStorylineAndScreenshots(button, slotIndex);
		}
		else
		{
			GoBack();
		}
	}

	private void PrevPage()
	{
		if (currentPage > 0)
		{
			currentPage--;
			RefreshPage();
		}
	}

		private void NextPage()
		{
			if ((currentPage + 1) * slotsPerPage < totalSlots)
			{
				currentPage++;
				RefreshPage();
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

			if (saveData.savedSpeakingContent != null)
			{
				var textComponents = button.GetComponentsInChildren<TextMeshProUGUI>();
				textComponents[0].text = saveData.savedSpeakingContent;
				textComponents[1].text = File.GetLastWriteTime(savePath).ToString("G");
			}
		}
	}

	private string GenerateDataPath(int slotIndex)
	{
		return Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH, slotIndex + Constants.SAVE_FILE_EXTENSION);
	}
}
