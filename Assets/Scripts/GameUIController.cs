using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Abandoned.Managers;
using Abandoned.Models;
using Abandoned.Battle;

namespace Abandoned.UI
{
    /// <summary>
    /// ゲームUIコントローラーくん
    /// </summary>
    public class GameUIController : MonoBehaviour
    {
        [Header("ステータス表示")] 
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI expText;
        [SerializeField] private TextMeshProUGUI attackText;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI hpRegainText;
        [SerializeField] private TextMeshProUGUI upgradePointsText;

        [Header("ボタン")] 
        [SerializeField] private Button idleStartButton;
        [SerializeField] private Button idleFinishButton;
        [SerializeField] private Button levelUpButton;
        [SerializeField] private Button upgradeAttackButton;
        [SerializeField] private Button upgradeSpeedButton;
        [SerializeField] private Button upgradeHPRegainButton;
        [SerializeField] private Button evolveButton;

        [Header("通知")] 
        [SerializeField] private TextMeshProUGUI notificationText;
        [SerializeField] private float notificationDuration = 2f;

        private float notificationTimer = 0f;

        private void Start()
        {
            // ボタンイベント設定だわよ
            if (idleStartButton != null)
                idleStartButton.onClick.AddListener(OnIdleStartClicked);

            if (idleStartButton != null)
                idleFinishButton.onClick.AddListener(OnIdleFinishClicked);

            if (levelUpButton != null)
                levelUpButton.onClick.AddListener(OnLevelUpClicked);

            if (upgradeAttackButton != null)
                upgradeAttackButton.onClick.AddListener(() => OnUpgradeClicked(UpgradeType.Attack));

            if (upgradeSpeedButton != null)
                upgradeSpeedButton.onClick.AddListener(() => OnUpgradeClicked(UpgradeType.Speed));

            if (upgradeHPRegainButton != null)
                upgradeHPRegainButton.onClick.AddListener(() => OnUpgradeClicked(UpgradeType.HPRegain));

            if (evolveButton != null)
                evolveButton.onClick.AddListener(OnEvolveClicked);

            // 通知テキストを非表示にしちゃうよ
            if (notificationText != null)
                notificationText.gameObject.SetActive(false);
        }

        private void Update()
        {
            UpdateUI();
            UpdateNotification();
        }

        /// <summary>
        /// UI更新さん
        /// </summary>
        private void UpdateUI()
        {
            var user = GameManager.Instance.CurrentUser;
            if (user == null) return;

            //ステータス表示のところだわさ
            if (levelText != null)
                levelText.text = $"Lv.{user.level}";

            if (expText != null)
            {
                long requiredExp = user.level * 100;
                expText.text = $"Exp: {user.exp} / {requiredExp}";
            }

            if (attackText != null)
                attackText.text = $"攻撃: {GameManager.Instance.GetAttackPower():F1} (+{user.attack_up})";

            if (attackText != null)
                attackText.text = $"攻撃速度: {GameManager.Instance.GetAttackSpeed():F2}x (+{user.speed_up})";

            if (attackText != null)
                attackText.text = $"体力: {GameManager.Instance.GetHPRegain():F3}/s (+{user.hp_regain_up})";

            if (upgradePointsText != null)
            {
                int maxUpgrades = CalculateMaxUpgrades(user.level);
                int currentUpgrades = user.attack_up + user.speed_up + user.hp_regain_up;
                int availablePoints = maxUpgrades - currentUpgrades;
                upgradePointsText.text = $"強化ポイント: {availablePoints}";
            }

            // ボタンの有効・無効
            if (idleStartButton != null)
                idleStartButton.interactable = !user.is_idle;

            if (idleFinishButton != null)
                idleFinishButton.interactable = user.is_idle;

            // レベルアップボタン
            if (levelUpButton != null)
            {
                long requiredExp = user.level * 100;
                levelUpButton.interactable = user.exp >= requiredExp;
            }

            // 強化ボタン
            int maxUpgradesForButtons = CalculateMaxUpgrades(user.level);
            int currentUpgradesForButtons = user.attack_up + user.speed_up + user.hp_regain_up;
            bool canUpgrade = currentUpgradesForButtons < maxUpgradesForButtons;

            if (upgradeAttackButton != null)
                upgradeAttackButton.interactable = canUpgrade;
            if (upgradeSpeedButton != null)
                upgradeSpeedButton.interactable = canUpgrade;
            if (upgradeHPRegainButton != null)
                upgradeHPRegainButton.interactable = canUpgrade;

            // 進化ボタン
            if (evolveButton != null)
            {
                int requiredLevel = (user.evolution_stage + 1) * 10;
                evolveButton.interactable = user.level >= requiredLevel;
            }
        }

