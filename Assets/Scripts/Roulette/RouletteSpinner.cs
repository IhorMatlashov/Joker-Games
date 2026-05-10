using System.Collections;
using Audio;
using Core;
using Data;
using UnityEngine;

namespace Roulette
{
    public class RouletteSpinner : MonoBehaviour
    {
        [SerializeField] private RouletteWheel wheel;
        [SerializeField] private RouletteBall ball;
        [SerializeField] private GameConfigSo config;

        public RouletteWheel Wheel { get => wheel; set => wheel = value; }
        public RouletteBall Ball { get => ball; set => ball = value; }
        public GameConfigSo Config { get => config; set => config = value; }

        [SerializeField] private SpinAnimationProfileSo[] profiles;

        public bool IsSpinning { get; private set; }
        public int LastWinningNumber { get; private set; }
        public bool LastWasDeterministic { get; private set; }
        public RouletteVariant Variant { get; set; } = RouletteVariant.European;

        private readonly DeterministicOutcomeProvider _provider = new();
        private int _lastProfileIndex = -1;
        private static readonly AnimationCurve DefaultCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public void SetDeterministicNext(int number) => _provider.SetNext(number);
        public void ClearDeterministic() => _provider.Clear();

        public void Spin()
        {
            if (IsSpinning) return;
            StartCoroutine(SpinRoutine());
        }

        private IEnumerator SpinRoutine()
        {
            IsSpinning = true;
            EventBus.RaiseSpinStarted();

            var (number, deterministic) = _provider.Roll(Variant);
            LastWinningNumber = number;
            LastWasDeterministic = deterministic;

            var profile = PickProfile();
            float duration = profile != null
                ? profile.duration
                : (config != null ? config.wheelSpinDuration : 4f);

            Coroutine wheelCo = null;
            if (wheel != null)
            {
                var curve = profile != null ? profile.sweepCurve : DefaultCurve;
                wheelCo = StartCoroutine(wheel.SpinTo(number, Variant, duration, curve));
            }
            else yield return new WaitForSeconds(duration);

            if (profile != null && ball != null)
            {
                StartCoroutine(FireFretEvents(profile));
                yield return StartCoroutine(ball.OrbitTo(0f, profile));
            }
            if (wheelCo != null) yield return wheelCo;

            IsSpinning = false;
            EventBus.RaiseSpinEnded(number, PocketColors.ColorOf(number));
        }

        private SpinAnimationProfileSo PickProfile()
        {
            if (profiles == null || profiles.Length == 0) return null;
            if (profiles.Length == 1) { _lastProfileIndex = 0; return profiles[0]; }

            int offset = Random.Range(1, profiles.Length);
            int next = _lastProfileIndex < 0
                ? Random.Range(0, profiles.Length)
                : (_lastProfileIndex + offset) % profiles.Length;
            _lastProfileIndex = next;
            return profiles[next];
        }

        private IEnumerator FireFretEvents(SpinAnimationProfileSo profile)
        {
            var times = profile.fretHitTimes;
            if (times == null || times.Length == 0) yield break;

            int i = 0;
            float t = 0f;
            while (t < profile.duration && i < times.Length)
            {
                t += Time.deltaTime;
                if (t / profile.duration >= times[i])
                {
                    AudioManager.Instance?.PlayFretClack();
                    i++;
                }
                yield return null;
            }
        }
    }
}
