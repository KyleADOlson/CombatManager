using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.PF2
{
    public enum PF2MonsterType
    {
        PC = 1,
        Creature = 2,
        Hazard = 3,
        Item = 4
    }

    public class PF2Monster : BaseMonster, ICloneable
    {
        PF2MonsterType _type;

        ObservableCollection<string> _traits;
        ObservableCollection<PF2Action> _actions;

        public PF2Monster()
        {
        }

        public PF2Monster(PF2Monster m)
        {
            CopyFromInternal(m, true);
        }



        public void CopyFrom(PF2Monster m)
        {
            CopyFromInternal(m, false);
        }

        public void CopyFromInternal(PF2Monster m, bool newc)
        {
            BaseMonsterCopy(m);
            _type = m.Type;
            if (newc)
            {
                _traits = m.Traits.CloneContents();
                _actions = m.Actions.CloneContents();
            }
            else
            {
                Traits.ReplaceContents(m.Traits);
                Actions.ReplaceContents(m.Actions);
            }

        }

        public object Clone()
        {
            return new PF2Monster(this); ;
        }

        public PF2MonsterType Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    Notify("Type");
                }
            }
        }



        [DBLoaderIgnore]
        public ObservableCollection<string> Traits
        {
            get
            {
                if (_traits == null)
                {
                    _traits = new ObservableCollection<string>();

                }

                return _traits;

            }
        }

        [DBLoaderIgnore]
        public ObservableCollection<PF2Action> Actions
        {
            get
            {
                if (_actions == null)
                {
                    _actions = new ObservableCollection<PF2Action>();

                }

                return _actions;

            }
        }

        public bool AddTrait(string trait)
        {
            bool add = true;
            if (_traits == null)
            {
                _traits = new ObservableCollection<string>();
            }
            else
            {
                add = !_traits.Contains(trait);
            }
            if (add)
            {
                _traits.Add(trait);
            }
            return add;
        }

        public bool RemoveTrait(string trait)
        {
            if (_traits == null)
            {
                return false;
            }
            if (!_traits.Contains(trait))
            {
                return false;
            }
            _traits.Remove(trait);
            return true;

        }
    }
}
