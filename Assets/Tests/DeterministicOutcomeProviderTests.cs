using Data;
using NUnit.Framework;
using Roulette;

namespace JokerTests
{
    [TestFixture, Category("Domain")]
    public class DeterministicOutcomeProviderTests
    {
        [Test]
        public void Returns_random_when_unset()
        {
            var p = new DeterministicOutcomeProvider(seed: 1);
            var (n, deterministic) = p.Roll(RouletteVariant.European);
            Assert.IsFalse(deterministic);
            Assert.GreaterOrEqual(n, 0);
        }

        [Test]
        public void Returns_queued_number_when_set()
        {
            var p = new DeterministicOutcomeProvider();
            p.SetNext(17);
            var (n, deterministic) = p.Roll(RouletteVariant.European);
            Assert.AreEqual(17, n);
            Assert.IsTrue(deterministic);
        }

        [Test]
        public void Override_is_sticky_across_rolls()
        {
            var p = new DeterministicOutcomeProvider();
            p.SetNext(7);
            for (int i = 0; i < 5; i++)
            {
                var (n, deterministic) = p.Roll(RouletteVariant.European);
                Assert.AreEqual(7, n);
                Assert.IsTrue(deterministic);
            }
        }

        [Test]
        public void Clear_returns_to_random()
        {
            var p = new DeterministicOutcomeProvider(seed: 1);
            p.SetNext(7);
            p.Clear();
            var (_, deterministic) = p.Roll(RouletteVariant.European);
            Assert.IsFalse(deterministic);
        }

        [Test]
        public void Random_picks_from_correct_layout_for_variant()
        {
            var p = new DeterministicOutcomeProvider(seed: 42);
            for (int i = 0; i < 200; i++)
            {
                var (n, _) = p.Roll(RouletteVariant.American);
                bool found = false;
                foreach (var v in WheelLayout.American)
                    if (v == n) { found = true; break; }
                Assert.IsTrue(found, $"Roll returned {n}, not in American layout");
            }
        }
    }
}
