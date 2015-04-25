/*
 *  RulesTab.cs
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
using CoreGraphics;

using CombatManager;
namespace CombatManagerMono
{
	public class RulesTab : LookupTab<Rule>
	{
		ButtonStringPopover typePopover;
		ButtonStringPopover subtypePopover;
		
		GradientButton typeFilterButton;
		GradientButton subtypeFilterButton;
		
		
		string typeFilter = null;
		string subtypeFilter = null;
		
		const string AllSubtypeText = "All";
		const string AllTypeText = "All";
		
		public RulesTab (CombatState state) : base(state)
		{		
			BuildFilters();
		}
		
		private void BuildFilters()
		{
			float locX = 0;
			float locY = 5;
			float bHeight = 30;
			float marginX = 10;
			
			GradientButton b;
			
			//type filter
			b = new GradientButton();
			StyleFilterButton(b);
			b.Frame = new CGRect(locX, locY, 220, bHeight);
            locX += (float)b.Frame.Width + marginX;
			
			b.SetText(AllTypeText);
			typePopover = new ButtonStringPopover(b);
			
			typePopover.Items.Add(new ButtonStringPopoverItem() {Text = AllTypeText});
			foreach (String type in Rule.Types)
			{			
				typePopover.Items.Add(new ButtonStringPopoverItem() {Text = type.Capitalize(), Tag=type});
			}
			typePopover.SetButtonText = true;
			typePopover.ItemClicked += HandleTypePopoverItemClicked;
			typeFilterButton = b;
			typeFilterButton.Hidden = false;
			
			FilterView.AddSubview(b);
			
			
			//subtype filter
			b = new GradientButton();
			StyleFilterButton(b);
			b.Frame = new CGRect(locX, locY, 100, bHeight);
            locX += (float)b.Frame.Width + marginX;
			
			b.SetText(AllSubtypeText);
			subtypePopover = new ButtonStringPopover(b);
			
			subtypePopover.Items.Add(new ButtonStringPopoverItem() {Text = AllSubtypeText});
			
			subtypePopover.SetButtonText = true;
			subtypePopover.ItemClicked += HandleSubtypePopoverItemClicked;
			subtypeFilterButton = b;
			
			subtypeFilterButton.Hidden = true;
			
			FilterView.AddSubview(b);
			
		}

		void HandleTypePopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			string newFilter = (string) e.Tag;
			if (newFilter != typeFilter)
			{
				typeFilter = (string) e.Tag;
				UpdateSubtypeFilterBox();
				Filter();
			}
		}

		void HandleSubtypePopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			
			subtypeFilter = (string) e.Tag;
			Filter();
		}
		
		void UpdateSubtypeFilterBox()
		{
			subtypePopover.Items.Clear();
			
			if (typeFilter == null || !Rule.Subtypes.ContainsKey(typeFilter))
			{
				subtypeFilterButton.Hidden = true;
				subtypeFilter = null;
			}
			else
			{
				subtypeFilterButton.Hidden = false;
				subtypePopover.Items.Add(new ButtonStringPopoverItem() {Text=AllSubtypeText, Tag=null});
				
				
				foreach (string type in Rule.Subtypes[typeFilter].Values)
				{
					subtypePopover.Items.Add(new ButtonStringPopoverItem() {Text = type.Capitalize(), Tag=type});
			
				}
				subtypeFilter = null;
				subtypeFilterButton.SetText(AllSubtypeText);
			}
		}

		protected override bool ItemFilter (Rule item)
		{
			return (typeFilter == null) || (
				(item.Type.CompareTo(typeFilter) == 0) && 
				((subtypeFilter == null) || (item.Subtype == subtypeFilter)));
		}

		protected override string ItemFilterText (Rule item)
		{
			return item.Name;
		}

		protected override string SortText (Rule item)
		{
			return item.Name;
		}

		protected override string DisplayText (Rule item)
		{
			return item.Name;
		}

		protected override string ItemHtml (Rule item)
		{
			return RuleHtmlCreator.CreateHtml(item);
		}

		protected override bool CompareItems (Rule item1, Rule item2)
		{
			return item1 == item2;
		}

		protected override System.Collections.Generic.IEnumerable<Rule> ItemsSource 
		{
			
			get 
			{
				return Rule.Rules;
			}
		}
	}
}

