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
        EnsureInstance().Show("\uB358\uC804 \uD074\uB9AC\uC5B4!");
    }

    public static void ShowGameOver()
    {
        EnsureInstance().Show("\uAC8C\uC784 \uC624\uBC84");
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
        float panelHeight = 405.0f;
        float x = Screen.width * 0.5f - panelWidth * 0.5f;
        float y = Screen.height * 0.5f - panelHeight * 0.5f;

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 42;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = resultText == "\uAC8C\uC784 \uC624\uBC84"
            ? new Color(1.0f, 0.25f, 0.25f)
            : new Color(1.0f, 0.82f, 0.25f);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 22;
        buttonStyle.fontStyle = FontStyle.Bold;

        GUI.Label(new Rect(x, y, panelWidth, 72.0f), resultText, titleStyle);

        GUIStyle recordStyle = new GUIStyle(GUI.skin.label);
        recordStyle.fontSize = 20;
        recordStyle.fontStyle = FontStyle.Bold;
        recordStyle.alignment = TextAnchor.MiddleCenter;
        recordStyle.normal.textColor = Color.white;
        GUI.Label(
            new Rect(x, y + 72.0f, panelWidth, 38.0f),
            $"\uB09C\uC774\uB3C4 {GameData.GetDifficultyName()}  |  \uAE30\uB85D {GameData.FormatElapsedTime()}",
            recordStyle
        );

        if (GUI.Button(new Rect(x + 70.0f, y + 125.0f, panelWidth - 140.0f, 58.0f), "\uB2E4\uC2DC \uC2DC\uC791", buttonStyle))
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (GUI.Button(new Rect(x + 70.0f, y + 200.0f, panelWidth - 140.0f, 58.0f), "\uC9C1\uC5C5 \uC120\uD0DD", buttonStyle))
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(titleSceneName);
        }

        if (GUI.Button(new Rect(x + 70.0f, y + 275.0f, panelWidth - 140.0f, 58.0f), "\uAC8C\uC784 \uC885\uB8CC", buttonStyle))
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
