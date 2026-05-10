using Betting;
using Core;
using Data;
using NUnit.Framework;
using Player;

namespace JokerTests
{
    [TestFixture, Category("Domain"), Category("Persistence")]
    public class BetManagerTests
    {
        private PlayerWallet _wallet;
        private BetManager _bets;

        [SetUp]
        public void Setup()
        {
            SaveSystem.WipeAll();
            _wallet = new PlayerWallet(1000);
            _bets = new BetManager(_wallet);
        }

        [TearDown]
        public void Teardown() => SaveSystem.WipeAll();

        [Test]
        public void Place_debits_wallet_and_appends()
        {
            Assert.IsTrue(_bets.TryPlaceBet(new Bet(BetCategory.Straight, 17, 0, 100)));
            Assert.AreEqual(900, _wallet.Balance);
            Assert.AreEqual(1, _bets.ActiveBets.Count);
            Assert.AreEqual(100, _bets.TotalWagered);
        }

        [Test]
        public void Place_rejects_when_insufficient_funds()
        {
            Assert.IsFalse(_bets.TryPlaceBet(new Bet(BetCategory.Straight, 17, 0, 5000)));
            Assert.AreEqual(1000, _wallet.Balance);
            Assert.AreEqual(0, _bets.ActiveBets.Count);
        }

        [Test]
        public void Remove_refunds_and_subtracts()
        {
            var bet = new Bet(BetCategory.Straight, 17, 0, 100);
            _bets.TryPlaceBet(bet);
            Assert.IsTrue(_bets.RemoveBet(bet));
            Assert.AreEqual(1000, _wallet.Balance);
            Assert.AreEqual(0, _bets.ActiveBets.Count);
        }

        [Test]
        public void ClearAll_refunds_every_bet()
        {
            _bets.TryPlaceBet(new Bet(BetCategory.Straight, 1, 0, 100));
            _bets.TryPlaceBet(new Bet(BetCategory.RedBlack, 1, 0, 50));
            _bets.ClearAll();
            Assert.AreEqual(1000, _wallet.Balance);
            Assert.AreEqual(0, _bets.ActiveBets.Count);
        }

        [Test]
        public void Resolve_pays_winning_straight_at_36x()
        {
            _bets.TryPlaceBet(new Bet(BetCategory.Straight, 17, 0, 10));
            var result = _bets.Resolve(17, false);
            Assert.AreEqual(1350, _wallet.Balance);
            Assert.IsTrue(result.PlayerWon);
            Assert.AreEqual(360, result.TotalPayout);
            Assert.AreEqual(10, result.TotalWagered);
            Assert.AreEqual(350, result.NetProfit);
        }

        [Test]
        public void Resolve_pays_nothing_on_loss()
        {
            _bets.TryPlaceBet(new Bet(BetCategory.Straight, 17, 0, 10));
            var result = _bets.Resolve(18, false);
            Assert.AreEqual(990, _wallet.Balance);
            Assert.IsFalse(result.PlayerWon);
            Assert.AreEqual(-10, result.NetProfit);
        }

        [Test]
        public void Resolve_clears_active_bets()
        {
            _bets.TryPlaceBet(new Bet(BetCategory.Straight, 17, 0, 10));
            _bets.Resolve(17, false);
            Assert.AreEqual(0, _bets.ActiveBets.Count);
        }

        [Test]
        public void Mixed_round_resolves_correctly()
        {
            _bets.TryPlaceBet(new Bet(BetCategory.RedBlack, 1, 0, 10));
            _bets.TryPlaceBet(new Bet(BetCategory.RedBlack, 0, 0, 10));
            var result = _bets.Resolve(1, false);
            Assert.AreEqual(1000, _wallet.Balance);
            Assert.AreEqual(20, result.TotalWagered);
            Assert.AreEqual(20, result.TotalPayout);
            Assert.AreEqual(0, result.NetProfit);
        }
    }
}
