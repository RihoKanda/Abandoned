using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IdleGame.Battle
{
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
        private float maxHp;
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

        public void SetCharacterData(string name, float hp, float maxHp)
        {
            characterName = name;
            currentHp = hp;
            maxHp = maxHp;
            UpdateDisplay();
        }

        public void UpdateHp(float hp)
        {
            float previousHP = currentHp;
            currentHp = hp;
            UpdateDisplay();

            if (hp < previousHP)
            {
                PlayDamageAnimation();
            }
        }

        private void UpdateDisplay()
        {
            if (nameText != null)
            {
                nameText.text = characterName;
            }

            if (hpSlider != null)
            {
                hpSlider.maxValue = maxHp;
                hpSlider.value = currentHp;
            }

            if (hpText != null)
            {
                hpText.text = $"{Mathf.CeilToInt(currentHp)} / {Mathf.CeilToInt(maxHp)}";
            }
        }

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

            while (elapsed < attackAnimationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (attackAnimationDuration / 2);
                characterImage.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, t);
                yield return null;
            }
            
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

            while (elapsed < damageAnimationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (damageAnimationDuration / 2);
                characterImage.color = Color.Lerp(damageColor, originalColor, t);
                yield return null;
            }
            elapsed = 0f;
            while (elapsed < damageAnimationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (damageAnimationDuration / 2);
                characterImage.color = Color.Lerp(damageColor, originalColor, t);
                yield return null;
            }
        }

        public void SetCharacterSprite(Sprite sprite)
        {
            if (characterImage != null && sprite != null)
            {
                characterImage.sprite = sprite;
            }
        }
    }
}