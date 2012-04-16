using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using System.Xml;
using System.Globalization;
using System.IO;

namespace CombatManager
{


    public enum ItemLevel
    {
        Minor,
        Medium,
        Major
    }

    public class MagicItem : INotifyPropertyChanged
    {



        public event PropertyChangedEventHandler PropertyChanged;

        private String _Name;
        private String _Aura;
        private int _CL;
        private String _Slot;
        private String _Price;
        private String _Weight;
        private String _Description;
        private String _Requirements;
        private String _Cost;
        private String _Group;
        private String _Source;
        private String _FullText;
        private String _Destruction;
        private String _MinorArtifactFlag;
        private String _MajorArtifactFlag;
        private String _Abjuration;
        private String _Conjuration;
        private String _Divination;
        private String _Enchantment;
        private String _Evocation;
        private String _Necromancy;
        private String _Transmutation;
        private String _AuraStrength;
        private String _WeightValue;
        private String _PriceValue;
        private String _CostValue; private String _AL;
        private String _Int;
        private String _Wis;
        private String _Cha;
        private String _Ego;
        private String _Communication;
        private String _Senses;
        private String _Powers;
        private String _MagicItem;
        private String _DescHTML;





        private static Dictionary<string, MagicItem> itemMap;
        private static SortedDictionary<string, string> groups;
        private static SortedDictionary<int, int> cls;


        public static void LoadMagicItems()
        {

            List<MagicItem> set = XmlListLoader<MagicItem>.Load("MagicItems.xml");


            groups = new SortedDictionary<string, string>();
            cls = new SortedDictionary<int, int>();
            itemMap = new Dictionary<string, MagicItem>(new InsensitiveEqualityCompararer());

            foreach (MagicItem item in set)
            {
                itemMap[item.Name] = item;

                groups[item.Group] = item.Group;
                cls[item.CL] = item.CL;
            }
        }

        public static Dictionary<string, MagicItem> Items
        {
            get
            {
                if (itemMap == null)
                {
                    LoadMagicItems();
                }
                return itemMap;
            }
        }

