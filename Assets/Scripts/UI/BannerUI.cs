using System.Collections;
using Core;
using TMPro;
using UnityEngine;

namespace UI
{
    public class BannerUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup root;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private float fadeTime = 0.25f;

        private Coroutine _co;

        private void Awake() { if (root != null) root.alpha = 0f; }
        private void OnEnable()  { EventBus.OnUiBanner += Show; }
        private void OnDisable() { EventBus.OnUiBanner -= Show; }

        private void Show(string text, float duration)
        {
            if (label == null || root == null) return;
            label.text = text;
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(Fade(duration));
        }

        private IEnumerator Fade(float visibleSeconds)
        {
            float t = 0f;
            while (t < fadeTime) { t += Time.deltaTime; root.alpha = Mathf.Clamp01(t / fadeTime); yield return null; }
            root.alpha = 1f;
            yield return new WaitForSeconds(Mathf.Max(0.1f, visibleSeconds));
            t = 0f;
            while (t < fadeTime) { t += Time.deltaTime; root.alpha = 1f - Mathf.Clamp01(t / fadeTime); yield return null; }
            root.alpha = 0f;
        }
    }
}
