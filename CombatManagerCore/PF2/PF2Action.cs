using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CombatManager.PF2
{
    public enum PF2ActionType
    {
        Free = 0,
        Single = 1,
        Two = 2,
        Three = 3,
        Reaction = int.MinValue

    }

    public class PF2Action : SimpleNotifyClass, ICloneable
    {
        PF2ActionType _type;
        string _name;
        string _weapon;
        int? _mod;
        ObservableCollection<PF2ActionTrait> _traits;
        string _trigger;
        string _requirements;
        string _effect;
        int? _dc;
        PF2SpellList _spellList;
        ObservableCollection<PF2ActionDamage> _damage;

        public PF2Action()
        {

        }

        public PF2Action(PF2Action r)
        {
            CopyFromInternal(r, true);
        }

        public void CopyFrom(PF2Action r)
        {
            CopyFromInternal(r, false);
        }

         public void CopyFromInternal(PF2Action r, bool newc)
        {
            Type = r._type;
            Name = r._name;
            Weapon = r._weapon;
            Mod = r._mod;
            if (newc)
            {
                _traits = r.Traits.CloneContents();
                _damage = r.Damage.CloneContents();
            }
            else
            {
                Traits.ReplaceClone(r.Traits);
                Damage.ReplaceClone(r.Damage);
            }
            Effect = r.Effect;
            Trigger = r.Trigger;
            DC = r.DC;
            SpellList = r._spellList;

        }

        public object Clone()
        {
            return new PF2Action(this);

        }



        public PF2ActionType Type
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

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    Notify("Name");
                }
            }
        }

        public string Weapon
        {
            get
            {
                return _weapon;
            }
            set
            {
                if (_weapon != value)
                {
                    _weapon = value;
                    Notify("Weapon");
                }
            }
        }

        public int? Mod
        {
            get
            {
                return _mod;
            }
            set
            {
                if (_mod != value)
                {
                    _mod = value;
                    Notify("Mod");
                }
            }
        }


        ObservableCollection<PF2ActionTrait> Traits
        {
            get
            {
                if (_traits == null)
                {
                    _traits = new ObservableCollection<PF2ActionTrait>();
                }

                return _traits;
            }

        }

        ObservableCollection<PF2ActionDamage> Damage
        {
            get
            {
                if (_damage == null)
                {
                    _damage = new ObservableCollection<PF2ActionDamage>();
                }

                return _damage;
            }

        }
        public string Requirements
        {
            get
            {
                return _requirements;
            }
            set
            {
                if (_requirements != value)
                {
                    _requirements = value;
                    Notify("Requirements");
                }
            }
        }

        public string Trigger
        {
            get
            {
                return _trigger;
            }
            set
            {
                if (_trigger != value)
                {
                    _trigger = value;
                    Notify("Trigger");
                }
            }
        }

        public string Effect
        {
            get
            {
                return _effect;
            }
            set
            {
                if (_effect != value)
                {
                    _effect = value;
                    Notify("Effect");
                }
            }
        }
        public int? DC
        {
            get
            {
                return _dc;
            }
            set
            {
                if (_dc != value)
                {
                    _dc = value;
                    Notify("DC");
                }
            }
        }

        public PF2SpellList SpellList
        {
            get
            {
                return _spellList;
            }
            set
            {
                if (_spellList != value)
                {
                    _spellList = value.CloneOrNull();
                    Notify("SpellList");
                }
            }
        }
    }
}