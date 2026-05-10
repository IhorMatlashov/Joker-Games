using System;
using Data;

namespace Roulette
{
    public sealed class DeterministicOutcomeProvider
    {
        private readonly Random _rng;
        private int? _next;

        public DeterministicOutcomeProvider(int? seed = null)
        {
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public void SetNext(int number) => _next = number;
        public void Clear() => _next = null;

        public (int number, bool deterministic) Roll(RouletteVariant variant)
        {
            if (_next.HasValue) return (_next.Value, true);

            var arr = WheelLayout.For(variant);
            return (arr[_rng.Next(0, arr.Length)], false);
        }
    }
}
