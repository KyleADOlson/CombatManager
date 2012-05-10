/*
 *  ConditionSelector.xaml.cs
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using WinInterop = System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace CombatManager
{
    /// <summary>
    /// Interaction logic for ConditionSelector.xaml
    /// </summary>
    public partial class ConditionSelector : Window
    {
		private ICollectionView conditionView;

        private Condition visibleCondition;

        private bool loadedDuration;

        private InitiativeCount _InitiativeCount;

        private List<Character> characters;

        private List<Condition> conditionList;
		
        public ConditionSelector()
        {
            InitializeComponent();

            conditionList = new List<Condition>();
            conditionList.AddRange(Condition.Conditions);
            conditionList.AddRange(Condition.CustomConditions);

            conditionList.Sort((a, b) => String.Compare(a.Name, b.Name, true));
            

            conditionView = new ListCollectionView(conditionList);

            conditionView.CurrentChanged += new EventHandler(ConditionView_CurrentChanged);
            conditionView.Filter += new Predicate<object>(ConditionViewFilter);
            conditionView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            ConditionsListBox.DataContext = conditionView;


            UpdatePageSelection();
            UpdateConditionDisplay();

        }

        bool ConditionViewFilter(object ob)
        {
            Condition c = (Condition)ob;

            return ConditionViewTypeFilter(c) && ConditionViewTextFilter(c);
        }


        bool ConditionViewTextFilter(Condition c)
        {

            if (ConditionFilterText.Text == null || ConditionFilterText.Text.Length == 0)
            {
                return true;
            }
            else
            {
                return new Regex(Regex.Escape(ConditionFilterText.Text.Trim()), RegexOptions.IgnoreCase).Match(c.Name).Success;

            }
        }


        bool ConditionViewTypeFilter(Condition c)
        {
            bool showingSpells = SpellsRadioButton.IsChecked == true;
			bool showingConditions = ConditionsRadioButton.IsChecked == true;
			bool showingAfflictions = AfflictionsRadioButton.IsChecked == true;
            bool showingCustom = CustomRadioButton.IsChecked == true;
            bool showingFavorites = FavoritesRadioButton.IsChecked == true ;

            if (showingFavorites)
            {
                return Condition.FavoriteConditions.FirstOrDefault(a => String.Compare(a.Name, c.Name, true ) == 0
                    && a.Type == c.Type) != null;                      
            }
            else if (c.Custom)
            {
                return showingCustom;
            }
			else if (c.Spell != null)
			{
                return showingSpells;
			}
			else if (c.Affliction != null)
			{
				return showingAfflictions;
			}
			else
			{
				return showingConditions;
			}
        }

        void UpdateConditionDisplay()
        {
            Condition c = (Condition)conditionView.CurrentItem;            


            if (c != visibleCondition)
            {

                visibleCondition = c;

                if (c != null)
                {
                    

                    ConditionDocument.Blocks.Clear();


                    if (c != null)
                    {
                        if (c.Spell != null)
                        {
                            SpellBlockCreator cs = new SpellBlockCreator(ConditionDocument, null);
                            ConditionDocument.Blocks.AddRange(cs.CreateBlocks(c.Spell, true, true));

                        }
                        else if (c.Affliction != null)
                        {

                            Paragraph p = new Paragraph();
                            p.Inlines.Add(new Bold(new Run(c.Name)));
                            p.Inlines.Add(new LineBreak());
                            p.Inlines.Add(new Run(c.Affliction.Text));
                            p.TextAlignment = TextAlignment.Left;
                            Thickness m = p.Margin;
                            m.Bottom = 0;
                            p.Margin = m;

                            ConditionDocument.Blocks.Add(p);
                        }
                        else
                        {

                            Paragraph p = new Paragraph();
                            p.Inlines.Add(new Bold(new Run(c.Name)));
                            p.Inlines.Add(new LineBreak());
                            p.Inlines.Add(new Run(c.Text));
                            p.TextAlignment = TextAlignment.Left;
                            Thickness m = p.Margin;
                            m.Bottom = 0;
                            p.Margin = m;

                            ConditionDocument.Blocks.Add(p);
                        }
                    }

                    
                }
                
            }
        }




        void ConditionView_CurrentChanged(object sender, EventArgs e)
        {
            UpdateConditionDisplay();
        }

        private void ConditionFilterText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            conditionView.Refresh();

            if (conditionView.CurrentItem == null)
            {
                conditionView.MoveCurrentToFirst();
            }
        }

        private void Border_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void Popup_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
			
        }

        private void Popup_PreviewDragEnter(object sender, System.Windows.DragEventArgs e)
        {
        }

        private void Popup_Opened(object sender, System.EventArgs e)
        {
        	ConditionFilterText.Focus();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			AddCurrentCondition();
        }
		
		private void AddCurrentCondition()
		{
            ActiveCondition ac = null;

            if (characters != null)
            {
                foreach (Character ch in characters)
                {

                    
                    Condition c = (Condition)conditionView.CurrentItem;

                    if (c != null)
                    {

                        ac = new ActiveCondition();

                        ac.Condition = c;

                    }
                   
                    if (ac != null)
                    {


                        if (this.RoundsRadioButton.IsChecked == true)
                        {
                            int val;
                            if (int.TryParse(ConditionDurationText.Text, out val))
                            {
                                ac.Turns = val;
                            }
                        }
                        else
                        {
                            ac.Turns = null;
                        }

                        ac.InitiativeCount = InitiativeCount;

                        ac.Details = DetailsTextBox.Text;

                        ch.Stats.AddCondition(ac);
                        Condition.PushRecentCondition(ac.Condition);
                    }
                }


                DialogResult = true;
                Close();
            }
		}

        private void ConditionsRadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdatePageSelection();
        }

        private void UpdatePageSelection()
        {
            if (ConditionFlowDocumentViewer != null && FavoritesRadioButton != null
    && ConditionsRadioButton != null
    && SpellsRadioButton != null
    && AfflictionsRadioButton != null
    && AddToFavoritesButton != null)
            {
                bool showingCustom =
                    (CustomRadioButton.IsChecked == true);

                bool showingFavorites =
                    (FavoritesRadioButton.IsChecked == true);


                CustomControlsGrid.Visibility =
                    showingCustom ? Visibility.Visible : Visibility.Collapsed;


                AddToFavoritesButton.Content =
                    !showingFavorites ? "Add to Favorites" : "Remove from Favorites";

                if (conditionView != null)
                {
                    conditionView.Refresh();


                    if (conditionView.CurrentItem == null)
                    {
                        conditionView.MoveCurrentToFirst();
                    }
                }
            }
        }

        private void ConditionFilterText_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        	if (e.Key == Key.Enter)
			{
				AddCurrentCondition();
			}
        }

        private void ConditionsListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (CMUIUtilities.ClickedListBoxItem((ListBox)sender, e) != null)
            {
                AddCurrentCondition();
            }
        }

        private void ConditionDurationText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!loadedDuration)
            {
                loadedDuration = true;
            }
            else
            {
                RoundsRadioButton.IsChecked = true;
            }
            
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			DialogResult = false;
        	Close();
        }

        public InitiativeCount InitiativeCount
        {
            get { return _InitiativeCount; }
            set
            {
                if (_InitiativeCount != value)
                {
                    _InitiativeCount = new InitiativeCount((InitiativeCount)value);
                }
            }
        }

        public List<Character> Characters
        {
            get
            {
                return characters;
            }
            set
            {
                characters = value;
            }
        }



        private void DeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Condition c = (Condition)((Button)sender).DataContext;

            if (c != null)
            {
                if (CustomRadioButton.IsChecked == true)
                {

                    Condition.CustomConditions.Remove(c);
                    conditionList.Remove(c);
                    conditionView.Refresh();
                    Condition.SaveCustomConditions();
                }
                else if (FavoritesRadioButton.IsChecked == true)
                {
                    DeleteFromFavorites(c);
                }
            }
            
        }

        private void AddToFavoritesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Condition c = (Condition)conditionView.CurrentItem;

            if (c != null)
            {
                if (FavoritesRadioButton.IsChecked == true)
                {
                    DeleteFromFavorites(c);
                }
                else
                {
                    AddToFavorites(c);
                }

            }
        }

        private void AddToFavorites(Condition c)
        {

            if (Condition.FavoriteConditions.FirstOrDefault(
                a => (String.Compare(a.Name, c.Name, true) == 0) &&
                    (a.Type == c.Type)) == null)
            {
                Condition.FavoriteConditions.Add(new FavoriteCondition(c));
                Condition.FavoriteConditions.Sort((a, b) => String.Compare(a.Name, b.Name, true));
                Condition.SaveFavoriteConditions();
            }
        }

        private void DeleteFromFavorites(Condition c)
        {

            Condition.FavoriteConditions.RemoveAll(a => String.Compare(a.Name, c.Name, true) == 0
                                 && a.Type == c.Type);
            Condition.SaveFavoriteConditions();
            conditionView.Refresh();
        }

        private void Grid_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {

        }

        private void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            FrameworkElement el = (FrameworkElement)sender;

            Button delete = (Button)LogicalTreeHelper.FindLogicalNode(el, "DeleteButton");

            Condition c = (Condition)el.DataContext;

            if (CustomRadioButton.IsChecked == true || FavoritesRadioButton.IsChecked == true)
            {
                delete.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                delete.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void EditCustomCondition_Click(object sender, System.Windows.RoutedEventArgs e)
        {
             Condition c = (Condition)conditionView.CurrentItem;
             if (c != null)
             {
                 CustomConditionDialog dlg = new CustomConditionDialog();


                 dlg.Condition = c;
                 dlg.Owner = this;
                 if (dlg.ShowDialog() == true)
                 {
                     Condition.CustomConditions.Remove(c);
                     conditionList.Remove(c);
                     Condition.CustomConditions.Add(dlg.Condition);
                     conditionList.Add(dlg.Condition);
                     conditionView.Refresh();
                     Condition.SaveCustomConditions();
                     conditionView.MoveCurrentTo(dlg.Condition);
                 }
             }
        }

        private void NewCustomCondition_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CustomConditionDialog dlg = new CustomConditionDialog();
            dlg.Owner = this;
            if (dlg.ShowDialog() == true)
            {

                Condition.CustomConditions.Add(dlg.Condition);
                conditionList.Add(dlg.Condition);
                conditionView.Refresh();
                Condition.SaveCustomConditions();
                conditionView.MoveCurrentTo(dlg.Condition);
            }
        }



    }
}
