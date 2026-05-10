using System;
using Betting;
using Data;

namespace Core
{
    public static class EventBus
    {
        public static event Action<GameState> OnGameStateChanged;
        public static event Action OnSpinStarted;
        public static event Action<int, PocketColor> OnSpinEnded;
        public static event Action<SpinResult> OnRoundResolved;

        public static event Action<Bet> OnBetPlaced;
        public static event Action<Bet> OnBetRemoved;
        public static event Action OnBetsCleared;

        public static event Action<int, int> OnBalanceChanged;
        public static event Action<PlayerStatsSnapshot> OnStatsChanged;

        public static event Action<RouletteVariant> OnVariantChanged;
        public static event Action<string, float> OnUiBanner;

        public static void RaiseGameStateChanged(GameState s) => OnGameStateChanged?.Invoke(s);
        public static void RaiseSpinStarted() => OnSpinStarted?.Invoke();
        public static void RaiseSpinEnded(int number, PocketColor c) => OnSpinEnded?.Invoke(number, c);
        public static void RaiseRoundResolved(SpinResult r) => OnRoundResolved?.Invoke(r);

        public static void RaiseBetPlaced(Bet b) => OnBetPlaced?.Invoke(b);
        public static void RaiseBetRemoved(Bet b) => OnBetRemoved?.Invoke(b);
        public static void RaiseBetsCleared() => OnBetsCleared?.Invoke();

        public static void RaiseBalanceChanged(int oldB, int newB) => OnBalanceChanged?.Invoke(oldB, newB);
        public static void RaiseStatsChanged(PlayerStatsSnapshot s) => OnStatsChanged?.Invoke(s);

        public static void RaiseVariantChanged(RouletteVariant v) => OnVariantChanged?.Invoke(v);
        public static void RaiseUiBanner(string msg, float dur = 2f) => OnUiBanner?.Invoke(msg, dur);
    }
}
