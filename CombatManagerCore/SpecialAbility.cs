/*
 *  SpecialAbility.cs
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CombatManager
{
    public class SpecialAbility : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _Name;
        private String _Type;
        private String _Text;
        private int? _ConstructionPoints;



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
        public String Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    if (PropertyChanged != null) 
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Type"));
                        PropertyChanged(this, new PropertyChangedEventArgs("AbilityTypeIndex")); 
                    }
                }
            }
        }
        public String Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Text")); }
                }
            }
        }
        public int? ConstructionPoints
        {

            get { return _ConstructionPoints; }
            set
            {
                if (_ConstructionPoints != value)
                {
                    _ConstructionPoints = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ConstructionPoints")); }
                }
            }
        }

        [XmlIgnore]
        public int AbilityTypeIndex
        {
            get
            {
                if (String.Compare(_Type, "Ex") == 0)
                {
                    return 0;
                }
                else if (String.Compare(_Type, "Sp") == 0)
                {
                    return 1;
                }
                else if (String.Compare(_Type, "Su") == 0)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
            set
            {
                if (value == 0)
                {
                    Type = "Ex";
                }
                else if (value == 1)
                {
                    Type = "Sp";
                }
                else if (value == 2)
                {
                    Type = "Su";
                }
                else
                {
                    Type = "";
                }
            }
        }

        public object Clone()
        {
            SpecialAbility s = new SpecialAbility();
            s.Name = Name;
            s.Text = Text;
            s.Type = Type;
            s.ConstructionPoints = ConstructionPoints;

            return s;

        }
    }
}
