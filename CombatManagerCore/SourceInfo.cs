/*
 *  SourceInfo.cs
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
    public enum SourceType
    {
        Core,
        APG,
        AdventuresAndModules,
        ChroniclesAndCompanions,
        UltimateMagic,
        UltimateCombat,
        Other
    }


    public sealed class SourceInfo
    {
        private SourceInfo() { }

        private struct NameTypePair
        {
            public string Name;
            public SourceType Type;

            public NameTypePair(string text, SourceType type)
            {
                this.Name = text;
                this.Type = type;
            }
        }

        private static Dictionary<string, NameTypePair> sourceMap;

        static void AddToMap(string displayName, SourceType sourceType, params string[] sourceNames)
        {
            NameTypePair pair = new NameTypePair(displayName, sourceType);

            foreach (string source in sourceNames)
            {
                sourceMap.Add(source, pair);
            }

            sourceMap.Add(displayName, pair);
        }

        static SourceInfo()
        {
            sourceMap = new Dictionary<string, NameTypePair>(new InsensitiveEqualityCompararer());

            AddToMap("Academy Of Secrets", SourceType.AdventuresAndModules);
            AddToMap("Advanced Player's Guide", SourceType.APG, "AGP", "APG");
            AddToMap("Advanced Race Guide", SourceType.Other, "ARG");
            AddToMap("Adventurer's Armory", SourceType.ChroniclesAndCompanions, "AA", "PF CS", "PFC: AA");
            AddToMap("Andoran, Spirit of Liberty", SourceType.ChroniclesAndCompanions, "Andoran", "ASL", "PCh:ASoL");
            AddToMap("Bonus Bestiary", SourceType.Core);
            AddToMap("Book of the Damned - Volume 1: Princes of Darkness", SourceType.ChroniclesAndCompanions, "Book of the Dammed V1", "Book of the Damned V1");
            AddToMap("Book of the Damned - Volume 2: Lords of Chaos", SourceType.ChroniclesAndCompanions, "Book of the Dammed V2", "Book of the Damned V2");
            AddToMap("Carrion Crown: The Haunting of Harrowstone", SourceType.AdventuresAndModules, "AP 43", "AP43", "PAP43:CC1");
            AddToMap("Carrion Crown: Trial of the Beast", SourceType.AdventuresAndModules, "AP 44", "AP44", "PAP44:CC2");
            AddToMap("Carrion Crown: Broken Moon", SourceType.AdventuresAndModules, "AP 45", "AP45", "PAP45:CC3");
            AddToMap("Carrion Crown: Wake of the Watcher", SourceType.AdventuresAndModules, "AP 46", "AP46", "PAP46:CC4");
            AddToMap("Carrion Crown: Ashes at Dawn", SourceType.AdventuresAndModules, "AP 47", "AP47", "PAP47:CC5");
            AddToMap("Carrion Crown: Shadows of Gallowspire", SourceType.AdventuresAndModules, "AP 48", "AP48", "PAP48:CC6");
            AddToMap("Character Traits Web Enhancement", SourceType.ChroniclesAndCompanions, "P:CTWE");
            AddToMap("Cheliax, Empire Of Devils", SourceType.ChroniclesAndCompanions, "Cheliax", "Cheliax Empire Of Devils", "PCo:CEoD", "PFC: Cheliax");
            AddToMap("Cities of Golarion", SourceType.ChroniclesAndCompanions, "PCh:CoG");
            AddToMap("City of Golden Death", SourceType.AdventuresAndModules, "Golden Death");
            AddToMap("City of Strangers", SourceType.ChroniclesAndCompanions, "PCh:CoS");
            AddToMap("Classic Horrors Revisited", SourceType.ChroniclesAndCompanions, "PCh:CHR", "PFCh: CHR", "ClassicHorrors");
            AddToMap("Classic Monsters Revisited", SourceType.ChroniclesAndCompanions, "CMR", "PCh:CMR", "PFCh: CMR");
            AddToMap("Classic Treasures Revisited", SourceType.ChroniclesAndCompanions, "Classic Treasures", "PCh:CTR");
            AddToMap("Council of Thieves: Mother of Flies", SourceType.AdventuresAndModules, "AP 29", "AP29", "PAP29:CoT5");
            AddToMap("Council of Thieves: The Bastards of Erebus", SourceType.AdventuresAndModules, "AP 25", "AP25", "PAP25:CoT1");
            AddToMap("Council of Thieves: The Infernal Syndrome", SourceType.AdventuresAndModules, "AP 28", "AP28", "PAP28:CoT4");
            AddToMap("Council of Thieves: The Sixfold Trial", SourceType.AdventuresAndModules, "AP 26", "AP26", "PAP26:CoT2");
            AddToMap("Council of Thieves: The Twice-Damned Prince", SourceType.AdventuresAndModules, "AP 30", "AP30", "PAP30:CoT6");
            AddToMap("Council of Thieves: What Lies in Dust", SourceType.AdventuresAndModules, "AP 27", "AP27", "PAP27:CoT3");
            AddToMap("Cult Of The Ebon Destroyers", SourceType.AdventuresAndModules);
            AddToMap("Curse of the Crimson Throne Player's Guide", SourceType.AdventuresAndModules, "CCTPG", "PAP:CotCTPG");
            AddToMap("Curse of the Crimson Throne: A History of Ashes", SourceType.AdventuresAndModules, "AP 10", "AP10", "PAP10:CotCT4");
            AddToMap("Curse of the Crimson Throne: Crown of Fangs", SourceType.AdventuresAndModules, "AP 12", "AP12", "PAP12:CotCT6");
            AddToMap("Curse of the Crimson Throne: Edge of Anarchy", SourceType.AdventuresAndModules, "AP 7", "AP7", "PAP7:CotCT1");
            AddToMap("Curse of the Crimson Throne: Escape from Old Korvosa", SourceType.AdventuresAndModules, "AP 9", "AP9", "PAP9:CotCT3");
            AddToMap("Curse of the Crimson Throne: Seven Days to the Grave", SourceType.AdventuresAndModules, "AP 8", "AP8", "PAP8:CotCT2");
            AddToMap("Curse of the Crimson Throne: Skeletons of Scarwall", SourceType.AdventuresAndModules, "AP 11", "AP11", "PAP11:CotCT5");
            AddToMap("Curse of the Riven Sky", SourceType.AdventuresAndModules);
            AddToMap("Dark Markets - A Guide to Katapesh", SourceType.ChroniclesAndCompanions, "PCh:DM");
            AddToMap("Distant Worlds", SourceType.ChroniclesAndCompanions);
            AddToMap("Dragons Revisited", SourceType.ChroniclesAndCompanions, "PCh:DR");
            AddToMap("Dungeon Denizens Revisited", SourceType.ChroniclesAndCompanions, "PCh:DDR");
            AddToMap("Dungeons of Golarion", SourceType.ChroniclesAndCompanions, "DungeonsOfGolarion");
            AddToMap("Dwarves of Golarion", SourceType.ChroniclesAndCompanions, "DwarvesOfGolarion", "PCo:DoG", "PFC: DoG");
            AddToMap("Elves of Golarion", SourceType.ChroniclesAndCompanions, "PFC: Elves");
            AddToMap("Faction Guide", SourceType.ChroniclesAndCompanions);
            AddToMap("Faiths of Balance", SourceType.ChroniclesAndCompanions);
            AddToMap("Faiths of Purity", SourceType.ChroniclesAndCompanions);
            AddToMap("The Feast of Ravenmoor", SourceType.AdventuresAndModules);
            AddToMap("From Shore to Sea", SourceType.AdventuresAndModules);
            AddToMap("GameMastery Guide", SourceType.Core, "GMG");
            AddToMap("Gnomes of Golarion", SourceType.ChroniclesAndCompanions, "GG", "Gnomes", "PFC: GoG");
            AddToMap("Goblins of Golarion", SourceType.ChroniclesAndCompanions);
            AddToMap("The Godsmouth Heresy", SourceType.AdventuresAndModules, "Godsmouth Heresy");
            AddToMap("Guide to Darkmoon Vale", SourceType.ChroniclesAndCompanions, "PCh:GtDV");
            AddToMap("Guide to the River Kingdoms", SourceType.ChroniclesAndCompanions, "PCh:GttRK");
            AddToMap("The Harrowing", SourceType.AdventuresAndModules);
            AddToMap("Heart of the Jungle", SourceType.ChroniclesAndCompanions, "PCh:HotJ");
            AddToMap("Inner Sea Bestiary", SourceType.ChroniclesAndCompanions);
            AddToMap("Inner Sea Magic", SourceType.ChroniclesAndCompanions);
            AddToMap("Inner Sea World Guide", SourceType.ChroniclesAndCompanions);
            AddToMap("Jade Regent: The Brinewall Legacy", SourceType.AdventuresAndModules, "AP 49", "AP49");
            AddToMap("Jade Regent: Night of Frozen Shadows", SourceType.AdventuresAndModules, "AP 50", "AP50");
            AddToMap("Jade Regent: The Hungry Storm", SourceType.AdventuresAndModules, "AP 51", "AP51");
            AddToMap("Jade Regent: Forest of Spirits", SourceType.AdventuresAndModules, "AP 52", "AP52");
            AddToMap("Jade Regent: Tide of Honor", SourceType.AdventuresAndModules, "AP 53", "AP53");
            AddToMap("Jade Regent: The Empty Throne", SourceType.AdventuresAndModules, "AP 54", "AP54");
            AddToMap("Kingmaker: Blood for Blood", SourceType.AdventuresAndModules, "AP 34", "AP34", "PAP34:K4");
            AddToMap("Kingmaker: Rivers Run Red", SourceType.AdventuresAndModules, "AP 32", "AP32", "PAP32:K2");
            AddToMap("Kingmaker: Sound of a Thousand Screams", SourceType.AdventuresAndModules, "AP 36", "AP36", "PAP36:K6");
            AddToMap("Kingmaker: Stolen Land", SourceType.AdventuresAndModules, "AP 31", "AP31", "PAP31:K1");
            AddToMap("Kingmaker: The Varnhold Vanishing", SourceType.AdventuresAndModules, "AP 33", "AP33", "PAP33:K3");
            AddToMap("Kingmaker: War of the River Kings", SourceType.AdventuresAndModules, "AP 35", "AP35", "PAP35:K5");
            AddToMap("Legacy of Fire Player's Guide", SourceType.AdventuresAndModules, "LFPG", "LoF: PG", "PCo:LoFPG");
            AddToMap("Legacy of Fire: House of the Beast", SourceType.AdventuresAndModules, "AP 20", "AP20", "PAP20:LoF2");
            AddToMap("Legacy of Fire: Howl of the Carrion King", SourceType.AdventuresAndModules, "AP 19", "AP19", "PAP19:LoF1");
            AddToMap("Legacy of Fire: The End of Eternity", SourceType.AdventuresAndModules, "AP 22", "AP22", "PAP22:LoF4");
            AddToMap("Legacy of Fire: The Final Wish", SourceType.AdventuresAndModules, "AP 24", "AP24", "PAP24:LoF6");
            AddToMap("Legacy of Fire: The Impossible Eye", SourceType.AdventuresAndModules, "AP 23", "AP23", "PAP23:LoF5");
            AddToMap("Legacy of Fire: The Jackal's Price", SourceType.AdventuresAndModules, "AP 21", "AP21", "PAP21:LoF3");
            AddToMap("Lost Cities Of Golarion", SourceType.ChroniclesAndCompanions);
            AddToMap("NPC Guide", SourceType.ChroniclesAndCompanions);
            AddToMap("NPC Codex", SourceType.ChroniclesAndCompanions);
            AddToMap("NPC Guide Web", SourceType.ChroniclesAndCompanions);
            AddToMap("Master of the Fallen Fortress", SourceType.AdventuresAndModules);
            AddToMap("Misfit Monsters Redeemed", SourceType.ChroniclesAndCompanions, "Misfit Monsters");
            AddToMap("Module J2: Guardians of Dragonfall", SourceType.AdventuresAndModules, "PM:J2");
            AddToMap("Orcs of Golarion", SourceType.ChroniclesAndCompanions);
            AddToMap("Osirion, Land of Pharaohs", SourceType.ChroniclesAndCompanions, "Osirion", "PCo:OLoP");
            AddToMap("Pathfinder Bestiary", SourceType.Core, "Bestiary", "PFRPG Bestiary");
            AddToMap("Pathfinder Bestiary 2", SourceType.Core, "Bestiary 2", "PFRPG Bestiary 2");
            AddToMap("Pathfinder Bestiary 3", SourceType.Core, "Bestiary 3", "PFRPG Bestiary 3");
            AddToMap("Pathfinder Chronicles Campaign Setting", SourceType.ChroniclesAndCompanions, "Pathfinder Campaign Setting", "PCh:CS", "PCS", "PFCS", "PFRPG Chronicles Campaign Setting");
            AddToMap("Pathfinder Core Rulebook", SourceType.Core, "Core", "PF Core", "PFRPG CORE", "PFRPG Core Rulebook");
            AddToMap("Pathfinder Society Field Guide", SourceType.ChroniclesAndCompanions, "PFRPG Society Field Guide");
            AddToMap("Pathfinder Society S0-05", SourceType.AdventuresAndModules, "PFS S0-05");
            AddToMap("Pathfinder Society S1-29", SourceType.AdventuresAndModules, "PFS S1-29");
            AddToMap("Pathfinder Society S1-30", SourceType.AdventuresAndModules, "PFS S1-30");
            AddToMap("Pathfinder Society S1-31", SourceType.AdventuresAndModules, "PFS S1-31");
            AddToMap("Pathfinder Society S1-32", SourceType.AdventuresAndModules, "PFS S1-32");
            AddToMap("Pathfinder Society S1-33", SourceType.AdventuresAndModules, "PFS S1-33");
            AddToMap("Pathfinder Society S1-34", SourceType.AdventuresAndModules, "PFS S1-34");
            AddToMap("Pathfinder Society S1-35", SourceType.AdventuresAndModules, "PFS S1-35");
            AddToMap("Pathfinder Society S1-36", SourceType.AdventuresAndModules, "PFS S1-36");
            AddToMap("Pathfinder Society S1-37", SourceType.AdventuresAndModules, "PFS S1-37");
            AddToMap("Pathfinder Society S1-38", SourceType.AdventuresAndModules, "PFS S1-38");
            AddToMap("Pathfinder Society S1-39", SourceType.AdventuresAndModules, "PFS S1-39");
            AddToMap("Pathfinder Society S1-40", SourceType.AdventuresAndModules, "PFS S1-40");
            AddToMap("Pathfinder Society S1-41", SourceType.AdventuresAndModules, "PFS S1-41");
            AddToMap("Pathfinder Society S1-42", SourceType.AdventuresAndModules, "PFS S1-42");
            AddToMap("Pathfinder Society S1-43", SourceType.AdventuresAndModules, "PFS S1-43");
            AddToMap("Pathfinder Society S1-44", SourceType.AdventuresAndModules, "PFS S1-44");
            AddToMap("Pathfinder Society S1-45", SourceType.AdventuresAndModules, "PFS S1-45");
            AddToMap("Pathfinder Society S1-46", SourceType.AdventuresAndModules, "PFS S1-46");
            AddToMap("Pathfinder Society S1-47", SourceType.AdventuresAndModules, "PFS S1-47");
            AddToMap("Pathfinder Society S1-48", SourceType.AdventuresAndModules, "PFS S1-48");
            AddToMap("Pathfinder Society S1-49", SourceType.AdventuresAndModules, "PFS S1-49");
            AddToMap("Pathfinder Society S1-50", SourceType.AdventuresAndModules, "PFS S1-50");
            AddToMap("Pathfinder Society S1-51", SourceType.AdventuresAndModules, "PFS S1-51");
            AddToMap("Pathfinder Society S1-52", SourceType.AdventuresAndModules, "PFS S1-52");
            AddToMap("Pathfinder Society S1-53", SourceType.AdventuresAndModules, "PFS S1-53");
            AddToMap("Pathfinder Society S1-54", SourceType.AdventuresAndModules, "PFS S1-54");
            AddToMap("Pathfinder Society S1-55", SourceType.AdventuresAndModules, "PFS S1-55");
            AddToMap("Pathfinder Society S1-56", SourceType.AdventuresAndModules, "PFS S1-56");
            AddToMap("Qadira, Gateway to the East", SourceType.ChroniclesAndCompanions, "PCo:QGttE", "Qadira");
            AddToMap("Rival Guide", SourceType.ChroniclesAndCompanions);
            AddToMap("Realm of the Fellnight Queen", SourceType.AdventuresAndModules, "Fellnight Queen");
            AddToMap("Reign of Winter: The Snows of Summer", SourceType.AdventuresAndModules, "AP 67", "AP67");
            AddToMap("Reign of Winter: The Shackled Hut", SourceType.AdventuresAndModules, "AP 68", "AP68");
            AddToMap("Reign of Winter: Maiden, Mother, Crone", SourceType.AdventuresAndModules, "AP 69", "AP69");
            AddToMap("Reign of Winter: The Frozen Stars", SourceType.AdventuresAndModules, "AP 70", "AP70");
            AddToMap("Reign of Winter: Rasputin Must Die!", SourceType.AdventuresAndModules, "AP 71", "AP71");
            AddToMap("Reign of Winter: The Witch Queen’s Revenge", SourceType.AdventuresAndModules, "AP 72", "AP72");
            AddToMap("Rise of the Runelords", SourceType.AdventuresAndModules, "RotRL-AE-Appendix");
            AddToMap("Rise of the Runelords Player's Guide", SourceType.AdventuresAndModules, "PAP:RotRPG", "RRPG");
            AddToMap("Rise of the Runelords: Burnt Offerings", SourceType.AdventuresAndModules, "AP 1", "AP1", "PAP1:RotR1", "RotRL-AE-Burnt Offerings");
            AddToMap("Rise of the Runelords: Fortress of the Stone Giants", SourceType.AdventuresAndModules, "AP 4", "AP4", "PAP4:RotR4", "RotRL-AE-Fortress of the Stone Giants");
            AddToMap("Rise of the Runelords: Hook Mountain Massacre", SourceType.AdventuresAndModules, "AP 3", "AP3", "PAP3:RotR3", "RotRL-AE-Hook Mountain Massacre");
            AddToMap("Rise of the Runelords: Sins of the Saviors", SourceType.AdventuresAndModules, "AP 5", "AP5", "PAP5:RotR5", "RotRL-AE-Sins of the Saviors");
            AddToMap("Rise of the Runelords: Spires of Xin-Shalast", SourceType.AdventuresAndModules, "AP 6", "AP6", "PAP6:RotR6", "RotRL-AE-Spires of Xin-Shalast");
            AddToMap("Rise of the Runelords: The Skinsaw Murders", SourceType.AdventuresAndModules, "AP 2", "AP2", "PAP2:RotR2", "Pathfinder 2: The Skinsaw Murders", "RotRL-AE-The Skinsaw Murders");
            AddToMap("Sargava, the Lost Colony", SourceType.ChroniclesAndCompanions, "Pathfinder Companion: Sargava, the Lost Colony", "Sargava");
            AddToMap("Sanctum of the Serpent God", SourceType.AdventuresAndModules, "AP 42");
            AddToMap("Second Darkness Player's Guide", SourceType.AdventuresAndModules, "PCo:SD", "SDPG");
            AddToMap("Second Darkness: A Memory of Darkness", SourceType.AdventuresAndModules, "AP 17", "AP17", "PAP17:SD5");
            AddToMap("Second Darkness: Children of the Void", SourceType.AdventuresAndModules, "AP 14", "AP14", "PAP14:SD2", "PF #14");
            AddToMap("Second Darkness: Descent into Midnight", SourceType.AdventuresAndModules, "AP 18", "AP18", "PAP18:SD6");
            AddToMap("Second Darkness: Endless Night", SourceType.AdventuresAndModules, "AP 16", "AP16", "PAP16:SD4");
            AddToMap("Second Darkness: Shadow in the Sky", SourceType.AdventuresAndModules, "AP 13", "AP13", "PAP13:SD1");
            AddToMap("Second Darkness: The Armageddon Echo", SourceType.AdventuresAndModules, "AP 15", "AP15", "PAP15:SD3", "PF #15");
            AddToMap("Seekers of Secrets", SourceType.ChroniclesAndCompanions, "PCh:SoS", "Seeker of Secrets", "Seekers");
            AddToMap("Serpent's Skull: City of Seven Spears", SourceType.AdventuresAndModules, "AP 39", "AP39");
            AddToMap("Serpent's Skull: Racing to Ruin", SourceType.AdventuresAndModules, "AP 38", "AP38");
            AddToMap("Serpent's Skull: Souls for Smuggler's Shiv", SourceType.AdventuresAndModules, "AP 37", "AP37");
            AddToMap("Serpent's Skull: The Thousand Fangs Below", SourceType.AdventuresAndModules, "AP 41", "AP41");
            AddToMap("Serpent's Skull: Vaults of Madness", SourceType.AdventuresAndModules, "AP 40", "AP40");
            AddToMap("Shattered Star: Shards of Sin", SourceType.AdventuresAndModules, "AP 61", "AP61");
            AddToMap("Shattered Star: Curse of the Lady's Light", SourceType.AdventuresAndModules, "AP 62", "AP62");
            AddToMap("Shattered Star: The Asylum Stone", SourceType.AdventuresAndModules, "AP 63", "AP63");
            AddToMap("Shattered Star: Beyond the Doomsday Door", SourceType.AdventuresAndModules, "AP 64", "AP64");
            AddToMap("Shattered Star: Into the Nightmare Rift", SourceType.AdventuresAndModules, "AP 65", "AP65");
            AddToMap("Shattered Star: The Dead Heart of Xin", SourceType.AdventuresAndModules, "AP 66", "AP66");
            AddToMap("Skull & Shackles: The Wormwood Mutiny", SourceType.AdventuresAndModules, "AP 55", "AP55");
            AddToMap("Skull & Shackles: Raiders of the Fever Sea", SourceType.AdventuresAndModules, "AP 56", "AP56");
            AddToMap("Skull & Shackles: Tempest Rising", SourceType.AdventuresAndModules, "AP 57", "AP57");
            AddToMap("Skull & Shackles: Island of Empty Eyes", SourceType.AdventuresAndModules, "AP 58", "AP58");
            AddToMap("Skull & Shackles: The Price of Infamy", SourceType.AdventuresAndModules, "AP 59", "AP59");
            AddToMap("Skull & Shackles: From Hell's Heart", SourceType.AdventuresAndModules, "AP 60", "AP60");
            AddToMap("Taldor, Echoes of Glory", SourceType.ChroniclesAndCompanions, "PCo:TEoG", "Taldor");
            AddToMap("Tomb Of The Iron Medusa", SourceType.AdventuresAndModules);
            AddToMap("Ultimate Combat", SourceType.UltimateCombat, "UC");
            AddToMap("Ultimate Equipment", SourceType.Other, "UE");
            AddToMap("Ultimate Magic", SourceType.UltimateMagic, "UM");
            AddToMap("Undead Revisited", SourceType.ChroniclesAndCompanions);
            AddToMap("Wayfinder #1", SourceType.Other, "Wayfinder 1");
            AddToMap("Wayfinder #2", SourceType.Other, "Wayfinder 2");
            AddToMap("Wayfinder #3", SourceType.Other, "Wayfinder 3");
            AddToMap("We Be Goblins!", SourceType.AdventuresAndModules);
            AddToMap("The Witchwar Legacy", SourceType.AdventuresAndModules, "Witch War");
            AddToMap("Wrath of the Righteous: The Worldwound Incursion", SourceType.AdventuresAndModules, "AP 73", "AP73");
            AddToMap("Wrath of the Righteous: Sword of Valor", SourceType.AdventuresAndModules, "AP 74", "AP74");
            AddToMap("Wrath of the Righteous: Demon's Heresy", SourceType.AdventuresAndModules, "AP 75", "AP75");
            AddToMap("Wrath of the Righteous: The Midnight Isles", SourceType.AdventuresAndModules, "AP 76", "AP76");
            AddToMap("Wrath of the Righteous: Herald of the Ivory Labyrinth", SourceType.AdventuresAndModules, "AP 77", "AP77");
            AddToMap("Wrath of the Righteous: City of Locusts", SourceType.AdventuresAndModules, "AP 78", "AP78");

        }

        public static string GetSourceOrDie(string code)
        {
            NameTypePair source;
            if (sourceMap.TryGetValue(code, out source))
            {
                return source.Name;
            }

            System.Diagnostics.Debug.WriteLine("Source not found: " + code);

            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }

            throw new Exception("Source not found: " + code);
        }

        public static string GetSource(string code)
        {
            if (code != null)
            {
                NameTypePair source;
                if (sourceMap.TryGetValue(code, out source))
                {
                    return source.Name;
                }
                System.Diagnostics.Debug.WriteLine("Source not found: " + code);
            }

            return "";
        }

        public static SourceType GetSourceType(string code)
        {
            if (code != null)
            {
                NameTypePair source;
                if (sourceMap.TryGetValue(code, out source))
                {
                    return source.Type;
                }
                System.Diagnostics.Debug.WriteLine("Source not found: " + code);
            }

            return SourceType.Other;
        }

        public static SourceType GetSourceTypeOrDie(string code)
        {
            if (code != null)
            {
                NameTypePair source;

                if (sourceMap.TryGetValue(code, out source))
                {
                    return source.Type;
                }

                System.Diagnostics.Debug.WriteLine("Source not found: " + code);

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Break();
                }

                throw new Exception("Source not found: " + code);
            }

            return SourceType.Other;
        }
    }
}
