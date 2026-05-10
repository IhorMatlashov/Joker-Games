using System.Collections.Generic;
using Core;
using Data;
using Player;

namespace Betting
{
    public sealed class BetManager
    {
        private readonly PlayerWallet _wallet;
        private readonly List<Bet> _bets = new();
        private readonly List<Bet> _winners = new(8);
        private readonly List<Bet> _losers = new(8);

        public BetManager(PlayerWallet wallet) => _wallet = wallet;

        public IReadOnlyList<Bet> ActiveBets => _bets;

        public int TotalWagered
        {
            get
            {
                int sum = 0;
                for (int i = 0; i < _bets.Count; i++) sum += _bets[i].Amount;
                return sum;
            }
        }

        public bool TryPlaceBet(Bet bet)
        {
            if (bet == null || bet.Amount <= 0) return false;
            if (!_wallet.TryDebit(bet.Amount)) return false;
            _bets.Add(bet);
            EventBus.RaiseBetPlaced(bet);
            return true;
        }

        public bool RemoveBet(Bet bet)
        {
            if (!_bets.Remove(bet)) return false;
            _wallet.Credit(bet.Amount);
            EventBus.RaiseBetRemoved(bet);
            return true;
        }

        public void ClearAll()
        {
            for (int i = 0; i < _bets.Count; i++) _wallet.Credit(_bets[i].Amount);
            _bets.Clear();
            EventBus.RaiseBetsCleared();
        }

        public SpinResult Resolve(int winningNumber, bool wasDeterministic)
        {
            int totalWager = TotalWagered;
            int totalPayout = 0;

            _winners.Clear();
            _losers.Clear();

            for (int i = 0; i < _bets.Count; i++)
            {
                var b = _bets[i];
                int payout = b.CalculatePayout(winningNumber);
                if (payout > 0) { _winners.Add(b); totalPayout += payout; }
                else            { _losers.Add(b); }
            }

            if (totalPayout > 0) _wallet.Credit(totalPayout);

            var result = new SpinResult(
                winningNumber,
                PocketColors.ColorOf(winningNumber),
                totalWager,
                totalPayout,
                wasDeterministic,
                _winners,
                _losers);

            _bets.Clear();
            return result;
        }
    }
}
