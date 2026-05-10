using System.Collections;
using Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Roulette
{
    public class RouletteWheel : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("disc")] private Transform discTransform;
        [SerializeField, Min(1)] private int rotations = 6;

        [Header("Calibration")]
        [SerializeField] private float pocket0OffsetDegrees;

        [SerializeField] private bool clockwiseLayout = true;

        private Quaternion _baseRotation;
        private bool _baseCaptured;
        private float _accumulatedY;

        public float OffsetDegrees { get => pocket0OffsetDegrees; set => pocket0OffsetDegrees = value; }
        public bool ClockwiseLayout { get => clockwiseLayout; set => clockwiseLayout = value; }
        public Transform DiscTransform => discTransform != null ? discTransform : transform;

        private void Awake()
        {
            if (discTransform == null) discTransform = transform;
        }

        public IEnumerator SpinTo(int targetNumber, RouletteVariant variant, float duration, AnimationCurve curve)
        {
            if (!_baseCaptured)
            {
                _baseRotation = discTransform.localRotation;
                _baseCaptured = true;
            }

            float layoutAngle = WheelLayout.AngleOf(targetNumber, variant);
            float pocketAngleOnMesh = (clockwiseLayout ? layoutAngle : -layoutAngle) + pocket0OffsetDegrees;

            float startY = _accumulatedY;
            float delta = Mathf.DeltaAngle(startY, -pocketAngleOnMesh);
            float totalSweep = rotations * 360f + delta;

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / duration);
                float eased = curve.Evaluate(k);
                Apply(startY + totalSweep * eased);
                yield return null;
            }
            Apply(startY + totalSweep);
        }

        private void Apply(float yDeg)
        {
            _accumulatedY = yDeg;
            discTransform.localRotation = _baseRotation * Quaternion.AngleAxis(yDeg, Vector3.up);
        }
    }
}
