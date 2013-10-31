/*
 *  MonsterHtmlCreator.cs
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

using System;
using System.Text;
using CombatManager;
using System.Collections.Generic;


namespace CombatManager
{
	public class MonsterHtmlCreator
	{
		public static string CreateHtml(Monster monster)
		{
			return CreateHtml(monster, null);
		}
		
		public static string CreateHtml(Monster monster, Character ch)
		{
			return CreateHtml(monster, ch, true);
		}
		
		public static string CreateHtml(Monster monster, Character ch, bool addDescription)
		{
			StringBuilder blocks = new StringBuilder();
			blocks.CreateHtmlHeader();
			

            CreateTopSection(monster, ch, blocks);

            CreateDefenseSection(monster, blocks);

            CreateOffenseSection(monster, blocks);
            CreateTacticsSection(monster, blocks);
            CreateStatisticsSection(monster, blocks);
            CreateEcologySection(monster, blocks);
            CreateSpecialAbilitiesSection(monster, blocks);
            if (addDescription)
            {
                CreateDescriptionSection(monster, blocks);
            }

           

            if (SourceInfo.GetSourceType(monster.Source) != SourceType.Core)
            {
                
                blocks.CreateItemIfNotNull("Source ", SourceInfo.GetSource(monster.Source));
                
            }

            
			
			blocks.CreateHtmlFooter();
		
			return blocks.ToString();
		}
		
		private static void CreateTopSection(Monster monster, Character ch, StringBuilder blocks)
        {
            string name = monster.Name;

            if ((name == null || name.Length == 0) && ch != null)
            {
                name = ch.Name;
            }

            string header = "CR " + monster.CR;
            if (monster.MR != null && monster.MR > 0)
            {
                header += "/MR " + monster.MR;
            }

            blocks.CreateHeader(name, header);
			
			
			blocks.AppendOpenTag("p");
			blocks.AppendTag("b", "XP: " + monster.XP);
			blocks.AppendOpenTag ("br");
            if (monster.Race.NotNullString () && monster.Class.NotNullString ())
            {
				blocks.AppendHtml (monster.Race + " " + monster.Class);
				blocks.AppendOpenTag ("br");
        	}
			
			blocks.AppendHtml (monster.Alignment + " " + monster.Size).AppendSpace ();
            blocks.AppendLineBreak ();
			
			blocks.CreateItemIfNotNull(null, false, monster.Type, " ", false);
            blocks.CreateItemIfNotNull(null, false, monster.SubType, null, false);
            blocks.AppendLineBreak ();
            blocks.CreateItemIfNotNull("Init ", true, monster.Init.PlusFormat(), "; ", false);
            blocks.CreateItemIfNotNull("Senses ", true, monster.Senses, null, false);
            if (monster.Aura != null && monster.Aura.Length > 0)
            {
            	blocks.AppendLineBreak ();
                blocks.CreateItemIfNotNull("Aura ", true, monster.Aura, null, false);
            }

            blocks.AppendLineBreak ();

			blocks.AppendCloseTag ("p");
        }
		
		private static void CreateDefenseSection(Monster monster, StringBuilder blocks)
        {
            blocks.CreateSectionHeader("DEFENSE");

            
			blocks.AppendOpenTag("p");
            blocks.CreateItemIfNotNull("AC ", true, monster.FullAC.ToString(), ", ", false);
            blocks.CreateItemIfNotNull("touch ", false, monster.TouchAC.ToString(), ", ", false);
            blocks.CreateItemIfNotNull("flat-footed ", false, monster.FlatFootedAC.ToString(), null, false);
            blocks.CreateItemIfNotNull(" ", false, monster.AC_Mods, null, false);
            blocks.AppendLineBreak();
            blocks.CreateItemIfNotNull("HP ", true, monster.HP.ToString(), null, false);
            if (monster.HD != "1d1")
            {
                blocks.CreateItemIfNotNull(" ", false, monster.HD, null, false);
            }
            blocks.CreateItemIfNotNull("; ", false, monster.HP_Mods, null, false);
            blocks.AppendLineBreak();
            blocks.CreateItemIfNotNull("Fort ", true, monster.Fort.PlusFormat(), ", ", false);
            blocks.CreateItemIfNotNull("Ref ", true, monster.Ref.PlusFormat(), ", ", false);
            blocks.CreateItemIfNotNull("Will ", true, monster.Will.PlusFormat(), null, false);
            blocks.CreateItemIfNotNull("; ", true, monster.Save_Mods, null, false);
            blocks.AppendLineBreak();
            List<TitleValuePair> defLine = new List<TitleValuePair>();
            defLine.Add(new TitleValuePair { Title = "Defensive Abilities ", Value = monster.DefensiveAbilities });
            defLine.Add(new TitleValuePair { Title = "DR ", Value = monster.DR });
            defLine.Add(new TitleValuePair { Title = "Immune ", Value = monster.Immune });
            defLine.Add(new TitleValuePair { Title = "SR ", Value = monster.SR });
            defLine.Add(new TitleValuePair { Title = "Resist ", Value = monster.Resist });
            defLine.Add(new TitleValuePair { Title = "Weaknesses ", Value = monster.Weaknesses });

            blocks.CreateMultiValueLine(defLine, "; ");


            blocks.AppendCloseTag("p");
        }

        private static void CreateOffenseSection(Monster monster, StringBuilder blocks)
        {
            blocks.CreateSectionHeader("OFFENSE");
			
			blocks.AppendOpenTag("p");
            blocks.CreateItemIfNotNull("Speed ", monster.Speed);
            blocks.CreateItemIfNotNull("Melee ", monster.Melee);
            blocks.CreateItemIfNotNull("Ranged ", monster.Ranged);


            List<TitleValuePair> reachLine = new List<TitleValuePair>();
            reachLine.Add(new TitleValuePair { Title = "Space ", Value = monster.Space });
            reachLine.Add(new TitleValuePair { Title = "Reach ", Value = monster.Reach });
            blocks.CreateMultiValueLine(reachLine, ", ");

            
            blocks.CreateItemIfNotNull("Special Attacks ", monster.SpecialAttacks);
            
            string spellLike = monster.SpellLikeAbilities;
            if (spellLike != null && spellLike.Length > 0)
            {

                if (spellLike.IndexOf("Spell-Like Abilities ") == 0)
                {
                    spellLike = spellLike.Substring("Spell-Like Abilities ".Length);
                }

                blocks.CreateItemIfNotNull("Spell-Like Abilities ", spellLike);
                

            }
            string spellsKnown = monster.SpellsKnown;
            if (spellsKnown != null && spellsKnown.Length > 0)
            {
                if (spellsKnown.IndexOf("Spells Known ") == 0)
                {
                    spellsKnown = spellsKnown.Substring("Spells Known ".Length);

                }
                blocks.CreateItemIfNotNull("Spells Known ", spellsKnown);
                
            }
            string spellsPrepared = monster.SpellsPrepared;
            if (spellsPrepared != null && spellsPrepared.Length > 0)
            {
                if (spellsPrepared.IndexOf("Spells Prepared ") == 0)
                {
                    spellsPrepared = spellsPrepared.Substring("Spells Prepared ".Length);

                }
                blocks.CreateItemIfNotNull("Spells Prepared ", spellsPrepared);
            
            }

			
			blocks.AppendCloseTag("p");
        }

		 
        private static void CreateTacticsSection(Monster monster, StringBuilder blocks)
        {
            if (monster.BeforeCombat.NotNullString() ||
                monster.DuringCombat.NotNullString() ||
                monster.Morale.NotNullString())
            {

                blocks.CreateSectionHeader("TACTICS");

                blocks.AppendOpenTag("p");


                blocks.CreateItemIfNotNull("Before Combat ", monster.BeforeCombat);
                blocks.CreateItemIfNotNull("During Combat ", monster.DuringCombat);
                blocks.CreateItemIfNotNull("Morale ", monster.Morale);

                blocks.AppendCloseTag("p");


            }
        }

        private static void CreateStatisticsSection(Monster monster, StringBuilder blocks)
        {
            blocks.CreateSectionHeader("STATISTICS");

			
            blocks.AppendOpenTag("p");

            List<TitleValuePair> statsLine = new List<TitleValuePair>();
            statsLine.Add(new TitleValuePair
            {
                Title = "Str ",
                Value = (monster.Strength == null) ? "-" : monster.Strength.ToString()
            });
            statsLine.Add(new TitleValuePair
            {
                Title = "Dex ",
                Value = (monster.Dexterity == null) ? "-" : monster.Dexterity.ToString()
            });
            statsLine.Add(new TitleValuePair
            {
                Title = "Con ",
                Value = (monster.Constitution == null) ? "-" : monster.Constitution.ToString()
            });
            statsLine.Add(new TitleValuePair
            {
                Title = "Int ",
                Value = (monster.Intelligence == null) ? "-" : monster.Intelligence.ToString()
            });
            statsLine.Add(new TitleValuePair
            {
                Title = "Wis ",
                Value = (monster.Wisdom == null) ? "-" : monster.Wisdom.ToString()
            });
            statsLine.Add(new TitleValuePair
            {
                Title = "Cha ",
                Value = (monster.Charisma == null) ? "-" : monster.Charisma.ToString()
            });

            blocks.CreateMultiValueLine(statsLine, ", ");

            List<TitleValuePair> combatLine = new List<TitleValuePair>();
            combatLine.Add(new TitleValuePair { Title = "Base Atk ", Value = monster.BaseAtk.PlusFormat() });
            combatLine.Add(new TitleValuePair { Title = "CMB ", Value = monster.CMB });
            combatLine.Add(new TitleValuePair { Title = "CMD ", Value = monster.CMD });
            blocks.CreateMultiValueLine(combatLine, "; ");


            if (monster.FeatsList.Count > 0)
            {

                int count = 0;


                blocks.AppendEscapedTag("sp", "bolded", "Feats");
				blocks.AppendSpace();
				string text = "";
                foreach (string feat in monster.FeatsList)
                {
                    if (count > 0)
                    {
                        text += ", ";
                    }

                    text += feat;

                    count++;
                }
                blocks.AppendHtml(text);
				blocks.AppendLineBreak();

            }

            if (monster.SkillValueDictionary.Count > 0)
            {
                string skillList = "";

                int count = 0;

                foreach (SkillValue val in monster.SkillValueDictionary.Values)
                {
                    if (count > 0)
                    {
                        skillList += ", ";
                    }

                    skillList += val.Text;
                    count++;
                }

                blocks.CreateItemIfNotNull("Skills ", skillList);
            }

            blocks.CreateItemIfNotNull("  Racial Modifiers ", monster.RacialMods);
            blocks.CreateItemIfNotNull("Languages ", monster.Languages);
            blocks.CreateItemIfNotNull("SQ ", monster.SQ);
            blocks.CreateItemIfNotNull("Gear ", monster.Gear);


            
            blocks.AppendCloseTag("p");
        }

        private static void CreateEcologySection(Monster monster, StringBuilder  blocks)
        {
            if ((monster.Environment != null && monster.Environment.Length > 0) ||
                (monster.Organization != null && monster.Organization.Length > 0) ||
                (monster.Treasure != null && monster.Treasure.Length > 0))
            {

                blocks.CreateSectionHeader("ECOLOGY");

				
				blocks.AppendOpenTag("p");
				
				blocks.CreateItemIfNotNull("Environment ", monster.Environment);
				blocks.CreateItemIfNotNull("Organization ", monster.Organization);
				blocks.CreateItemIfNotNull("Treasure ", monster.Treasure);
				
				
				blocks.AppendCloseTag("p");

            }
        }

        private static void CreateSpecialAbilitiesSection(Monster monster, StringBuilder  blocks)
        {
            if (monster.SpecialAbilitiesList.Count > 0)
            {

                blocks.CreateSectionHeader("SPECIAL ABILITIES");

				blocks.AppendOpenTag("p");
				

                foreach (SpecialAbility spec in monster.SpecialAbilitiesList)
                {
                    if (spec.Name.Length > 0)
                    {

                        string type = spec.Type;

                        if (spec.ConstructionPoints != null)
                        {
                            type += ", " + spec.ConstructionPoints + " CP";
                        }

                        blocks.CreateItemIfNotNull(spec.Name + " (" + type + ") "
                            , spec.Text);
                    }
                    else
                    {

                        blocks.CreateItemIfNotNull(null, spec.Text);
                    }

                }
				
				blocks.AppendCloseTag("p");


            }
        }

        private static void CreateDescriptionSection(Monster monster, StringBuilder  blocks)
        {

            if (monster.Description_Visual != null && monster.Description_Visual.Length > 0)
            {
				
				blocks.AppendOpenTagWithClass("p", "visualdescription");
				
                blocks.AppendHtml(monster.Description_Visual);
				
				blocks.AppendCloseTag("p");

            }



            if (monster.DescHTML != null && monster.DescHTML.Length > 0)
            {
				
				blocks.AppendOpenTagWithClass("p", "description");
                blocks.Append(monster.DescHTML);
				blocks.AppendCloseTag("p");

            }
            else if (monster.Description != null && monster.Description.Length > 0)
            {
				
				blocks.AppendOpenTagWithClass("span", "description");

                blocks.AppendHtml(monster.Description);


				blocks.AppendCloseTag("span");

            }
			
        }

	}
}

