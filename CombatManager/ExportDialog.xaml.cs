/*
 *  ExportDialog.xaml.cs
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CombatManager
{
    /// <summary>
    /// Interaction logic for ExportDialog.xaml
    /// </summary>
    public partial class ExportDialog : Window
    {

        bool importMode;

        const string exportFileFilter = "Export Files (*.cmx)|*.cmx";

        ObservableCollection<ExportItem> monstersList;
        ListCollectionView monstersView;
        ListCollectionView selectedMonstersView;
        ObservableCollection<ExportItem> spellsList;
        ListCollectionView spellsView;
        ListCollectionView selectedSpellsView;
        ObservableCollection<ExportItem> featsList;
        ListCollectionView featsView;
        ListCollectionView selectedFeatsView;
        ObservableCollection<ExportItem> conditionsList;
        ListCollectionView conditionsView;
        ListCollectionView selectedConditionsView;

        public ExportDialog() : this(null)
        {

        }

        public ExportDialog(ExportData d)
        {
            InitializeComponent();

            if (d != null)
            {
                //import mode
                importMode = true;

                Title = "Import";
                OKButton.Content = "OK";

                if (d.Monsters != null)
                {
                    monstersList = new ObservableCollection<ExportItem>
                                (from m in d.Monsters
                                 select new ExportItem()
                                 {
                                     IsSelected = true,
                                     Item = m
                                 });



                }
                else
                {
                    monstersList = new ObservableCollection<ExportItem>();
                }
                if (d.Spells != null)
                {
                    spellsList = new ObservableCollection<ExportItem>
                                    (from s in d.Spells
                                     select new ExportItem()
                                     {
                                         IsSelected = true,
                                         Item = s
                                     });
                }
                else
                {
                    spellsList = new ObservableCollection<ExportItem>();
                }
                if (d.Feats != null)
                {
                    featsList = new ObservableCollection<ExportItem>
                                    (from s in d.Feats
                                     select new ExportItem()
                                     {
                                         IsSelected = true,
                                         Item = s
                                     });
                }
                else
                {
                    featsList = new ObservableCollection<ExportItem>();
                }
                if (d.Conditions != null)
                {
                    conditionsList = new ObservableCollection<ExportItem>
                                    (from s in d.Conditions
                                     select new ExportItem()
                                     {
                                         IsSelected = true,
                                         Item = s
                                     });
                }
                else
                {
                    conditionsList = new ObservableCollection<ExportItem>();
                }
            }
            else
            {
                monstersList = new ObservableCollection<ExportItem>
                    (from m in Monster.Monsters
                     where m.IsCustom
                     select new ExportItem()
                     {
                         IsSelected = true,
                         Item = m
                     });

                spellsList = new ObservableCollection<ExportItem>
                    (from s in Spell.Spells
                     where s.IsCustom
                     select new ExportItem()
                     {
                         IsSelected = true,
                         Item = s
                     });
				
				

                featsList = new ObservableCollection<ExportItem>
                    (from s in Feat.Feats
                     where s.IsCustom
                     select new ExportItem()
                     {
                         IsSelected = true,
                         Item = s
                     });

                conditionsList = new ObservableCollection<ExportItem>
                    (from s in Condition.CustomConditions
                     select new ExportItem()
                     {
                         IsSelected = true,
                         Item = s
                     });
            }                            

            monstersView = new ListCollectionView(monstersList);
            monstersView.SortDescriptions.Add(new SortDescription("Item.Name", ListSortDirection.Ascending));

            spellsView = new ListCollectionView(spellsList);
            spellsView.SortDescriptions.Add(new SortDescription("Item.Name", ListSortDirection.Ascending));

			featsView = new ListCollectionView(featsList);
            featsView.SortDescriptions.Add(new SortDescription("Item.Name", ListSortDirection.Ascending));

            conditionsView = new ListCollectionView(conditionsList);
            conditionsView.SortDescriptions.Add(new SortDescription("Item.Name", ListSortDirection.Ascending));

			
            MonstersList.DataContext = monstersView;
			SelectAllMonstersCheckbox.IsEnabled = monstersList.Count > 0;
			
            SpellsList.DataContext = spellsView;
			SelectAllSpellsCheckbox.IsEnabled = spellsList.Count > 0;
			
            FeatsList.DataContext = featsView;
            SelectAllFeatsCheckbox.IsEnabled = featsList.Count > 0;

            ConditionsList.DataContext = conditionsView;
            SelectAllConditionsCheckbox.IsEnabled = conditionsList.Count > 0;


            selectedMonstersView = new ListCollectionView(monstersList);
            selectedMonstersView.Filter = m => ((ExportItem)m).IsSelected; 
            monstersView.Filter = a => MatchesFilter(((Monster)((ExportItem)a).Item).Name, MonsterFilterTextBox);
            MonsterFilterTextBox.TextChanged += (a, b) => { monstersView.Refresh(); selectedMonstersView.Refresh(); };
            MonstersTab.DataContext = selectedMonstersView;
			
            selectedSpellsView = new ListCollectionView(spellsList);
            selectedSpellsView.Filter = m => ((ExportItem)m).IsSelected;
            spellsView.Filter = a => MatchesFilter(((Spell)((ExportItem)a).Item).Name, SpellFilterTextBox);
            SpellFilterTextBox.TextChanged += (a, b) => { spellsView.Refresh(); selectedSpellsView.Refresh(); };
            SpellsTab.DataContext = selectedSpellsView;

            selectedFeatsView = new ListCollectionView(featsList);
            selectedFeatsView.Filter = m => ((ExportItem)m).IsSelected;
            featsView.Filter = a => MatchesFilter(((Feat)((ExportItem)a).Item).Name, FeatFilterTextBox);
            FeatFilterTextBox.TextChanged += (a, b) => { featsView.Refresh(); selectedFeatsView.Refresh(); };
            FeatsTab.DataContext = selectedFeatsView;

            selectedConditionsView = new ListCollectionView(conditionsList);
            selectedConditionsView.Filter = m => ((ExportItem)m).IsSelected;
            conditionsView.Filter = a => MatchesFilter(((Condition)((ExportItem)a).Item).Name, ConditionFilterTextBox);
            ConditionFilterTextBox.TextChanged += (a, b) => { conditionsView.Refresh(); selectedConditionsView.Refresh(); };
            ConditionsTab.DataContext = selectedConditionsView;



			
        }

        public bool MatchesFilter(string name, TextBox textBox)
        {
            if (name == null || name == "")
            {
                return true;
            }
            return (name.ToUpper().Contains(textBox.Text.Trim().ToUpper()));
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!importMode)
            {
                Export();
            }
            else
            {
                Import();
            }
        }

        private void Export()
        {

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.Filter = exportFileFilter;

            if (dlg.ShowDialog() == true)
            {
                ExportData d = new ExportData();
                d.Monsters.AddRange(from x in monstersList where x.IsSelected select (Monster)x.Item);
                foreach (var source in d.Monsters.Where(source => source.SkillsParsed))
                {
                    source.SkillsParsed = false;
                }
                d.Spells.AddRange(from x in spellsList where x.IsSelected select (Spell)x.Item);
                d.Feats.AddRange(from x in featsList where x.IsSelected select (Feat)x.Item);
                d.Conditions.AddRange(from x in conditionsList where x.IsSelected select (Condition)x.Item);
                XmlLoader<ExportData>.Save(d, dlg.FileName);
                this.Close();
            }
        }

        private void Import()
        {

            foreach (Monster m in from x in monstersList where x.IsSelected select (Monster)x.Item)
            {
                m.DBLoaderID = 0;
                MonsterDB.DB.AddMonster(m);
                Monster.Monsters.Add(m);
            }
            foreach (Spell s in from x in spellsList where x.IsSelected select (Spell)x.Item)
            {
                s.DBLoaderID = 0;
                Spell.AddCustomSpell(s);
            }
            foreach (Feat s in from x in featsList where x.IsSelected select (Feat)x.Item)
            {
                s.DBLoaderID = 0;
                Feat.AddCustomFeat(s);
            }
            bool loadedConditions = false;
            foreach (Condition s in from x in conditionsList where x.IsSelected select (Condition)x.Item)
            {
                Condition.CustomConditions.Add(s);
                loadedConditions = true;
            }
            if (loadedConditions)
            {
                Condition.SaveCustomConditions();
            }
            this.Close();

        }



        private void SelectAllMonstersCheckbox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (var m in from x in monstersList where MatchesFilter(GetExportItemName(x), MonsterFilterTextBox) select x)
            {

                m.IsSelected = SelectAllMonstersCheckbox.IsChecked == true;
            }
			UpdateOK();
        }



        private void SelectAllSpellsCheckbox_Click(object sender, RoutedEventArgs e)
        {

            foreach (var m in from x in spellsList where MatchesFilter(GetExportItemName(x), SpellFilterTextBox) select x)
            {
                m.IsSelected = SelectAllSpellsCheckbox.IsChecked == true;
            }
			UpdateOK();
        }

		private void SelectAllFeatsCheckbox_Click(object sender, System.Windows.RoutedEventArgs e)
		{
            foreach (var m in from x in featsList where MatchesFilter(GetExportItemName(x), FeatFilterTextBox) select x)
            {
                m.IsSelected = SelectAllFeatsCheckbox.IsChecked == true;
            }
			UpdateOK();
		}

        private void ItemCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	ExportItem item = (ExportItem)((FrameworkElement)sender).DataContext;

            CheckBox clickedBox = (CheckBox)sender;
            
            ListBox listBox = null;
			ObservableCollection<ExportItem> view  = null;
			CheckBox box = null;
            TextBox textbox = null;
			if (item.Item is Monster)
			{
				view = monstersList;
				box = SelectAllMonstersCheckbox;
                listBox = MonstersList;
                textbox = MonsterFilterTextBox;
			}
			else if (item.Item is Spell)
			{
				view = spellsList;
				box = SelectAllSpellsCheckbox;
                listBox = SpellsList;
                textbox = SpellFilterTextBox;
			}
			else if (item.Item is Feat)
			{
				view = featsList;
				box = SelectAllFeatsCheckbox;
                listBox = FeatsList;
                textbox = FeatFilterTextBox;
			}
            else if (item.Item is Condition)
            {
                view = conditionsList;
                box = SelectAllConditionsCheckbox;
                listBox = ConditionsList;
                textbox = ConditionFilterTextBox;
            }
            System.Diagnostics.Debug.Assert(view != null);
           
			bool selected = false;
			bool unselected = false;
            foreach (ExportItem x in view)
			{
                if (MatchesFilter(GetExportItemName(x), textbox))
                {

                    if (x.IsSelected)
                    {
                        selected = true;
                    }
                    else
                    {
                        unselected = true;
                    }
                    if (selected && unselected)
                    {
                        break;
                    }
                }
			}
			
			
			
			if (selected && unselected)
			{
                box.IsChecked = null;
			}
			else 
			{
				box.IsChecked = selected;
			}
			UpdateOK();
        }

        string GetExportItemName(ExportItem item)
        {
            return (string)item.Item.GetType().GetProperty("Name").GetGetMethod().Invoke(item.Item, new object[] { });
        }
		
		void UpdateOK()
		{
            OKButton.IsEnabled = (SelectAllSpellsCheckbox.IsChecked != false) 
                || (SelectAllMonstersCheckbox.IsChecked != false)
			|| (SelectAllFeatsCheckbox.IsChecked != false) ||
            (SelectAllConditionsCheckbox.IsChecked != false);

            selectedMonstersView.Refresh();
            selectedSpellsView.Refresh();
            selectedFeatsView.Refresh();
            selectedConditionsView.Refresh();
		}

        class ExportItem : INotifyPropertyChanged
        {

            public event PropertyChangedEventHandler PropertyChanged;

            private bool _IsSelected;
            private object _Item;

            public bool IsSelected
            {
                get { return _IsSelected; }
                set
                {
                    if (_IsSelected != value)
                    {
                        _IsSelected = value;
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("IsSelected")); }
                    }
                }
            }
            public object Item
            {
                get { return _Item; }
                set
                {
                    if (_Item != value)
                    {
                        _Item = value;
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Item")); }
                    }
                }
            }


        }

        private void SelectAllConditionsCheckbox_Click(object sender, RoutedEventArgs e)
        {

            foreach (var m in from x in conditionsList where MatchesFilter(GetExportItemName(x), ConditionFilterTextBox) select x)
            {
                m.IsSelected = SelectAllConditionsCheckbox.IsChecked == true;
            }
            UpdateOK();
        }
    }
}
