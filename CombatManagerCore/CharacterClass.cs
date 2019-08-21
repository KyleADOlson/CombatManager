/*
 *  CharacterClass.cs
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

    public enum CharacterClassEnum
    {
        None,
        Barbarian,
        Bard,
        Cleric,
        Druid,
        Fighter,
        Monk,
        Paladin,
        Ranger,
        Rogue,
        Sorcerer,
        Wizard,
        Alchemist,
        Antipaladin,
        Cavalier,
        Gunslinger,
        Inquisitor,
        Magus,
        Oracle,
        Summoner,
        Witch,
        Arcanist,
        Bloodrager,
        Hunter,
        Investigator,
        Shaman,
        Skald,
        Slayer,
        Swashbuckler,
        Warpriest,
        ArcaneArcher,
        ArcaneTrickster,
        Assassin,
        DragonDisciple,
        Duelist,
        EldritchKnight,
        Loremaster,
        MysticTheurge,
        PathfinderChronicler,
        Shadowdancer,
        BattleHerald,
        HolyVindicator,
        HorizonWalker,
        Kineticist,
        MasterChymist,
        MasterSpy,
        Medium,
        Mesmerist,
        NatureWarden,
        Ninja,
        Occultist,
        Psychic,
        RageProphet,
        Samurai,
        Spiritualist,
        StalwartDefender,
        Adept,
        Aristocrat,
        Commoner,
        Expert,
        Warrior,

    }

    public static class CharacterClass
    {
        static Dictionary<CharacterClassEnum, string> _EnumToName = new Dictionary<CharacterClassEnum, string>();
        static Dictionary<string, CharacterClassEnum> _NameToEnum = new Dictionary<string, CharacterClassEnum>(new InsensitiveEqualityCompararer());

        static CharacterClass()
        {
            _EnumToName[CharacterClassEnum.None] = "None";
            _EnumToName[CharacterClassEnum.Barbarian] = "Barbarian";
            _EnumToName[CharacterClassEnum.Bard] = "Bard";
            _EnumToName[CharacterClassEnum.Cleric] = "Cleric";
            _EnumToName[CharacterClassEnum.Druid] = "Druid";
            _EnumToName[CharacterClassEnum.Fighter] = "Fighter";
            _EnumToName[CharacterClassEnum.Monk] = "Monk";
            _EnumToName[CharacterClassEnum.Paladin] = "Paladin";
            _EnumToName[CharacterClassEnum.Ranger] = "Ranger";
            _EnumToName[CharacterClassEnum.Rogue] = "Rogue";
            _EnumToName[CharacterClassEnum.Sorcerer] = "Sorcerer";
            _EnumToName[CharacterClassEnum.Wizard] = "Wizard";
            _EnumToName[CharacterClassEnum.Alchemist] = "Alchemist";
            _EnumToName[CharacterClassEnum.Antipaladin] = "Antipaladin";
            _EnumToName[CharacterClassEnum.Cavalier] = "Cavalier";
            _EnumToName[CharacterClassEnum.Gunslinger] = "Gunslinger";
            _EnumToName[CharacterClassEnum.Inquisitor] = "Inquisitor";
            _EnumToName[CharacterClassEnum.Magus] = "Magus";
            _EnumToName[CharacterClassEnum.Oracle] = "Oracle";
            _EnumToName[CharacterClassEnum.Summoner] = "Summoner";
            _EnumToName[CharacterClassEnum.Witch] = "Witch";
            _EnumToName[CharacterClassEnum.Arcanist] = "Arcanist";
            _EnumToName[CharacterClassEnum.Bloodrager] = "Bloodrager";
            _EnumToName[CharacterClassEnum.Hunter] = "Hunter";
            _EnumToName[CharacterClassEnum.Investigator] = "Investigator";
            _EnumToName[CharacterClassEnum.Shaman] = "Shaman";
            _EnumToName[CharacterClassEnum.Skald] = "Skald";
            _EnumToName[CharacterClassEnum.Slayer] = "Slayer";
            _EnumToName[CharacterClassEnum.Swashbuckler] = "Swashbuckler";
            _EnumToName[CharacterClassEnum.Warpriest] = "Warpriest";
            _EnumToName[CharacterClassEnum.ArcaneArcher] = "Arcane Archer";
            _EnumToName[CharacterClassEnum.ArcaneTrickster] = "Arcane Trickster";
            _EnumToName[CharacterClassEnum.Assassin] = "Assassin";
            _EnumToName[CharacterClassEnum.DragonDisciple] = "Dragon Disciple";
            _EnumToName[CharacterClassEnum.Duelist] = "Duelist";
            _EnumToName[CharacterClassEnum.EldritchKnight] = "Eldritch Knight";
            _EnumToName[CharacterClassEnum.Loremaster] = "Loremaster";
            _EnumToName[CharacterClassEnum.MysticTheurge] = "Mystic Theurge";
            _EnumToName[CharacterClassEnum.PathfinderChronicler] = "Pathfinder Chronicler";
            _EnumToName[CharacterClassEnum.Shadowdancer] = "Shadowdancer";
            _EnumToName[CharacterClassEnum.BattleHerald] = "Battle Herald";
            _EnumToName[CharacterClassEnum.HolyVindicator] = "Holy Vindicator";
            _EnumToName[CharacterClassEnum.HorizonWalker] = "Horizon Walker";
            _EnumToName[CharacterClassEnum.Kineticist] = "Kineticist";
            _EnumToName[CharacterClassEnum.MasterChymist] = "Master Chymist";
            _EnumToName[CharacterClassEnum.MasterSpy] = "Master Spy";
            _EnumToName[CharacterClassEnum.Medium] = "Medium";
            _EnumToName[CharacterClassEnum.Mesmerist] = "Mesmerist";
            _EnumToName[CharacterClassEnum.NatureWarden] = "Nature Warden";
            _EnumToName[CharacterClassEnum.Ninja] = "Ninja";
            _EnumToName[CharacterClassEnum.Occultist] = "Occultist";
            _EnumToName[CharacterClassEnum.Psychic] = "Psychic";
            _EnumToName[CharacterClassEnum.RageProphet] = "Rage Prophet";
            _EnumToName[CharacterClassEnum.Samurai] = "Samurai";
            _EnumToName[CharacterClassEnum.Spiritualist] = "Spiritualist";
            _EnumToName[CharacterClassEnum.StalwartDefender] = "Stalwart Defender";
            _EnumToName[CharacterClassEnum.Adept] = "Adept";
            _EnumToName[CharacterClassEnum.Aristocrat] = "Aristocrat";
            _EnumToName[CharacterClassEnum.Commoner] = "Commoner";
            _EnumToName[CharacterClassEnum.Expert] = "Expert";
            _EnumToName[CharacterClassEnum.Warrior] = "Warrior";


            foreach (KeyValuePair<CharacterClassEnum, string> pair in _EnumToName)
            {
                _NameToEnum[pair.Value] = pair.Key;
            }
        }

        public static string GetName(CharacterClassEnum el)
        {
            return _EnumToName[el];
        }

        public static CharacterClassEnum GetEnum(string name)
        {
            return _NameToEnum[name];
        }

    }
}
