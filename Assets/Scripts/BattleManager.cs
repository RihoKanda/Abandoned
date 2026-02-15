using System.Collections;
using UnityEngine;
using Abandoned.Models;
using Abandoned.Managers;

namespace Abandoned.Battle
{
    /// <summary>
    /// バトルシステム管理
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        [Header("バトル設定")] 
        //攻撃間隔（秒）
        [SerializeField] private float attackInterval = 1f;
        // HP回復間隔（秒）
        [SerializeField] private float hpRegainInterval = 1f;

        [Header("キャラクター参照")] 
        [SerializeField] private Character playerCharacter;
        [SerializeField] private Character enemyCharacter;

        private Enemy[] enemies;
        private int currentEnemyIndex = 0;
        private Enemy currentEnemy;
        private float currentEnemyHP;
        private float playerHP = 100f;
        private float playerMaxHP = 100f;

        private bool isBattleActive = false;

        public float PlayerHP => playerHP;
        public float PlayerMaxHP => playerMaxHP;
        public float EnemyHP => currentEnemyHP;
        public float EnemyMax => currentEnemy?.hp ?? 0;
        public Enemy CurrentEnemy => currentEnemy;

        private static BattleManager instance;
        public static BattleManager Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void Start()
        {
            // GameManagerの初期化待ち
            StartCoroutine(InitializeBattle());
        }
        
        /// <summary>
        /// バトル初期化
        /// </summary>
        private IEnumerator InitializeBattle()
        {
            // GameManagerの準備まち
            while (GameManager.Instance == null || GameManager.Instance.Enemies == null)
            {
                yield return new WaitForSeconds(0.5f);
            }

            enemies = GameManager.Instance.Enemies;
            if (enemies != null && enemies.Length > 0)
            {
                LoadEnemy(0);
                StartBattle();
            }
        }
        
        /// <summary>
        ///敵をロード
        /// </summary>
        private void LoadEnemy(int index)
        {
            if (enemies == null || index >= enemies.Length)
            {
                //敵いなくなったらさいしょから（るーぷ）
                index = 0;
            }

            currentEnemyIndex = index;
            currentEnemy = enemies[currentEnemyIndex];
            currentEnemyHP = currentEnemy.hp;

            Debug.Log($"新しい敵登場: {currentEnemy.name} (HP: {currentEnemyHP})");
            
            //キャラクター表示　更新
            if (enemyCharacter != null)
            {
                enemyCharacter.SetCharacterData(currentEnemy.name, currentEnemyHP, currentEnemy.hp);
            }
        }
        
        /// <summary>
        /// バトル開始
        /// </summary>
        public void StartBattle()
        {
            if (isBattleActive) return;

            isBattleActive = true;
            StartCoroutine(BattleLoop());
            StartCoroutine(HPRegainLoop());
            Debug.Log("バトル開始");
        }
        
        /// <summary>
        /// バトル停止
        /// </summary>
        public void StopBattle()
        {
            isBattleActive = false;
            StopAllCoroutines();
            Debug.Log("バトル停止");
        }
        
        /// <summary>
        /// バトルるーぷ（自動攻撃）
        /// </summary>
        private IEnumerator BattleLoop()
        {
            while (isBattleActive)
            {
                // プレイヤーの攻撃
                float playerAttack = GameManager.Instance.GetAttackPower();
                float attackSpeed = GameManager.Instance.GetAttackSpeed();

                currentEnemyHP -= playerAttack;

                Debug.Log($"プレーヤーの攻撃 ダメージ: {playerAttack} → 敵HP: {currentEnemyHP}");
                
                //キャラクター表示　kousinn
                if (enemyCharacter != null)
                {
                    enemyCharacter.UpdateHP(currentEnemyHP);
                }

                if (playerCharacter != null)
                {
                    playerCharacter.PlayAttackAnimation();
                }

                // 敵撃破してるかのチェック
                if (currentEnemyHP <= 0)
                {
                    OnEnemyDefeated();
                    yield return new WaitForSeconds(1f);
                }
                
                //敵の攻撃っっ
                if (currentEnemy != null)
                {
                    playerHP -= currentEnemy.attack;
                    Debug.Log($"敵の攻撃 ダメージ: {currentEnemy.attack} → プレイヤーHP: {playerHP}");

                    if (playerCharacter != null)
                    {
                        playerCharacter.UpdateHP(playerHP);
                    }

                    // プレイヤーがしんでないかのチェック　はいりまああす
                    if (playerHP <= 0)
                    {
                        OnPlayerDeath();
                        yield return new WaitForSeconds(1f);
                    }
                }

                //攻撃速度に応じた間隔
                float interval = attackInterval / attackSpeed;
                yield return new WaitForSeconds(interval);
            }
        }

        /// <summary>
        /// HP自動回復るーぷ
        /// </summary>
        private IEnumerator HPRegainLoop()
        {
            while (isBattleActive)
            {
                yield return new WaitForSeconds(hpRegainInterval);

                float regain = GameManager.Instance.GetHPRegain();
                playerHP = Mathf.Min(playerHP +  regain, playerMaxHP);

                if (playerCharacter != null)
                {
                    playerCharacter.UpdateHP(playerHP);
                }
            }
        }
        
        /// <summary>
        /// 敵撃破ジ
        /// </summary>
        private void OnEnemyDefeated()
        {
            Debug.Log($"{currentEnemy.name}を撃破 経験値+{currentEnemy.exp_reward}");
            
            //経験値取得（ろーかる加算）
            var user = GameManager.Instance.CurrentUser;
            if (user != null)
            {
                user.exp += currentEnemy.exp_reward;
            }
            // 次の敵をろーどしますよーー
            LoadEnemy(currentEnemyIndex + 1);
        }
        
        /// <summary>
        /// プレイヤーが死亡したとき
        /// </summary>
        private void OnPlayerDeath()
        {
            Debug.Log("Death プレイヤーを強化しましょう");
            
            // HP全回復
            playerHP = playerMaxHP;
            if (playerCharacter != null)
            {
                playerCharacter.UpdateHP(playerHP);
            }
            
            //一個前の敵ろーど
            int previousIndex = currentEnemyIndex > 0 ? currentEnemyIndex -  1 : 0;
            LoadEnemy(previousIndex);
        }
        
        /// <summary>
        /// プレイヤーステータス　更新
        /// </summary>
        public void UpdatePlayerStats()
        {
            if (playerCharacter != null)
            {
                var user = GameManager.Instance.CurrentUser;
                playerCharacter.SetCharacterData($"プレイヤー Lv.{user?.level ?? 1}", playerHP, playerMaxHP);
            }
        }
    }
}