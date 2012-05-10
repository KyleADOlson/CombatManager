/*
 *  FeatEditorWindow.xaml.cs
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
    public class FeatEditorTypeItem
    {
        public bool Selected {get; set;}
        public String Type {get; set;}
    }


    /// <summary>
    /// Interaction logic for FeatEditorWindow.xaml
    /// </summary>
    public partial class FeatEditorWindow : Window
    {
        private List<FeatEditorTypeItem> typeItems;
		
		private bool loaded;

        public FeatEditorWindow()
        {
            InitializeComponent();

        }

        private Feat _Feat;

        public Feat Feat
        {
            get
            {
                return _Feat;
            }
            set
            {
                _Feat = value;
                DataContext = _Feat;

				loaded = false;
                typeItems = new List<FeatEditorTypeItem>();
                foreach (string type in Feat.FeatTypes)
                {
                    if (String.Compare(type, "General", true) != 0)
                    {
                        FeatEditorTypeItem item = new FeatEditorTypeItem();
                        item.Type = type;
                        if (Feat.Types.Contains(type, new InsensitiveEqualityCompararer()))
                        {
                            item.Selected = true;
                        }
                        typeItems.Add(item);
                    }
                }

                TypesListBox.DataContext = typeItems;
				loaded = true;
                UpdateOK();
            }
        }

        private void UpdateTypes()
        {
            if (typeItems != null && _Feat != null)
            {
                string types = "";

                foreach (FeatEditorTypeItem item in typeItems)
                {
                    if (item.Selected)
                    {
                        if (types.Length > 0)
                        {
                            types += ", ";
                            
                        }
                        types += item.Type;
                    }
                }

                if (types.Length == 0)
                {
                    _Feat.Type = "General";
                }
                else
                {
                    _Feat.Type = types;
                }
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void FeatNameText_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateOK();
        }

        private void UpdateOK()
        {
            OKButton.IsEnabled = NameText.Text.Length > 0;
        }

        private void TypesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TypesPopup.IsOpen = true;
        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
        	if (loaded)
			{
				UpdateTypes();
			}
        }


    }
}
