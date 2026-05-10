using NUnit.Framework;
using Player;
using Core;

namespace JokerTests
{
    [TestFixture, Category("Domain"), Category("Persistence")]
    public class PlayerWalletTests
    {
        [SetUp]
        public void Setup() => SaveSystem.WipeAll();

        [TearDown]
        public void Teardown() => SaveSystem.WipeAll();

        [Test]
        public void Seeds_with_starting_balance_on_first_launch()
        {
            var w = new PlayerWallet(1000);
            Assert.AreEqual(1000, w.Balance);
        }

        [Test]
        public void Debit_succeeds_when_funds_available()
        {
            var w = new PlayerWallet(1000);
            Assert.IsTrue(w.TryDebit(250));
            Assert.AreEqual(750, w.Balance);
        }

        [Test]
        public void Debit_rejects_when_insufficient()
        {
            var w = new PlayerWallet(100);
            Assert.IsFalse(w.TryDebit(101));
            Assert.AreEqual(100, w.Balance);
        }

        [Test]
        public void Debit_rejects_zero_or_negative()
        {
            var w = new PlayerWallet(100);
            Assert.IsFalse(w.TryDebit(0));
            Assert.IsFalse(w.TryDebit(-10));
            Assert.AreEqual(100, w.Balance);
        }

        [Test]
        public void Credit_adds_positive_amount()
        {
            var w = new PlayerWallet(100);
            w.Credit(50);
            Assert.AreEqual(150, w.Balance);
        }

        [Test]
        public void Credit_ignores_non_positive()
        {
            var w = new PlayerWallet(100);
            w.Credit(0);
            w.Credit(-5);
            Assert.AreEqual(100, w.Balance);
        }

        [Test]
        public void SetBalance_clamps_negatives_to_zero()
        {
            var w = new PlayerWallet(100);
            w.SetBalance(-50);
            Assert.AreEqual(0, w.Balance);
        }

        [Test]
        public void Persists_balance_across_instances()
        {
            new PlayerWallet(1000).TryDebit(300);
            var wallet2 = new PlayerWallet(1000);
            Assert.AreEqual(700, wallet2.Balance);
        }
    }
}
