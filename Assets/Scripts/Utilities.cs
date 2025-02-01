using System;
using System.Text;

namespace Forest
{
    public static class Utilities
    {
        public static string ToFormatedString(this double number, int zeros = 0)
        {
            StringBuilder result = new StringBuilder();
            if (number - Math.Round(number) == 0)
            {
                long v = (long)number;
                string numStr = v.ToString($"D{(zeros > 0 ? zeros : string.Empty)}");
                int len = numStr.Length;

                for (int i = 0; i < len; i++)
                {
                    if (i > 0 && i % 3 == 0)
                    {
                        result.Insert(0, ' ');
                    }

                    result.Insert(0, numStr[len - 1 - i]);
                }
            }
            else
            {
                result.Append(number);
            }
            return result.ToString();
        }

        public static string GrayZeros(string s)
        {
            string r = "<color=grey>";
            bool b = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (!b && s[i] != '0' && s[i] != ' ')
                {
                    b = true;
                    r += "</color>";
                }
                r += s[i];
            }
            return r;
        }
    }
}
