using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Abandoned.Battle
{
    /// <summary>
    /// キャラクター　表示
    /// </summary>
    public class Character : MonoBehaviour
    {
        [Header("UI参照")] [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private Image characterImage;

        [Header("アニメーション設定")] [SerializeField] private float attackAnimationDuration = 0.3f;
        [SerializeField] private float damageAnimationDuration = 0.2f;

        private string characterName;
        private float currentHp;
        private float maxHP;
        private Vector3 originalPosition;
        private Color originalColor;

        private void Awake()
        {
            if (characterImage != null)
            {
                originalPosition = characterImage.transform.localPosition;
                originalColor = characterImage.color;
            }
        }
        
        /// <summary>
        /// キャラクターデータ 設定
        /// </summary>
        public void SetCharacterData(string name, float hp, float maxHp)
        {
            characterName = name;
            currentHp = hp;
            maxHP = maxHp;
            UpdateDisplay();
        }
        
        //ＨＰ　更新
        public void UpdateHP(float hp)
        {
            float previousHP = currentHp;
            currentHp = hp;
            UpdateDisplay();

            //ダメージ受けた時のアニメーション
            if (hp < previousHP)
            {
                PlayDamageAnimation();
            }
        }
        
        /// <summary>
        /// 表示の更新
        /// </summary>
        private void UpdateDisplay()
        {
            if (nameText != null)
            {
                nameText.text = characterName;
            }

            if (hpSlider != null)
            {
                hpSlider.maxValue = maxHP;
                hpSlider.value = currentHp;
            }

            if (hpText != null)
            {
                hpText.text = $"{Mathf.CeilToInt(currentHp)} / {Mathf.CeilToInt(maxHP)}";
            }
        }
        
        /// <summary>
        /// 攻撃アニメーション
        /// </summary>
        public void PlayAttackAnimation()
        {
            if (characterImage != null) return;
            
            StopAllCoroutines();
            StartCoroutine(AttackAnimationCoroutine());
        }

        private System.Collections.IEnumerator AttackAnimationCoroutine()
        {
            float elapsed = 0f;
            Vector3 targetPosition = originalPosition + Vector3.right * 30;
            
            //前に進んでいるよ
            while (elapsed < attackAnimationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (attackAnimationDuration / 2);
                characterImage.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, t);
                yield return null;
            }
            //後ろに下がったよ
            elapsed = 0f;
            while (elapsed < damageAnimationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (damageAnimationDuration / 2);
                characterImage.transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, t);
                yield return null;
            }
            
            characterImage.transform.localPosition = originalPosition;
        }
        
        /// <summary>
        /// ダメージ食らった時のアニメーション
        /// </summary>
        public void PlayDamageAnimation()
        {
            if (characterImage != null) return;
            StopAllCoroutines();
            StartCoroutine(DamageAnimationCoroutine());
        }

        private System.Collections.IEnumerator DamageAnimationCoroutine()
        {
            float elapsed = 0f;
            Color damageColor = new Color(1f, 0.5f, 0.5f, 1f);
            
            //あかくなるよ
            while (elapsed < damageAnimationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (damageAnimationDuration / 2);
                characterImage.color = Color.Lerp(damageColor, originalColor, t);
                yield return null;
            }
            //元に戻りまする
            elapsed = 0f;
            while (elapsed < damageAnimationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (damageAnimationDuration / 2);
                characterImage.color = Color.Lerp(damageColor, originalColor, t);
                yield return null;
            }
        }
        
        /// <summary>
        /// キャラクター画像　設定
        /// </summary>
        /// <param name="sprite"></param>
        public void SetCharacterSprite(Sprite sprite)
        {
            if (characterImage != null && sprite != null)
            {
                characterImage.sprite = sprite;
            }
        }
    }
}