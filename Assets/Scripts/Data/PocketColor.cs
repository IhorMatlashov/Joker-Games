using System.Collections.Generic;

namespace Data
{
    public enum PocketColor { Green, Red, Black }

    public static class PocketColors
    {
        private static readonly HashSet<int> Red = new()
        {
            1, 3, 5, 7, 9, 12, 14, 16, 18,
            19, 21, 23, 25, 27, 30, 32, 34, 36
        };

        public static PocketColor ColorOf(int number)
        {
            if (number == 0 || number == RouletteVariantExtensions.DoubleZero)
                return PocketColor.Green;
            return Red.Contains(number) ? PocketColor.Red : PocketColor.Black;
        }

        public static bool IsRed(int n) => ColorOf(n) == PocketColor.Red;
    }
}
