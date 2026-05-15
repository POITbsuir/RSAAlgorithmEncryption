namespace ThirdTaskApplication.Models
{
    public class Algorithms
    {
        public Algorithms() { }

        public long[] AlgorithmEuclidex(long a, long b)
        {
            long d0 = a;
            long d1 = b;
            long x0 = 1;
            long x1 = 0;
            long y0 = 0;
            long y1 = 1;
            while (d1 > 1)
            {
                long q = d0 / d1;
                long d2 = d0 % d1;
                long x2 = x0 - q * x1;
                long y2 = y0 - q * y1;
                d0 = d1;
                d1 = d2;
                x0 = x1;
                x1 = x2;
                y0 = y1;
                y1 = y2;

            }
            return new long[] { x1, y1, d1 };
        }

        public long ModPow(long a, long z, long n)
        {
            long result = 1;
            long baseVal = a % n;
            long exponent = z;

            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                {
                    result = (result * baseVal) % n;
                }
                baseVal = (baseVal * baseVal) % n;
                exponent >>= 1;
            }
            return result;
        }
    }
}
