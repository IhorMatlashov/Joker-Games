using System.Collections;
using UnityEngine;

namespace Roulette
{
    public class RouletteBall : MonoBehaviour
    {
        [SerializeField] private Transform pivot;

        [Header("Start position (preferred)")]
        [SerializeField] private Transform startAnchor;

        [Header("Auto-fit (used if startAnchor is null)")]
        [SerializeField] private bool autoFitOrbitToScene = true;

        [SerializeField] private float startRadiusScale = 1.20f;

        [SerializeField] private float startHeightLift = 0.15f;

        [Header("Manual override (used if autoFit is OFF and no startAnchor)")]
        [SerializeField] private float startRadius = 0.95f;
        [SerializeField] private float endRadius   = 0.74f;
        [SerializeField] private float startHeight = 0.16f;
        [SerializeField] private float endHeight   = 0.05f;

        [Header("Visual rolling")]
        [SerializeField] private float rollSpeedMultiplier = 1f;

        [Header("Cinematic")]
        [SerializeField, Range(0.25f, 1f)] private float slowMoFactor = 0.5f;

        [SerializeField, Range(0f, 0.4f)] private float slowMoTail = 0.10f;

        private bool _restCaptured;
        private Vector3 _lastPlacedPos;

        private void Awake()
        {
            if (pivot == null) pivot = ResolvePivot();
            if (autoFitOrbitToScene) CaptureRest();
        }

        private Transform ResolvePivot()
        {
            for (var t = transform.parent; t != null; t = t.parent)
            {
                for (var i = 0; i < t.childCount; i++)
                {
                    var c = t.GetChild(i);
                    if (c == transform) continue;
                    var n = c.name.ToLowerInvariant();
                    if (n.Contains("pivot") || n.Contains("center") || n.Contains("roulate"))
                        return c;
                }
            }
            return transform.parent != null ? transform.parent : transform;
        }

        private void CaptureRest()
        {
            if (_restCaptured || pivot == null) return;

            var offset = transform.position - pivot.position;
            var horizontal = new Vector2(offset.x, offset.z).magnitude;
            if (horizontal < 0.001f) return;

            endRadius = horizontal;
            endHeight = offset.y;

            if (startAnchor != null)
            {
                var startOffset = startAnchor.position - pivot.position;
                startRadius = new Vector2(startOffset.x, startOffset.z).magnitude;
                startHeight = startOffset.y;
            }
            else
            {
                startRadius = horizontal * Mathf.Max(1.10f, startRadiusScale);
                startHeight = offset.y + horizontal * Mathf.Max(0.05f, startHeightLift);
            }

            _restCaptured = true;
        }

        public IEnumerator OrbitTo(float targetAngleDeg, SpinAnimationProfileSo profile)
        {
            if (pivot == null) pivot = ResolvePivot();
            if (autoFitOrbitToScene) CaptureRest();
            if (pivot == null || profile == null) yield break;

            var startAngle = targetAngleDeg + profile.totalSweepDegrees;

            ApplyPose(startAngle, startRadius, startHeight, snap: true);

            var t = 0f;
            float slowMoEntry = 1f - Mathf.Clamp01(slowMoTail);
            while (t < profile.duration)
            {
                t += Time.deltaTime;
                var k = Mathf.Clamp01(t / profile.duration);

                if (k > slowMoEntry && slowMoFactor < 1f)
                {
                    float local = (k - slowMoEntry) / (1f - slowMoEntry);
                    k = slowMoEntry + local * slowMoFactor * (1f - slowMoEntry);
                }

                var angle  = Mathf.LerpUnclamped(startAngle, targetAngleDeg,
                                                 profile.sweepCurve.Evaluate(k));
                var radius = Mathf.LerpUnclamped(startRadius, endRadius,
                                                 profile.radiusCurve.Evaluate(k));
                var height = Mathf.LerpUnclamped(startHeight, endHeight,
                                                 profile.heightCurve.Evaluate(k));

                ApplyPose(angle, radius, height, snap: false);
                yield return null;
            }
            ApplyPose(targetAngleDeg, endRadius, endHeight, snap: false);
        }

        private void ApplyPose(float angleDeg, float radius, float height, bool snap)
        {
            var rad = angleDeg * Mathf.Deg2Rad;
            var offset = new Vector3(Mathf.Sin(rad) * radius, height, Mathf.Cos(rad) * radius);
            var newPos = pivot.position + offset;

            if (!snap && rollSpeedMultiplier > 0f)
            {
                var travel = newPos - _lastPlacedPos;
                if (travel.sqrMagnitude > 1e-8f)
                {
                    var ballRadiusWorld = Mathf.Max(0.001f, transform.lossyScale.x * 0.5f);
                    var rollAngleDeg = (travel.magnitude / ballRadiusWorld) * Mathf.Rad2Deg
                                       * rollSpeedMultiplier;
                    var rollAxis = Vector3.Cross(Vector3.up, travel.normalized);
                    if (rollAxis.sqrMagnitude > 1e-6f)
                        transform.Rotate(rollAxis, rollAngleDeg, Space.World);
                }
            }

            transform.position = newPos;
            _lastPlacedPos = newPos;
        }
    }
}
