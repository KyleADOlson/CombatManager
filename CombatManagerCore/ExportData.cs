using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CombatManager
{
    public class ExportData : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private List<Monster> _Monsters = new List<Monster>();
        private List<Spell> _Spells = new List<Spell>();
        private List<Feat> _Feats = new List<Feat>();
        private List<Condition> _Conditions = new List<Condition>();

        public List<Monster> Monsters
        {
            get { return _Monsters; }
            set
            {
                if (_Monsters != value)
                {
                    _Monsters = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Monsters")); }
                }
            }
        }
        public List<Spell> Spells
        {
            get { return _Spells; }
            set
            {
                if (_Spells != value)
                {
                    _Spells = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Spells")); }
                }
            }
        }
        public List<Feat> Feats
        {
            get { return _Feats; }
            set
            {
                if (_Feats != value)
                {
                    _Feats = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Feats")); }
                }
            }
        }
        public List<Condition> Conditions
        {
            get { return _Conditions; }
            set
            {
                if (_Conditions != value)
                {
                    _Conditions = value;
                    if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Conditions")); }
                }
            }
        }

    }
}
