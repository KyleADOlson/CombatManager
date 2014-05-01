using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CombatManager
{
    public class Bookmark : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private String _Type;
        private String _Name;
        private String _ID;
        private String _Data;

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
        public String Data
        {
            get { return _Data; }
            set
            {
                if (_Data != value)
                {
                    _Data = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Data")); }
                }
            }
        }
        
    }
}
