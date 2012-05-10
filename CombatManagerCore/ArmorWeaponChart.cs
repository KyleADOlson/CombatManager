/*
 *  ArmorWeaponChart.cs
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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace CombatManager
{
    public class ArmorWeaponChart : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _Weight;
        private String _Name;
        private String _Cost;
        private String _Materials;
        private String _Type;

        private static List<ArmorWeaponChart> _Chart;

        private static Dictionary<String, int> _TotalWeights;

        static ArmorWeaponChart()
        {
            try
            {
                _Chart = XmlListLoader<ArmorWeaponChart>.Load("ArmorWeaponChart.xml");

                _TotalWeights = new Dictionary<string, int>();

                foreach (ArmorWeaponChart chart in _Chart)
                {
                    int weight = int.Parse(chart._Weight);

                    if (!_TotalWeights.ContainsKey(chart._Type))
                    {
                        _TotalWeights[chart._Type] = weight;
                    }
                    else
                    {
                        _TotalWeights[chart.Type] = _TotalWeights[chart.Type] + weight;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        public String Weight
        {
            get { return _Weight; }
            set
            {
                if (_Weight != value)
                {
                    _Weight = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Weight")); }
                }
            }
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
        public String Cost
        {
            get { return _Cost; }
            set
            {
                if (_Cost != value)
                {
                    _Cost = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Cost")); }
                }
            }
        }
        public String Materials
        {
            get { return _Materials; }
            set
            {
                if (_Materials != value)
                {
                    _Materials = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Materials")); }
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
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Type")); }
                }
            }
        }

        public static List<ArmorWeaponChart> Chart
        {
            get
            {
                return _Chart;
            }
        }

        public static Dictionary<String, int> TotalWeights
        {
            get
            {
                return _TotalWeights;
            }
        }

    }
}
