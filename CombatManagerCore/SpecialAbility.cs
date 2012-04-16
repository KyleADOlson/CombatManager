using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace CombatManager
{
    public class SpecialAbility : INotifyPropertyChanged, ICloneable
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
