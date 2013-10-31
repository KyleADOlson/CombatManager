/*
 *  Rule.cs
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
using ScottsUtils;

#if ANDROID
using Android.Content.PM;
using Android.Content;
using Android.App;
#endif

#if !MONO
#else
using Mono.Data.Sqlite;
#endif

namespace CombatManager
{
    public class Rule : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private String _ID;
        private String _Name;
        private String _Details;
        private String _Source;
        private String _Type;
        private String _AbilityType;
        private String _Format;
        private String _Location;
        private String _Format2;
        private String _Location2; 
        private bool _Untrained;
        private String _Ability; 
        private String _Subtype;

        private bool _DBDetailsLoaded;

        private static List<Rule> ruleList;
        private static SortedDictionary<string, string> types;
        private static Dictionary<string, SortedDictionary<string, string>> subtypes;

        private static bool _RulesLoaded;

        public static void LoadRules()
        {
            if (!_RulesLoaded)
            {
                List<Rule> set = XmlListLoader<Rule>.Load("RuleShort.xml");


                types = new SortedDictionary<string, string>();
                subtypes = new Dictionary<string, SortedDictionary<string, string>>();
                ruleList = new List<Rule>();

                foreach (Condition c in Condition.Conditions)
                {
                    if (c.Text != null)
                    {
                        Rule r = new Rule();
                        r.Name = c.Name;
                        r.Type = "Condition";
                        r.Source = "PFRPG Core";
                        r.Details = c.Text;
                        set.Add(r);
                    }
                }

                foreach (Rule rule in set)
                {
                    ruleList.Add(rule);

                    types[rule.Type] = rule.Type;

                    if (rule.Subtype != null && rule.Subtype.Length > 0)
                    {
                        if (!subtypes.ContainsKey(rule.Type))
                        {
                            subtypes[rule.Type] = new SortedDictionary<string, string>();
                        }
                        subtypes[rule.Type][rule.Subtype] = rule.Subtype;
                    }
                }

                _RulesLoaded = true;
            }


        }

        public static bool RulesLoaded
        {
            get
            {
                return _RulesLoaded;
            }
        }


        public static List<Rule> Rules
        {
            get
            {
                if (ruleList == null)
                {
                    LoadRules();
                }
                return ruleList;
            }
        }

#if ANDROID
        static string _DBVersion;
        public static void PrepareDetailDB(String version)
        {
            _DBVersion = version;
            if (!Directory.Exists(DBFolder))
            {
                Directory.CreateDirectory(DBFolder);
            }
            //if !db exists
            if (!File.Exists(DBFullFilename))
            {
                //open stream
                using (Stream io = CoreContext.Context.Assets.Open("Details.db"))
                {
                    using (FileStream fs = File.Open(DBFullFilename, FileMode.Create))
                    {
                        //copy db
                        byte[] bytes = new byte[20480];
                        int read = io.Read(bytes, 0, bytes.Length);
                        while (read > 0)
                        {
                            fs.Write(bytes, 0, read);
                            read = io.Read(bytes, 0, bytes.Length);
                        }
                        fs.Close();
                    }
                    io.Close();
                }

                
                //delete old dbs
                List<string> files = new List<string>(Directory.EnumerateFiles(DBFolder));

                foreach (string s in from x in files where x != DBFullFilename select x)
                {
                    File.Delete(s);
                }
            }


        }

        private static string DBFolder
        {
            get
            {
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                dir = Path.Combine(dir, "Database");
                return dir;
            }
        }

        private static string DBFullFilename
        {
            get
            {
                string filename = DBFilename;
                return Path.Combine(DBFolder, filename);
            }
        }

        private static string DBFilename
        {
            get
            {
                return "detail" + _DBVersion + ".db";

            }
        }
                 

#endif


        public static ICollection<string> Types
        {
            get
            {
                if (ruleList == null)
                {
                    LoadRules();
                }
                return types.Values;
            }
        }
        public static Dictionary<string, SortedDictionary<string, string> > Subtypes
        {
            get
            {
                if (ruleList == null)
                {
                    LoadRules();
                }
                return subtypes;
            }
        }

        public String ID
        {
            get { return _ID; }
            set
            {
                if (_ID != value)
                {
                    _ID = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("ID")); }
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
        public String Details
        {
            get 
            {

                if (_Details == null && _ID != null && !_DBDetailsLoaded)
                {
                    _DBDetailsLoaded = true;
                    _Details = DetailsDB.LoadDetails(_ID, "Rules", "details");
                }

                return _Details; 
            

            }
            set
            {
                if (_Details != value)
                {
                    _Details = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Details")); }
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

        public String AbilityType
        {
            get { return _AbilityType; }
            set
            {
                if (_AbilityType != value)
                {
                    _AbilityType = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AbilityType")); }
                }
            }
        }
        public String Format
        {
            get { return _Format; }
            set
            {
                if (_Format != value)
                {
                    _Format = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Format")); }
                }
            }
        }
        public String Location
        {
            get { return _Location; }
            set
            {
                if (_Location != value)
                {
                    _Location = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Location")); }
                }
            }
        }
        public String Format2
        {
            get { return _Format2; }
            set
            {
                if (_Format2 != value)
                {
                    _Format2 = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Format2")); }
                }
            }
        }
        public String Location2
        {
            get { return _Location2; }
            set
            {
                if (_Location2 != value)
                {
                    _Location2 = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Location2")); }
                }
            }
        }
        public bool Untrained
        {
            get { return _Untrained; }
            set
            {
                if (_Untrained != value)
                {
                    _Untrained = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Untrained")); }
                }
            }
        }
        public String Ability
        {
            get { return _Ability; }
            set
            {
                if (_Ability != value)
                {
                    _Ability = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Ability")); }
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




    }
}
