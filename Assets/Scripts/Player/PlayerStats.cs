using Core;
using Data;

namespace Player
{
    public sealed class PlayerStats
    {
        private int _spins, _won, _lost, _wagered, _returned;
        private int _biggestWin, _biggestLoss, _streak;

        public PlayerStats()
        {
            var s = SaveSystem.ReadStats();
            _spins = s.SpinsPlayed;
            _won = s.RoundsWon;
            _lost = s.RoundsLost;
            _wagered = s.TotalWagered;
            _returned = s.TotalReturned;
            _biggestWin = s.BiggestWin;
            _biggestLoss = s.BiggestLoss;
            _streak = s.CurrentStreak;
        }

        public void Record(SpinResult result)
        {
            _spins++;
            _wagered += result.TotalWagered;
            _returned += result.TotalPayout;

            int net = result.NetProfit;
            if (result.PlayerWon)
            {
                _won++;
                if (net > _biggestWin) _biggestWin = net;
                _streak = _streak < 0 ? 1 : _streak + 1;
            }
            else if (result.TotalWagered > 0)
            {
                _lost++;
                int loss = -net;
                if (loss > _biggestLoss) _biggestLoss = loss;
                _streak = _streak > 0 ? -1 : _streak - 1;
            }

            Persist();
            EventBus.RaiseStatsChanged(Snapshot());
        }

        public PlayerStatsSnapshot Snapshot()
            => new(_spins, _won, _lost, _wagered, _returned, _biggestWin, _biggestLoss, _streak);

        public void Reset()
        {
            _spins = _won = _lost = _wagered = _returned = 0;
            _biggestWin = _biggestLoss = _streak = 0;
            Persist();
            EventBus.RaiseStatsChanged(Snapshot());
        }

        public static PlayerStatsSnapshot LoadSnapshot() => SaveSystem.ReadStats();

        private void Persist()
            => SaveSystem.WriteStats(_spins, _won, _lost, _wagered, _returned,
                                     _biggestWin, _biggestLoss, _streak);
    }
}
