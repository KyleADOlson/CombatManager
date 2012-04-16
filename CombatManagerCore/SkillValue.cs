using System;
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

    public class SkillValue : ICloneable, INotifyPropertyChanged
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

            Regex regName = new Regex("([ a-zA-Z]+)( \\(([- a-zA-Z]+)\\))?");

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
