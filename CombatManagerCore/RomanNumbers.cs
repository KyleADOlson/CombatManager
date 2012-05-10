/*
 *  RomanNumbers.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ScottsUtils
{
    public static class RomanNumbers
    {
        enum RomanDigit
        {
            I = 1,
            V = 5,
            X = 10,
            L = 50,
            C = 100,
            D = 500,
            M = 1000
        }

        public static string FindAndReplace(string value)
        {
            return Regex.Replace(value, "\\b([IVXLCDMN]+)\\b", delegate(Match match)
            {
                try
                {
                    return RomanToNumber(match.Groups[1].Value).ToString("0000");
                }
                catch
                {
                    return match.Groups[1].Value;
                }
            });
        }

        public static int RomanToNumber(string roman)
        {
            roman = roman.ToUpper().Trim();

            if (roman == "N")
            {
                return 0;
            }

            if (roman.Split('V').Length > 2 ||
                roman.Split('L').Length > 2 ||
                roman.Split('D').Length > 2)
            {
                throw new ArgumentException("V, L, and D can only appear once in a number");
            }

            int count = 1;
            char last = 'Z';
            foreach (char numeral in roman)
            {
                if ("IVXLCDM".IndexOf(numeral) == -1)
                {
                    throw new ArgumentException("Invalid numeral");
                }

                if (numeral == last)
                {
                    count++;
                    if (count == 5)
                    {
                        throw new ArgumentException("A digit can only be repeated 4 times");
                    }
                }
                else
                {
                    count = 1;
                    last = numeral;
                }
            }

            int ptr = 0;
            List<int> values = new List<int>();
            int maxDigit = 1000;
            while (ptr < roman.Length)
            {
                char numeral = roman[ptr];
                int digit = (int)Enum.Parse(typeof(RomanDigit), numeral.ToString());

                if (digit > maxDigit)
                {
                    throw new ArgumentException("Invalid subtraction construct");
                }

                int nextDigit = 0;
                if (ptr < roman.Length - 1)
                {
                    char nextNumeral = roman[ptr + 1];
                    nextDigit = (int)Enum.Parse(typeof(RomanDigit), nextNumeral.ToString());

                    if (nextDigit > digit)
                    {
                        if ("IXC".IndexOf(numeral) == -1 ||
                            nextDigit > (digit * 10) ||
                            roman.Split(numeral).Length > 3)
                        {
                            throw new ArgumentException("Invalid subtraction construct");
                        }

                        maxDigit = digit - 1;
                        digit = nextDigit - digit;
                        ptr++;
                    }
                }

                values.Add(digit);

                ptr++;
            }

            for (int i = 0; i < values.Count - 1; i++)
            {
                if ((int)values[i] < (int)values[i + 1])
                {
                    throw new ArgumentException("Invalid subtraction construct");
                }
            }

            int total = 0;
            foreach (int digit in values)
            {
                total += digit;
            }

            return total;
        }

        public static string NumberToRoman(int number)
        {
            if (number < 0 || number > 3999)
            {
                throw new ArgumentException("Value must be in the range 0 - 3999");
            }

            if (number == 0)
            {
                return "N";
            }

            int[] values = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            string[] numerals = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < 13; i++)
            {
                while (number >= values[i])
                {
                    number -= values[i];
                    result.Append(numerals[i]);
                }
            }

            return result.ToString();
        }
    }
}