        /// <summary>
        /// 最大強化回数を計算してるよ🐎
        /// </summary>
        private int CalculateMaxUpgrades(int level)
        {
            int count = 0;
            for (int i = 1; i <= level; i++)
            {
                if (i % 10 == 0 || i % 5 == 0)
                    count += 2;
                else
                    count += 1;
            }

            return count;
        }

        /// <summary>
        /// 通知更新してるよ
        /// </summary>
        private void UpdateNotification()
        {
            if (notificationTimer > 0f)
            {
                notificationTimer -= Time.deltaTime;
                if (notificationTimer <= 0f && notificationText != null)
                {
                    notificationText.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 通知を表示してるよ
        /// </summary>
        private void ShowNotification(string message)
        {
            if (notificationText != null)
            {
                notificationText.text = message;
                notificationText.gameObject.SetActive(true);
                notificationTimer = notificationDuration;
            }

            Debug.Log($"[通知] {message}");
        }

        /// <summary>
        /// 放置開始ボタンくん
        /// </summary>
        private void OnIdleStartClicked()
        {
            GameManager.Instance.StartIdleMode();
            ShowNotification("放置開始");

            // バトルを停止するよ
            // if (BattleManager.Instance != null)
            //     BattleManager.Instance.StopBattle();
        }

        /// <summary>
        /// 放置終了ぼたんさん
        /// </summary>
        private void OnIdleFinishClicked()
        {
            GameManager.Instance.FinishIdleMode();
            ShowNotification("再開");

            // バトルを再開しようね
            // if (BattleManager.Instance != null)
            //     BattleManager.Instance.StartBattle();
        }

        /// <summary>
        /// レベルアップボタンね
        /// </summary>
        private void OnLevelUpClicked()
        {
            var user = GameManager.Instance.CurrentUser;
            long requiredExp = user.level * 100;

            if (user.exp >= requiredExp)
            {
                GameManager.Instance.LevelUp();
                ShowNotification($"レベルアップ Lv.{user.level + 1}");

                // バトルマネージャーのプレイヤーステータス更新
                if (BattleManager.Instance != null)
                    BattleManager.Instance.UpdatePlayerStats();
            }
            else
            {
                ShowNotification("経験値が不足");
            }
        }

        /// <summary>
        /// 強化ボタン
        /// </summary>
        private void OnUpgradeClicked(UpgradeType upgradeType)
        {
            GameManager.Instance.Upgrade(upgradeType);

            string typeName = upgradeType switch
            {
                UpgradeType.Attack => "攻撃力",
                UpgradeType.Speed => "攻撃速度",
                UpgradeType.HPRegain => "HP回復",
                _ => "能力"
            };

            ShowNotification($"{typeName}を強化");
        }

        /// <summary>
        /// 進化ボタン
        /// </summary>
        private void OnEvolveClicked()
        {
            var user = GameManager.Instance.CurrentUser;
            int requiredLevel = (user.evolution_stage + 1) * 10;

            if (user.level >= requiredLevel)
            {
                GameManager.Instance.Evolve();
                ShowNotification($"進化成功 ステージ{user.evolution_stage + 1}");
            }
            else
            {
                ShowNotification($"進化にはLv.{requiredLevel}が必要");
            }
        }
    }
}