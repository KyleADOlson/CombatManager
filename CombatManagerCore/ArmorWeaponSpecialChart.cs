using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Globalization;
using System.IO;


namespace CombatManager
{
    public class ArmorWeaponSpecialChart : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private String _Minor;
        private String _Medium;
        private String _Major;
        private String _Name;
        private String _Cost;
        private String _Bonus;
        private String _Type;

        
        private static List<ArmorWeaponSpecialChart> _Chart;

        static ArmorWeaponSpecialChart()
        {
            _Chart = XmlListLoader<ArmorWeaponSpecialChart>.Load("ArmorWeaponSpecialChart.xml");
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
        public String Bonus
        {
            get { return _Bonus; }
            set
            {
                if (_Bonus != value)
                {
                    _Bonus = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Bonus")); }
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
        
        public string LevelWeight(ItemLevel level)
        {
            if (level == ItemLevel.Minor)
            {
                return _Minor;
            }
            else if (level == ItemLevel.Medium)
            {
                return _Medium;
            }
            else if (level == ItemLevel.Major)
            {
                return _Major;
            }

            return null;

        }

        public static List<ArmorWeaponSpecialChart> Chart
        {
            get
            {
                return _Chart;
            }
        }

    }
}
