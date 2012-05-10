/*
 *  ValidationRules.cs
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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CombatManager
{
    public class MinMaxRule : ValidationRule
    {
        private int _min;
        private int _max;

        public MinMaxRule()
        {
        }

        public int Min
        {
            get { return _min; }
            set { _min = value; }
        }

        public int Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int age = 0;

            try
            {
                if (((string)value).Length > 0)
                    age = Int32.Parse((String)value);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Illegal characters or " + e.Message);
            }

            if ((age < Min) || (age > Max))
            {
                return new ValidationResult(false,
                    "Please enter a value in the range: " + Min + " - " + Max + ".");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }

    }

    public class PlusTextRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                string text = (string)value;

                if (Regex.Match(text, "([\r\n])|(or)", RegexOptions.IgnoreCase).Success)
                {
                    
                    return new ValidationResult(false, "Illegal characters");
                }


            }
            catch (Exception e)
            {
                return new ValidationResult(false, e.Message);
            }

            
           return new ValidationResult(true, null);
            
        }
    }
}
