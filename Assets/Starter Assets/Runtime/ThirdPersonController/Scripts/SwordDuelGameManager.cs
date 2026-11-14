using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StarterAssets
{
    [DefaultExecutionOrder(-100)]
    public class SwordDuelGameManager : MonoBehaviour
    {
        private const int EnemyCount = 4;
        private const int SceneEnemyCount = 2;
        private const float PlayerMaxHealth = 350f;
        private const float EnemyMaxHealth = 100f;
        private const float PlayerAttackDamage = 25f;
        private const float EnemyAttackDamage = 15f;
        private const float PlayerAttackRange = 2.7f;
        private const float EnemyAttackRange = 1.8f;
        private const float EnemyDetectionRange = 4.5f;
        private const float EnemyAttackInterval = 1.8f;
        private const float EnemyAttackDuration = 0.9f;
        private const float EnemyMoveSpeed = 2.2f;

        [SerializeField] private RuntimeAnimatorController enemyAnimatorController;
        [SerializeField] private Avatar enemyAvatar;

        private readonly List<Enemy> _enemies = new List<Enemy>();
        private ThirdPersonController _player;
        private float _playerHealth = PlayerMaxHealth;
        private int _defeatedEnemies;
        private bool _gameEnded;
        private string _endMessage;

        public static SwordDuelGameManager Instance { get; private set; }

        private sealed class Enemy
        {
            public GameObject Object;
            public Animator Animator;
            public float Health = EnemyMaxHealth;
            public float NextAttackTime;
            public float AttackStartTime;
            public float AttackImpactTime;
            public float AttackEndTime;
            public Vector3 AttackOrigin;
            public bool IsAttacking;
            public bool AttackResolved;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("Only one SwordDuelGameManager can be active in a scene.", this);
                enabled = false;
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            _player = FindFirstObjectByType<ThirdPersonController>();
            if (_player == null)
            {
                Debug.LogError("SwordDuelGameManager could not find a ThirdPersonController in the active scene.", this);
                enabled = false;
                return;
            }

            if (enemyAnimatorController == null)
            {
                Debug.LogError("Assign the humanoid enemy Animator Controller to SwordDuelGameManager.", this);
                enabled = false;
                return;
            }

            SpawnEnemies();
        }

        private void Update()
        {
            if (_gameEnded || _player == null)
            {
                return;
            }

            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                UpdateEnemy(_enemies[i]);
            }
        }

        private void SpawnEnemies()
        {
            GameObject[] enemyTemplates = new GameObject[SceneEnemyCount];
            for (int i = 0; i < EnemyCount; i++)
            {
                string villainName = "Villain " + (i + 1);
                GameObject enemyObject;
                if (i < SceneEnemyCount)
                {
                    enemyObject = GameObject.Find(villainName);
                    if (enemyObject == null)
                    {
                        Debug.LogError("Place '" + villainName + "' in the scene so its position stays fixed when Play starts.", this);
                        enabled = false;
                        return;
                    }

                    enemyTemplates[i] = enemyObject;
                }
                else
                {
                    int templateIndex = i - SceneEnemyCount;
                    GameObject template = enemyTemplates[templateIndex];
                    Vector3 spawnOffset = templateIndex == 0
                        ? new Vector3(18f, 0f, 20f)
                        : new Vector3(18f, 0f, -18f);
                    Vector3 spawnPosition = template.transform.position + spawnOffset;
                    enemyObject = Instantiate(template, spawnPosition, template.transform.rotation);
                    enemyObject.name = villainName;
                }

                Animator animator = enemyObject.GetComponentInChildren<Animator>(true);
                if (animator == null)
                {
                    animator = enemyObject.AddComponent<Animator>();
                }

                if (enemyAvatar != null)
                {
                    animator.avatar = enemyAvatar;
                }

                if (animator.GetComponent<EnemyAnimationEventReceiver>() == null)
                {
                    animator.gameObject.AddComponent<EnemyAnimationEventReceiver>();
                }

                animator.runtimeAnimatorController = enemyAnimatorController;
                animator.applyRootMotion = false;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                animator.SetBool("Grounded", true);
                animator.SetBool("FreeFall", false);
                animator.SetFloat("MotionSpeed", 1f);
                animator.SetFloat("Speed", 0f);

                _enemies.Add(new Enemy
                {
                    Object = enemyObject,
                    Animator = animator,
                    NextAttackTime = Time.time + Random.Range(0.5f, 1.5f)
                });
            }
        }

        private void UpdateEnemy(Enemy enemy)
        {
            if (enemy.Object == null)
            {
                _enemies.Remove(enemy);
                return;
            }

            Vector3 toPlayer = _player.transform.position - enemy.Object.transform.position;
            toPlayer.y = 0f;
            float distance = toPlayer.magnitude;

            if (distance > EnemyDetectionRange)
            {
                SetEnemyAnimationSpeed(enemy, 0f);
                return;
            }

            if (distance > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(toPlayer);
                enemy.Object.transform.rotation = Quaternion.RotateTowards(
                    enemy.Object.transform.rotation, targetRotation, 360f * Time.deltaTime);
            }

            if (enemy.IsAttacking)
            {
                SetEnemyAnimationSpeed(enemy, 0f);
                float attackDuration = Mathf.Max(enemy.AttackEndTime - enemy.AttackStartTime, 0.01f);
                float progress = Mathf.Clamp01((Time.time - enemy.AttackStartTime) / attackDuration);
                float lungeDistance = progress < 0.5f ? progress * 0.7f : (1f - progress) * 0.7f;
                enemy.Object.transform.position = enemy.AttackOrigin + enemy.Object.transform.forward * lungeDistance;

                if (!enemy.AttackResolved && Time.time >= enemy.AttackImpactTime)
                {
                    enemy.AttackResolved = true;
                    if (distance <= EnemyAttackRange + 0.35f)
                    {
                        DamagePlayer(EnemyAttackDamage);
                    }
                }

                if (Time.time >= enemy.AttackEndTime)
                {
                    enemy.IsAttacking = false;
                }

                return;
            }

            if (distance > EnemyAttackRange)
            {
                enemy.Object.transform.position = Vector3.MoveTowards(
                    enemy.Object.transform.position,
                    _player.transform.position,
                    EnemyMoveSpeed * Time.deltaTime);
                SetEnemyAnimationSpeed(enemy, EnemyMoveSpeed);

                return;
            }

            SetEnemyAnimationSpeed(enemy, 0f);
            if (Time.time >= enemy.NextAttackTime)
            {
                enemy.IsAttacking = true;
                enemy.AttackResolved = false;
                enemy.AttackStartTime = Time.time;
                enemy.AttackOrigin = enemy.Object.transform.position;
                enemy.NextAttackTime = Time.time + EnemyAttackInterval;
                enemy.AttackImpactTime = Time.time + 0.4f;
                enemy.AttackEndTime = Time.time + EnemyAttackDuration;
                enemy.Animator.SetTrigger("SwordAttack");
            }
        }

        private static void SetEnemyAnimationSpeed(Enemy enemy, float speed)
        {
            if (enemy.Animator != null)
            {
                enemy.Animator.SetFloat("Speed", speed);
            }
        }

        public bool TryDamageNearestEnemy(Transform attacker)
        {
            if (_gameEnded || attacker == null)
            {
                return false;
            }

            Enemy nearestEnemy = null;
            float nearestDistance = PlayerAttackRange;
            Vector3 attackerPosition = attacker.position;
            attackerPosition.y = 0f;

            foreach (Enemy enemy in _enemies)
            {
                if (enemy.Object == null || enemy.Health <= 0f)
                {
                    continue;
                }

                Vector3 enemyPosition = enemy.Object.transform.position;
                enemyPosition.y = 0f;
                float distance = Vector3.Distance(attackerPosition, enemyPosition);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy == null)
            {
                return false;
            }

            nearestEnemy.Health = Mathf.Max(0f, nearestEnemy.Health - PlayerAttackDamage);
            if (nearestEnemy.Health <= 0f)
            {
                _enemies.Remove(nearestEnemy);
                Destroy(nearestEnemy.Object);
                _defeatedEnemies++;
                if (_defeatedEnemies >= EnemyCount)
                {
                    EndGame(true);
                }
            }

            return true;
        }

        private void DamagePlayer(float damage)
        {
            if (_gameEnded)
            {
                return;
            }

            _playerHealth = Mathf.Max(0f, _playerHealth - damage);
            if (_playerHealth <= 0f)
            {
                EndGame(false);
            }
        }

        private void EndGame(bool victory)
        {
            _gameEnded = true;
            _endMessage = victory ? "VICTORY! All four skeletons defeated." : "YOU WERE DEFEATED";
            if (_player != null)
            {
                _player.enabled = false;
                PlayerSwordAttack swordAttack = _player.GetComponent<PlayerSwordAttack>();
                if (swordAttack != null)
                {
                    swordAttack.enabled = false;
                }
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }

        private void OnGUI()
        {
            if (_player == null)
            {
                return;
            }

            DrawHealthBar(new Rect(20f, 42f, 240f, 20f), _playerHealth / PlayerMaxHealth, "Player  " + Mathf.CeilToInt(_playerHealth) + " / " + Mathf.CeilToInt(PlayerMaxHealth));
            GUI.Label(new Rect(20f, 12f, 300f, 26f), "Defeat all four skeletons  (" + _defeatedEnemies + " / " + EnemyCount + ")");
            GUI.Label(new Rect(20f, 68f, 320f, 24f), "WASD: move    Left click: attack");

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                foreach (Enemy enemy in _enemies)
                {
                    if (enemy.Object == null)
                    {
                        continue;
                    }

                    Vector3 screenPosition = mainCamera.WorldToScreenPoint(enemy.Object.transform.position + Vector3.up * 3.1f);
                    if (screenPosition.z <= 0f)
                    {
                        continue;
                    }

                    float x = screenPosition.x - 55f;
                    float y = Screen.height - screenPosition.y;
                    DrawHealthBar(new Rect(x, y, 110f, 14f), enemy.Health / EnemyMaxHealth, "Skeleton  " + Mathf.CeilToInt(enemy.Health) + " / 100");
                }
            }

            if (_gameEnded)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 32,
                    fontStyle = FontStyle.Bold
                };
                GUI.Label(new Rect(0f, Screen.height * 0.38f, Screen.width, 60f), _endMessage, style);

                if (GUI.Button(new Rect(Screen.width * 0.5f - 80f, Screen.height * 0.38f + 70f, 160f, 40f), "Play Again"))
                {
                    RestartGame();
                }
            }
        }

        private static void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private static void DrawHealthBar(Rect rect, float healthFraction, string label)
        {
            Rect labelRect = new Rect(rect.x, rect.y - 20f, rect.width, 20f);
            Color previousColor = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.75f);
            GUI.DrawTexture(labelRect, Texture2D.whiteTexture);
            GUI.color = previousColor;
            GUI.Label(labelRect, label);
            GUI.Box(rect, GUIContent.none);

            Rect fillRect = new Rect(rect.x + 2f, rect.y + 2f, (rect.width - 4f) * Mathf.Clamp01(healthFraction), rect.height - 4f);
            GUI.color = healthFraction > 0.5f ? Color.green : healthFraction > 0.25f ? Color.yellow : Color.red;
            GUI.DrawTexture(fillRect, Texture2D.whiteTexture);
            GUI.color = previousColor;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                Time.timeScale = 1f;
            }
        }

    }
}
