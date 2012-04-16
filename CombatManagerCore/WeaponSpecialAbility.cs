using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CombatManager
{
    public class WeaponSpecialAbility : INotifyPropertyChanged
    {



        public event PropertyChangedEventHandler PropertyChanged;

        private String _Minor;
        private String _Medium;
        private String _Major;
        private String _RangedMinor;
        private String _RangedMedium;
        private String _RangedMajor;
        private String _Name;
        private String _AltName;
        private int _BasePriceMod;
        private bool _Melee;
        private bool _Ranged;
        private String _Plus;
        private String _Text;

        private static List<WeaponSpecialAbility> abilities;
        static WeaponSpecialAbility()
        {

            Load();

        }

        static void Load()
        {
            abilities = XmlListLoader<WeaponSpecialAbility>.Load("WeaponSpecialAbility.xml");

        }

        public static List<WeaponSpecialAbility> SpecialAbilities
        {
            get
            {
                return new List<WeaponSpecialAbility>(abilities);
            }
        }

        public static List<WeaponSpecialAbility> RangedAbilities
        {
            get
            {
                List<WeaponSpecialAbility> list = new List<WeaponSpecialAbility>();

                foreach (WeaponSpecialAbility w in SpecialAbilities)
                {
                    if (w.Ranged)
                    {
                        list.Add(w);
                    }
                }

                return list;
            }
        }



        public static List<WeaponSpecialAbility> MeleeAbilities
        {
            get
            {
                List<WeaponSpecialAbility> list = new List<WeaponSpecialAbility>();

                foreach (WeaponSpecialAbility w in SpecialAbilities)
                {
                    if (w.Melee)
                    {
                        list.Add(w);
                    }
                }

                return list;
            }
        }


        public String Minor
        {
            get { return _Minor; }
            set
            {
                if (_Minor != value)
                {
                    _Minor = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Minor")); }
                }
            }
        }
        public String Medium
        {
            get { return _Medium; }
            set
            {
                if (_Medium != value)
                {
                    _Medium = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Medium")); }
                }
            }
        }
        public String Major
        {
            get { return _Major; }
            set
            {
                if (_Major != value)
                {
                    _Major = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Major")); }
                }
            }
        }
        public String RangedMinor
        {
            get { return _RangedMinor; }
            set
            {
                if (_RangedMinor != value)
                {
                    _RangedMinor = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("RangedMinor")); }
                }
            }
        }
        public String RangedMedium
        {
            get { return _RangedMedium; }
            set
            {
                if (_RangedMedium != value)
                {
                    _RangedMedium = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("RangedMedium")); }
                }
            }
        }
        public String RangedMajor
        {
            get { return _RangedMajor; }
            set
            {
                if (_RangedMajor != value)
                {
                    _RangedMajor = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("RangedMajor")); }
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
        public int BasePriceMod
        {
            get { return _BasePriceMod; }
            set
            {
                if (_BasePriceMod != value)
                {
                    _BasePriceMod = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("BasePriceMod")); }
                }
            }
        }
        public bool Melee
        {
            get { return _Melee; }
            set
            {
                if (_Melee != value)
                {
                    _Melee = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Melee")); }
                }
            }
        }
        public bool Ranged
        {
            get { return _Ranged; }
            set
            {
                if (_Ranged != value)
                {
                    _Ranged = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Ranged")); }
                }
            }
        }
        public String Plus
        {
            get { return _Plus; }
            set
            {
                if (_Plus != value)
                {
                    _Plus = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Plus")); }
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
    }
}
