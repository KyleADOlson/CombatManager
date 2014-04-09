/*
 *  DieRollEditWindow.xaml.cs
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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CombatManager
{
	/// <summary>
	/// Interaction logic for DieRollEditWindow.xaml
	/// </summary>
	public partial class DieRollEditWindow : Window
	{
        DieRoll _Roll;
        NotifyValue<int> _Mod;
        ObservableCollection<DieStep> _Steps;

		public DieRollEditWindow()
		{
			this.InitializeComponent();
            _Steps = new ObservableCollection<DieStep>();
            _Mod = new NotifyValue<int>();
            _Mod.PropertyChanged += new PropertyChangedEventHandler(Mod_PropertyChanged);
            DieRollBonusText.DataContext = _Mod;
			UpdateUI();
		}     
        public int HPstatmod { get; set; }
        public bool HasToughness { get; set; }

        public DieRoll Roll
        {
            get
            {
                return _Roll;
                
            }
            set
            {
                _Roll = value;
                if (_Roll != null)
                {
                    _Steps = new ObservableCollection<DieStep>();
                    _Steps.Add(new DieStep(_Roll.Step.Count, _Roll.Step.Die));

                    if (_Roll.extraRolls != null)
                    {
                        foreach (DieStep s in _Roll.extraRolls)
                        {
                            _Steps.Add(s);
                        }
                    }

                    foreach (DieStep s in _Steps)
                    {
                        s.PropertyChanged += new PropertyChangedEventHandler(DieStep_PropertyChanged);
                    }
                    if (HasToughness)
                    {
                        _Mod.Value = _Roll.mod != HPstatmod*_Roll.TotalCount + (_Roll.TotalCount < 3 ? 3 : _Roll.TotalCount)? _Roll.mod : 0;
                    }
                    else
                    {
                        _Mod.Value = _Roll.mod != HPstatmod * _Roll.TotalCount ? _Roll.mod : 0;
                    }
                    //_Mod.Value = _Roll.mod;

                    DieStepList.DataContext = _Steps;
                    UpdateUI();
                }
                
            }
        }

        private DieRoll MakeRoll()
        {
            DieRoll roll = new DieRoll();

            if (_Steps.Count > 0)
            {
                roll.die = _Steps[0].Die;
                roll.count = _Steps[0].Count;
                

                if (_Steps.Count > 1)
                {
                    roll.extraRolls = new List<DieStep>();
                    for (int i = 1; i < _Steps.Count; i++)
                    {
                        roll.extraRolls.Add(_Steps[i]);
                    }

                }
            }
            
            
                if (HasToughness)
                {                  
                        roll.mod = _Mod.Value == 0?HPstatmod*roll.TotalCount + (roll.TotalCount < 3 ? 3 : roll.TotalCount):roll.mod = _Mod.Value;       
                }
                else
                {
                    roll.mod = _Mod.Value == 0? HPstatmod * roll.TotalCount : roll.mod = _Mod.Value;
                
                }
            

            return roll;
        }

		private void UpdateUI()
		{
			UpdateDieText();
			UpdateOK();
		}
		
        private void UpdateDieText()
        {
            DieRollText.Text = MakeRoll().Text;
        }

        private void UpdateOK()
        {
            OKButton.IsEnabled = MakeRoll().TotalCount > 0;
        }

        void Mod_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
            UpdateUI();
        }

        void DieStep_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateUI();
        }

        private void DeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DieStep d = (DieStep)((FrameworkElement)sender).DataContext;

            _Steps.Remove(d);

            d.PropertyChanged -= new PropertyChangedEventHandler(DieStep_PropertyChanged);

            UpdateUI();

        }

        private void AddDieButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	DieStep d = new DieStep();
			d.PropertyChanged += new PropertyChangedEventHandler(DieStep_PropertyChanged);
			_Steps.Add(d);
			
			UpdateUI();
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	Roll = MakeRoll();
			DialogResult = true;
			Close();
        }

	}
}