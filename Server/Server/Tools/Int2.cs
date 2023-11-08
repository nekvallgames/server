using System;

namespace Plugin.Tools
{
    public struct Int2
    {
        public int x;
        public int y;

        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static float Distance(Int2 a, Int2 b)
        {
            int num1 = a.x - b.x;
            int num2 = a.y - b.y;
            return (float)Math.Sqrt((double)num1 * (double)num1 + (double)num2 * (double)num2);
        }
    }
}
