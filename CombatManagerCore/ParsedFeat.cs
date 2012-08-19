using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace CombatManager
{
    public class ParsedFeat : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private String _Name;
        private String _Choice;
        private String _FeatSource;

        public ParsedFeat()
        {

        }

        public ParsedFeat(string details)
        {
            ParseFeat(details);
        }

        public void ParseFeat(string details)
        {
            Regex reg = new Regex("(?<name>.+?) \\((?<choice>.+?)\\)");


            Match m = reg.Match(details);

            if (m.Success)
            {

                this.Name = m.Groups["name"].Value;
                this.Choice = m.Groups["choice"].Value;
            }

            else
            {
                Name = details;
            }
            
            _FeatSource = details;


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
        public String Choice
        {
            get { return _Choice; }
            set
            {
                if (_Choice != value)
                {
                    _Choice = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Choice")); }
                }
            }
        }
        
        public String FeatSource
        {
            
            get { return _FeatSource; }
            set
            {
                if (_FeatSource != value)
                {
                    _FeatSource = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("FeatSource")); }
                }
            }
        }

        [XmlIgnore]
        public String Text
        {
            get
            {
                string text = _Name;

                if (_Choice != null && _Choice.Length > 0)
                {
                    text += " (" + _Choice + ")";  
                }
                return text;
            }
        }

        public override string ToString()
        {
            return Text;
        }




    }
}

