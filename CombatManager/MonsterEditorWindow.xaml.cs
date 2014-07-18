/*
 *  MonsterEditorWindow.xaml.cs
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
﻿using System.Globalization;
﻿using System.Text;
﻿using System.Text.RegularExpressions;
﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Threading;

namespace CombatManager
{
	/// <summary>
	/// Interaction logic for MonsterEditorWindow.xaml
	/// </summary>
	public partial class MonsterEditorWindow : Window
	{
		Monster _OriginalMonster;
		Monster _Monster;
        ICollectionView _SkillsView;
        ICollectionView _SelectableSkillsView;
        ICollectionView _SpecialAbilitiesView;

        private bool _Initialized;

        private static List<String> _OptionsSkills = new List<string>(new string[]
            {"Craft", "Knowledge", "Perform", "Profession"});
		
		public MonsterEditorWindow()
		{
			this.InitializeComponent();

            List<String> skillList = new List<string>(Monster.SkillsList.Keys);

            _SelectableSkillsView = new ListCollectionView(skillList);
            _SelectableSkillsView.Filter += new Predicate<object>(SelectableSkillsFilter);
            
            AvailableSkillsList.DataContext = _SelectableSkillsView;
			AvailableSkillsList.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(AvailableSkillsList_SelectionChanged);
            bool loaded = false;
            SkillSubtypeTextBox.GotFocus += delegate
            {
                if (!loaded)
                {

                    TextBox tb = SkillSubtypeTextBox.Template.FindName("PART_EditableTextBox", SkillSubtypeTextBox) as TextBox;
                    if (tb != null)
                    {
                        tb.TextChanged += new TextChangedEventHandler(SkillSubtypeTextBox_TextChanged);

                        loaded = true;
                    }
                }
            };

            SkillSubtypeTextBox.IsVisibleChanged += delegate
            {
                if (!loaded)
                {

                    TextBox tb = SkillSubtypeTextBox.Template.FindName("PART_EditableTextBox", SkillSubtypeTextBox) as TextBox;
                    if (tb != null)
                    {
                        tb.TextChanged += new TextChangedEventHandler(SkillSubtypeTextBox_TextChanged);

                        loaded = true;
                    }
                }
            };
            
            
            _Initialized = true;
            EnabledOK();
        }


		
		public Monster Monster
		{
			get
			{
				return _Monster;
			}
			set
			{
				if (_OriginalMonster != value)
				{
                    _OriginalMonster = value;


					if (_Monster != null)
					{
                        _Monster.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Monster_PropertyChanged);
                    }
                    _Monster = (Monster)((Monster)value).Clone();
                    DataContext = _Monster;
					FeatChangeControl.Monster = _Monster;

					UpdateFieldsForMonster();

                    //UpdateMonsterFlowDocument();

                    if (_Monster != null)
                    {
                        _Monster.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Monster_PropertyChanged);
                    }
				}				
			}
		}

        void Monster_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Description" && sender == _Monster)
            {
                _Monster.DescHTML = null;
            }

            //UpdateMonsterFlowDocument();
        }

        /*private void UpdateMonsterFlowDocument()
        {

            MonsterFlowDocument.Document.Blocks.Clear() ;
            if (_Monster != null)
            {
                MonsterBlockCreator bc = new MonsterBlockCreator(MonsterFlowDocument.Document, null, null);
                MonsterFlowDocument.Document.Blocks.AddRange(bc.CreateBlocks(_Monster, true));
            }
        }*/

		private void UpdateFieldsForMonster()
		{
            _SkillsView = new ListCollectionView(_Monster.SkillValueList);
            SkillsListBox.DataContext = _SkillsView;
            _SelectableSkillsView.Refresh();
            _SpecialAbilitiesView = new ListCollectionView(_Monster.SpecialAbilitiesList);
            SpecialAbilitiesListBox.DataContext = _SpecialAbilitiesView;
		}

        private void TextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;

            box.SelectAll();
        }

        private void EditAttackButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HDButton_Click(object sender, RoutedEventArgs e)
        {
            DieRollEditWindow editWindow = new DieRollEditWindow();
            editWindow.Roll = DieRoll.FromString(Monster.HD);
            editWindow.HasToughness = _Monster.HasFeat("Toughness");
            
            editWindow.HPstatmod = (int) (_Monster.Type != "undead" ? 
                ((((_Monster.Constitution!= null)?_Monster.Constitution:10)/2) - 5) : 
                ((((_Monster.Charisma!=null)?_Monster.Charisma:10)/2) - 5));
            editWindow.Owner = this;
            editWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (editWindow.ShowDialog() == true)
            {
                Monster.HD = "(" + editWindow.Roll.Text + ")";
                Monster.HP = editWindow.Roll.AverageRoll();
            }


        }

        private void AuraCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
        	_Monster.Aura = null;
        }


        private void AttacksEditorButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AttacksWindow window = new AttacksWindow();
            window.Monster = _Monster;
            window.Owner = this;
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        private void ManualEditButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ManualAttacksEditWindow window = new ManualAttacksEditWindow();
            window.Monster = _Monster;
            window.Owner = this;
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        private void SkillDeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	SkillValue sv = (SkillValue)((FrameworkElement)sender).DataContext;
            _Monster.SkillValueDictionary.Remove(sv.FullName);
            _Monster.UpdateSkillValueList();
            _SkillsView.Refresh();
            _SelectableSkillsView.Refresh();
        }

        public bool SelectableSkillsFilter(object ob)
        {
            string skill = (string)ob;

            if (_Monster == null)
            {

                return true;
            }
            else if (_OptionsSkills.Contains(skill))
            {
                return true;
            }
            else
            {
                return !_Monster.SkillValueDictionary.ContainsKey(skill);
            }
        }

        private void AddSkillButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			AddSelectedSkill();
            
        }

		private void AddSelectedSkill()
		{            
			bool add = true;
            String skill = (string)AvailableSkillsList.SelectedValue;

            SkillValue sv = new SkillValue(skill);

            if (_OptionsSkills.Contains(skill))
            {
                string subtype = SkillSubtypeTextBox.Text.Trim().ToLower();

                if (subtype.Length == 0)
                {
                    add = false;
                }
                sv.Subtype = subtype;
            }

            if (_Monster.SkillValueDictionary.ContainsKey(sv.FullName))
            {
                add = false;
            }

            if (add)
            {
                _Monster.AddOrChangeSkill(sv.Name, sv.Subtype, 0);
                _Monster.UpdateSkillValueList();
                _SkillsView.Refresh();
                _SelectableSkillsView.Refresh();
                
            }
		}
		
        private void AvailableSkillsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        	String skill = (string)AvailableSkillsList.SelectedValue;
			
			if (skill == null)
			{
                SkillSubtypeTextBox.Visibility = Visibility.Hidden;
			}
			else				
			{
                SkillSubtypeTextBox.Visibility = _OptionsSkills.Contains(skill)?Visibility.Visible:Visibility.Hidden;
                SkillSubtypeTextBox.Items.Clear();

                if (SkillSubtypeTextBox.Visibility == System.Windows.Visibility.Visible)
                {

                    Monster.SkillInfo det = Monster.SkillsDetails[skill];

                    foreach (var v in det.Subtypes)
                    {
                        SkillSubtypeTextBox.Items.Add(v);
                    }
                }


			}
            UpdateAddSkillButton();
        }



        void SkillSubtypeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        
        {
            UpdateAddSkillButton();
        }


        private void UpdateAddSkillButton()
        {
            String skill = (string)AvailableSkillsList.SelectedValue;

            if (skill == null)
            {
                AddSkillButton.IsEnabled = false;
            }
            else
            {
                if (!_OptionsSkills.Contains(skill))
                {
                    AddSkillButton.IsEnabled = true;
                }
                else
                {
                    string subtype = SkillSubtypeTextBox.Text.Trim().ToLower();

                    if (subtype.Length == 0)
                    {
                        AddSkillButton.IsEnabled = false;
                    }
                    else
                    {
                        SkillValue sv = new SkillValue(skill);

                        sv.Subtype = subtype;

                        if (_Monster.SkillValueDictionary.ContainsKey(sv.FullName))
                        {
                            AddSkillButton.IsEnabled = false;
                        }
                        else
                        {
                            AddSkillButton.IsEnabled = true;
                        }
                    }
                }
            }

        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }

        private void SpecialAbilityDeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	SpecialAbility ab = (SpecialAbility)((FrameworkElement)sender).DataContext;
			
			_Monster.SpecialAbilitiesList.Remove(ab);
        }

        private void AddSpecialAbilityButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	SpecialAbility ab = new SpecialAbility();
			ab.Name = "";
			ab.Type = "Ex";
			
            
			_Monster.SpecialAbilitiesList.Add(ab);
            SpecialAbilitiesListBox.DataContext = _SpecialAbilitiesView;
            _SpecialAbilitiesView.Refresh();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            FixSenses();
            _Monster.CreateSkillString();
            _OriginalMonster.CopyFrom(_Monster);
            DialogResult = true;
            Close();
        }


        private void AvailableSkillsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
			if (CMUIUtilities.ClickedListBoxItem((ListBox)sender, e) != null)
            {
				AddSelectedSkill();
			}
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnabledOK();
        }

        private void EnabledOK()
        {
            if (_Initialized)
            {
                OKButton.IsEnabled = NameTextBox.Text.Length > 0;
            }
        }

	    private void FixSenses()
	    {
	        Type targeTypes = typeof(string);
	        var objects = new object[2];
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
	        var xConverter = new SensesConverter();
	        objects[0] = _Monster.Senses;
	        objects[1] = _Monster.Perception;
	        var senses = (string)xConverter.Convert(objects, targeTypes, null, currentCulture);
	        string sensesText = "";
	        if (senses.Length > 0)
	        {
	            sensesText += senses + "; ";
	        }
	       _Monster.Senses = sensesText + "Perception " + CMStringUtilities.PlusFormatNumber(_Monster.Perception);
	    }

			
	}
}