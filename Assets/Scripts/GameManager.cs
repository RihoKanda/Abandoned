using System;
using UnityEngine;
using IdleGame.API;
using IdleGame.Models;

namespace IdleGame.Managers
{
    /// <summary>
    /// ゲーム全体を管理するマネージャー
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("設定")]
        [SerializeField] private bool useTestDeviceId = true;
        [SerializeField] private string testDeviceId = "unity-test-device-001";

        private User currentUser;
        private Enemy[] enemies;

        public User CurrentUser => currentUser;
        public Enemy[] Enemies => enemies;

        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeGame();
        }

        /// <summary>
        /// ゲーム初期化
        /// </summary>
        private void InitializeGame()
        {
            string deviceId = useTestDeviceId ? testDeviceId : SystemInfo.deviceUniqueIdentifier;
            
            Debug.Log($"ゲーム初期化開始: DeviceID = {deviceId}");
            
            // ログイン
            APIClient.Instance.Login(
                deviceId,
                OnLoginSuccess,
                OnLoginError
            );
        }

        /// <summary>
        /// ログイン成功
        /// </summary>
        private void OnLoginSuccess(LoginResponse response)
        {
            currentUser = response.user;
            Debug.Log($"ログイン成功: UserID = {currentUser.user_id}, Level = {currentUser.level}");
            
            if (response.is_new_user)
            {
                Debug.Log("新規ユーザー作成完了!");
            }

            // ゲーム状態を取得
            LoadGameState();
        }

        /// <summary>
        /// ログイン失敗
        /// </summary>
        private void OnLoginError(string error)
        {
            Debug.LogError($"ログインエラー: {error}");
        }

        /// <summary>
        /// ゲーム状態読み込み
        /// </summary>
        public void LoadGameState()
        {
            if (currentUser == null)
            {
                Debug.LogError("ユーザーがログインしていません");
                return;
            }

            APIClient.Instance.GetGameState(
                currentUser.user_id,
                OnGameStateLoaded,
                OnGameStateError
            );
        }

        /// <summary>
        /// ゲーム状態読み込み成功
        /// </summary>
        private void OnGameStateLoaded(GameStateResponse response)
        {
            currentUser = response.user;
            enemies = response.enemies;
            
            Debug.Log($"ゲーム状態読み込み完了: Level = {currentUser.level}, 敵数 = {enemies.Length}");
        }

        /// <summary>
        /// ゲーム状態読み込み失敗
        /// </summary>
        private void OnGameStateError(string error)
        {
            Debug.LogError($"ゲーム状態読み込みエラー: {error}");
        }

        /// <summary>
        /// 放置開始
        /// </summary>
        public void StartIdleMode()
        {
            if (currentUser == null) return;

            APIClient.Instance.StartIdle(
                currentUser.user_id,
                response =>
                {
                    currentUser = response.user;
                    Debug.Log($"放置開始: {response.started_at}");
                },
                error => Debug.LogError($"放置開始エラー: {error}")
            );
        }

        /// <summary>
        /// 放置終了
        /// </summary>
        public void FinishIdleMode()
        {
            if (currentUser == null) return;

            APIClient.Instance.FinishIdle(
                currentUser.user_id,
                response =>
                {
                    currentUser = response.user;
                    Debug.Log($"放置終了: 経験値+{response.exp_gained} ({response.idle_minutes}分)");
                },
                error => Debug.LogError($"放置終了エラー: {error}")
            );
        }

        /// <summary>
        /// レベルアップ
        /// </summary>
        public void LevelUp()
        {
            if (currentUser == null) return;

            APIClient.Instance.LevelUp(
                currentUser.user_id,
                response =>
                {
                    currentUser = response.user;
                    if (response.leveled_up)
                    {
                        Debug.Log($"レベルアップ成功! 新レベル: {response.new_level}");
                    }
                    else
                    {
                        Debug.Log("経験値が不足しています");
                    }
                },
                error => Debug.LogError($"レベルアップエラー: {error}")
            );
        }

        /// <summary>
        /// 能力強化
        /// </summary>
        public void Upgrade(UpgradeType upgradeType)
        {
            if (currentUser == null) return;

            APIClient.Instance.Upgrade(
                currentUser.user_id,
                upgradeType,
                response =>
                {
                    currentUser = response.user;
                    Debug.Log($"強化成功: {response.upgrade_type}");
                },
                error => Debug.LogError($"強化エラー: {error}")
            );
        }

        /// <summary>
        /// 進化
        /// </summary>
        public void Evolve()
        {
            if (currentUser == null) return;

            APIClient.Instance.Evolve(
                currentUser.user_id,
                response =>
                {
                    currentUser = response.user;
                    Debug.Log($"進化成功! ステージ: {response.evolution_stage}");
                },
                error => Debug.LogError($"進化エラー: {error}")
            );
        }

        /// <summary>
        /// 攻撃力計算
        /// </summary>
        public float GetAttackPower()
        {
            if (currentUser == null) return 0f;
            
            float baseAttack = 10f;
            float attackMultiplier = 1f + (currentUser.attack_up * 0.1f);
            return baseAttack * attackMultiplier;
        }

        /// <summary>
        /// 攻撃速度計算
        /// </summary>
        public float GetAttackSpeed()
        {
            if (currentUser == null) return 1f;
            
            float baseSpeed = 1f;
            float speedMultiplier = 1f + (currentUser.speed_up * 0.1f);
            return baseSpeed * speedMultiplier;
        }

        /// <summary>
        /// HP回復量計算
        /// </summary>
        public float GetHPRegen()
        {
            if (currentUser == null) return 0f;
            
            float baseRegen = 1f;
            float regenMultiplier = 1f + (currentUser.hp_regen_up * 0.1f);
            return baseRegen * regenMultiplier;
        }
    }
}