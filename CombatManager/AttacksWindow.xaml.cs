/*
 *  AttacksWindow.xaml.cs
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
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace CombatManager
{
    /// <summary>
    /// Interaction logic for AttacksWindow.xaml
    /// </summary>
    public partial class AttacksWindow : Window
    {
        private Monster _Monster;
        private ICollectionView meleeView;
        private ICollectionView rangedView;
        private ICollectionView naturalView;
        CharacterAttacks attacks;

        private class WeaponSpecialAbilityItem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private bool _Checked;
            private WeaponSpecialAbility _Special;
            private WeaponItem _WeaponItem;
            
            public WeaponSpecialAbilityItem() { }

            public WeaponSpecialAbilityItem(bool Checked, WeaponSpecialAbility Special, WeaponItem WeaponItem)
            {
                this._Checked = Checked;
                this._Special = Special;
                this._WeaponItem = WeaponItem;
            }

            public bool Checked
            {
                get { return _Checked; }
                set
                {
                    if (_Checked != value)
                    {
                        _Checked = value;

                        SortedDictionary<string, string> abilities = WeaponItem.SpecialAbilitySet;

                        if (_Checked)
                        {
                            abilities.Add(Special.Name, Special.Name);

                            if (WeaponItem.MagicBonus < 1)
                            {
                                WeaponItem.MagicBonus = 1;
                                WeaponItem.Masterwork = false;
                            }
                        }
                        else
                        {
                            abilities.Remove(Special.Name);

                        }


                        WeaponItem.SpecialAbilitySet = abilities;

                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Checked")); }
                    }
                }
            }
            public WeaponSpecialAbility Special
            {
                get { return _Special; }
                set
                {
                    if (_Special != value)
                    {
                        _Special = value;
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Special")); }
                    }
                }
            }
            public WeaponItem WeaponItem
            {
                get { return _WeaponItem; }
                set
                {
                    if (_WeaponItem != value)
                    {
                        _WeaponItem = value;
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("WeaponItem")); }
                    }
                }
            }
        }

		private CharacterAttacks characterAttacks;

        private bool changingItem;

        public AttacksWindow()
        {
            InitializeComponent();
        }

        public Monster Monster
        {
            get
            {
                return _Monster;
            }
            set
            {
                if (_Monster != value)
                {
                    _Monster = value;

                    this.DataContext = _Monster;

                    SetupGrid();
                }
            }
        }

        public CharacterAttacks Attacks
        {
            get
            {
                return characterAttacks;
            }
        }

        public void SetupGrid()
        {
            if (_Monster != null)
            {
                ObservableCollection<AttackSet> sets = new ObservableCollection<AttackSet>(_Monster.MeleeAttacks);
                ObservableCollection<Attack> ranged = new ObservableCollection<Attack>(_Monster.RangedAttacks);
                attacks = new CharacterAttacks(sets, ranged);
				
				characterAttacks = attacks;

				
                meleeView = new ListCollectionView( attacks.MeleeWeaponSets);

                MeleeTabControl.DataContext = meleeView;

                rangedView = new ListCollectionView(attacks.RangedWeapons);
                RangedList.DataContext = rangedView;

                naturalView = new ListCollectionView(attacks.NaturalAttacks);
                NaturalList.DataContext = naturalView;

                if (_Monster.Shield > 0)
                {
                    attacks.Hands = 1;
                }
				
				HandsBorder.DataContext = this;

                UpdateMeleeString();
                UpdateRangedString();

            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            WeaponItem item = (WeaponItem)((Button)sender).DataContext;



            if (attacks.RangedWeapons.Contains(item))
            {
                attacks.RangedWeapons.Remove(item);

                UpdateRangedString();
            }
            else if  (attacks.NaturalAttacks.Contains(item))
            {                
                attacks.NaturalAttacks.Remove(item);

                UpdateMeleeString();
            }
            else if (!item.Weapon.Natural)
            {
                List<WeaponItem> list = (List<WeaponItem>)meleeView.CurrentItem;

                list.Remove(item);

                int i = 0;

                foreach (WeaponItem listItem in list)
                {
                    listItem.MainHand = (i == 0);
                    i++;
                }

                meleeView.Refresh();

                UpdateMeleeString();
            }
        }


        private void MagicBonusComboBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            WeaponItem item = (WeaponItem)box.DataContext;
            item.PropertyChanged += delegate(object send, PropertyChangedEventArgs eChanged)
            {
                if ((eChanged.PropertyName == "MagicBonus") || (eChanged.PropertyName == "Masterwork") || (eChanged.PropertyName == "Broken"))
                {

                    if (!changingItem)
                    {
                        UpdateMagicCombo(box, item);
                    }
                }
            };

            UpdateMagicCombo(box, item);
			
        }

        private void UpdateMagicCombo(ComboBox box, WeaponItem item)
        {
            
            if (item.MagicBonus > 0)
            {
                box.SelectedIndex = item.MagicBonus + 2;

            }
            else if (item.Masterwork)
            {
                box.SelectedIndex = 2;
            }
            else if (item.Broken)
            {
                box.SelectedIndex = 0;
            }
            else
            {
                box.SelectedIndex = 1;
            }

        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
        }

        private void MagicBonusComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            WeaponItem item = (WeaponItem)box.DataContext;

            if (e.RemovedItems.Count > 0)
            {
                changingItem = true;
                if (box.SelectedIndex == 0)
                {
                    item.Broken = true;
                    item.Masterwork = false;
                    item.MagicBonus = 0;
                    item.SpecialAbilitySet = new SortedDictionary<string, string>();
                }
                else if (box.SelectedIndex == 1)
                {
                    item.Broken = false;
                    item.Masterwork = false;
                    item.MagicBonus = 0;
                    item.SpecialAbilitySet = new SortedDictionary<string, string>();
                }
                else if (box.SelectedIndex == 2)
                {
                    item.Broken = false;
                    item.Masterwork = true;
                    item.MagicBonus = 0;
                    item.SpecialAbilitySet = new SortedDictionary<string, string>();
                }
                else if (box.SelectedIndex > 2)
                {
                    item.Broken = false;
                    item.Masterwork = false;
                    item.MagicBonus = box.SelectedIndex - 2;
                }
                changingItem = false;

                if (attacks.RangedWeapons.Contains(item))
                {
                    UpdateRangedString();
                }
                else
                {
                    UpdateMeleeString();
                }
            }
				
        }

        private void SpecialAbilitiesPopup_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        	
        }			

        private void SpecialAbilitesListBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        	ListBox box = (ListBox)sender;
            if (box.DataContext.GetType() == typeof(WeaponItem))
            {
                WeaponItem item = (WeaponItem)box.DataContext;

                List<WeaponSpecialAbilityItem> list = new List<WeaponSpecialAbilityItem>();

                if (item.Weapon.Ranged)
                {
                    foreach (WeaponSpecialAbility ability in WeaponSpecialAbility.RangedAbilities)
                    {

                        bool contains = false;

                        if (item.SpecialAbilitySet.ContainsKey(ability.Name))
                        {
                            contains = true;
                        }
                        else if (ability.AltName != null && ability.AltName.Length > 0 &&
                            item.SpecialAbilitySet.ContainsKey(ability.AltName))
                        {
                            contains = true;
                        }

                        list.Add(new WeaponSpecialAbilityItem(contains,
                            ability, item));

                    }

                }
                else
                {
                    foreach (WeaponSpecialAbility ability in WeaponSpecialAbility.MeleeAbilities)
                    {
                        bool contains = false;

                        if (item.SpecialAbilitySet.ContainsKey(ability.Name))
                        {
                            contains = true;
                        }
                        else if (ability.AltName != null && ability.AltName.Length > 0 &&
                            item.SpecialAbilitySet.ContainsKey(ability.AltName))
                        {
                            contains = true;
                        }


                        list.Add(new WeaponSpecialAbilityItem(contains,
                            ability, item));



                    }
                }

                box.DataContext = list;
            }
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void AddWeaponButton_Click(object sender, RoutedEventArgs e)
        {
			
            Button button = (Button)sender;
			
            List<WeaponItem> items = (List<WeaponItem>)button.DataContext;

            WeaponItem item = ShowMeleeWeaponDialog(items, true, false, false);
			
			
			if (item != null)
			{
                if (items.Count == 0)
                {
                    item.MainHand = true;
                }

                items.Add(item);
				
        		meleeView.Refresh();

                UpdateMeleeString();
			}

            button.IsEnabled = (characterAttacks.Hands > CountHands(items)) ? true : false;
        }

        private WeaponItem ShowMeleeWeaponDialog(IEnumerable<WeaponItem> items, bool melee, bool ranged, bool natural)
        {
            WeaponSelectWindow wind = new WeaponSelectWindow();
            wind.Owner = this;
            wind.Melee = melee;
			wind.Ranged = ranged;
			wind.Natural = natural;
            wind.Hands = characterAttacks.Hands - CountHands(items);
            wind.Size = SizeMods.GetSize(_Monster.Size);

            wind.WindowStartupLocation = WindowStartupLocation.CenterOwner;

			WeaponItem item = null;

            if (wind.ShowDialog() == true)
            {
                if (wind.Weapon != null)
                {
                    item = new WeaponItem(wind.Weapon);

                    
                }
            }
			
			return item;
        }

        private void UpdateMeleeString()
        {

            MeleeTextBlock.Text = _Monster.MeleeString(characterAttacks);
        }


        private void UpdateRangedString()
        {

            RangedTextBlock.Text = _Monster.RangedString(characterAttacks);
        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
        	 UpdateMeleeString();
             UpdateRangedString();
        }

        private void CheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateMeleeString();
            UpdateRangedString();
        }

        private int CountHands(IEnumerable<WeaponItem> items)
        {
            int count = 0;

            foreach (WeaponItem item in items)
            {
                count += item.Weapon.HandsUsed;
            }


            return count;
        }

        private void AddWeaponButton_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
			Button button = (Button)sender;

            List<WeaponItem> items = (List<WeaponItem>)button.DataContext;
			

			button.IsEnabled = (characterAttacks.Hands > CountHands(items))?true:false;
        }

        private void AddWeaponButton_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
        
			Button button = (Button)sender;

            if (button.DataContext != null && button.DataContext.GetType() == typeof(List<WeaponItem>))
            {

                List<WeaponItem> items = (List<WeaponItem>)button.DataContext;

                button.IsEnabled = (characterAttacks.Hands > CountHands(items)) ? true : false;
            }
            else
            {
                button.IsEnabled = true;
            }

        }

        private void AddSetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	if (characterAttacks.Hands > 0)
			{
                characterAttacks.MeleeWeaponSets.Add(new List<WeaponItem>());
			}

            meleeView.Refresh();
			if (meleeView.CurrentItem == null)
			{
				meleeView.MoveCurrentToFirst();
			}

            UpdateMeleeString();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			Button button = (Button)sender;
			

            WeaponItem item = ShowMeleeWeaponDialog(attacks.RangedWeapons, false, false, true);
			
			
			if (item != null)
			{
                bool bAdded = false;
                foreach (WeaponItem wi in attacks.NaturalAttacks)
                {
                    if (String.Compare(wi.Name, item.Name, true) == 0)
                    {
                        wi.Count++;
                        bAdded = true;
                        break;
                    }

                }

                if (!bAdded)
                {
                    attacks.NaturalAttacks.Add(item);
                }
				
        		naturalView.Refresh();

                UpdateMeleeString();
			}
        }

        private void RemoveSetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            List<WeaponItem> items = (List<WeaponItem>)MeleeTabControl.SelectedItem;



            characterAttacks.MeleeWeaponSets.Remove(items);

            meleeView.Refresh();
			if (meleeView.CurrentItem == null)
			{
				meleeView.MoveCurrentToFirst();
			}

            UpdateMeleeString();
	
        }

        private void AddRangedWeaponButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
			
            Button button = (Button)sender;
			

            WeaponItem item = ShowMeleeWeaponDialog(attacks.RangedWeapons, false, true, false);
			
			
			if (item != null)
			{

                attacks.RangedWeapons.Add(item);
				
        		rangedView.Refresh();

                UpdateRangedString();
			}

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _Monster.Melee = _Monster.MeleeString(attacks);
            _Monster.Ranged = _Monster.RangedString(attacks);

            DialogResult = true;

            Close();
        }

        private void TextBlock_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBlock block = (TextBlock)sender;

            List<WeaponItem> list = (List<WeaponItem>)block.DataContext;

            int index = characterAttacks.MeleeWeaponSets.IndexOf(list);

            block.Text = "Set #" + (index + 1);
        }

        private void EditNaturalAttackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NaturalAttackWindow window = new NaturalAttackWindow();
            window.WeaponItem = (WeaponItem)((Button)sender).DataContext;
            window.Monster = this._Monster;
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                naturalView.Refresh();
                UpdateMeleeString();
            }
        }

        private void AddNaturalAttackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            NaturalAttackWindow window = new NaturalAttackWindow();
            window.Monster = this._Monster;
            window.Owner = this;
            if (window.ShowDialog() == true)
            {
                attacks.NaturalAttacks.Add(window.WeaponItem);
                naturalView.Refresh();
                UpdateMeleeString();
            }
        }

		private void Attack_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Image image = sender as Image;
			WeaponItem item = image.DataContext as WeaponItem;
			if (item != null && item.Weapon.Hands.Equals("One-Handed", StringComparison.InvariantCultureIgnoreCase))
			{
				item.TwoHanded = !item.TwoHanded;
				UpdateMeleeString();
			}
		}
    }
}
