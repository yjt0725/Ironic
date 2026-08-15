using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SampleScene";
    [SerializeField] private string[] characterNames = {
        "\uAD81\uC218",
        "\uB3C4\uC801",
        "\uB9C8\uBC95\uC0AC"
    };
    [SerializeField] private string[] difficultyNames = {
        "\uC26C\uC6C0",
        "\uBCF4\uD1B5",
        "\uC5B4\uB824\uC6C0"
    };

    private int menuState = 0;

    private void OnGUI()
    {
        float width = 240.0f;
        float height = 60.0f;
        float gap = 12.0f;
        float centerX = Screen.width * 0.5f - width * 0.5f;

        GUI.skin.label.fontSize = 28;
        GUI.skin.button.fontSize = 20;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        if (0 == menuState)
        {
            GUI.Label(new Rect(centerX, Screen.height * 0.25f, width, 50.0f), "\uC544\uC774\uB7EC\uB2C9");

            float startY = Screen.height * 0.45f;

            if (true == GUI.Button(new Rect(centerX, startY, width, height), "\uAC8C\uC784 \uC2DC\uC791"))
            {
                menuState = 1;
            }

            if (true == GUI.Button(new Rect(centerX, startY + (height + gap), width, height), "\uAC8C\uC784 \uC885\uB8CC"))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
        else if (1 == menuState)
        {
            GUI.Label(new Rect(centerX, Screen.height * 0.2f, width, 50.0f), "\uC9C1\uC5C5 \uC120\uD0DD");

            float startY = Screen.height * 0.35f;

            for (int i = 0; i < characterNames.Length; i++)
            {
                Rect rect = new Rect(centerX, startY + i * (height + gap), width, height);

                if (true == GUI.Button(rect, characterNames[i]))
                {
                    GameData.selectedCharacter = i;
                    menuState = 2;
                }
            }

            float backY = startY + characterNames.Length * (height + gap) + gap;

            if (true == GUI.Button(new Rect(centerX, backY, width, height), "\uB4A4\uB85C"))
            {
                menuState = 0;
            }
        }
        else
        {
            GUI.Label(new Rect(centerX, Screen.height * 0.2f, width, 50.0f), "\uB09C\uC774\uB3C4 \uC120\uD0DD");

            float startY = Screen.height * 0.35f;

            for (int i = 0; i < difficultyNames.Length; i++)
            {
                Rect rect = new Rect(centerX, startY + i * (height + gap), width, height);

                if (true == GUI.Button(rect, difficultyNames[i]))
                {
                    GameData.difficulty = i;
                    SceneManager.LoadScene(gameSceneName);
                }
            }

            float backY = startY + difficultyNames.Length * (height + gap) + gap;

            if (true == GUI.Button(new Rect(centerX, backY, width, height), "\uB4A4\uB85C"))
            {
                menuState = 1;
            }
        }
    }
}
