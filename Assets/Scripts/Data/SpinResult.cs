using System.Collections.Generic;
using Betting;

namespace Data
{
    public readonly struct SpinResult
    {
        public readonly int WinningNumber;
        public readonly PocketColor Color;
        public readonly int TotalWagered;
        public readonly int TotalPayout;
        public readonly bool WasDeterministic;
        public readonly IReadOnlyList<Bet> WinningBets;
        public readonly IReadOnlyList<Bet> LosingBets;

        public SpinResult(int winningNumber, PocketColor color, int totalWagered,
                          int totalPayout, bool wasDeterministic,
                          IReadOnlyList<Bet> winning, IReadOnlyList<Bet> losing)
        {
            WinningNumber = winningNumber;
            Color = color;
            TotalWagered = totalWagered;
            TotalPayout = totalPayout;
            WasDeterministic = wasDeterministic;
            WinningBets = winning;
            LosingBets = losing;
        }

        public int NetProfit => TotalPayout - TotalWagered;
        public bool PlayerWon => TotalPayout > TotalWagered;
        public string DisplayNumber => RouletteVariantExtensions.DisplayName(WinningNumber);
    }
}
