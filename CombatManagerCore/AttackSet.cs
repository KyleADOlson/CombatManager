/*
 *  AttackSet.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatManager
{
    public class AttackSet
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
