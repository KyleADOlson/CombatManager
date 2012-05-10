/*
 *  SpellBlockCreator.cs
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace CombatManager
{
    public class SpellBlockCreator : BlockCreator
    {



        public delegate void SpellLinkHander(object sender, DocumentLinkEventArgs e);

        SpellLinkHander _LinkHandler;

        public SpellBlockCreator(FlowDocument document, SpellLinkHander linkHandler)
            : base(document)
        {
            _LinkHandler = linkHandler;
        }

        public List<Block> CreateBlocks(Spell spell)
        {
            return CreateBlocks(spell, false, true);
        }

        public List<Block> CreateBlocks(Spell spell, bool shortForm, bool showTitle)
        {

            List<Block> blocks = new List<Block>();

            if (!shortForm)
            {
                if (showTitle)
                {
                    blocks.Add(CreateHeaderParagraph(spell.name));
                }

                Paragraph details = new Paragraph();
                details.Margin = new Thickness(0, 4, 0, 0);
                Span span1 = new Span();
                span1.Inlines.Add(new Bold(new Run("School ")));

                if (_LinkHandler == null)
                {
                    span1.Inlines.Add(new Run(spell.school));
                }
                else
                {
                    Hyperlink link = new Hyperlink(new Run(spell.school));
                    link.Click += new RoutedEventHandler(link_Click);
                    link.DataContext = spell.school;

                    Rule rule = Rule.Rules.FirstOrDefault
                        (a => String.Compare(a.Name, spell.school, true) == 0 &&
                          String.Compare(a.Type, "Magic") == 0);

                    if (rule != null)
                    {
                        link.Tag = rule;
                        ToolTip t = (ToolTip)App.Current.MainWindow.FindResource("ObjectToolTip");

                        if (t != null)
                        {

                            ToolTipService.SetShowDuration(link, 360000);
                            link.ToolTip = t;
                            link.ToolTipOpening += new ToolTipEventHandler(link_ToolTipOpening);

                        }
                    }

                    span1.Inlines.Add(link);
                }

                CreateItemIfNotNull(span1.Inlines, " (", false, spell.subschool, ")", false);
                CreateItemIfNotNull(span1.Inlines, " [", false, spell.descriptor, "]", false);

                if (showTitle)
                {
                    span1.Inlines.Add("     ");
                }
                else
                {
                    span1.Inlines.Add(new LineBreak());
                }
                span1.Inlines.Add(new Bold(new Run("Level ")));
                span1.Inlines.Add(spell.spell_level);
                span1.Inlines.Add(new LineBreak());
                details.Inlines.Add(span1);
                CreateItemIfNotNull(details.Inlines, "Preparation Time ", spell.preparation_time);
                CreateItemIfNotNull(details.Inlines, "Casting Time ", spell.casting_time);
                CreateItemIfNotNull(details.Inlines, "Components ", spell.components);
                if (spell.range != null && spell.range.Length > 0)
                {
                    details.Inlines.Add(new Bold(new Run("Range ")));
                    details.Inlines.Add(spell.range);
                    details.Inlines.Add(new LineBreak());
                }
                if (spell.area != null && spell.area.Length > 0)
                {
                    details.Inlines.Add(new Bold(new Run("Area ")));
                    details.Inlines.Add(spell.area);
                    details.Inlines.Add(new LineBreak());
                }
                if (spell.targets != null && spell.targets.Length > 0)
                {
                    details.Inlines.Add(new Bold(new Run("Targets ")));
                    details.Inlines.Add(spell.targets);
                    details.Inlines.Add(new LineBreak());
                }
                if (spell.effect != null && spell.effect.Length > 0)
                {
                    details.Inlines.Add(new Bold(new Run("Effect ")));
                    details.Inlines.Add(spell.effect);
                    details.Inlines.Add(new LineBreak());
                }

                CreateItemIfNotNull(details.Inlines, "Duration ", spell.duration);
                if (spell.saving_throw != null && spell.saving_throw.Length > 0)
                {
                    details.Inlines.Add(new Bold(new Run("Saving Throw ")));
                    details.Inlines.Add(spell.saving_throw);
                    details.Inlines.Add(new LineBreak());
                }
                if (spell.spell_resistence != null && spell.spell_resistence.Length > 0)
                {
                    details.Inlines.Add(new Bold(new Run("Spell Resistance ")));
                    details.Inlines.Add(spell.spell_resistence);
                    details.Inlines.Add(new LineBreak());
                }

                if (spell.source != "PFRPG Core")
                {
                    CreateItemIfNotNull(details.Inlines, "Source ", SourceInfo.GetSource(spell.source));
                }


                blocks.Add(details);

                List<Block> flow = new List<Block>();

                Thickness th =  new Thickness(0, showTitle ? 5 : 0, 0, 0);

                if (spell.description_formated != null && spell.description_formated.Length > 0)
                {
                    flow = CreateFlowFromDescription(spell.description_formated);

                    if (flow.Count > 0)
                    {
                        flow[0].Margin = th;
                    }

                    blocks.AddRange(flow);
                }
                else if (spell.description != null && spell.description.Length > 0)
                {
                    Paragraph p = new Paragraph();
                    p.Margin = th;

                    p.Inlines.Add(new Run(spell.description));
                    blocks.Add(p);

                }
            }
            else
            {
                if (showTitle)
                {
                    Paragraph titleParagraph = new Paragraph();
                    titleParagraph.Inlines.Add(new Bold(new Run(spell.name)));
                    titleParagraph.Margin = new Thickness(0);
                    titleParagraph.Padding = new Thickness(0);


                    blocks.Add(titleParagraph);
                }
                List<Block> flow = new List<Block>(); ;
                if (spell.description_formated != null && spell.description_formated.Length > 0)
                {
                    flow = CreateFlowFromDescription(spell.description_formated);


                    Block block = flow[0];
                    Thickness m = block.Margin;
                    m.Top = 0;
                    block.Margin = m;

                    blocks.AddRange(flow);
                }
                else if (spell.description != null && spell.description.Length > 0)
                {
                    Paragraph p = new Paragraph();

                    Thickness th = new Thickness(0, showTitle ? 5 : 0, 0, 0);
                    p.Margin = th;

                    p.Inlines.Add(new Run(spell.description));
                    flow.Add(p);

                }

                foreach (Block b in flow)
                {
                    b.TextAlignment = TextAlignment.Left;
                    
                }

                blocks.AddRange(flow);


            }



            return blocks;

        }

        void link_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            Hyperlink l = (Hyperlink)sender;
            ((ToolTip)l.ToolTip).DataContext = l.Tag;
        }

        void link_Click(object sender, RoutedEventArgs e)
        {
            if (_LinkHandler != null)
            {
                string school = (string)((Hyperlink)sender).DataContext;
                _LinkHandler(this, new DocumentLinkEventArgs(school, "School"));
            }
        }

    }
}
