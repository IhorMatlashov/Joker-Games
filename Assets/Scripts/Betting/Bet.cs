using Data;

namespace Betting
{
    public sealed class Bet
    {
        public BetCategory Category { get; }
        public int A { get; }
        public int B { get; }
        public int Amount { get; }

        public Bet(BetCategory category, int a, int b, int amount)
        {
            Category = category;
            A = a;
            B = b;
            Amount = amount;
        }

        public int PayoutMultiplier => Category switch
        {
            BetCategory.Straight => 35,
            BetCategory.Split    => 17,
            BetCategory.Street   => 11,
            BetCategory.Corner   => 8,
            BetCategory.SixLine  => 5,
            BetCategory.Dozen    => 2,
            BetCategory.Column   => 2,
            _                    => 1
        };

        public int CalculatePayout(int n) => IsWinning(n) ? Amount * (PayoutMultiplier + 1) : 0;

        public bool IsWinning(int n) => Category switch
        {
            BetCategory.Straight => n == A,
            BetCategory.Split    => n == A || n == B,
            BetCategory.Street   => n >= A && n <= A + 2,
            BetCategory.Corner   => n == A || n == A + 1 || n == A + 3 || n == A + 4,
            BetCategory.SixLine  => n >= A && n <= A + 5,
            BetCategory.RedBlack => Numeric(n) && PocketColors.IsRed(n) == (A == 1),
            BetCategory.EvenOdd  => Numeric(n) && (n % 2 == 0) == (A == 1),
            BetCategory.HighLow  => Numeric(n) && (A == 1 ? n >= 19 : n <= 18),
            BetCategory.Dozen    => Numeric(n) && n >= 1 + 12 * A && n <= 12 + 12 * A,
            BetCategory.Column   => Numeric(n) && n % 3 == A % 3,
            _                    => false
        };

        private static bool Numeric(int n) => n != 0 && n != RouletteVariantExtensions.DoubleZero;
    }
}
