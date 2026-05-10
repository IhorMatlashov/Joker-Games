using System.Collections;
using Core;
using Data;
using UnityEngine;

namespace VFX
{
    public class WinningPocketHighlight : MonoBehaviour
    {
        [SerializeField] private Light pulseLight;
        [SerializeField] private Renderer pulseRenderer;
        [SerializeField] private Color pulseColor = new(1f, 0.9f, 0.2f, 1f);
        [SerializeField] private float duration = 1.4f;
        [SerializeField] private AnimationCurve pulseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
        private MaterialPropertyBlock _mpb;
        private Coroutine _co;

        private void OnEnable()  { EventBus.OnSpinEnded += OnSpinEnded; }
        private void OnDisable() { EventBus.OnSpinEnded -= OnSpinEnded; }

        private void OnSpinEnded(int _, PocketColor __)
        {
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(Pulse());
        }

        private IEnumerator Pulse()
        {
            float t = 0f;
            if (pulseRenderer != null && _mpb == null) _mpb = new MaterialPropertyBlock();

            while (t < duration)
            {
                t += Time.deltaTime;
                float k = Mathf.PingPong(t * 4f, 1f);
                float intensity = pulseCurve.Evaluate(t / duration) * (0.4f + 0.6f * k);

                if (pulseLight != null) pulseLight.intensity = intensity * 4f;
                if (pulseRenderer != null && _mpb != null)
                {
                    pulseRenderer.GetPropertyBlock(_mpb);
                    _mpb.SetColor(EmissionColorId, pulseColor * intensity * 2f);
                    pulseRenderer.SetPropertyBlock(_mpb);
                }
                yield return null;
            }

            if (pulseLight != null) pulseLight.intensity = 0f;
            if (pulseRenderer != null && _mpb != null)
            {
                pulseRenderer.GetPropertyBlock(_mpb);
                _mpb.SetColor(EmissionColorId, Color.black);
                pulseRenderer.SetPropertyBlock(_mpb);
            }
        }
    }
}
