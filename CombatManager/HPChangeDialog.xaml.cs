/*
 *  HPChangeDialog.xaml.cs
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
    /// Interaction logic for HPChangeDialog.xaml
    /// </summary>
    public partial class HPChangeDialog : Window
    {
        ListBox _view;
        int hpchange;

        ObservableCollection<HPChangeItem> items;
		
        public HPChangeDialog()
        {
            InitializeComponent();
            HPChangeBox.DataContext = this;
        }
		
		public ListBox ListBox
		{
			get
			{
				return _view;
			}
			set
			{
				if (_view != value)
				{
					_view = value;
                    items = new ObservableCollection<HPChangeItem>();
                    foreach (Character c in _view.Items)
                    {
                        HPChangeItem item = new HPChangeItem();
                        item.Character = c;
                        item.Selected = _view.SelectedItems.Contains(c);
                        item.Half = false;

                        items.Add(item);

                    }

					CharacterList.DataContext = items;

				}
			}
		}


        public int HPChange
        {
            get
            {
                return hpchange;
            }
            set
            {
                hpchange = value;
            }
        }

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Close();
		}

        private class HPChangeItem : INotifyPropertyChanged
        {

            public event PropertyChangedEventHandler PropertyChanged;

            private Character _Character;
            private bool _Selected;
            private bool _Half;

            public Character Character
            {
                get { return _Character; }
                set
                {
                    if (_Character != value)
                    {
                        if (_Character != null)
                        {
                            _Character.PropertyChanged -= new PropertyChangedEventHandler(Character_PropertyChanged);
                        }

                        _Character = value;

                        _Character.PropertyChanged += new PropertyChangedEventHandler(Character_PropertyChanged);
                        
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Character")); }
                    }
                }
            }

            void  Character_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
 	            if (e.PropertyName == "HP" || e.PropertyName == "MaxHP")
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("HPText"));
                    }
                }
            }

            public bool Selected
            {
                get { return _Selected; }
                set
                {
                    if (_Selected != value)
                    {
                        _Selected = value;
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Selected")); }
                    }
                }
            }
            public bool Half
            {
                get { return _Half; }
                set
                {
                    if (_Half != value)
                    {
                        _Half = value;
                        if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs("Half")); }
                    }
                }
            }

            public string HPText
            {
                get { return Character.HP + "/" + Character.MaxHP; }
            } 

        }

        private void DamageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (HPChangeItem item in items)
                {
                    if (item.Selected)
                    {
                        item.Character.AdjustHP(item.Half ? -(HPChange / 2) : -HPChange);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Damage Character error:\r\n" + ex.ToString());
            }
        }

        private void HealButton_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                foreach (HPChangeItem item in items)
                {
                    if (item.Selected)
                    {
                        int change = item.Half ? (HPChange / 2) : HPChange;

                        if (change + item.Character.HP > item.Character.MaxHP)
                        {
                            change = item.Character.MaxHP - item.Character.HP;
                        }

                        item.Character.AdjustHP(change);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Heal Character error:\r\n" + ex.ToString());
            }
        }

        private void SelectAllButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	foreach (HPChangeItem item in items)
			{
				item.Selected = true;
			}
        }

        private void UnselectAllButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	foreach (HPChangeItem item in items)
			{
				item.Selected = false;
			}
        }
		
    }
}
