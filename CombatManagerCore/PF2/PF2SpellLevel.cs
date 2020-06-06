using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CombatManager.PF2
{
    public class PF2SpellLevel : SimpleNotifyClass, ICloneable
    {
        public const int Cantrip = 0;

        int _level;
        int? _cantripLevel;

        ObservableCollection<string> _spells;

        public PF2SpellLevel() { }

        public PF2SpellLevel(PF2SpellLevel r)
        {
            CopyFromInternal(r, true);
        }

        public void CopyFrom(PF2SpellLevel r)
        {
            CopyFromInternal(r, false);
        }

        public void CopyFromInternal(PF2SpellLevel r, bool newc)
        {
            Level = r._level;
            if (newc)
            {
                _spells = r._spells.CloneContents();
            }
            else
            {
                _spells.ReplaceClone(r.Spells);
            }
        }

        public object Clone()
        {
            return new PF2SpellLevel(this);
        }

        public int Level
        {
            get
            {
                return _level;
            }
            set
            {
                if (_level != value)
                {
                    _level = value;
                    Notify("Level");
                }
            }
        }
        public int? CantripLevel
        {
            get
            {
                return _cantripLevel;
            }
            set
            {
                if (_cantripLevel != value)
                {
                    _cantripLevel = value;
                    Notify("CantripLevel");
                }
            }
        }


        ObservableCollection<string> Spells
        {
            get
            {
                if (_spells == null)
                {
                    _spells = new ObservableCollection<string>();
                }

                return _spells;
            }

        }

        [XmlIgnore]
        public string Title
        {
            get
            {
                if (_level == Cantrip)
                {
                    return _level.PastTense();
                }
                else
                {
                    string text = "Cantrips";
                    if (_cantripLevel != null)
                    {
                        text += " (" + _cantripLevel.Value.PastTense() + ")";
                    }
                    return text;

                }
            }
        }

        [XmlIgnore]
        public string AllSpells
        {
            get
            {
                return Spells.WeaveString(", ");
            }
        }
    }
}