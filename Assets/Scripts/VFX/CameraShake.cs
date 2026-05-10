using System.Collections;
using Core;
using Data;
using UnityEngine;

namespace VFX
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private float duration = 0.45f;
        [SerializeField] private float magnitude = 0.05f;
        [SerializeField] private int shakeThreshold = 200;

        private Vector3 _origin;
        private Coroutine _co;

        private void Awake() => _origin = transform.localPosition;
        private void OnEnable()  => EventBus.OnRoundResolved += OnResolved;
        private void OnDisable() => EventBus.OnRoundResolved -= OnResolved;

        private void OnResolved(SpinResult r)
        {
            if (r.NetProfit < shakeThreshold) return;
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(Shake());
        }

        private IEnumerator Shake()
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                var rand = Random.insideUnitSphere * magnitude;
                rand.z = 0f;
                transform.localPosition = _origin + rand;
                yield return null;
            }
            transform.localPosition = _origin;
            _co = null;
        }
    }
}
