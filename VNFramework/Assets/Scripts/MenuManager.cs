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
        startButton.onClick.AddListener(StartGame);
        continueButton.onClick.AddListener(ContinueGame);
        loadButton.onClick.AddListener(LoadGame);
    }

    private void StartGame()
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
        menuPanel.SetActive(false);
        VNManager.Instance.ShowLoadPanel(ShowGamePanel);
    }

    private void ShowGamePanel()
    {
        hasStarted = true;
        menuPanel.SetActive(false);
        VNManager.Instance.gamePanel.SetActive(true);
    }
}
