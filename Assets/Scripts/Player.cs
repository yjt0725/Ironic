using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 6.0f;
    public float radius = 0.3f;
    [SerializeField]
    private GameObject shurikenPrefab;

    [SerializeField]
    private Transform shurikenSpawnPoint;
    [SerializeField]
    private GameObject arrowPrefab;

    [SerializeField]
    private Transform arrowSpawnPoint;
    [SerializeField]
    private float rollSpeed = 12.0f;

    [SerializeField]
    private float rollDuration = 0.5f;

    [Header("Combat")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int rollDamage = 2;
    [SerializeField] private float rollHitRadius = 0.55f;
    [SerializeField] private float damageInvincibility = 0.6f;

    private TileMap tileMap;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isAttacking;
    private bool isRolling;
    private bool isUsingSkill;
    private bool isDead;
    private int health;
    private float invincibilityRemaining;
    private Vector2 lastMoveDirection = Vector2.right;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = maxHealth;
    }

    private void SetMoving(bool moving)
    {
        if (null == animator)
        {
            return;
        }

        animator.speed = (true == moving) ? 1.0f : 0.0f;
    }

    public void Init(TileMap tileMap, Vector2 startPosition)
    {
        Time.timeScale = 1.0f;
        this.tileMap = tileMap;
        transform.position = new Vector3(startPosition.x, startPosition.y, 0.0f);
    }

    private void Update()
    {
        invincibilityRemaining = Mathf.Max(0.0f, invincibilityRemaining - Time.deltaTime);

        if (true == isDead || true == GameEndUI.IsShowing)
        {
            return;
        }

        if (null == tileMap)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Z) && false == isAttacking && false == isRolling && false == isUsingSkill)
        {
            StartCoroutine(Attack());
        }
        if (Input.GetKeyDown(KeyCode.X) && false == isRolling && false == isAttacking && false == isUsingSkill)
        {
            if (null != arrowPrefab)
{
    StartCoroutine(ScatterShot());
}
else if (null != shurikenPrefab)
{
    StartCoroutine(Roll());
}
else
{
    StartCoroutine(MageSpin());
}
        }

        if (true == isRolling)
        {
            return;
        }

        float horizontal = 0.0f;
        float vertical = 0.0f;

        if (true == Input.GetKey(KeyCode.LeftArrow) || true == Input.GetKey(KeyCode.A))
        {
            horizontal -= 1.0f;
        }

        if (true == Input.GetKey(KeyCode.RightArrow) || true == Input.GetKey(KeyCode.D))
        {
            horizontal += 1.0f;
        }

        if (true == Input.GetKey(KeyCode.DownArrow) || true == Input.GetKey(KeyCode.S))
        {
            vertical -= 1.0f;
        }

        if (true == Input.GetKey(KeyCode.UpArrow) || true == Input.GetKey(KeyCode.W))
        {
            vertical += 1.0f;
        }

        Vector2 direction = new Vector2(horizontal, vertical);
        if (0.0f == direction.sqrMagnitude)
        {
            if (false == isAttacking)
            {
                SetMoving(false);
            }

            return;
        }

        lastMoveDirection = direction.normalized;
        SetMoving(true);

        if (null != spriteRenderer && 0.0f != horizontal)
        {
            spriteRenderer.flipX = (horizontal < 0.0f);
        }

        direction = direction.normalized;

        float distance = moveSpeed * Time.deltaTime;
        Vector2 position = transform.position;

        Vector2 movedX = new Vector2(position.x + direction.x * distance, position.y);
        if (true == CanMove(movedX))
        {
            position = movedX;
        }

        Vector2 movedY = new Vector2(position.x, position.y + direction.y * distance);
        if (true == CanMove(movedY))
        {
            position = movedY;
        }

        transform.position = new Vector3(position.x, position.y, 0.0f);

        Camera camera = Camera.main;
        if (null != camera)
        {
            camera.transform.position = new Vector3(transform.position.x, transform.position.y, -10.0f);
        }
    }
    private void FireArrowSpread()
    {
        if (null == arrowPrefab || null == arrowSpawnPoint)
        {
            return;
        }

        Vector2 baseDirection = lastMoveDirection.normalized;

        float spawnDistance =
            Mathf.Abs(arrowSpawnPoint.localPosition.x);

        Vector3 spawnPosition =
            transform.position
            + (Vector3)(baseDirection * spawnDistance);

        for (int i = -2; i <= 2; ++i)
        {
            float angle = i * 10.0f;

            Vector2 direction =
                Quaternion.Euler(0.0f, 0.0f, angle)
                * baseDirection;

            GameObject arrow = Instantiate(
                arrowPrefab,
                spawnPosition,
                Quaternion.identity
            );

            ArrowProjectile projectile =
                arrow.GetComponent<ArrowProjectile>();

            if (null != projectile)
            {
                projectile.Initialize(direction);
            }
        }
    }
    public void ThrowArrow()
    {
        if (null == arrowPrefab || null == arrowSpawnPoint)
        {
            return;
        }

        Vector2 direction = lastMoveDirection.normalized;

        float spawnDistance =
            Mathf.Abs(arrowSpawnPoint.localPosition.x);

        Vector3 spawnPosition =
            transform.position
            + (Vector3)(direction * spawnDistance);

        GameObject arrow = Instantiate(
            arrowPrefab,
            spawnPosition,
            Quaternion.identity
        );

        ArrowProjectile projectile =
            arrow.GetComponent<ArrowProjectile>();

        if (null != projectile)
        {
            projectile.Initialize(direction);
        }
    }
    public void ThrowShuriken()
    {
        if (null == shurikenPrefab || null == shurikenSpawnPoint)
        {
            return;
        }

        Vector2 direction =
            spriteRenderer.flipX ? Vector2.left : Vector2.right;

        Vector3 offset = shurikenSpawnPoint.localPosition;
        offset.x = Mathf.Abs(offset.x) * direction.x;

        Vector3 spawnPosition = transform.TransformPoint(offset);

        GameObject shuriken = Instantiate(
            shurikenPrefab,
            spawnPosition,
            Quaternion.identity
        );

        ShurikenProjectile projectile =
            shuriken.GetComponent<ShurikenProjectile>();

        if (null != projectile)
        {
            projectile.Initialize(direction);
        }
    }
    private void ThrowBasicProjectile()
    {
        if (null != shurikenPrefab)
        {
            ThrowShuriken();
            return;
        }

        if (null != arrowPrefab)
        {
            ThrowArrow();
        }
    }
    private IEnumerator ScatterShot()
    {
        isUsingSkill = true;

        animator.speed = 1.0f;
        animator.SetTrigger("skill");

        yield return new WaitForSeconds(0.3f);

        FireArrowSpread();

        yield return new WaitForSeconds(0.17f);

        isUsingSkill = false;
    }
    private IEnumerator Attack()
    {
        isAttacking = true;

        animator.speed = 1.0f;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.3f);

        if (null == shurikenPrefab && null == arrowPrefab)
        {
            Vector2 attackCenter =
                (Vector2)transform.position + lastMoveDirection * 0.6f;
            DamageMonsters(attackCenter, 0.75f, 1);
        }
        else
        {
            ThrowBasicProjectile();
        }

        yield return new WaitForSeconds(0.5f);
        if (null == shurikenPrefab && null == arrowPrefab)
{
    animator.Play("mage_walk");
}

        isAttacking = false;
    }

    private IEnumerator MageSpin()
{
    isUsingSkill = true;

    animator.speed = 1.0f;
    animator.SetTrigger("Skill");

    yield return new WaitForSeconds(0.3f);

    DamageMonsters(transform.position, 1.25f, 2);

    yield return new WaitForSeconds(0.3f);

    animator.Play("mage_walk");
    isUsingSkill = false;
}

    private void DamageMonsters(Vector2 center, float hitRadius, int damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, hitRadius);
        foreach (Collider2D hit in hits)
        {
            Monster monster = hit.GetComponent<Monster>();
            if (null != monster)
            {
                monster.TakeDamage(damage);
            }
        }
    }

    private void DamageMonstersOnce(
        Vector2 center,
        float hitRadius,
        int damage,
        HashSet<Monster> damagedMonsters
    )
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, hitRadius);
        foreach (Collider2D hit in hits)
        {
            Monster monster = hit.GetComponent<Monster>();
            if (null == monster || damagedMonsters.Contains(monster))
            {
                continue;
            }

            damagedMonsters.Add(monster);
            monster.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (true == isDead || true == isRolling || 0.0f < invincibilityRemaining || 0 >= damage)
        {
            return;
        }

        health = Mathf.Max(0, health - damage);
        invincibilityRemaining = damageInvincibility;

        if (0 >= health)
        {
            isDead = true;
            SetMoving(false);
            GameEndUI.ShowGameOver();
        }
    }

    private void OnGUI()
    {
        if (true == isDead || true == GameEndUI.IsShowing)
        {
            return;
        }

        GUIStyle healthStyle = new GUIStyle(GUI.skin.label);
        healthStyle.fontSize = 24;
        healthStyle.fontStyle = FontStyle.Bold;
        healthStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(24.0f, 18.0f, 220.0f, 42.0f), $"HP  {health} / {maxHealth}", healthStyle);
    }

    private IEnumerator Roll()
    {
        isRolling = true;

        animator.speed = 1.0f;
        animator.SetTrigger("Roll");

        Vector2 rollDirection = lastMoveDirection.normalized;
        HashSet<Monster> damagedMonsters = new HashSet<Monster>();

        float elapsedTime = 0.0f;

        while (elapsedTime < rollDuration)
        {
            float distance = rollSpeed * Time.deltaTime;

            Vector2 nextPosition =
                (Vector2)transform.position
                + rollDirection * distance;

            if (false == CanMove(nextPosition))
            {
                break;
            }

            transform.position = new Vector3(
                nextPosition.x,
                nextPosition.y,
                0.0f
            );

            DamageMonstersOnce(transform.position, rollHitRadius, rollDamage, damagedMonsters);

            Camera camera = Camera.main;

            if (null != camera)
            {
                camera.transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    -10.0f
                );
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isRolling = false;
    }

    private bool CanMove(Vector2 position)
    {
        if (false == IsFloor(position.x - radius, position.y - radius))
        {
            return false;
        }

        if (false == IsFloor(position.x + radius, position.y - radius))
        {
            return false;
        }

        if (false == IsFloor(position.x - radius, position.y + radius))
        {
            return false;
        }

        if (false == IsFloor(position.x + radius, position.y + radius))
        {
            return false;
        }

        return true;
    }

    private bool IsFloor(float worldX, float worldY)
    {
        int x = Mathf.FloorToInt(worldX);
        int y = Mathf.FloorToInt(worldY);

        Tile tile = tileMap.GetTile(x, y);
        if (null == tile)
        {
            return false;
        }

        if (Tile.Type.Floor != tile.type)
        {
            return false;
        }

        if (null != tile.door && Door.State.Open != tile.door.state)
        {
            return false;
        }

        if (true == PropBlock.IsBlocked(tile.index))
        {
            return false;
        }

        return true;
    }
}
