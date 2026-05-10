using System.Collections;
using Core;
using Data;
using UnityEngine;

namespace VFX
{
    [RequireComponent(typeof(Light))]
    public class PulseLightOnWin : MonoBehaviour
    {
        [SerializeField] private Light pulseLight;
        [SerializeField] private float peakIntensity = 6f;
        [SerializeField] private float duration = 0.6f;
        [SerializeField] private Color winColor = new(1f, 0.85f, 0.2f);

        private float _baseIntensity;
        private Color _baseColor;
        private Coroutine _co;

        private void Reset() => pulseLight = GetComponent<Light>();

        private void Awake()
        {
            _baseIntensity = pulseLight.intensity;
            _baseColor = pulseLight.color;
        }

        private void OnEnable()  => EventBus.OnRoundResolved += OnResolved;
        private void OnDisable() => EventBus.OnRoundResolved -= OnResolved;

        private void OnResolved(SpinResult r)
        {
            if (!r.PlayerWon) return;
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            pulseLight.color = winColor;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float k = t / duration;
                pulseLight.intensity = Mathf.Lerp(peakIntensity, _baseIntensity, k);
                yield return null;
            }
            pulseLight.intensity = _baseIntensity;
            pulseLight.color = _baseColor;
            _co = null;
        }
    }
}
