using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject menuPanel;
    public Button startButton;
    public Button continueButton;
    public Button loadButton;
    public Button settingsButton;
    public Button quitButton;
    
    public Button galleryButton;

    private bool hasStarted = false;

    public static MenuManager Instance { get; private set; }

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
        menuButtonsAddListener();
    }

    void menuButtonsAddListener()
    {
        //startButton.onClick.AddListener(StartGame);
        startButton.onClick.AddListener(ShowInputPanel);
        continueButton.onClick.AddListener(ContinueGame);
        loadButton.onClick.AddListener(LoadGame);
        settingsButton.onClick.AddListener(ShowSettingsPanel);
        galleryButton.onClick.AddListener(ShowGalleryPanel);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void StartGame()
    {
        VNManager.Instance.StartGame();
        ShowGamePanel();
    }

    private void ContinueGame()
    {
        if (hasStarted)
        {
            ShowGamePanel();
            VNManager.Instance.RecoverLastBackgroundAndAction();
        }
    }

    private void LoadGame()
    {
        VNManager.Instance.ShowLoadPanel(ShowGamePanel);
    }

    private void ShowInputPanel()
    {
        InputManager.Instance.ShowInputPanel();
    }

    private void ShowGamePanel()
    {
        hasStarted = true;
        menuPanel.SetActive(false);
        VNManager.Instance.gamePanel.SetActive(true);
    }

    private void ShowGalleryPanel()
    {
        GalleryManager.Instance.ShowGalleryPanel();
    }

    private void ShowSettingsPanel()
    {
        SettingsManager.Instance.ShowSettingsPanel();
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
