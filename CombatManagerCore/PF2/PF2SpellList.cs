using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CombatManager.PF2
{
    public class PF2SpellList : SimpleNotifyClass, ICloneable
    {
        string _class;
        ObservableCollection<PF2SpellLevel> _levels;

        public PF2SpellList() { }

        public object Clone()
        {
            return new PF2SpellList(this);
        }

        public PF2SpellList(PF2SpellList r)
        {
            CopyFromInternal(r, true);
        }

        public void CopyFrom(PF2SpellList r)
        {
            CopyFromInternal(r, false);
        }

        public void CopyFromInternal(PF2SpellList r, bool newc)
        {
            Class = r._class;
            if (newc)
            {
                _levels = r.Levels.CloneContents();
            }
            else
            {
                Levels.ReplaceClone(r.Levels);
            }
        }


        public string Class
        {
            get
            {
                return _class;
            }
            set
            {
                if (_class != value)
                {
                    _class = value;
                    Notify("Class");
                }
            }
        }


        ObservableCollection<PF2SpellLevel> Levels
        {
            get
            {
                if (_levels == null)
                {
                    _levels = new ObservableCollection<PF2SpellLevel>();
                }

                return _levels;
            }

        }

    }

   
}