/*
 *  MonstersTab.cs
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using CoreGraphics;
using CombatManager;
using System.Text.RegularExpressions;

namespace CombatManagerMono
{
	public class MonstersTab : LookupTab<Monster>
	{
		ButtonStringPopover typePopover;
		ButtonStringPopover setFilterPopover;
		ButtonStringPopover crFilterPopover;
		
		GradientButton setFilterButton;
		GradientButton typeFilterButton;
		GradientButton crFilterButton;
		
		AdvancerPanel advancerPanel;
		
		
		string typeFilter = null;	
		bool ? npcFilter = null; 
		string crFilter;
		
		const string AllSetText = "All";
		const string AllTypeText = "All Types";
		const string AllCRText = "All CRs";
		
		public MonstersTab (CombatState state) : base (state)
		{
			BuildFilters();
			
			BuildAdvancer();
		}
		
		private void BuildAdvancer()
		{
			
			advancerPanel = new AdvancerPanel();
			
			this.BottomView = advancerPanel.View;
            BottomViewHeight = (float)advancerPanel.View.Frame.Height;
			advancerPanel.AddMonsterClicked += HandleAdvancerPanelAddMonsterClicked;
			advancerPanel.AdvancementChanged += HandleAdvancerPanelAdvancementChanged;
		}

		void HandleAdvancerPanelAdvancementChanged (object sender, EventArgs e)
		{
			ModifiedItemChanged();
		}

		void HandleAdvancerPanelAddMonsterClicked (object sender, EventArgs e)
		{
			CombatState.AddMonster(DisplayItem, true);
		}
		
		
		private void BuildFilters()
		{
			float locX = 0;
			float locY = 5;
			float bHeight = 30;
			float marginX = 10;
			
			GradientButton b;
			
			
			//set filter
			b = new GradientButton();
			StyleFilterButton(b);
			b.Frame = new CGRect(locX, locY, 100, bHeight);
            locX += (float)b.Frame.Width + marginX;
			b.SetText(AllSetText);
			setFilterPopover = new ButtonStringPopover(b);
			setFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = AllSetText, Tag=null});
			setFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = "Monsters", Tag=false});
			setFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = "NPCs", Tag=true});
			setFilterPopover.SetButtonText = true;
			setFilterPopover.ItemClicked += HandleSetFilterPopoverItemClicked;
			setFilterButton = b;
			
			FilterView.AddSubview(b);
			
			//type filter
			b = new GradientButton();
			StyleFilterButton(b);
			b.Frame = new CGRect(locX, locY, 150, bHeight);
            locX += (float)b.Frame.Width + marginX;
			
			b.SetText(AllTypeText);
			typePopover = new ButtonStringPopover(b);
			
			typePopover.Items.Add(new ButtonStringPopoverItem() {Text = AllTypeText});
			foreach (String type in Monster.CreatureTypeNames)
			{			
				typePopover.Items.Add(new ButtonStringPopoverItem() {Text = type.Capitalize(), Tag=type});
			}
			typePopover.SetButtonText = true;
			typePopover.ItemClicked += HandleTypePopoverItemClicked;
			typeFilterButton = b;
			
			FilterView.AddSubview(b);
			
			
			//CR filter
			b = new GradientButton();
			StyleFilterButton(b);
			b.Frame = new CGRect(locX, locY, 100, bHeight);
            locX += (float)b.Frame.Width + marginX;
			
			b.SetText(AllCRText);
			crFilterPopover = new ButtonStringPopover(b);
			
			crFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = AllCRText});
			foreach (KeyValuePair<double, string> cr in LoadCRs())
			{			
				crFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = cr.Value, Tag=cr.Value});
			}
			crFilterPopover.SetButtonText = true;
			crFilterPopover.ItemClicked += HandleCrFilterPopoverItemClicked;
			crFilterButton = b;
			
			FilterView.AddSubview(b);
			
		}

		void HandleCrFilterPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			
			crFilter = (string) e.Tag;
			Filter();
		}
		

		void HandleSetFilterPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			npcFilter = (bool ?) e.Tag;
			Filter();
		}

		void HandleTypePopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			typeFilter = (string) e.Tag;
			Filter();
		}
		

		
		protected override bool ItemFilter (Monster item)
		{
			return NPCFilterMatch(item) && TypeFilterMatch(item) && CRFilterMatch(item);	
		}
		
		private bool NPCFilterMatch(Monster item)
		{
			return npcFilter == null || item.NPC == npcFilter;
		}
		
		private bool TypeFilterMatch(Monster item)
		{
			return typeFilter == null || String.Compare(item.Type, typeFilter) == 0;
		}
		private bool CRFilterMatch(Monster item)
		{
			return crFilter == null || Monster.GetCRValue(item.CR) == Monster.GetCRValue(crFilter);
		}
		
		

		protected override string ItemFilterText (Monster item)
		{
			return item.Name;
		}

		protected override string SortText (Monster item)
		{
			return item.Name;
		}

		protected override string DisplayText (Monster item)
		{
			return item.Name;
		}

		protected override string ItemHtml (Monster item)
		{
			return MonsterHtmlCreator.CreateHtml(item);
		}

		protected override bool CompareItems (Monster item1, Monster item2)
		{
			return item1 == item2;
		}

		protected override IEnumerable<Monster> ItemsSource 
		{
			get 
			{
				return Monster.Monsters;
			}
		}
		
		protected override void ResetButtonClicked ()
		{
			typeFilter = null;	
			npcFilter = false; 
			crFilter = null;
			
			typeFilterButton.SetText(AllTypeText);
			setFilterButton.SetText(AllSetText);
			crFilterButton.SetText(AllCRText);
			
			filterField.Text = "";
			Filter();
			
		}
		
		private SortedDictionary<double, string> LoadCRs()
		{
			
			Dictionary<string, string> crs = new Dictionary<string, string>();
			
			foreach (Monster monster in Monster.Monsters)
			{
                if (monster.CR != null && monster.CR.Length > 0)
                {
                    if (!crs.ContainsKey(monster.CR.Trim()))
                    {
						crs[monster.CR.Trim()] = monster.CR.Trim();
                    }
                }		
			}
			
			SortedDictionary<double, string> sortedCrs = new SortedDictionary<double, string>();
			foreach (string cr in crs.Values)
			{
				
                Regex regslash = new Regex("/");
				
                Match match = regslash.Match(cr);
                if (match.Success)
                {
                    string text = cr.Substring(match.Index + match.Length);

                    double val;
                    if (double.TryParse(text, out val))
                    {
                        sortedCrs.Add(1.0 / val,cr);
                    }

                }
                else
                {
                    double val;
                    if (double.TryParse(cr, out val))
                    {

                        sortedCrs.Add(val, cr);
                    }
                }
			}
			
			return sortedCrs;
		}
		
		protected override Monster ModifiedItem (Monster item)
		{
			if (advancerPanel != null)
			{
				return advancerPanel.AdvanceMonster(item);
			}
			return item;
		}
	}
}

