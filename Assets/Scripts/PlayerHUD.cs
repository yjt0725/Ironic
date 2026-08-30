using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private float screenMargin = 18.0f;
    [SerializeField] private float barWidthRatio = 0.22f;

    private static PlayerHUD instance;

    private DungeonGenerator generator;
    private TitleMenu titleMenu;
    private Player player;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateAutomatically()
    {
        if (null != FindAnyObjectByType<PlayerHUD>())
        {
            return;
        }

        GameObject hudObject = new GameObject("PlayerHUD");
        hudObject.AddComponent<PlayerHUD>();
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
        titleMenu = FindAnyObjectByType<TitleMenu>();

        if (null != titleMenu && true == titleMenu.enabled)
        {
            player = null;
            return;
        }

        if (null == generator)
        {
            generator = FindAnyObjectByType<DungeonGenerator>();
        }

        if (null == generator)
        {
            player = null;
            return;
        }

        if (null == player)
        {
            player = FindAnyObjectByType<Player>();
        }

        // 일시정지와 결과 화면에서는 timeScale이 0이라 deltaTime도 0이 된다.
        // 그래서 시간이 저절로 멈춘다. 별도 처리가 필요 없다.
        if (null != player && false == player.IsDead)
        {
            GameData.elapsedTime += Time.deltaTime;
        }
    }

    private void OnGUI()
    {
        if (null == player || null == generator)
        {
            return;
        }

        if (null != titleMenu && true == titleMenu.enabled)
        {
            return;
        }

        if (true == GameEndUI.IsShowing)
        {
            return;
        }

        float barWidth = Screen.width * barWidthRatio;
        float barHeight = Screen.height * 0.028f;
        float gap = Screen.height * 0.010f;
        float x = screenMargin;
        float y = screenMargin;

        GUIStyle textStyle = new GUIStyle(GUI.skin.label);
        textStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.022f);
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.alignment = TextAnchor.MiddleLeft;
        textStyle.normal.textColor = Color.white;

        // --- 경과 시간 -------------------------------------------------
        GUI.Label(
            new Rect(x, y, barWidth, barHeight),
            $"시간  {GameData.FormatElapsedTime()}   난이도  {GameData.GetDifficultyName()}",
            textStyle
        );

        y += barHeight + gap;

        // --- 체력 ------------------------------------------------------
        float healthRatio = 0.0f;
        if (0 < player.MaxHealth)
        {
            healthRatio = Mathf.Clamp01((float)player.CurrentHealth / player.MaxHealth);
        }

        DrawBar(
            new Rect(x, y, barWidth, barHeight),
            healthRatio,
            new Color(0.85f, 0.16f, 0.20f, 1.0f)
        );

        GUIStyle centerStyle = new GUIStyle(textStyle);
        centerStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Label(
            new Rect(x, y, barWidth, barHeight),
            $"HP  {player.CurrentHealth} / {player.MaxHealth}",
            centerStyle
        );

        y += barHeight + gap;

        // --- 특수공격 쿨타임 -------------------------------------------
        // SkillCooldownRatio는 1이면 방금 썼다는 뜻이라 뒤집어서 채운다.
        float readyRatio = 1.0f - player.SkillCooldownRatio;
        bool ready = 0.999f <= readyRatio;

        Color skillColor = true == ready
            ? new Color(0.30f, 0.75f, 1.0f, 1.0f)
            : new Color(0.35f, 0.42f, 0.60f, 1.0f);

        DrawBar(new Rect(x, y, barWidth, barHeight), readyRatio, skillColor);

        string skillText = true == ready ? "특수공격 [X]  준비" : "특수공격 [X]";
        GUI.Label(new Rect(x, y, barWidth, barHeight), skillText, centerStyle);
    }

    private void DrawBar(Rect rect, float ratio, Color fillColor)
    {
        DrawSolidRect(rect, new Color(0.05f, 0.03f, 0.08f, 0.85f));

        Rect fillRect = new Rect(
            rect.x,
            rect.y,
            rect.width * Mathf.Clamp01(ratio),
            rect.height
        );

        DrawSolidRect(fillRect, fillColor);
        DrawBorder(rect, 2.0f, new Color(0.58f, 0.43f, 0.68f, 1.0f));
    }

    private void DrawBorder(Rect rect, float thickness, Color color)
    {
        DrawSolidRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
        DrawSolidRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
        DrawSolidRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
        DrawSolidRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
    }

    private void DrawSolidRect(Rect rect, Color color)
    {
        Color previousColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previousColor;
    }
}
