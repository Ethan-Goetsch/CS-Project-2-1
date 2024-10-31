namespace FPSSystem.Utils
{
    public static class NumberExtensions
    {
        public static float Normalize(this float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        public static int Normalize(this int value, int min, int max)
        {
            return (value - min) / (max - min);
        }
    }
}
