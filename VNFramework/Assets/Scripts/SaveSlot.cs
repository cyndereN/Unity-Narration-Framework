using Newtonsoft.Json;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public Button slotButton;
    public Button deleteButton;
    public RawImage thumbnail;
    public TextMeshProUGUI topText;
    public TextMeshProUGUI bottomText;

    private int slotIndex;
    private SaveLoadManager owner;
    private bool hasFile;

    public void Init(SaveLoadManager mgr, int index)
    {
        owner = mgr;
        slotIndex = index;

        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(OnSlotClick);

        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(OnDeleteClick);
    }

    public void Refresh()
    {
        string path = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH, slotIndex + Constants.SAVE_FILE_EXTENSION);
        hasFile = File.Exists(path);
        bool isLoad = owner.panelTitle.text == Constants.LOAD_GAME;

        deleteButton.gameObject.SetActive(hasFile);
        slotButton.interactable = hasFile || !isLoad;
        thumbnail.texture = null;

        if (!hasFile)
        {
            topText.text = "";
            bottomText.text = (slotIndex + 1) + " " + Constants.EMPTY_SLOT;
            return;
        }

        string json = File.ReadAllText(path);
        var data = JsonConvert.DeserializeObject<VNManager.SaveData>(json);

        if (data.screenShotData != null)
        {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(data.screenShotData);
            thumbnail.texture = tex;
        }

        if (data.savedSpeakingContent != null)
        {
            topText.text = data.savedSpeakingContent;
        }
        bottomText.text = File.GetLastWriteTime(path).ToString("G");
    }

    private void OnSlotClick()
    {
        if (hasFile)
        {
            owner.HandleExistingSlot(slotIndex, this);
        }
        else
        {
            owner.HandleEmptySlot(slotIndex, this);
        }
    }

    private void OnDeleteClick()
    {
        owner.RequestDelete(slotIndex, this);
    }
}
