using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeeSharpSoft
{
    public static class Score
    {
        public static class Convert
        {
            public static bool IsEmpty(object o)
            {
                return o == null || o is DBNull || o.ToString() == "";
            }

            public static String ToString(object o)
            {
                if (IsEmpty(o)) return "";

                return o.ToString();
            }

            public static Boolean ToBoolean(object o)
            {
                try
                {
                    return System.Convert.ToBoolean(o);
                }
                catch
                {

                }
                return false;
            }

            public static Byte ToByte(object o)
            {
                try
                {
                    return System.Convert.ToByte(o);
                }
                catch
                {

                }
                return Byte.MinValue;
            }

            public static Byte[] ToByteArray(object o)
            {
                if (o is Byte[]) return (Byte[])o;

                try
                {
                    return Encoding.ASCII.GetBytes(ToCharArray(o));
                }
                catch
                {

                }
                return new Byte[0];
            }

            public static Char ToChar(object o)
            {
                try
                {
                    return System.Convert.ToChar(o);
                }
                catch
                {

                }
                return Char.MinValue;
            }

            public static Char[] ToCharArray(object o)
            {
                if (o is Char[]) return (Char[])o;

                try
                {
                    return ToString(o).ToCharArray();
                }
                catch
                {

                }
                return new Char[0];
            }

            public static DateTime ToDateTime(object o)
            {
                try
                {
                    return System.Convert.ToDateTime(o);
                }
                catch
                {

                }
                return DateTime.MinValue;
            }

            public static Decimal ToDecimal(object o)
            {
                try
                {
                    return System.Convert.ToDecimal(o);
                }
                catch
                {

                }
                return Decimal.MinValue;
            }

            public static Double ToDouble(object o)
            {
                try
                {
                    return System.Convert.ToDouble(o);
                }
                catch
                {

                }
                return Double.MinValue;
            }

            public static float ToFloat(object o)
            {
                try
                {
                    return (float)System.Convert.ToDouble(o);
                }
                catch
                {

                }
                return float.MinValue;
            }

            public static Int16 ToInt16(object o)
            {
                try
                {
                    return System.Convert.ToInt16(o);
                }
                catch
                {

                }
                return Int16.MinValue;
            }

            public static Int32 ToInt32(object o)
            {
                try
                {
                    return System.Convert.ToInt32(o);
                }
                catch
                {

                }
                return Int32.MinValue;
            }

            public static Int64 ToInt64(object o)
            {
                try
                {
                    return System.Convert.ToInt64(o);
                }
                catch
                {

                }
                return Int64.MinValue;
            }
        }

        public static class Math
        {
            
        }
    }
}