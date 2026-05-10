namespace Data
{
    public enum RouletteVariant
    {
        European,
        American
    }

    public static class RouletteVariantExtensions
    {
        public const int DoubleZero = -1;

        public static string DisplayName(int number)
            => number == DoubleZero ? "00" : number.ToString();
    }
}
