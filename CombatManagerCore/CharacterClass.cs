using System;
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
        ArcaneArcher,
        ArcaneTrickster,
        Assassin,
        DragonDisciple,
        Duelist,
        EldritchKnight,
        Loremaster,
        MysticTheurge,
        PathfinderChronicler,
        Shadowdancer
    }

    public static class CharacterClass
    {
        static Dictionary<CharacterClassEnum, string> _EnumToName = new Dictionary<CharacterClassEnum, string>();
        static Dictionary<string, CharacterClassEnum> _NameToEnum = new Dictionary<string, CharacterClassEnum>(new InsensitiveEqualityCompararer());

        static CharacterClass()
        {
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
