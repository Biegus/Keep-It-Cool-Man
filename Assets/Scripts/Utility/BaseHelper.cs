using System;
using System.Globalization;
namespace LetterBattle
{
    public static class BaseHelper
    {
        public static bool Flag<T>(this T val, T other)
            where T : Enum
        {
            return val.HasFlag(other);
        }
        public static void ApplyOr<T>(this ref  T org, T flag, bool value)
            where T:struct,Enum, IConvertible
        {
            if (!value) return;
            int resultValue = org.ToInt32(NumberFormatInfo.CurrentInfo) | flag.ToInt32(NumberFormatInfo.CurrentInfo);
            org= (T)(object)(resultValue);

        }
        
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }
        
    }
}