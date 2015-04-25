/*
 *  FeatsTab.cs
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
using CombatManager;
using CoreGraphics;
using UIKit;


namespace CombatManagerMono
{
	public class FeatsTab: LookupTab<Feat>
	{
		ButtonStringPopover typeFilterPopover;
		
		GradientButton typeFilterButton;

        GradientButton editButton;
        GradientButton deleteButton;
		
		string typeFilter = null;	
		
		const string AllTypesText = "All Types";
		
		public FeatsTab (CombatState state) : base (state)
		{
			BuildFilters();
		}

		protected override bool ItemFilter (Feat item)
		{
			return typeFilter == null || String.Compare(item.Type.Trim(), typeFilter, true) == 0;
		}
		
		protected override string ItemFilterText (Feat item)
		{
			return item.Name;
		}

		protected override string SortText (Feat item)
		{
			return item.Name;
		}

		protected override string DisplayText (Feat item)
		{
			return item.Name;
		}

		protected override string ItemHtml (Feat item)
		{
			return FeatHtmlCreator.CreateHtml(item);
		}

		protected override bool CompareItems (Feat item1, Feat item2)
		{
			return item1 == item2;
		}

		protected override System.Collections.Generic.IEnumerable<Feat> ItemsSource 
		{
			get 
			{
				return Feat.Feats;
			}
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
			b.Frame = new CGRect(locX, locY, 100, bHeight);
            locX += (float)b.Frame.Width + marginX;
			b.SetText(AllTypesText);
			typeFilterPopover = new ButtonStringPopover(b);
			typeFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = AllTypesText, Tag=null});
			
			foreach (String s in Feat.FeatTypes)
			{
				typeFilterPopover.Items.Add(new ButtonStringPopoverItem() {Text = s, Tag=s});
			}
			typeFilterPopover.SetButtonText = true;
			typeFilterPopover.ItemClicked += HandleTypeFilterPopoverItemClicked;;
			typeFilterButton = b;
			
			FilterView.AddSubview(b);

            b = new GradientButton();
            StyleDBButton(b);
            b.Frame = new CGRect(locX, locY, 90, bHeight);
            locX += (float)b.Frame.Width + marginX;
            b.SetText("New");
            b.TouchUpInside += NewButtonClicked;

            FilterView.AddSubview(b);

            b = new GradientButton();
            StyleDBButton(b);
            b.Frame = new CGRect(locX, locY, 90, bHeight);
            locX += (float)(b.Frame.Width + marginX);
            b.SetText("Customize");
            b.TouchUpInside += CustomizeButtonClicked;
            FilterView.AddSubview(b);


            b = new GradientButton();
            StyleDBButton(b);
            b.Frame = new CGRect(locX, locY, 90, bHeight);
            locX += (float)b.Frame.Width + marginX;
            b.SetText("Edit");
            b.TouchUpInside += EditButtonClicked;
            FilterView.AddSubview(b);

            editButton = b;

            b = new GradientButton();
            StyleDBButton(b);
            b.Frame = new CGRect(locX, locY, 90, bHeight);
            locX += (float)b.Frame.Width + marginX;
            b.SetText("Delete");
            b.TouchUpInside += DeleteButtonClicked;
            FilterView.AddSubview(b);

            deleteButton = b;


		}

        protected override void ShowItem(Feat item)
        {
            base.ShowItem(item);
            if (editButton != null)
            {
                editButton.Enabled = item != null && item.IsCustom;
            }
            if (deleteButton != null)
            {
                deleteButton.Enabled = item != null && item.IsCustom;
            }
        }

		void HandleTypeFilterPopoverItemClicked (object sender, ButtonStringPopover.PopoverEventArgs e)
		{
			typeFilter = (string) e.Tag;
			
			Filter();
		}
		
		protected override void ResetButtonClicked ()
		{
			typeFilter = null;
			typeFilterButton.SetText(AllTypesText);
			filterField.Text = "";
			Filter();
		}

        void NewButtonClicked(object sender, EventArgs e)
        {
            Feat f = new Feat();
            FeatEditorDialog dlg = new FeatEditorDialog(f);
            dlg.OKClicked += (object se, EventArgs ea) => 
            {
                Feat.AddCustomFeat(f);
                Filter(true);
            };
            MainUI.MainView.AddSubview(dlg.View);
        }

        void CustomizeButtonClicked(object sender, EventArgs e)
        {
            if (DisplayItem != null)
            {
                Feat clone = (Feat)DisplayItem.Clone();

                FeatEditorDialog dlg = new FeatEditorDialog(clone);
                dlg.OKClicked += (object se, EventArgs ea) => 
                {
                    clone.DBLoaderID = 0;
                    Feat.AddCustomFeat(clone);
                    Filter(true);
                };
                MainUI.MainView.AddSubview(dlg.View);
            }
        }

        void EditButtonClicked(object sender, EventArgs e)
        {
            if (DisplayItem != null && DisplayItem.IsCustom)
            {
                Feat clone = (Feat)DisplayItem.Clone();

                FeatEditorDialog dlg = new FeatEditorDialog(clone);
                dlg.OKClicked += (object se, EventArgs ea) => 
                {
                    DisplayItem.CopyFrom(clone);
                    Feat.UpdateCustomFeat(DisplayItem);
                    Filter(true);
                };
                MainUI.MainView.AddSubview(dlg.View);
            }
        }

        void DeleteButtonClicked(object sender, EventArgs e)
        {
            if (DisplayItem != null && DisplayItem.IsCustom)
            {
                UIAlertView alertView = new UIAlertView    
                {        
                    Title = "Are you sure you want to delete this feat?",
                    Message = "This feat will be deleted permanently"

                };        
                alertView.AddButton("Cancel");    
                alertView.AddButton("OK");
                alertView.Show();
                alertView.Clicked += (object se, UIButtonEventArgs ea) => 
                {
                    if (ea.ButtonIndex == 1)
                    {

                        Feat.RemoveCustomFeat(DisplayItem);
                        Filter(true);
                    }
                };

            }
        }


	}
}

