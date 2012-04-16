using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatManager
{
    public class AttackSet : ICloneable
    {
        List<Attack> _WeaponAttacks;
        List<Attack> _NaturalAttacks;
		
		string _Name;

        public AttackSet()
        {
            _WeaponAttacks = new List<Attack>();
            _NaturalAttacks = new List<Attack>();
        }

        public List<Attack> WeaponAttacks
        {
            get
            {
                return _WeaponAttacks;
            }
            set
            {
                _WeaponAttacks = value;
            }
        }

        public List<Attack> NaturalAttacks
        {
            get
            {
                return _NaturalAttacks;
            }
            set
            {
                _NaturalAttacks = value;
            }
        }
		
		public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        public int Hands
        {
            get
            {
                int hands = 0;

                foreach (Attack attack in WeaponAttacks)
                {
                    if (attack.Weapon == null)
                    {
                        hands += attack.Count;
                    }
                    else
                    {
                        hands += attack.Weapon.HandsUsed * attack.Count;

                    }
                }

                foreach (Attack attack in NaturalAttacks)
                {
                    if (attack.Weapon != null)
                    {
                        hands += attack.Weapon.HandsUsed * attack.Count;
                    }
                }

                return Math.Max(hands, 0);
            }
        }

        public object Clone()
        {
            AttackSet set = new AttackSet();

            set.NaturalAttacks = new List<Attack>();

            foreach (Attack attack in NaturalAttacks)
            {
                set.NaturalAttacks.Add((Attack)attack.Clone());
            }
            
            set.WeaponAttacks = new List<Attack>();

            foreach (Attack attack in WeaponAttacks)
            {
                set.WeaponAttacks.Add((Attack)attack.Clone());
            }

            return set;
        }

        public override string ToString()
        {
            string text = "";
            bool firstAttack = true;
            List<Attack> attacks = new List<Attack>();
            attacks.AddRange(_WeaponAttacks);
            attacks.AddRange(_NaturalAttacks);

            foreach (Attack atk in attacks)
            {
                if (firstAttack)
                {
                    firstAttack = false;
                }
                else
                {
                    text += ", ";
                }

                text += atk.Text;
            }

            return text;
        }

    }
}
