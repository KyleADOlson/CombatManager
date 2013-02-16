/*
 *  SkillValue.cs
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Globalization;
using System.IO;

namespace CombatManager
{

    public class SkillValue : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private String _Name;
        private String _Subtype;
        private int _Mod;



        public SkillValue()
        {

        }

        public SkillValue(string name)
        {

            Regex regName = new Regex("([ \\p{L}]+)( \\(([- \\p{L}]+)\\))?");

            Match match = regName.Match(name);

            if (match.Success)
            {
                Name = match.Groups[1].Value.Trim();

                if (Name == "Handle Animals")
                {
                    Name = "Handle Animal";
                }

                if (match.Groups[2].Success)
                {
                    Subtype = match.Groups[3].Value;
                }
            }
            else
            {
                Name = name;
            }

        }

        public SkillValue(string name, string subtype)
        {
            Name = name;
            Subtype = subtype;
        }

        public object Clone()
        {
            SkillValue m = new SkillValue();

            m.Name = Name;
            m.Subtype = Subtype;
            m.Mod = Mod;

            return m;
        }


        public String Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Name")); }
                }
            }
        }
        public String Subtype
        {
            get { return _Subtype; }
            set
            {
                if (_Subtype != value)
                {
                    _Subtype = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Subtype")); }
                }
            }
        }
        public int Mod
        {
            get { return _Mod; }
            set
            {
                if (_Mod != value)
                {
                    _Mod = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Mod")); }
                }
            }
        }

        [XmlIgnore]
        public string Text
        {
            get
            {
                string text = Name;
                if (Subtype != null && Subtype.Length > 0)
                {
                    text += " (" + Subtype + ")";
                }
                text += " " + CMStringUtilities.PlusFormatNumber(Mod);

                return text;
            }
        }

        [XmlIgnore]
        public string FullName
        {
            get
            {
                string key = Name;

                if (Subtype != null && Subtype.Length > 0)
                {


                    key += " (" + Subtype + ")";
                }

                return key;
            }

        }

        public override string ToString()
        {
            return Text;
        }


    }




}
