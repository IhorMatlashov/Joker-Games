namespace Data
{
    public static class WheelLayout
    {
        public static readonly int[] European =
        {
            0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10,
            5, 24, 16, 33, 1, 20, 14, 31, 9, 22, 18, 29, 7, 28, 12, 35, 3, 26
        };

        public static readonly int[] American =
        {
            0, 28, 9, 26, 30, 11, 7, 20, 32, 17, 5, 22, 34, 15, 3, 24, 36, 13, 1,
            -1, 27, 10, 25, 29, 12, 8, 19, 31, 18, 6, 21, 33, 16, 4, 23, 35, 14, 2
        };

        public static int[] For(RouletteVariant v)
            => v == RouletteVariant.European ? European : American;

        public static float AngleOf(int number, RouletteVariant variant)
        {
            var arr = For(variant);
            int idx = System.Array.IndexOf(arr, number);
            if (idx < 0) idx = 0;
            return idx * (360f / arr.Length);
        }
    }
}
