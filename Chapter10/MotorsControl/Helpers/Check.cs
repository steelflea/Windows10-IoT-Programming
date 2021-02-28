using System;
using System.Collections.Generic;

namespace SenseHat.Helpers
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

        public static void IsLengthInValidRange(int actualSize, int minSize, int maxSize)
        {
            if (actualSize < minSize || actualSize > maxSize)
            {
                throw new ArgumentOutOfRangeException();
            }
        }        

        public static void LengthNotLessThan(int actualSize, int minSize)
        {
            if(actualSize < minSize)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public static void IsPositive<T>(T value) where T: IComparable<T>
        {            
            if(Comparer<T>.Default.Compare(value, default(T)) < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public static void IsLengthEqualTo(int expectedValue, int actualValue)
        {
            if(expectedValue != actualValue)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
