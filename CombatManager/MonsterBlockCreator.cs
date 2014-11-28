/*
 *  MonsterBlockCreator.cs
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
using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
﻿using System.Windows.Forms.VisualStyles;
﻿using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace CombatManager
{



    public class MonsterBlockCreator : BlockCreator
    {




        public enum MonsterBlockSection
        {
            Top,
            Defense,
            Offense,
            Tactics,
            Statistics,
            Ecology,
            SpecialAbilities,
            Description
        }

        DocumentLinkHander _LinkHandler;


        static string _ClassRegexString;
        static MonsterBlockCreator()
        {
            bool first = true;
            _ClassRegexString = "(";
            foreach (Rule rule in Rule.Rules.Where(a => (a.Type == "Classes")))
            {
                if (!first)
                {
                    _ClassRegexString += "|";
                }
                first = false;
                _ClassRegexString += rule.Name;
            }
            _ClassRegexString += ")";
               
        }

        public MonsterBlockCreator(FlowDocument document, DocumentLinkHander linkHandler): base(document)
        {
            _LinkHandler = linkHandler;
        }




        public List<Block> CreateBlocks(Character ch, bool addDescription)
        {
            return CreateBlocks(ch.Monster, ch, addDescription);
        }

        public List<Block> CreateBlocks(Monster monster, bool addDescription)
        {
            return CreateBlocks(monster, null, addDescription);
        }


        private List<Block> CreateBlocks(Monster monster, Character ch, bool addDescription)
        {

            
            List<Block> blocks = new List<Block>();

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
                Paragraph sourceParagraph = new Paragraph();

                sourceParagraph.Margin = new Thickness(0);
                CreateItemIfNotNull(sourceParagraph.Inlines, "Source ", SourceInfo.GetSource(monster.Source));
                blocks.Add(sourceParagraph);
            }

            
            return blocks;
        }

        private void CreateTopSection(Monster monster, Character ch, List<Block> blocks)
        {
            string name = monster.Name;

            if ((name == null || name.Length == 0) && ch != null)
            {
                name = ch.Name;
            }

            string header =  "CR " + monster.CR;
            if (monster.MR != null && monster.MR > 0)
            {
                header += "/MR " + monster.MR;
            }

            blocks.Add(CreateHeaderParagraph(name, header));

            Paragraph topParagraph = new Paragraph();
            topParagraph.Inlines.Add(new Bold(new Run("XP: " + monster.XP)));
            topParagraph.Inlines.Add(new LineBreak());
            topParagraph.Margin = new Thickness(0);

            if (NotNullString(monster.Race) && NotNullString(monster.Class))
            {
                topParagraph.Inlines.Add(monster.Race + " " + monster.Class);
                topParagraph.Inlines.Add(new LineBreak());
            }
            topParagraph.Inlines.Add(monster.Alignment + " " + monster.Size + " ");
            topParagraph.Inlines.AddRange(CreateItemIfNotNull(null, false, monster.Type, " ", false));
            topParagraph.Inlines.AddRange(CreateItemIfNotNull(null, false, monster.SubType, null, false));
            topParagraph.Inlines.Add(new LineBreak());
            if (monster.DualInit != null)
            {
                topParagraph.Inlines.AddRange(CreateItemIfNotNull("Init ", true, monster.Init.PlusFormat(), "/", false));
                topParagraph.Inlines.AddRange(CreateItemIfNotNull(null, true, monster.DualInit.PlusFormat(), ", dual initiative; ", false));
            }
            else
            {
                topParagraph.Inlines.AddRange(CreateItemIfNotNull("Init ", true, monster.Init.PlusFormat(), "; ", false));
            }
            topParagraph.Inlines.AddRange(CreateItemIfNotNull("Senses ", true, monster.Senses, null, false));
            if (monster.Aura != null && monster.Aura.Length > 0)
            {
                topParagraph.Inlines.Add(new LineBreak());
                topParagraph.Inlines.AddRange(CreateItemIfNotNull("Aura ", true, monster.Aura, null, false));
            }

            topParagraph.Inlines.Add(new LineBreak());

            blocks.Add(topParagraph);
        }

        private void CreateDefenseSection(Monster monster, List<Block> blocks)
        {
            blocks.AddRange(CreateSectionHeader("DEFENSE"));

            Paragraph defensesParagraph = new Paragraph();
            defensesParagraph.Margin = new Thickness(0);
            defensesParagraph.Inlines.AddRange(CreateItemIfNotNull("AC ", true, monster.FullAC.ToString(), ", ", false));
            defensesParagraph.Inlines.AddRange(CreateItemIfNotNull("touch ", false, monster.TouchAC.ToString(), ", ", false));
            defensesParagraph.Inlines.AddRange(CreateItemIfNotNull("flat-footed ", false, monster.FlatFootedAC.ToString(), null, false));
            defensesParagraph.Inlines.AddRange(CreateItemIfNotNull(" ", false, monster.AC_Mods, null, false));
            defensesParagraph.Inlines.Add(new LineBreak());
            defensesParagraph.Inlines.AddRange(CreateItemIfNotNull("HP ", true, monster.HP.ToString(), null, false));
            if (monster.HD != "1d1")
            {
                defensesParagraph.Inlines.AddRange(CreateItemIfNotNull(" ", false, monster.HD, null, false));
            }
            defensesParagraph.Inlines.AddRange(CreateItemIfNotNull("; ", false, monster.HP_Mods, null, false));
            defensesParagraph.Inlines.Add(new LineBreak());
            defensesParagraph.Inlines.AddRange(CreateItemIfNotNull("Fort ", true, monster.Fort.PlusFormat(), ", ", false));
            defensesParagraph.Inlines.AddRange(CreateItemIfNotNull("Ref ", true, monster.Ref.PlusFormat(), ", ", false));
            defensesParagraph.Inlines.AddRange(CreateItemIfNotNull("Will ", true, monster.Will.PlusFormat(), null, false));
            defensesParagraph.Inlines.AddRange(CreateItemIfNotNull("; ", true, monster.Save_Mods, null, false));
            defensesParagraph.Inlines.Add(new LineBreak());
            List<TitleValuePair> defLine = new List<TitleValuePair>();
            defLine.Add(new TitleValuePair { Title = "Defensive Abilities ", Value = monster.DefensiveAbilities });
            defLine.Add(new TitleValuePair { Title = "DR ", Value = monster.DR });
            defLine.Add(new TitleValuePair { Title = "Immune ", Value = monster.Immune });
            defLine.Add(new TitleValuePair { Title = "SR ", Value = monster.SR });
            defLine.Add(new TitleValuePair { Title = "Resist ", Value = monster.Resist });
            defLine.Add(new TitleValuePair { Title = "Weaknesses ", Value = monster.Weaknesses });

            defensesParagraph.Inlines.AddRange(CreateMultiValueLine(defLine, "; "));


            blocks.Add(defensesParagraph);
        }

        private void CreateOffenseSection(Monster monster, List<Block> blocks)
        {
                    blocks.AddRange(CreateSectionHeader("OFFENSE"));


            Paragraph offenseParagraph = new Paragraph();
            offenseParagraph.Margin = new Thickness(0);
            offenseParagraph.Inlines.AddRange(CreateItemIfNotNull("Speed ", monster.Speed));
            offenseParagraph.Inlines.AddRange(CreateItemIfNotNull("Melee ", monster.Melee));
            offenseParagraph.Inlines.AddRange(CreateItemIfNotNull("Ranged ", monster.Ranged));


            List<TitleValuePair> reachLine = new List<TitleValuePair>();
            reachLine.Add(new TitleValuePair { Title = "Space ", Value = monster.Space });
            reachLine.Add(new TitleValuePair { Title = "Reach ", Value = monster.Reach });
            offenseParagraph.Inlines.AddRange(CreateMultiValueLine(reachLine, ", "));

            
            offenseParagraph.Inlines.AddRange(CreateItemIfNotNull("Special Attacks ", monster.SpecialAttacks));
            
            string spellLike = monster.SpellLikeAbilities;
            if (spellLike != null && spellLike.Length > 0)
            {
                List<Inline> spellInlines = null;
                if (_LinkHandler != null)
                {
                    spellInlines = CreateSpellsBlockItem(monster.SpellLikeAbilitiesBlock);
                }

                if (spellInlines != null && spellInlines.Count > 0)
                {
                    offenseParagraph.Inlines.AddRange(spellInlines);
                }
                else
                {
                    if (spellLike.IndexOf("Spell-Like Abilities ") == 0)
                    {
                        spellLike = spellLike.Substring("Spell-Like Abilities ".Length);
                    }

                    offenseParagraph.Inlines.AddRange(CreateItemIfNotNull("Spell-Like Abilities ", spellLike));
                }

            }
            if (monster.SpellsKnownBlock == null)
            {
                string spellsKnown = monster.SpellsKnown;
                if (spellsKnown != null && spellsKnown.Length > 0)
                {
                    List<Inline> spellInlines = null;
                    if (_LinkHandler != null)
                    {
                        spellInlines = CreateSpellsBlockItem(monster.SpellsKnownBlock);
                    }

                    if (spellInlines != null && spellInlines.Count > 0)
                    {
                        offenseParagraph.Inlines.AddRange(spellInlines);
                    }
                    else
                    {
                        if (spellsKnown.IndexOf("Spells Known ") == 0)
                        {
                            spellsKnown = spellsKnown.Substring("Spells Known ".Length);

                        }
                        offenseParagraph.Inlines.AddRange(CreateItemIfNotNull("Spells Known ", spellsKnown));
                    }
                }
            }
            else
            {
                List<Inline> spellInlines = null;
                if (_LinkHandler != null)
                {
                    spellInlines = CreateSpellsBlockItem(monster.SpellsKnownBlock);
                }
                if (spellInlines != null && spellInlines.Count > 0)
                {
                    offenseParagraph.Inlines.AddRange(spellInlines);
                }

            }
            if (monster.SpellsPreparedBlock == null)
            {
                string spellsPrepared = monster.SpellsPrepared;
                if (spellsPrepared != null && spellsPrepared.Length > 0)
                {
                    List<Inline> spellInlines = null;
                    if (_LinkHandler != null)
                    {
                        spellInlines = CreateSpellsBlockItem(monster.SpellsPreparedBlock);
                    }

                    if (spellInlines != null && spellInlines.Count > 0)
                    {
                        offenseParagraph.Inlines.AddRange(spellInlines);
                    }
                    else
                    {
                        if (spellsPrepared.IndexOf("Spells Prepared ") == 0)
                        {
                            spellsPrepared = spellsPrepared.Substring("Spells Prepared ".Length);

                        }
                        offenseParagraph.Inlines.AddRange(CreateItemIfNotNull("Spells Prepared ", spellsPrepared));
                    }
                }
            }
            else
            {
                List<Inline> spellInlines = null;
                if (_LinkHandler != null)
                {
                    spellInlines = CreateSpellsBlockItem(monster.SpellsPreparedBlock);
                }

                if (spellInlines != null && spellInlines.Count > 0)
                {
                    offenseParagraph.Inlines.AddRange(spellInlines);
                }
            }

                offenseParagraph.Inlines.AddRange(CreateItemIfNotNull("Opposition Schools: ", monster.ProhibitedSchools));
                offenseParagraph.Inlines.AddRange(CreateItemIfNotNull("Bloodline: ",monster.Bloodline));
                offenseParagraph.Inlines.AddRange(CreateItemIfNotNull("Domain: ",monster.SpellDomains));

            blocks.Add(offenseParagraph);
        }



        delegate void After();
        List<Inline> CreateSpellsBlockItem(ObservableCollection<SpellBlockInfo> list)
        {
            List<Inline> lines = new List<Inline>();

            bool multiblock = false;
            foreach (SpellBlockInfo blockinfo in list)
            {
                string titleText = "";

                if (blockinfo.SpellLikeAbilities)
                {
                    if (blockinfo.Class != null)
                    {
                        titleText += blockinfo.Class + " ";
                    }
                    titleText += "Spell-Like Abilities ";
                }
                else
                {
                    if (blockinfo.Class != null)
                    {
                        titleText += blockinfo.Class + " ";
                    }
                    titleText += "Spells " + blockinfo.BlockType + " ";
                }
                if (multiblock)
                {
                    lines.Add(new LineBreak());
                }
                
                multiblock = true;
                lines.Add(new Bold(new Run(titleText)));

                string text = "";


                text = "(CL " + BlockCreator.PastTenseNumber(blockinfo.CasterLevel.ToString());

                if (blockinfo.SpellFailure != null)
                {
                    text += "; " + blockinfo.SpellFailure.Value + "% spell failure";
                }
                if (blockinfo.Concentration != null)
                {
                    text += "; concentration " +  CMStringUtilities.PlusFormatNumber(blockinfo.Concentration.Value);
                }
                if (blockinfo.MeleeTouch != null)
                {
                    text += "; " +  CMStringUtilities.PlusFormatNumber(blockinfo.MeleeTouch.Value) + " melee touch";
                }
                if (blockinfo.RangedTouch != null)
                {
                    text += "; " + CMStringUtilities.PlusFormatNumber(blockinfo.RangedTouch.Value) + " ranged touch";
                }
                text += ") ";

                
                lines.Add(new Run(text));
                //start spells on their own line
                lines.Add(new LineBreak());
                foreach (SpellLevelInfo levelinfo in blockinfo.Levels)
                {
                    //indentation of spell list
                    text = "  ";
                    if (levelinfo.Level != null)
                    {
                        text += BlockCreator.PastTenseNumber(levelinfo.Level.Value);
                    }
                    if (levelinfo.PerDay != null || levelinfo.AtWill || levelinfo.Constant)
                    {
                        if (text.Length > 0)
                        {
                            // space between spell level and number/day [3rd(3/day or 3rd (3/day)]
                            text += " ";
                        }
                        if (!blockinfo.SpellLikeAbilities)
                        {
                            text += "(";
                        }
                        if (levelinfo.AtWill)
                        {
                            text += "At will";
                        }
                        else if (levelinfo.Constant)
                        {
                            text += "Constant";
                        }
                        else
                        {
                            text += levelinfo.PerDay + "/day";
                        }

                        if (levelinfo.Cast != null)
                        {
                            text += "; " + levelinfo.Cast + " cast";
                        }

                        if (!blockinfo.SpellLikeAbilities)
                        {
                            text += ")";
                        }
                    }
                    if (text.Length > 0)
                    {
                        text += "-";
                    }

                    lines.Add(new Run(text));

                    bool firstLink = true;
                    foreach (SpellInfo spellInfo in levelinfo.Spells)
                    {
                        if (!firstLink)
                        {
                            lines.Add(new Run(", "));
                        }
                        firstLink = false;




                        Hyperlink link = new Hyperlink(new Run(spellInfo.Name));
                        link.Tag = spellInfo.Name;
                        link.Click += new RoutedEventHandler(SpellLinkClicked);
                        if (spellInfo.Spell != null)
                        {
                            link.DataContext = spellInfo.Spell;
                            ToolTip t = (ToolTip)App.Current.MainWindow.FindResource("ObjectToolTip");

                            if (t != null)
                            {

                                ToolTipService.SetShowDuration(link, 360000);
                                link.ToolTip = t;
                                link.ToolTipOpening += new ToolTipEventHandler(link_ToolTipOpening);

                            }
                        }
                        lines.Add(link);

                        if (spellInfo.Mythic)
                        {
                            lines.Add(new Run(" [M]"));
                        }


                        String afterBlock = "";
                        After addAfter = delegate()
                            {
                                if (afterBlock.Length != 0)
                                {
                                    afterBlock += "; ";
                                }
                            };
                        if (spellInfo.Count != null)
                        {
                            afterBlock += spellInfo.Count;
                        }

                        if (spellInfo.DC != null)
                        {

                            addAfter();
                            afterBlock += "DC " + spellInfo.DC;
                        }
                        if (spellInfo.AlreadyCast || spellInfo.Cast != null)
                        {

                            addAfter();
                            if (spellInfo.AlreadyCast)
                            {
                                afterBlock += "already ";
                            }
                            else
                            {
                                afterBlock += spellInfo.Cast.Value + " ";
                            }
                            afterBlock += "cast";

                        }
                        if (spellInfo.Only != null)
                        {

                            addAfter();
                            afterBlock += spellInfo.Only;
                        }
                        if (spellInfo.Other != null)
                        {

                            addAfter();
                            afterBlock += spellInfo.Other;
                        }

                        if (afterBlock.Length > 0)
                        {
                            lines.Add(new Run(" (" + afterBlock + ")"));
                        }

                    }
                    if (lines.Count > 0)
                    {
                        //Put each spell level on seperate line
                        lines.Add(new LineBreak());
                    }

                    if (levelinfo.More != null)
                    {
                        lines.Add(new Run(", " + levelinfo.More + " more"));
                    }

                }
            }

            if (lines.Count > 0)
            {
                lines.Add(new LineBreak());
            }
            
            return lines;
        }

        void link_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            Hyperlink l = (Hyperlink)sender;
            ((ToolTip)l.ToolTip).DataContext = l.DataContext;
        }

        private void CreateTacticsSection(Monster monster, List<Block> blocks)
        {
            if (NotNullString(monster.BeforeCombat) ||
                NotNullString(monster.DuringCombat) ||
                NotNullString(monster.Morale))
            {

                blocks.AddRange(CreateSectionHeader("TACTICS"));

                Paragraph tacticsParagraph = new Paragraph();
                tacticsParagraph.Margin = new Thickness(0);


                tacticsParagraph.Inlines.AddRange(CreateItemIfNotNull("Before Combat ", monster.BeforeCombat));
                tacticsParagraph.Inlines.AddRange(CreateItemIfNotNull("During Combat ", monster.DuringCombat));
                tacticsParagraph.Inlines.AddRange(CreateItemIfNotNull("Morale ", monster.Morale));
                tacticsParagraph.Inlines.AddRange(CreateItemIfNotNull("Base Statistics: ", monster.BaseStatistics));
                blocks.Add(tacticsParagraph);
            }
        }

        private void CreateStatisticsSection(Monster monster, List<Block> blocks)
        {
            blocks.AddRange(CreateSectionHeader("STATISTICS"));

            Paragraph statsParagraph = new Paragraph();
            statsParagraph.Margin = new Thickness(0);

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

            statsParagraph.Inlines.AddRange(CreateMultiValueLine(statsLine, ", "));

            List<TitleValuePair> combatLine = new List<TitleValuePair>();
            combatLine.Add(new TitleValuePair { Title = "Base Atk ", Value = monster.BaseAtk.PlusFormat() });
            combatLine.Add(new TitleValuePair { Title = "CMB ", Value = monster.CMB });
            combatLine.Add(new TitleValuePair { Title = "CMD ", Value = monster.CMD });
            statsParagraph.Inlines.AddRange(CreateMultiValueLine(combatLine, "; "));


            if (monster.FeatsList.Count > 0)
            {

                int count = 0;


                statsParagraph.Inlines.Add(new Bold(new Run("Feats ")));
                foreach (string feat in monster.FeatsList)
                {
                    if (count > 0)
                    {
                        statsParagraph.Inlines.Add(", ");
                    }

                    Hyperlink link = new Hyperlink(new Run(feat));
                    Regex regFeat = new Regex("(?<name>[-'\\p{L} ]+) +\\(");
                    string featname = feat;
                    Match m = regFeat.Match(feat);
                    if (m.Success)
                    {
                        featname = m.Groups["name"].Value;
                    }

                    link.Tag = featname;
                    link.Click += new RoutedEventHandler(FeatLinkClicked);

                    Feat featObject = Feat.Feats.FirstOrDefault<Feat>(a => String.Compare(a.Name, featname, true) == 0);
                    if (featObject != null)
                    {
                        link.DataContext = featObject;
                        ToolTip t = (ToolTip)App.Current.MainWindow.FindResource("ObjectToolTip");

                        if (t != null)
                        {

                            ToolTipService.SetShowDuration(link, 360000);
                            link.ToolTip = t;
                            link.ToolTipOpening += new ToolTipEventHandler(link_ToolTipOpening);

                        }
                    }

                    statsParagraph.Inlines.Add(link);

                    count++;
                }
                statsParagraph.Inlines.Add(new LineBreak());

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

                statsParagraph.Inlines.AddRange(CreateItemIfNotNull("Skills ", skillList));
            }

            statsParagraph.Inlines.AddRange(CreateItemIfNotNull("  Racial Modifiers ", monster.RacialMods));
            statsParagraph.Inlines.AddRange(CreateItemIfNotNull("Languages ", monster.Languages));
            statsParagraph.Inlines.AddRange(CreateItemIfNotNull("SQ ", monster.SQ));
            statsParagraph.Inlines.AddRange(CreateItemIfNotNull("Gear ", monster.Gear));
            statsParagraph.Inlines.AddRange(CreateItemIfNotNull("Other Gear ", monster.OtherGear));

            blocks.Add(statsParagraph);
        }

        private void CreateEcologySection(Monster monster, List<Block> blocks)
        {
            if ((monster.Environment != null && monster.Environment.Length > 0) ||
                (monster.Organization != null && monster.Organization.Length > 0) ||
                (monster.Treasure != null && monster.Treasure.Length > 0))
            {

                blocks.AddRange(CreateSectionHeader("ECOLOGY"));


                Paragraph ecoParagraph = new Paragraph();
                ecoParagraph.Margin = new Thickness(0);


                ecoParagraph.Inlines.AddRange(CreateItemIfNotNull("Environment ", monster.Environment));
                ecoParagraph.Inlines.AddRange(CreateItemIfNotNull("Organization ", monster.Organization));
                ecoParagraph.Inlines.AddRange(CreateItemIfNotNull("Treasure ", monster.Treasure));

                blocks.Add(ecoParagraph);


            }
        }

        private void CreateSpecialAbilitiesSection(Monster monster, List<Block> blocks)
        {
            if (monster.SpecialAbilitiesList.Count > 0)
            {

                blocks.AddRange(CreateSectionHeader("SPECIAL ABILITIES"));

                Paragraph specialParagraph = new Paragraph();
                specialParagraph.Margin = new Thickness(0);


                foreach (SpecialAbility spec in monster.SpecialAbilitiesList)
                {
                    if (spec.Name.Length > 0)
                    {

                        string type = spec.Type;

                        if (spec.ConstructionPoints != null)
                        {
                            type += ", " + spec.ConstructionPoints + " CP";
                        }

                        specialParagraph.Inlines.AddRange(
                            CreateItemIfNotNull(spec.Name + " (" + type + ") "
                            , spec.Text));
                    }
                    else
                    {

                        specialParagraph.Inlines.AddRange(CreateItemIfNotNull(null, spec.Text));
                    }

                }


                blocks.Add(specialParagraph);
            }
        }

        private void CreateDescriptionSection(Monster monster, List<Block> blocks)
        {


            if (monster.Description_Visual != null && monster.Description_Visual.Length > 0)
            {
                Paragraph descriptionParagraph = new Paragraph();

                if (Fonts.SystemFontFamilies.Contains(new FontFamily("Nyala")))
                {

                    descriptionParagraph.FontFamily = new FontFamily("Nyala");
                    descriptionParagraph.FontSize = Document.FontSize * 1.2;
                }
                else
                {
                    descriptionParagraph.FontFamily = new FontFamily("Georgia");
                }


                descriptionParagraph.FontStyle = FontStyles.Italic;

                descriptionParagraph.Inlines.Add(new Run(monster.Description_Visual));


                blocks.Add(descriptionParagraph);


            }



            if (monster.DescHTML != null && monster.DescHTML.Length > 0)
            {

                blocks.AddRange(CreateFlowFromDescription(monster.DescHTML));
            }
            else if (monster.Description != null && monster.Description.Length > 0)
            {
                Paragraph descriptionParagraph = new Paragraph();

                if (Fonts.SystemFontFamilies.Contains(new FontFamily("Nyala")))
                {

                    descriptionParagraph.FontFamily = new FontFamily("Nyala");
                    descriptionParagraph.FontSize = Document.FontSize * 1.2;
                }
                else
                {
                    descriptionParagraph.FontFamily = new FontFamily("Georgia");
                }

                string description = FixBodyString(monster.Description);


                descriptionParagraph.Inlines.Add(new Run(description));


                blocks.Add(descriptionParagraph);
            }
        }

        void FeatLinkClicked(object sender, RoutedEventArgs e)
        {
            if (_LinkHandler != null)
            {
                Hyperlink link = (Hyperlink)sender;

                string feat = (string)link.Tag;

                _LinkHandler(this, new DocumentLinkEventArgs(feat, "Feat"));
                
            }
        }
        void SpellLinkClicked(object sender, RoutedEventArgs e)
        {
            if (_LinkHandler != null)
            {
                Hyperlink link = (Hyperlink)sender;

                string spell;

                if (link.Tag is Spell)
                {
                    spell = ((Spell)link.Tag).name;
                }
                else
                {
                    spell = (string)link.Tag;
                }

                spell = Spell.StandarizeSpellName(spell);

                _LinkHandler(this, new DocumentLinkEventArgs(spell, "Spell"));

            }
        }

    }
}
