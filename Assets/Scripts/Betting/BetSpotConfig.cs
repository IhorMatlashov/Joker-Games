using System;

namespace Betting
{
    [Serializable]
    public class BetSpotConfig
    {
        public BetCategory category;
        public int a;
        public int b;

        public Bet Build(int amount) => new(category, a, b, amount);
    }
}
