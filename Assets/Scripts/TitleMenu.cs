using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    private enum MenuState
    {
        Main = 0,
        Character = 1,
        Difficulty = 2
    }

    [SerializeField] private string gameSceneName = "SampleScene";
    [SerializeField] private string backgroundSpriteName = "Sprites/TitleBackground";

    [SerializeField] private float menuScale = 0.5f;
    [SerializeField] private float menuGap = 20.0f;
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float menuGapHorizontal = 60.0f;

    // 난이도 전용 이미지가 아직 없어서 글자 버튼으로 그린다.
    // Sprites/Menu.Easy, Menu.Normal, Menu.Hard 를 만들어 넣으면
    // 자동으로 이미지 버튼으로 바뀐다.
    [SerializeField] private string[] difficultyNames = {
        "쉬움",
        "보통",
        "어려움"
    };

    private Texture2D background;

    private Texture2D texStart;
    private Texture2D texQuit;
    private Texture2D texArcher;
    private Texture2D texRogue;
    private Texture2D texMage;
    private Texture2D texBack;

    private Texture2D[] texDifficulties;

    private MenuState menuState = MenuState.Main;

    private void Awake()
    {
        background = LoadTexture(backgroundSpriteName);

        texStart = LoadTexture("Sprites/Menu.Start");
        texQuit = LoadTexture("Sprites/Menu.Quit");
        texArcher = LoadTexture("Sprites/Menu.Archer");
        texRogue = LoadTexture("Sprites/Menu.Rogue");
        texMage = LoadTexture("Sprites/Menu.Mage");
        texBack = LoadTexture("Sprites/Menu.Back");

        texDifficulties = new Texture2D[]
        {
            LoadTexture("Sprites/Menu.Easy"),
            LoadTexture("Sprites/Menu.Normal"),
            LoadTexture("Sprites/Menu.Hard")
        };
    }

    private Texture2D LoadTexture(string path)
    {
        Sprite sprite = Resources.Load<Sprite>(path);
        if (null == sprite)
        {
            return null;
        }

        return sprite.texture;
    }

    private void OnGUI()
    {
        DrawBackground();

        if (MenuState.Main == menuState)
        {
            DrawMainMenu();
            return;
        }

        if (MenuState.Character == menuState)
        {
            DrawCharacterMenu();
            return;
        }

        DrawDifficultyMenu();
    }

    private void DrawMainMenu()
    {
        float totalHeight = GetHeight(texStart) + GetHeight(texQuit) + menuGap;
        float y = Screen.height * 0.62f - totalHeight * 0.5f;

        if (true == DrawMenuItem(texStart, ref y))
        {
            menuState = MenuState.Character;
        }

        if (true == DrawMenuItem(texQuit, ref y))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private void DrawCharacterMenu()
    {
        Texture2D[] characters = { texArcher, texRogue, texMage };

        float rowWidth = 0.0f;
        float rowHeight = 0.0f;

        for (int i = 0; i < characters.Length; i++)
        {
            rowWidth += GetWidth(characters[i]);
            rowHeight = Mathf.Max(rowHeight, GetHeight(characters[i]));
        }

        rowWidth += menuGapHorizontal * (characters.Length - 1);

        float rowY = Screen.height * 0.68f - rowHeight * 0.5f;
        float x = Screen.width * 0.5f - rowWidth * 0.5f;

        for (int i = 0; i < characters.Length; i++)
        {
            float itemHeight = GetHeight(characters[i]);
            float itemY = rowY + (rowHeight - itemHeight) * 0.5f;

            if (true == DrawMenuItemAt(characters[i], x, itemY))
            {
                GameData.selectedCharacter = i;
                menuState = MenuState.Difficulty;
            }

            x += GetWidth(characters[i]) + menuGapHorizontal;
        }

        float backY = rowY + rowHeight + menuGap;

        if (true == DrawMenuItem(texBack, ref backY))
        {
            menuState = MenuState.Main;
        }
    }

    private void DrawDifficultyMenu()
    {
        // 난이도 이미지가 전부 준비돼 있으면 캐릭터 선택과 같은 모양으로 그린다.
        bool hasImages = true;
        for (int i = 0; i < texDifficulties.Length; i++)
        {
            if (null == texDifficulties[i])
            {
                hasImages = false;
                break;
            }
        }

        if (true == hasImages)
        {
            DrawDifficultyImages();
            return;
        }

        DrawDifficultyButtons();
    }

    private void DrawDifficultyImages()
    {
        float rowWidth = 0.0f;
        float rowHeight = 0.0f;

        for (int i = 0; i < texDifficulties.Length; i++)
        {
            rowWidth += GetWidth(texDifficulties[i]);
            rowHeight = Mathf.Max(rowHeight, GetHeight(texDifficulties[i]));
        }

        rowWidth += menuGapHorizontal * (texDifficulties.Length - 1);

        float rowY = Screen.height * 0.68f - rowHeight * 0.5f;
        float x = Screen.width * 0.5f - rowWidth * 0.5f;

        for (int i = 0; i < texDifficulties.Length; i++)
        {
            float itemHeight = GetHeight(texDifficulties[i]);
            float itemY = rowY + (rowHeight - itemHeight) * 0.5f;

            if (true == DrawMenuItemAt(texDifficulties[i], x, itemY))
            {
                StartGame(i);
            }

            x += GetWidth(texDifficulties[i]) + menuGapHorizontal;
        }

        float backY = rowY + rowHeight + menuGap;

        if (true == DrawMenuItem(texBack, ref backY))
        {
            menuState = MenuState.Character;
        }
    }

    private void DrawDifficultyButtons()
    {
        // 해상도가 4K까지 올라가므로 버튼 크기를 화면 높이에 비례시킨다.
        float buttonWidth = Screen.height * 0.22f;
        float buttonHeight = Screen.height * 0.07f;
        float gap = Screen.height * 0.02f;
        float centerX = Screen.width * 0.5f - buttonWidth * 0.5f;
        float startY = Screen.height * 0.55f;

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.035f);
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = new Color(0.95f, 0.88f, 1.0f, 1.0f);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.028f);
        buttonStyle.fontStyle = FontStyle.Bold;

        GUI.Label(
            new Rect(centerX, startY - buttonHeight - gap, buttonWidth, buttonHeight),
            "난이도 선택",
            labelStyle
        );

        for (int i = 0; i < difficultyNames.Length; i++)
        {
            Rect rect = new Rect(
                centerX,
                startY + i * (buttonHeight + gap),
                buttonWidth,
                buttonHeight
            );

            if (true == GUI.Button(rect, difficultyNames[i], buttonStyle))
            {
                StartGame(i);
            }
        }

        float backY = startY + difficultyNames.Length * (buttonHeight + gap) + gap;

        if (true == GUI.Button(
            new Rect(centerX, backY, buttonWidth, buttonHeight),
            "뒤로",
            buttonStyle))
        {
            menuState = MenuState.Character;
        }
    }

    private void StartGame(int difficultyIndex)
    {
        GameData.difficulty = difficultyIndex;

        // 이전 판의 기록이 남아 있으면 안 된다.
        GameData.elapsedTime = 0.0f;

        // 일시정지 상태로 씬을 나갔을 수 있으므로 되돌린다.
        Time.timeScale = 1.0f;

        SceneManager.LoadScene(gameSceneName);
    }

    private float GetHeight(Texture2D texture)
    {
        if (null == texture)
        {
            return 0.0f;
        }

        return texture.height * menuScale;
    }

    private float GetWidth(Texture2D texture)
    {
        if (null == texture)
        {
            return 0.0f;
        }

        return texture.width * menuScale;
    }

    private bool DrawMenuItemAt(Texture2D texture, float x, float y)
    {
        if (null == texture)
        {
            return false;
        }

        float width = texture.width * menuScale;
        float height = texture.height * menuScale;

        Rect rect = new Rect(x, y, width, height);
        bool hover = rect.Contains(Event.current.mousePosition);

        Rect drawRect = rect;
        if (true == hover)
        {
            float grownWidth = width * hoverScale;
            float grownHeight = height * hoverScale;

            drawRect = new Rect(
                x - (grownWidth - width) * 0.5f,
                y - (grownHeight - height) * 0.5f,
                grownWidth,
                grownHeight
            );
        }

        GUI.DrawTexture(drawRect, texture, ScaleMode.StretchToFill);

        if (Event.current.type == EventType.MouseDown && true == hover)
        {
            Event.current.Use();
            return true;
        }

        return false;
    }

    private bool DrawMenuItem(Texture2D texture, ref float y)
    {
        if (null == texture)
        {
            return false;
        }

        float width = texture.width * menuScale;
        float height = texture.height * menuScale;
        float x = Screen.width * 0.5f - width * 0.5f;

        Rect rect = new Rect(x, y, width, height);
        bool hover = rect.Contains(Event.current.mousePosition);

        Rect drawRect = rect;
        if (true == hover)
        {
            float grownWidth = width * hoverScale;
            float grownHeight = height * hoverScale;

            drawRect = new Rect(
                Screen.width * 0.5f - grownWidth * 0.5f,
                y - (grownHeight - height) * 0.5f,
                grownWidth,
                grownHeight
            );
        }

        GUI.DrawTexture(drawRect, texture, ScaleMode.StretchToFill);

        bool clicked = false;
        if (Event.current.type == EventType.MouseDown && true == hover)
        {
            clicked = true;
            Event.current.Use();
        }

        y += height + menuGap;

        return clicked;
    }

    private void DrawBackground()
    {
        if (null == background)
        {
            return;
        }

        float screenRatio = (float)Screen.width / Screen.height;
        float imageRatio = (float)background.width / background.height;

        float drawWidth = Screen.width;
        float drawHeight = Screen.height;

        if (screenRatio > imageRatio)
        {
            drawHeight = Screen.width / imageRatio;
        }
        else
        {
            drawWidth = Screen.height * imageRatio;
        }

        float x = (Screen.width - drawWidth) * 0.5f;
        float y = (Screen.height - drawHeight) * 0.5f;

        GUI.DrawTexture(new Rect(x, y, drawWidth, drawHeight), background, ScaleMode.StretchToFill);
    }
}
