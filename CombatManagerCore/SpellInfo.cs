using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CombatManager
{
    public class SpellInfo : INotifyPropertyChanged, ICloneable
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private String _Name;
        private int? _DC;
        private Spell _Spell;
        private int? _Count;
        private int? _Cast;
        private bool _AlreadyCast;
        private String _Only;
        private String _Other;

        public SpellInfo() { }

        public SpellInfo(SpellInfo old)
        {
            _Name = old._Name;
            _DC = old._DC;
            _Spell = old._Spell;
            _Count = old._Count;
            _Cast = old._Cast;
            _AlreadyCast = old._AlreadyCast;
            _Only = old._Only;
            _Other = old._Other;
        }

        public object Clone()
        {
            return new SpellInfo(this);
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
        public int? DC
        {
            get { return _DC; }
            set
            {
                if (_DC != value)
                {
                    _DC = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("DC")); }
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
        public int? Count
        {
            get { return _Count; }
            set
            {
                if (_Count != value)
                {
                    _Count = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Count")); }
                }
            }
        }
        public int? Cast
        {
            get { return _Cast; }
            set
            {
                if (_Cast != value)
                {
                    _Cast = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Cast")); }
                }
            }
        }
        public bool AlreadyCast
        {
            get { return _AlreadyCast; }
            set
            {
                if (_AlreadyCast != value)
                {
                    _AlreadyCast = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("AlreadyCast")); }
                }
            }
        }
        public String Only
        {
            get { return _Only; }
            set
            {
                if (_Only != value)
                {
                    _Only = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Only")); }
                }
            }
        }

        public String Other
        {
            get { return _Other; }
            set
            {
                if (_Other != value)
                {
                    _Other = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Other")); }
                }
            }
        }


    }
}
