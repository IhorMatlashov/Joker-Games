using System.Collections;
using Betting;
using Data;
using Menu;
using Player;
using Roulette;
using UI;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Config")]
        [SerializeField] private GameConfigSo config;

        [Header("Scene wiring")]
        [SerializeField] private RouletteSpinner spinner;
        [SerializeField] private RouletteVariantSwitcher variantSwitcher;
        [SerializeField] private ChipSelectorUI chipSelector;

        public GameConfigSo Config => config;
        public ChipSelectorUI ChipSelector => chipSelector;
        public RouletteSpinner Spinner => spinner;

        public PlayerWallet Wallet { get; private set; }
        public PlayerStats Stats { get; private set; }
        public BetManager Bets { get; private set; }
        public RouletteVariant Variant { get; private set; } = RouletteVariant.European;

        private readonly RoundStateMachine _state = new();
        public GameState State => _state.State;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            int starting = config != null ? config.startingBalance : 1000;
            Wallet = new PlayerWallet(starting);
            Stats = new PlayerStats();
            Bets = new BetManager(Wallet);

            Variant = MainMenuController.PickedVariant
                      ?? SaveSystem.ReadVariant(config != null ? config.defaultVariant : RouletteVariant.European);
            SaveSystem.WriteVariant(Variant);

            if (variantSwitcher != null)
            {
                variantSwitcher.Spinner = spinner;
                variantSwitcher.Apply(Variant);
            }
            else if (spinner != null)
            {
                spinner.Variant = Variant;
            }
        }

        private void Start()
        {
            var ctx = new GameContext(config, Wallet, Stats, Bets, Variant, State, chipSelector, spinner);
            var components = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] is IGameInitializable initable)
                    initable.Initialize(ctx);
            }
        }

        public void Spin()
        {
            if (State != GameState.Betting) return;
            if (spinner == null || spinner.IsSpinning) return;
            if (Bets.ActiveBets.Count == 0) return;
            StartCoroutine(RunRound());
        }

        public void ClearAllBets()
        {
            if (State != GameState.Betting) return;
            Bets.ClearAll();
        }

        public void ToggleVariant()
        {
            if (State != GameState.Betting) return;
            SetVariant(Variant == RouletteVariant.European
                ? RouletteVariant.American
                : RouletteVariant.European);
        }

        public void SetVariant(RouletteVariant v)
        {
            if (Variant == v) return;
            Variant = v;
            SaveSystem.WriteVariant(v);
            if (variantSwitcher != null) variantSwitcher.Apply(v);
            else if (spinner != null) spinner.Variant = v;
            EventBus.RaiseVariantChanged(v);
        }

        public void SetDeterministicNext(int? number)
        {
            if (spinner == null) return;
            if (number.HasValue) spinner.SetDeterministicNext(number.Value);
            else                 spinner.ClearDeterministic();
        }

        public void ResetSession()
        {
            Bets.ClearAll();
            Wallet.SetBalance(config != null ? config.startingBalance : 1000);
            Stats.Reset();
        }


        private IEnumerator RunRound()
        {
            _state.TryTransition(GameState.Spinning);

            spinner.Variant = Variant;
            spinner.Spin();
            while (spinner.IsSpinning) yield return null;

            _state.TryTransition(GameState.Resolving);
            var result = Bets.Resolve(spinner.LastWinningNumber, spinner.LastWasDeterministic);
            Stats.Record(result);
            EventBus.RaiseRoundResolved(result);

            float settled = config != null ? config.settledDelay : 0f;
            if (settled > 0f) yield return new WaitForSeconds(settled);

            _state.TryTransition(GameState.Betting);
        }
    }
}
