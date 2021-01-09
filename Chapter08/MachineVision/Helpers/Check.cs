using System;

namespace ArgumentValidation
{
    public static class Check
    {
        public static void IsNull(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
        }

        public static void NotLessThan(int actualSize, int minSize)
        {
            if (actualSize <= minSize)
            {
                throw new ArgumentException();
            }
        }

        public static void IsLargerThanZero(int length)
        {
            NotLessThan(length, 0);
        }
    }
}
