using Data;
using NUnit.Framework;

namespace JokerTests
{
    [TestFixture, Category("Domain")]
    public class WheelLayoutTests
    {
        [Test]
        public void European_layout_has_37_pockets()
            => Assert.AreEqual(37, WheelLayout.European.Length);

        [Test]
        public void American_layout_has_38_pockets()
            => Assert.AreEqual(38, WheelLayout.American.Length);

        [Test]
        public void European_starts_with_zero()
            => Assert.AreEqual(0, WheelLayout.European[0]);

        [Test]
        public void American_contains_double_zero_sentinel()
        {
            Assert.Contains(RouletteVariantExtensions.DoubleZero, WheelLayout.American);
        }

        [Test]
        public void Pocket_colors_match_standard_layout()
        {
            int[] reds = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
            int[] blacks = { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 };

            foreach (var r in reds) Assert.AreEqual(PocketColor.Red,   PocketColors.ColorOf(r), $"Pocket {r} should be Red");
            foreach (var b in blacks) Assert.AreEqual(PocketColor.Black, PocketColors.ColorOf(b), $"Pocket {b} should be Black");
        }

        [Test]
        public void Zero_is_green()
        {
            Assert.AreEqual(PocketColor.Green, PocketColors.ColorOf(0));
            Assert.AreEqual(PocketColor.Green, PocketColors.ColorOf(RouletteVariantExtensions.DoubleZero));
        }

        [Test]
        public void AngleOf_evenly_distributes_pockets()
        {
            float step = 360f / WheelLayout.European.Length;
            Assert.AreEqual(0f,        WheelLayout.AngleOf(0,  RouletteVariant.European), 0.01f);
            Assert.AreEqual(step,      WheelLayout.AngleOf(32, RouletteVariant.European), 0.01f);
            Assert.AreEqual(step * 2f, WheelLayout.AngleOf(15, RouletteVariant.European), 0.01f);
        }
    }
}
