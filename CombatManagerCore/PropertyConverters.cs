using System;
using System.Text.RegularExpressions;

namespace CombatManager
{

    static class FootConverter
    {
        public static int? Convert(string value)
        {

            string strVal = (string)value;


            int retVal = 0;

            if (strVal != null)
            {

                Regex regFt = new Regex("(?<num>[0-9]+) +ft\\.");
                Match m = regFt.Match(strVal);


                if (m.Success)
                {
                    retVal = int.Parse(m.Groups["num"].Value);
                }
                else
                {
                    return null;
                }
            }

            return retVal;
        }


        public static string ConvertBack(int? value)
        {
            if (value != null)
            {
                return (int)value + " ft.";
            }
            else
            {
                return "-";
            }
        }
    }

}

