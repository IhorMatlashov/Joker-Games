using Core;
using Data;
using UnityEngine;

namespace VFX
{
    [RequireComponent(typeof(ParticleSystem))]
    public class WinCelebrationVFX : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particles;

        private void Reset() => particles = GetComponent<ParticleSystem>();

        private void Awake()
        {
            if (particles != null) particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        private void OnEnable()  { EventBus.OnRoundResolved += OnResolved; }
        private void OnDisable() { EventBus.OnRoundResolved -= OnResolved; }

        private void OnResolved(SpinResult r)
        {
            if (!r.PlayerWon || particles == null) return;
            particles.Play(true);
        }
    }
}
