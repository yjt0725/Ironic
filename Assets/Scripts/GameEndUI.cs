using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndUI : MonoBehaviour
{
    private static GameEndUI instance;

    [SerializeField] private string titleSceneName = "Title";

    private bool showing;
    private string resultText;

    public static bool IsShowing
    {
        get { return null != instance && instance.showing; }
    }

    private void Awake()
    {
        if (null != instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public static void ShowClear()
    {
        EnsureInstance().Show("DUNGEON CLEAR!");
    }

    public static void ShowGameOver()
    {
        EnsureInstance().Show("GAME OVER");
    }

    private static GameEndUI EnsureInstance()
    {
        if (null != instance)
        {
            return instance;
        }

        GameObject uiObject = new GameObject("GameEndUI");
        instance = uiObject.AddComponent<GameEndUI>();
        return instance;
    }

    private void Show(string message)
    {
        if (true == showing)
        {
            return;
        }

        resultText = message;
        showing = true;
        Time.timeScale = 0.0f;
    }

    private void OnGUI()
    {
        if (false == showing)
        {
            return;
        }

        Color previousColor = GUI.color;
        GUI.color = new Color(0.02f, 0.01f, 0.04f, 0.88f);
        GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = previousColor;

        float panelWidth = 420.0f;
        float panelHeight = 385.0f;
        float x = Screen.width * 0.5f - panelWidth * 0.5f;
        float y = Screen.height * 0.5f - panelHeight * 0.5f;

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 42;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = resultText == "GAME OVER"
            ? new Color(1.0f, 0.25f, 0.25f)
            : new Color(1.0f, 0.82f, 0.25f);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 22;
        buttonStyle.fontStyle = FontStyle.Bold;

        GUI.Label(new Rect(x, y, panelWidth, 90.0f), resultText, titleStyle);

        if (GUI.Button(new Rect(x + 70.0f, y + 120.0f, panelWidth - 140.0f, 58.0f), "RETRY", buttonStyle))
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (GUI.Button(new Rect(x + 70.0f, y + 195.0f, panelWidth - 140.0f, 58.0f), "TITLE", buttonStyle))
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(titleSceneName);
        }

        if (GUI.Button(new Rect(x + 70.0f, y + 270.0f, panelWidth - 140.0f, 58.0f), "EXIT", buttonStyle))
        {
            Time.timeScale = 1.0f;

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
