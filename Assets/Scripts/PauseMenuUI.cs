using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    private static PauseMenuUI instance;

    [SerializeField] private string titleSceneName = "Title";

    private DungeonGenerator generator;
    private bool paused;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateAutomatically()
    {
        if (null != FindAnyObjectByType<PauseMenuUI>())
        {
            return;
        }

        GameObject menuObject = new GameObject("PauseMenuUI");
        menuObject.AddComponent<PauseMenuUI>();
    }

    private void Awake()
    {
        if (null != instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Update()
    {
        if (null == generator)
        {
            generator = FindAnyObjectByType<DungeonGenerator>();
        }

        if (null == generator)
        {
            paused = false;
            return;
        }

        if (true == GameEndUI.IsShowing)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPaused(false == paused);
        }
    }

    private void SetPaused(bool shouldPause)
    {
        paused = shouldPause;
        Time.timeScale = true == paused ? 0.0f : 1.0f;
    }

    private void OnGUI()
    {
        if (false == paused)
        {
            return;
        }

        Color previousColor = GUI.color;
        GUI.color = new Color(0.02f, 0.01f, 0.04f, 0.86f);
        GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = previousColor;

        float panelWidth = 420.0f;
        float panelHeight = 390.0f;
        float x = Screen.width * 0.5f - panelWidth * 0.5f;
        float y = Screen.height * 0.5f - panelHeight * 0.5f;

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 42;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = new Color(0.95f, 0.85f, 1.0f, 1.0f);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 22;
        buttonStyle.fontStyle = FontStyle.Bold;

        GUI.Label(new Rect(x, y, panelWidth, 85.0f), "\uC77C\uC2DC\uC815\uC9C0", titleStyle);

        if (GUI.Button(new Rect(x + 60.0f, y + 105.0f, panelWidth - 120.0f, 60.0f), "\uACC4\uC18D\uD558\uAE30", buttonStyle))
        {
            SetPaused(false);
        }

        if (GUI.Button(new Rect(x + 60.0f, y + 185.0f, panelWidth - 120.0f, 60.0f), "\uC9C1\uC5C5 \uC120\uD0DD", buttonStyle))
        {
            SetPaused(false);
            generator = null;
            SceneManager.LoadScene(titleSceneName);
        }

        if (GUI.Button(new Rect(x + 60.0f, y + 265.0f, panelWidth - 120.0f, 60.0f), "\uAC8C\uC784 \uC885\uB8CC", buttonStyle))
        {
            SetPaused(false);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
