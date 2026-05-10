using UnityEngine;

namespace Roulette
{
    [CreateAssetMenu(fileName = "SpinProfile_", menuName = "Joker/Spin Animation Profile")]
    public class SpinAnimationProfileSo : ScriptableObject
    {
        public string profileName = "Default";

        [Min(0.1f)] public float duration = 4f;
        public float totalSweepDegrees = -1440f;

        public AnimationCurve sweepCurve  = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public AnimationCurve radiusCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public AnimationCurve heightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public float[] fretHitTimes;
    }
}
