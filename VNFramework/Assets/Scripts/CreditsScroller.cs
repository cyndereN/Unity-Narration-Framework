using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroller : MonoBehaviour
{
    public RectTransform creditsText;

    void Start()
    {
        LoadCreditsFromFile();
        creditsText.anchoredPosition = new Vector2(creditsText.anchoredPosition.x, -Screen.height);
    }

    void Update()
    {
        float speedMultiplier = Input.GetMouseButton(0) ? 2f : 1f;
        creditsText.anchoredPosition += Vector2.up * Constants.CREDITS_SCROLL_SPEED * speedMultiplier * Time.deltaTime;

        // todo: determine scroll_end_y dynamically
        if (creditsText.anchoredPosition.y >= Constants.CREDITS_SCROLL_END_Y)
        {
            SceneManager.LoadScene(Constants.MENU_SCENE);
        }
    }

    void LoadCreditsFromFile()
    {
        string path = Path.Combine(Application.streamingAssetsPath,
            Constants.CREDITS_PATH,
            "en" + Constants.CREDITS_FILE_EXTENSION);
        if (File.Exists(path))
        {
            string content = File.ReadAllText(path);
            creditsText.GetComponent<TextMeshProUGUI>().text = content;
        }
        else
        {
            creditsText.GetComponent<TextMeshProUGUI>().text = "Cannot find credits file";
        }
    }
}
