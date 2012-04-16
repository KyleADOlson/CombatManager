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
    public class TreasureItem : INotifyPropertyChanged, ICloneable
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private String _Name;
        private decimal _Value;
        private Spell _Spell;
        private MagicItem _MagicItem;
        private String _Type;
        private Equipment _Equipment;
 

        public TreasureItem()
        {


        }

        public TreasureItem(TreasureItem t)
        {
            _Name = t._Name;
            _Value = t._Value;
            if (t.Spell != null)
            {
                _Spell = t.Spell;
            }
            if (t.MagicItem != null)
            {
                _MagicItem = t.MagicItem ;
            }
        }

        public object Clone()
        {
            return new TreasureItem(this);
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
        public decimal Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Value")); }
                }
            }
        }
        public Spell Spell
        {
            get { return _Spell; }
            set
            {
                if (_Spell != value)
                {
                    _Spell = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Spell")); }
                }
            }
        }
        public MagicItem MagicItem
        {
            get { return _MagicItem; }
            set
            {
                if (_MagicItem != value)
                {
                    _MagicItem = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("MagicItem")); }
                }
            }
        }

        public Equipment Equipment
        {
            get { return _Equipment; }
            set
            {
                if (_Equipment != value)
                {
                    _Equipment = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Equipment")); }
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

    }
}
