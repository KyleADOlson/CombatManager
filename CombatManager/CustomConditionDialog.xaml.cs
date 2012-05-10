/*
 *  CustomConditionDialog.xaml.cs
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

namespace CombatManager
{
    /// <summary>
    /// Interaction logic for CustomConditionDialog.xaml
    /// </summary>
    public partial class CustomConditionDialog : Window
    {
        public Condition _Condition;
        public Condition _OriginalCondition;

        public CustomConditionDialog()
        {
            InitializeComponent();
			
			CustomImageBox.SelectedIndex = 0;

            _Condition = new Condition();

            UpdateOK();
        }


        private void CustomBonusCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            CustomBonusBorder.DataContext = new ConditionBonus();
        }

        private void CustomBonusCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            CustomBonusBorder.DataContext = null;
        }

        private void ClearCustomButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CustomTextBox.Text = "";
            CustomNameBox.Text = "";
            CustomBonusCheckBox.IsChecked = false;
            CustomImageBox.SelectedIndex = 0;
        }

        public Condition Condition
        {
            get
            {
                return _Condition;
            }
            set
            {
                _Condition = new Condition(value);
                _OriginalCondition = value;
                UpdateCustomCondition();
                UpdateOK();
            }
        }

        private void UpdateCustomCondition()
        {

            CustomNameBox.Text = _Condition.Name;
            CustomTextBox.Text = _Condition.Text;

            CustomImageBox.SelectedItem = _Condition.Image;

            CustomBonusCheckBox.IsChecked = _Condition.Bonus != null;

            if (_Condition.Bonus != null)
            {
                CustomBonusBorder.DataContext = _Condition.Bonus;
            }


            CustomBonusCheckBox.IsChecked = (_Condition.Bonus != null);
			
			UpdateOK();
        }

        public Condition GetCustomCondition()
        {
            _Condition.Name = CustomNameBox.Text.Trim();
            if (CustomTextBox.Text.Length > 0)
            {
                _Condition.Text = CustomTextBox.Text;
            }

            if (CustomImageBox.SelectedIndex != -1)
            {
                _Condition.Image = (string)CustomImageBox.SelectedItem;
            }
            else
            {
                _Condition.Image = "star";
            }
            _Condition.Custom = true;

            _Condition.Bonus = null;
            if (CustomBonusCheckBox.IsChecked == true)
            {
                ConditionBonus b = (ConditionBonus)CustomBonusBorder.DataContext;

                _Condition.Bonus = b;
            }

            return _Condition;

        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
        	GetCustomCondition();

            bool conditionOK = true;

            if (_OriginalCondition == null || _OriginalCondition.Name.CompareTo(_Condition.Name) != 0)
            {
                Condition c = Condition.ByNameCustom(_Condition.Name);

                if (c != null)
                {
                    if (c != _OriginalCondition)
                    {
                        MessageBox.Show(this, "A custom condition with this name already exists. Please choose a different name.", "Custom Condition", MessageBoxButton.OK, MessageBoxImage.Warning);

                        conditionOK = false;
                    }
                }
                else
                {
                    c = Condition.ByName(_Condition.Name);

                    if (c != null)
                    {
                        MessageBox.Show(this, "A condition with this name already exists. Please choose a different name.", "Custom Condition", MessageBoxButton.OK, MessageBoxImage.Warning);

                        conditionOK = false;
                    }
                }
            }

            if (conditionOK)
            {
                DialogResult = true;
                this.Close();
            }
        }

        private void CustomNameBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
			UpdateOK();
        }
		
		private void UpdateOK()
		{
            if (OKButton != null && CustomNameBox != null)
            {
                OKButton.IsEnabled = CustomNameBox.Text.Trim().Length > 0;
            }
			
		}
    }
}
