/*
 *  NaturalAttackWindow.xaml.cs
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

namespace CombatManager
{
    /// <summary>
    /// Interaction logic for NaturalAttackWindow.xaml
    /// </summary>
    public partial class NaturalAttackWindow : Window
    {
        ICollectionView weaponView;
        WeaponItem weaponItem;
        Monster monster;


        public NaturalAttackWindow()
        {
            InitializeComponent();

            weaponView = new ListCollectionView(new List<Weapon>(Weapon.Weapons.Values));
            weaponView.Filter += new Predicate<object>(NaturalWeaponFilter);
            weaponView.SortDescriptions.Add(
                new SortDescription("Name", ListSortDirection.Ascending));
            weaponView.MoveCurrentToFirst();
            weaponView.CurrentChanged += new EventHandler(weaponView_CurrentChanged);
            NaturalAttackListBox.GotFocus += new RoutedEventHandler(NaturalAttackListBox_GotFocus);

            NaturalAttackListBox.DataContext = weaponView;

            DieRoll step = new DieRoll(0, 1, 0);

            for (int i = 0; i < 12; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                
                item.DataContext = step.Step;
                item.Content = step.Text;
                AttackDamage.Items.Add(item);
                step = DieRoll.StepDie(step, 1);
            }

            for (int i = 1; i < 12; i++)
            {

                ComboBoxItem item = new ComboBoxItem();

                item.DataContext = i;
                item.Content = i.ToString();
                AttackCountComboBox.Items.Add(item);
            }

            EnableOK();

        }

        void NaturalAttackListBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Weapon weapon = (Weapon)weaponView.CurrentItem;

            if (weapon != null)
            {
                CreateWeaponItem(weapon);
            }
        }


        void weaponView_CurrentChanged(object sender, EventArgs e)
        {
            Weapon weapon = (Weapon)weaponView.CurrentItem;

            if (weapon != null)
            {
                CreateWeaponItem(weapon);
            }
        }

        public bool NaturalWeaponFilter(object ob)
        {
            Weapon weapon = (Weapon)ob;

            return weapon.Natural;
        }

        private void CreateWeaponItem(Weapon weapon)
        {

            WeaponItem item = new WeaponItem(weapon);

            DieRoll roll = DieRoll.FromString(item.Weapon.DmgM);
            roll = DieRoll.StepDie(roll, ((int)SizeMods.GetSize(monster.Size)) - (int)MonsterSize.Medium);
            item.Step = roll.Step;

            WeaponItem = item;
        }

        public WeaponItem WeaponItem
        {
            get
            {
                return weaponItem;
            }
            set
            {
                if (weaponItem != value)
                {
                    weaponItem = value;

                    DataContext = weaponItem;
                    UpdateWeaponItemFields();
                    
                    if (weaponItem.PlusList != null)
                    {

                    }
                }
            }
        }

        public Monster Monster
        {
            get
            {
                return monster;
            }
            set
            {
                if (monster != value)
                {
                    monster = value;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (weaponItem != null)
            {
                UpdateWeaponItemFields();
            }
        }

        private void UpdateWeaponItemFields()
        {

            if (weaponItem != null && this.IsLoaded)
            {
                if (weaponItem.Step == null)
                {
                    DieRoll roll = DieRoll.FromString(weaponItem.Weapon.DmgM);
                    roll = DieRoll.StepDie(roll, ((int)SizeMods.GetSize(monster.Size)) - (int)MonsterSize.Medium);
                    weaponItem.Step = roll.Step;
                }
                AttackDamage.Text = weaponItem.Step.Text;
                AttackName.Text = weaponItem.Name;
                AttackType.Text = weaponItem.Weapon.Light ? "Secondary" : "Primary";
                AttackCountComboBox.Text = weaponItem.Count.ToString();
				PlusTextBox.Text = weaponItem.Plus;
            }

        }

        private void AttackName_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableOK();
        }

        private void AttackDamage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnableOK();
        }

        private void AttackCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AttackType_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        private void PlusTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (weaponItem == null)
            {
                weaponItem = new WeaponItem();

            }
            Weapon weapon = null;
            string atkname = AttackName.Text.Trim();

            if (Weapon.Weapons.ContainsKey(atkname))
            {
                weapon = Weapon.Weapons[atkname];
            }

            if (weapon == null)
            {
                weapon = new Weapon();
                weapon.Name = atkname;
                weapon.Hands = "One-Handed";
                weapon.Class = "Natural";
                DieRoll roll = DieRoll.FromString(AttackDamage.Text);
                roll = DieRoll.StepDie(roll, ((int)SizeMods.GetSize(monster.Size)) - (int)MonsterSize.Medium);
                weapon.DmgM = roll.Text;
                weapon.DmgS = DieRoll.StepDie(roll, -1).Text;
                weaponItem.Weapon = weapon;
            }

                
            weaponItem.Weapon = weapon;
            weaponItem.Count = AttackCountComboBox.SelectedIndex + 1;
            weaponItem.Plus = PlusTextBox.Text;
            weaponItem.Step = DieRoll.FromString(AttackDamage.Text).Step;

            DialogResult = true;
            Close();
            
        }

        private void EnableOK()
        {
            bool hasDamage =  AttackDamage.Text.Length > 0;

            bool hasAttack = false;


            if (AttackName.Text.Trim().Length > 0)
            {
                hasAttack = true;
                string atkname = AttackName.Text.Trim();

                if (Weapon.Weapons.ContainsKey(atkname))
                {
                    Weapon weapon = Weapon.Weapons[atkname];

                    if (!weapon.Natural)
                    {
                        hasAttack = false;
                    }
                    else
                    {
                        AttackType.Text = weapon.Light ? "Secondary" : "Primary";

                    }
                }
                else
                {
                    AttackType.Text = "Primary";
                }
            }


            OKButton.IsEnabled =
                hasAttack && hasDamage;

        }

    }
}
