/*
 *  CMRegexUtilities.cs
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
using System.Text.RegularExpressions;

namespace CombatManager
{
    public static class CMRegexUtilities
    {
        public static bool GroupSuccess(this Match m, string group)
        {
            return m.Groups[group].Success;
        }

        public static bool AnyGroupSuccess(this Match m, IEnumerable<string> groups)
        {
            foreach (string s in groups)
            {
                if (m.GroupSuccess(s))
                {
                    return true;
                }
            }

            return false;
        }

        public static string Value(this Match m, string group)
        {
            string val = null;

            if (m.Groups[group].Success)
            {
                val = m.Groups[group].Value;
            }

            return val;
        }


        public static int? IntValue(this Match m, string name)
        {
            if (m.Groups[name].Success)
            {
                int val;
                if (int.TryParse(m.Groups[name].Value, out val))
                {
                    return val;
                }
            }
            return null;
        }
    }
}
