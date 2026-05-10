using Betting;
using Data;
using NUnit.Framework;

namespace JokerTests
{
    [TestFixture, Category("Domain")]
    public class BetTests
    {

        [Test] public void Straight_pays_35_to_1()  => Assert.AreEqual(35, new Bet(BetCategory.Straight, 17, 0, 1).PayoutMultiplier);
        [Test] public void Split_pays_17_to_1()     => Assert.AreEqual(17, new Bet(BetCategory.Split, 1, 2, 1).PayoutMultiplier);
        [Test] public void Street_pays_11_to_1()    => Assert.AreEqual(11, new Bet(BetCategory.Street, 1, 0, 1).PayoutMultiplier);
        [Test] public void Corner_pays_8_to_1()     => Assert.AreEqual(8,  new Bet(BetCategory.Corner, 1, 0, 1).PayoutMultiplier);
        [Test] public void SixLine_pays_5_to_1()    => Assert.AreEqual(5,  new Bet(BetCategory.SixLine, 1, 0, 1).PayoutMultiplier);
        [Test] public void Dozen_pays_2_to_1()      => Assert.AreEqual(2,  new Bet(BetCategory.Dozen, 0, 0, 1).PayoutMultiplier);
        [Test] public void Column_pays_2_to_1()     => Assert.AreEqual(2,  new Bet(BetCategory.Column, 1, 0, 1).PayoutMultiplier);
        [Test] public void RedBlack_pays_1_to_1()   => Assert.AreEqual(1,  new Bet(BetCategory.RedBlack, 1, 0, 1).PayoutMultiplier);
        [Test] public void EvenOdd_pays_1_to_1()    => Assert.AreEqual(1,  new Bet(BetCategory.EvenOdd, 1, 0, 1).PayoutMultiplier);
        [Test] public void HighLow_pays_1_to_1()    => Assert.AreEqual(1,  new Bet(BetCategory.HighLow, 0, 0, 1).PayoutMultiplier);


        [Test]
        public void Straight_returns_amount_times_36_when_winning()
        {
            var bet = new Bet(BetCategory.Straight, 17, 0, 10);
            Assert.AreEqual(360, bet.CalculatePayout(17));
        }

        [Test]
        public void Straight_pays_zero_on_loss()
        {
            var bet = new Bet(BetCategory.Straight, 17, 0, 10);
            Assert.AreEqual(0, bet.CalculatePayout(18));
        }


        [Test]
        public void Red_wins_on_red_pocket_18()
        {
            var bet = new Bet(BetCategory.RedBlack, 1, 0, 10);
            Assert.IsTrue(bet.IsWinning(18));
        }

        [Test]
        public void Red_loses_on_black_pocket_17()
        {
            var bet = new Bet(BetCategory.RedBlack, 1, 0, 10);
            Assert.IsFalse(bet.IsWinning(17));
        }

        [Test]
        public void Red_loses_on_zero()
        {
            var bet = new Bet(BetCategory.RedBlack, 1, 0, 10);
            Assert.IsFalse(bet.IsWinning(0));
        }

        [Test]
        public void Even_wins_on_18_loses_on_17_and_0()
        {
            var bet = new Bet(BetCategory.EvenOdd, 1, 0, 10);
            Assert.IsTrue(bet.IsWinning(18));
            Assert.IsFalse(bet.IsWinning(17));
            Assert.IsFalse(bet.IsWinning(0));
        }

        [Test]
        public void High_wins_on_19_loses_on_18()
        {
            var bet = new Bet(BetCategory.HighLow, 1, 0, 10);
            Assert.IsTrue(bet.IsWinning(19));
            Assert.IsTrue(bet.IsWinning(36));
            Assert.IsFalse(bet.IsWinning(18));
        }


        [Test]
        public void FirstDozen_wins_on_1_through_12()
        {
            var bet = new Bet(BetCategory.Dozen, 0, 0, 10);
            Assert.IsTrue(bet.IsWinning(1));
            Assert.IsTrue(bet.IsWinning(12));
            Assert.IsFalse(bet.IsWinning(13));
            Assert.IsFalse(bet.IsWinning(0));
        }

        [Test]
        public void Street_wins_on_three_consecutive_numbers()
        {
            var bet = new Bet(BetCategory.Street, 4, 0, 10);
            Assert.IsTrue(bet.IsWinning(4));
            Assert.IsTrue(bet.IsWinning(5));
            Assert.IsTrue(bet.IsWinning(6));
            Assert.IsFalse(bet.IsWinning(7));
        }

        [Test]
        public void Corner_wins_on_four_neighbouring_numbers()
        {
            var bet = new Bet(BetCategory.Corner, 1, 0, 10);
            Assert.IsTrue(bet.IsWinning(1));
            Assert.IsTrue(bet.IsWinning(2));
            Assert.IsTrue(bet.IsWinning(4));
            Assert.IsTrue(bet.IsWinning(5));
            Assert.IsFalse(bet.IsWinning(3));
        }

        [Test]
        public void Split_wins_on_either_number()
        {
            var bet = new Bet(BetCategory.Split, 1, 2, 10);
            Assert.IsTrue(bet.IsWinning(1));
            Assert.IsTrue(bet.IsWinning(2));
            Assert.IsFalse(bet.IsWinning(3));
        }
    }
}
