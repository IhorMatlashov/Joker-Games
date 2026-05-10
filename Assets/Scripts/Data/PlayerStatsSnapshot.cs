namespace Data
{
    public readonly struct PlayerStatsSnapshot
    {
        public readonly int SpinsPlayed;
        public readonly int RoundsWon;
        public readonly int RoundsLost;
        public readonly int TotalWagered;
        public readonly int TotalReturned;
        public readonly int BiggestWin;
        public readonly int BiggestLoss;
        public readonly int CurrentStreak;

        public PlayerStatsSnapshot(int spinsPlayed, int roundsWon, int roundsLost,
            int totalWagered, int totalReturned, int biggestWin, int biggestLoss, int currentStreak)
        {
            SpinsPlayed = spinsPlayed;
            RoundsWon = roundsWon;
            RoundsLost = roundsLost;
            TotalWagered = totalWagered;
            TotalReturned = totalReturned;
            BiggestWin = biggestWin;
            BiggestLoss = biggestLoss;
            CurrentStreak = currentStreak;
        }

        public int NetProfit => TotalReturned - TotalWagered;
        public float WinRate => SpinsPlayed == 0 ? 0f : (float)RoundsWon / SpinsPlayed;
    }
}
