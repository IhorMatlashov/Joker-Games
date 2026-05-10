using Betting;
using Data;
using Player;
using Roulette;
using UI;

namespace Core
{
    public sealed class GameContext
    {
        public GameConfigSo Config { get; }
        public PlayerWallet Wallet { get; }
        public PlayerStats Stats { get; }
        public BetManager Bets { get; }
        public RouletteVariant Variant { get; }
        public GameState State { get; }
        public ChipSelectorUI ChipSelector { get; }
        public RouletteSpinner Spinner { get; }

        public GameContext(GameConfigSo cfg, PlayerWallet w, PlayerStats s,
            BetManager b, RouletteVariant v, GameState gs,
            ChipSelectorUI cs, RouletteSpinner sp)
        {
            Config = cfg;
            Wallet = w;
            Stats = s;
            Bets = b;
            Variant = v;
            State = gs;
            ChipSelector = cs;
            Spinner = sp;
        }
    }
}