        public static ICollection<string> Groups
        {
            get
            {
                if (itemMap == null)
                {
                    LoadMagicItems();
                }
                return groups.Values;
            }
        }
        public static ICollection<int> CLs
        {
            get
            {
                if (itemMap == null)
                {
                    LoadMagicItems();
                }
                return cls.Values;
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
        public String Aura
        {
            get { return _Aura; }
            set
            {
                if (_Aura != value)
                {
                    _Aura = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Aura")); }
                }
            }
        }
        public int CL
        {
            get { return _CL; }
            set
            {
                if (_CL != value)
                {
                    _CL = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CL")); }
                }
            }
        }
        public String Slot
        {
            get { return _Slot; }
            set
            {
                if (_Slot != value)
                {
                    _Slot = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Slot")); }
                }
            }
        }
        public String Price
        {
            get { return _Price; }
            set
            {
                if (_Price != value)
                {
                    _Price = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Price")); }
                }
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
        public String Description
        {
            get { return _Description; }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Description")); }
                }
            }
        }
        public String Requirements
        {
            get { return _Requirements; }
            set
            {
                if (_Requirements != value)
                {
                    _Requirements = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Requirements")); }
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
        public String Group
        {
            get { return _Group; }
            set
            {
                if (_Group != value)
                {
                    _Group = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Group")); }
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
        public String FullText
        {
            get { return _FullText; }
            set
            {
                if (_FullText != value)
                {
                    _FullText = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("FullText")); }
                }
            }
        }
        public String Destruction
        {
            get { return _Destruction; }
            set
            {
                if (_Destruction != value)
                {
                    _Destruction = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Destruction")); }
                }
            }
        }
        public String MinorArtifactFlag
        {
            get { return _MinorArtifactFlag; }
            set
            {
                if (_MinorArtifactFlag != value)
                {
                    _MinorArtifactFlag = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MinorArtifactFlag")); }
                }
            }
        }
        public String MajorArtifactFlag
        {
            get { return _MajorArtifactFlag; }
            set
            {
                if (_MajorArtifactFlag != value)
                {
                    _MajorArtifactFlag = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MajorArtifactFlag")); }
                }
            }
        }
        public String Abjuration
        {
            get { return _Abjuration; }
            set
            {
                if (_Abjuration != value)
                {
                    _Abjuration = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Abjuration")); }
                }
            }
        }
        public String Conjuration
        {
            get { return _Conjuration; }
            set
            {
                if (_Conjuration != value)
                {
                    _Conjuration = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Conjuration")); }
                }
            }
        }
        public String Divination
        {
            get { return _Divination; }
            set
            {
                if (_Divination != value)
                {
                    _Divination = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Divination")); }
                }
            }
        }
        public String Enchantment
        {
            get { return _Enchantment; }
            set
            {
                if (_Enchantment != value)
                {
                    _Enchantment = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Enchantment")); }
                }
            }
        }
        public String Evocation
        {
            get { return _Evocation; }
            set
            {
                if (_Evocation != value)
                {
                    _Evocation = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Evocation")); }
                }
            }
        }
        public String Necromancy
        {
            get { return _Necromancy; }
            set
            {
                if (_Necromancy != value)
                {
                    _Necromancy = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Necromancy")); }
                }
            }
        }
        public String Transmutation
        {
            get { return _Transmutation; }
            set
            {
                if (_Transmutation != value)
                {
                    _Transmutation = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Transmutation")); }
                }
            }
        }
        public String AuraStrength
        {
            get { return _AuraStrength; }
            set
            {
                if (_AuraStrength != value)
                {
                    _AuraStrength = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AuraStrength")); }
                }
            }
        }
        public String WeightValue
        {
            get { return _WeightValue; }
            set
            {
                if (_WeightValue != value)
                {
                    _WeightValue = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("WeightValue")); }
                }
            }
        }
        public String PriceValue
        {
            get { return _PriceValue; }
            set
            {
                if (_PriceValue != value)
                {
                    _PriceValue = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("PriceValue")); }
                }
            }
        }
        public String CostValue
        {
            get { return _CostValue; }
            set
            {
                if (_CostValue != value)
                {
                    _CostValue = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("CostValue")); }
                }
            }
        }
        public String AL
        {
            get { return _AL; }
            set
            {
                if (_AL != value)
                {
                    _AL = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AL")); }
                }
            }
        }
        public String Int
        {
            get { return _Int; }
            set
            {
                if (_Int != value)
                {
                    _Int = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Int")); }
                }
            }
        }
        public String Wis
        {
            get { return _Wis; }
            set
            {
                if (_Wis != value)
                {
                    _Wis = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Wis")); }
                }
            }
        }
        public String Cha
        {
            get { return _Cha; }
            set
            {
                if (_Cha != value)
                {
                    _Cha = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Cha")); }
                }
            }
        }
        public String Ego
        {
            get { return _Ego; }
            set
            {
                if (_Ego != value)
                {
                    _Ego = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Ego")); }
                }
            }
        }
        public String Communication
        {
            get { return _Communication; }
            set
            {
                if (_Communication != value)
                {
                    _Communication = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Communication")); }
                }
            }
        }
        public String Senses
        {
            get { return _Senses; }
            set
            {
                if (_Senses != value)
                {
                    _Senses = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Senses")); }
                }
            }
        }
        public String Powers
        {
            get { return _Powers; }
            set
            {
                if (_Powers != value)
                {
                    _Powers = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Powers")); }
                }
            }
        }

        [XmlElement (ElementName="MagicItem")]
        public String MagicItemRequired
        {
            get { return _MagicItem; }
            set
            {
                if (_MagicItem != value && value != "NULL")
                {
                    _MagicItem = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MagicItem")); }
                }
            }
        }

        public String DescHTML
        {
            get { return _DescHTML; }
            set
            {
                if (_DescHTML != value)
                {
                    _DescHTML = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DescHTML")); }
                }
            }
        }


        public static MagicItem ByName(string name)
        {
            MagicItem item = null;
            if (itemMap.TryGetValue(name, out item))
            {
                return item;
            }
            return null;
        }

    }
}
