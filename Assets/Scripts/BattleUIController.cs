using UnityEngine;
using TMPro;
using Abandoned.Battle;

namespace Abandoned.UI
{
    /// <summary>
    /// バトルシーン　UI表示　管理
    /// </summary>
    public class BattleUIController : MonoBehaviour
    {
        [Header("バトル情報表示")] 
        [SerializeField] private TextMeshProUGUI battleLogText;
        [SerializeField] private TextMeshProUGUI enemyCountText;

        private void Update()
        {
            UpdateBattleInfo();
        }

        private void UpdateBattleInfo()
        {
            if (BattleManager.Instance == null) return;

            var enemy = BattleManager.Instance.CurrentEnemy;
            if (enemy != null && enemyCountText != null)
            {
                enemyCountText.text = $"現在の敵: {enemy.name}";
            }
        }
    }
}