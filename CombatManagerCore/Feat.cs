/*
 *  Feat.cs
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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Globalization;
using System.IO;
using System.Collections.ObjectModel;

namespace CombatManager
{
    public class Feat : INotifyPropertyChanged, IDBLoadable
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private String _Name;
        private String _AltName;
        private String _Type;
        private String _Prerequistites;
        private String _Summary;
        private String _Source;
        private String _System;
        private String _License;
        private String _URL;
        private String _Detail; 
        private String _Benefit;
        private String _Normal;
        private String _Special;
        private SortedDictionary<String, String> _Types;

        private bool _DetailParsed;


        private int _DBLoaderID;


        private static Dictionary<String, Feat> featMap;
        private static Dictionary<String, String> altFeatMap;
        private static SortedDictionary<string, string> types;
        private static ObservableCollection<Feat> feats;

        private static bool _FeatsLoaded;
        private static DBLoader<Feat> _FeatsDB;


        public Feat()
        {
            _Types = new SortedDictionary<String, String>();
            _DetailParsed = false;
        }

        public Feat(Feat f)
        {
            CopyFrom(f);
        }

        public object Clone()
        {
            return new Feat(this);
        }

        public void CopyFrom(Feat f)
        {
            _Name = f._Name;
            _AltName = f._AltName;
            _Type = f._Type;
            _Prerequistites = f._Prerequistites;
            _Summary = f._Summary;
            _Source = f._Source;
            _System = f._System;
            _License = f._License;
            _URL = f._URL;
            _Detail = f._Detail; 
            _Benefit = f._Benefit;
            _Normal = f._Normal;
            _Special = f._Special;

            if (f._Types != null)
            {
                _Types = new SortedDictionary<string, string>();

                foreach (string s in f._Types.Values)
                {
                    _Types[s] = s;
                }
            }
            else
            {
                _Types = null;
            }
            
            _DetailParsed = f._DetailParsed;
            _DBLoaderID = f._DBLoaderID;
        }

        public static void LoadFeats()
        {


            List<Feat> set = XmlListLoader<Feat>.Load("Feats.xml");

            featMap = new Dictionary<string, Feat>();
            altFeatMap = new Dictionary<string, string>();
            types = new SortedDictionary<String, String>();

            foreach (Feat feat in set)
            {
                bool changed;
                string commaName = CMStringUtilities.DecommaText(feat.Name, out changed);

                if (changed)
                {
                    feat.AltName = feat.Name;
                    feat.Name = commaName;
                    altFeatMap.Add(feat.AltName, feat.Name);
                }

                featMap[feat.Name] = feat;
            


                //add to types list
                foreach (string type in feat.Types)
                {
                    types[type] = type;
                }

                if (feat.Types.Count == 0)
                {
                    Debug.WriteLine(feat.Name);
                }
				
				feats = new ObservableCollection<Feat>();
				foreach (Feat f in Feat.FeatMap.Values)
				{
					feats.Add(f);
				}
            }
            if (DBSettings.UseDB)
            {
                _FeatsDB = new DBLoader<Feat>("feats.db");

                foreach (Feat f in _FeatsDB.Items)
                {
                    feats.Add(f);

                    if (!FeatMap.ContainsKey(f.Name))
                    {
                        FeatMap[f.Name] = f;
                    }
                }
            }

            _FeatsLoaded = true;

        }

        public static bool FeatsLoaded
        {
            get
            {
                return _FeatsLoaded;
            }
        }


        public static Dictionary<String, Feat> FeatMap
        {
            get
            {
                return featMap;
            }
        }

        public static ObservableCollection<Feat> Feats
        {
            get
            {
				if (feats == null)
				{
					LoadFeats();
				}
                return feats;
            }
        }

        public static Dictionary<String, String> AltFeatMap
        {
            get
            {
                if (altFeatMap == null)
                {
                    LoadFeats();
                }
                return altFeatMap;
            }
        }

        public static IEnumerable<string> FeatTypes
        {
            get
            {
                return types.Values;
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
        public String AltName
        {
            get { return _AltName; }
            set
            {
                if (_AltName != value)
                {
                    _AltName = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AltName")); }
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

                    _Types.Clear();
                    foreach (string str in _Type.Split(new char[]{','}))
                    {
                        _Types[str.Trim()] = str.Trim();
                    }

                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Type")); }
                }
            }
        }
        public String Prerequistites
        {
            get { return _Prerequistites; }
            set
            {
                if (_Prerequistites != value)
                {
                    _Prerequistites = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Prerequistites")); }
                }
            }
        }
        public String Summary
        {
            get { return _Summary; }
            set
            {
                if (_Summary != value)
                {
                    _Summary = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Summary")); }
                }
            }
        }
        public String Source
        {
            get { return _Source; }
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Source")); }
                }
            }
        }
        public String System
        {
            get { return _System; }
            set
            {
                if (_System != value)
                {
                    _System = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("System")); }
                }
            }
        }
        public String License
        {
            get { return _License; }
            set
            {
                if (_License != value)
                {
                    _License = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("License")); }
                }
            }
        }

        public String URL
        {
            get { return _URL; }
            set
            {
                if (_URL != value)
                {
                    _URL = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("URL")); }
                }
            }
        }

        public String Detail
        {
            get { return _Detail; }
            set
            {
                if (_Detail != value)
                {
                    _Detail = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Detail")); }
                }
            }
        }
        public String Benefit
        {
            get 
            {
                if (!DetailParsed)
                {
                    ParseDetail();
                }
                return _Benefit; 
                
            }
            set
            {
                if (_Benefit != value)
                {
                    _Benefit = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Benefit")); }
                }
            }
        }
        public String Normal
        {
            get
            {
                if (!DetailParsed)
                {
                    ParseDetail();
                } 
                return _Normal;
            }
            set
            {
                if (_Normal != value)
                {
                    _Normal = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Normal")); }
                }
            }
        }
        public String Special
        {
            get
            {
                if (!DetailParsed)
                {
                    ParseDetail();
                }
                return _Special;
            }
            set
            {
                if (_Special != value)
                {
                    _Special = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Special")); }
                }
            }
        }

        public int DBLoaderID
        {
            get
            {
                return _DBLoaderID;
            }
            set
            {
                if (_DBLoaderID != value)
                {
                    _DBLoaderID = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DBLoaderID")); }

                }
            }
        }

        [XmlIgnore]
        public bool IsCustom
        {
            get
            {
                return DBLoaderID != 0;
            }
        }

        [XmlIgnore]
        public ICollection<String> Types
        {
            get
            {
                return _Types.Values;
            }
        }

        public bool DetailParsed
        {
            get
            {
                return _DetailParsed;
            }
            set
            {
                _DetailParsed = value;
            }
        }

        public void ParseDetail()
        {
            if (!DetailParsed)
            {
                if (_Detail != null)
                {
                    Match matchStart;

                    String detailCheck = _Detail;



                    Regex regexBenefit = CreateHeaderRegex("Benefit");
                    matchStart = (regexBenefit.Match(detailCheck));
                    if (matchStart.Success)
                    {
                        Benefit = FinishItem(matchStart, detailCheck);
                    }

                    Regex regexNormal = CreateHeaderRegex("Normal");
                    matchStart = (regexNormal.Match(detailCheck));
                    if (matchStart.Success)
                    {
                        Normal = FinishItem(matchStart, detailCheck);
                    }

                    Regex regexSpecial = CreateHeaderRegex("Special");
                    matchStart = (regexSpecial.Match(detailCheck));
                    if (matchStart.Success)
                    {
                        Special = FinishItem(matchStart, detailCheck);
                    }
                }
                DetailParsed = true;
            }
        }

        private Regex CreateHeaderRegex(String item)
        {
            string spanTag = "<span(.|\n|\t)*?>";

            return new Regex("(<b>|" + spanTag + ") *" + item + "s? *:? *(</b>|</span>|" + spanTag + ")+:?", RegexOptions.IgnoreCase);
        }

        private String FinishItem(Match matchStart, string detailCheck)
        {
            int start = matchStart.Index + matchStart.Length;

            Regex regexFont = new Regex("(</span>)?(</font> *(</div>|</p>)|</p> *</font>)");

            Regex regexReplace = new Regex("<[^<>]+>");


            Match matchEnd = regexFont.Match(_Detail, start);
            if (matchEnd.Success)
            {
                return regexReplace.Replace(_Detail.Substring(start, matchEnd.Index - start), "").Trim();
            }
            else
            {
                return null;
            }

        }
        public static void AddCustomFeat(Feat f)
        {
            _FeatsDB.AddItem(f);
            if (!featMap.ContainsKey(f.Name))
            {
                featMap[f.Name] = f;
            }
            feats.Add(f);
        }

        public static void RemoveCustomFeat(Feat f)
        {

            _FeatsDB.DeleteItem(f);
            feats.Remove(f);
            
            Feat old;
            if (FeatMap.TryGetValue(f.Name, out old))
            {
                if (old == f)
                {
                    FeatMap.Remove(f.Name);
                    Feat replace = feats.FirstOrDefault(a => a.Name == f.Name);

                    if (replace != null)
                    {
                        FeatMap[replace.Name] = replace;
                    }
                }
            }

        }

        public static void UpdateCustomFeat(Feat f)
        {
            _FeatsDB.UpdateItem(f);
        }


    }
}
